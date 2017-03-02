// Business Logic Layer
namespace BusinessLogic

open BusinessLogic.Types
open ServiceAccess.Twitter
open ServiceAccess.Twitter.Types
open Xamarin.Forms

// ApplicationManager singleton class
type ApplicationManager private () =

    // State variables of our domain model.
    let mutable mApplicationState = ApplicationState.LoggedOff 
    
    // Storage keys for persisting state
    let storeKeyAppState = "applicationState"

    // Access to Twitter services
    let twitterService = TwitterService.Instance()

    // Export ApplicationManager.Instance
    static let instance = ApplicationManager()
    static member Instance() = instance

    // Application state getter.
    member this.CurrentState =
        mApplicationState
    
    // Prepare application to go to bed.
    member this.ApplicationSleep() =
        if mApplicationState = ApplicationState.Listening then
            // Close network connection
            twitterService.StopStreaming()
            mApplicationState <- ApplicationState.Authenticated
        Application.Current.Properties.Clear()
        twitterService.SaveState()
        Application.Current.Properties.Add(storeKeyAppState, mApplicationState)
        Application.Current.SavePropertiesAsync() |> ignore

    // Wake up from sleep.
    member this.ApplicationRecover() =
        twitterService.LoadState()
        if Application.Current.Properties.ContainsKey(storeKeyAppState) then
            mApplicationState <- Application.Current.Properties.Item(storeKeyAppState) :?> ApplicationState
        System.Diagnostics.Debug.WriteLine(sprintf "--> recovered application state: %A" mApplicationState)

    // Change application state.
    member private this.SetApplicationState(state:ApplicationState) =
        if state <> mApplicationState then
            System.Diagnostics.Debug.WriteLine(sprintf "TwitListener app state %A -> %A" mApplicationState state)
            mApplicationState <- state
            // Notify subscribers that application state was changed.
            Device.BeginInvokeOnMainThread(fun _ -> MessagingCenter.Send<ApplicationManager, ApplicationState> (this, "onApplicationStateChanged", mApplicationState))

    // Start authentication to Twitter service.
    member this.StartSignIn() =
        if mApplicationState = ApplicationState.LoggedOff then
            match twitterService.StartPinAuthorization() with
                | None ->
                    Device.BeginInvokeOnMainThread(fun _ -> MessagingCenter.Send<ApplicationManager, string>(this, "displayWarningRequest", "Application authorization failed"))
                | Some uri ->
                    this.SetApplicationState(ApplicationState.Authenticating)
                    Device.BeginInvokeOnMainThread(fun _ -> MessagingCenter.Send<ApplicationManager>(this, "getPinFromUser"))
                    Device.OpenUri(uri)

    // Continue an ongoing authentication.
    member this.GotPinFromUser(pin:string) =
        System.Diagnostics.Debug.WriteLine(sprintf "GotPinFromUser(%s)" pin)
        if mApplicationState = ApplicationState.Authenticating then
            if twitterService.ResumePinAuthorization(pin) then
                this.SetApplicationState(ApplicationState.Authenticated)
            else
                this.SetApplicationState(ApplicationState.LoggedOff)
                Device.BeginInvokeOnMainThread(fun _ -> MessagingCenter.Send<ApplicationManager, string>(this, "displayWarningRequest", "Authorization failed"))

    // Cancel an ongoing authentication.
    member this.CancelAuthentication() =
        if mApplicationState = ApplicationState.Authenticating then
            twitterService.CancelPinAuthorization()
            this.SetApplicationState(ApplicationState.LoggedOff)

    // Are we authenticating?
    member this.IsAuthenticating() =
        mApplicationState = ApplicationState.Authenticating

    // Are we authenticated?
    member this.IsAuthenticated() =
        mApplicationState >= ApplicationState.Authenticated

    // Sign out from twitter.
    member this.SignOut() =
        if this.IsAuthenticated() then
            twitterService.InvalidateUserCredentials() |> ignore
            this.SetApplicationState(ApplicationState.LoggedOff)

    member this.OnTweetReceived(tweet:StrippedTweet) =
        System.Diagnostics.Debug.WriteLine(sprintf "--> (AM) %A tweeted at %A:\n%A" tweet.Who tweet.When tweet.What)

    // Handle start of twitter stream listening.
    member this.OnTwitterStreamStarted() =
        System.Diagnostics.Debug.WriteLine(sprintf "--> (AM) twitter stream started")
        MessagingCenter.Unsubscribe<TwitterService>(this, "streamStarted")
        MessagingCenter.Subscribe<TwitterService, string>(this, "streamStopped", (fun _ reason -> this.OnTwitterStreamStopped(reason)))
        MessagingCenter.Subscribe<TwitterService, StrippedTweet>(this, "tweetReceived", (fun _ tweet -> this.OnTweetReceived(tweet)))
        this.SetApplicationState(ApplicationState.Listening)

    // Handle stop of twitter stream listening.
    member this.OnTwitterStreamStopped(reason:string) =
        System.Diagnostics.Debug.WriteLine(sprintf "--> (AM) twitter stream stopped: %A" reason)
        MessagingCenter.Unsubscribe<TwitterService, string>(this, "streamStopped")
        MessagingCenter.Unsubscribe<TwitterService, StrippedTweet>(this, "tweetReceived")
        this.SetApplicationState(ApplicationState.Authenticated)

    // Start listening to tweets.
    member this.StartListening(filter:string) =
        if mApplicationState = ApplicationState.Authenticated then
            MessagingCenter.Subscribe<TwitterService>(this, "streamStarted", (fun _ -> this.OnTwitterStreamStarted()))
            twitterService.StartStreaming(filter)

    // Stop listening to tweets.
    member this.StopListening() =
        if mApplicationState = ApplicationState.Listening then
            twitterService.StopStreaming()
