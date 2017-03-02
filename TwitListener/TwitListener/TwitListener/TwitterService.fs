namespace ServiceAccess.Twitter

open ServiceAccess.Twitter.Types
open System.Threading.Tasks
open Tweetinvi
open Xamarin.Forms

// TwitterService singleton class.
type TwitterService private () =

    // Brick Abode Twitter Listener application keys
    // Registered by coolparadox
    let consumerCredential : Credential = {
        key = "Qz2wBeELJxvodeUe1iwJ4Z80s"
        secret = "KrWqBOC9qU9T00lO0zgM2NhkKzJkZN4jO70Pup6djxGUPY89SI"
    }

    // State variables
    let mutable mAuthorizationCredential : Credential option = None
    let mutable mUserCredential : Credential option = None
    let mutable mStream : Streaming.IFilteredStream option = None
    let mutable mListenTask : Task option = None

    // Storage keys for persisting state
    let storeKeyAuthCredential = "authorizationCredential"
    let storeKeyUserCredential = "userCredential"

    // Export TwitterService.Instance
    static let instance = TwitterService()
    static member Instance() = instance

    // Save state variables.
    member this.SaveState() =
        Application.Current.Properties.Add(storeKeyAuthCredential, mAuthorizationCredential)
        Application.Current.Properties.Add(storeKeyUserCredential, mUserCredential)

    // Load state variables.
    member this.LoadState() =
        mAuthorizationCredential <-
            if Application.Current.Properties.ContainsKey(storeKeyAuthCredential) then
                Application.Current.Properties.Item(storeKeyAuthCredential) :?> Credential option
            else None
        mUserCredential <-
            if Application.Current.Properties.ContainsKey(storeKeyUserCredential) then
                Application.Current.Properties.Item(storeKeyUserCredential) :?> Credential option
            else None

    // Start new (PIN) authorization process with Twitter
    // Answers a URL for getting a PIN.
    member this.StartPinAuthorization() =
        let applicationCredential = Auth.SetApplicationOnlyCredentials(consumerCredential.key, consumerCredential.secret)
        let context = AuthFlow.InitAuthentication(applicationCredential)
        mAuthorizationCredential <- 
            match context with
                | null -> None
                | _ ->
                    let token = context.Token
                    Some { key = token.AuthorizationKey; secret = token.AuthorizationSecret }
        match context with
            | null -> None
            | _ -> Some (System.Uri(context.AuthorizationURL))

    // Cancel PIN authorization process.
    member this.CancelPinAuthorization() =
        mAuthorizationCredential <- None

    // Continue PIN authorization process.
    // Answers if authorization was successfull.
    member this.ResumePinAuthorization(pin:string) =
        match mAuthorizationCredential with
            | None -> false
            | Some authorizationCredential ->
                let twitterCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pin, authorizationCredential.key, authorizationCredential.secret, consumerCredential.key, consumerCredential.secret)
                mAuthorizationCredential <- None
                match twitterCredentials with
                    | null ->
                        mUserCredential <- None
                        false
                    | _ ->
                       mUserCredential <- Some { key = twitterCredentials.AccessToken; secret = twitterCredentials.AccessTokenSecret }
                       Auth.SetCredentials(twitterCredentials)
                       true

    // Invalidade user access credentials.
    member this.InvalidateUserCredentials() =
        mUserCredential <- None

    // Get authenticated user name.
    member this.UserScreenName() =
        let authenticatedUser = User.GetAuthenticatedUser()
        if authenticatedUser = null then
            None
        else
            let accountSettings = authenticatedUser.GetAccountSettings()
            if accountSettings = null then
                None
            else
                Some accountSettings.ScreenName

    // Stop listening to Twitter stream.
    member this.StopStreaming() =
        if Option.isSome mStream then
            mStream.Value.StopStream()
            mStream <- None

    // Start listening to Twitter stream.
    member this.StartStreaming(filter:string) =
        let stream = Stream.CreateFilteredStream()
        stream.AddTrack(filter)
        stream.MatchingTweetReceived.Add(fun arg ->
            let tweet = arg.Tweet
            let user = tweet.CreatedBy.Name
            let timestamp = tweet.CreatedAt
            let message = tweet.Text
            let strippedTweet = { Who=user; When=timestamp; What=message }
            System.Diagnostics.Debug.WriteLine(sprintf "--> (TS) tweet received: %A" strippedTweet)
            MessagingCenter.Send<TwitterService, StrippedTweet>(this, "tweetReceived", strippedTweet)
        )
        stream.StreamStopped.Add(fun arg ->
            let reason =
                if arg.DisconnectMessage <> null then
                    let dMsg = arg.DisconnectMessage
                    sprintf "(%A on %A) %A" dMsg.Code dMsg.StreamName dMsg.Reason
                else
                    "unknown reason"
            System.Diagnostics.Debug.WriteLine(sprintf "--> (TS) twitter stream stopped: %A" reason)
            MessagingCenter.Send<TwitterService, string>(this, "streamStopped", reason)
        )
        stream.StreamStarted.Add(fun _ ->
            System.Diagnostics.Debug.WriteLine(sprintf "--> (TS) twitter stream started")
            MessagingCenter.Send<TwitterService>(this, "streamStarted")
        )
        mStream <- Some stream
        let task = stream.StartStreamMatchingAllConditionsAsync()
        mListenTask <- Some task