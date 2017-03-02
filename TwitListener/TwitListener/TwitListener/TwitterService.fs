namespace ServiceAccess.Twitter

open ServiceAccess.Twitter.Types
open Tweetinvi
open Xamarin.Forms

// TwitterServiceManager singleton class
type TwitterServiceManager private () =

    // Brick Abode Twitter Listener application keys
    // Registered by coolparadox
    let consumerCredential : Credential = {
        key = "Qz2wBeELJxvodeUe1iwJ4Z80s"
        secret = "KrWqBOC9qU9T00lO0zgM2NhkKzJkZN4jO70Pup6djxGUPY89SI"
    }

    // State variables
    let mutable mAuthorizationCredential : Credential option = None
    let mutable mUserCredential : Credential option = None

    // Storage keys for persisting state
    let storeKeyAuthCredential = "authorizationCredential"
    let storeKeyUserCredential = "userCredential"

    // Export TwitterServiceManager.Instance
    static let instance = TwitterServiceManager()
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
