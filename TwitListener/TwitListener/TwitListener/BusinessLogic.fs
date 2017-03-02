// Business Logic Layer
namespace BusinessLogic

open BusinessLogic.Types
open ServiceAccess.Twitter
open ServiceAccess.Twitter.Types
open Xamarin.Forms

// BusinessManager singleton class
type BusinessManager private () =

    // State variables of our domain model.
    let mutable mApplicationState = ApplicationState.LoggedOff 
    
    // Storage keys for persisting state
    let storeKeyAppState = "applicationState"

    // Access to Twitter services
    let twitterService = TwitterService.Instance()

    // Export BusinessManager.Instance
    static let instance = BusinessManager()
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
            MessagingCenter.Send<BusinessManager, ApplicationState> (this, "onApplicationStateChanged", mApplicationState);

    // Start authentication to Twitter service.
    member this.StartSignIn() =
        if mApplicationState = ApplicationState.LoggedOff then
            match twitterService.StartPinAuthorization() with
                | None ->
                    MessagingCenter.Send<BusinessManager, string>(this, "displayWarningRequest", "Application authorization failed")
                | Some uri ->
                    this.SetApplicationState(ApplicationState.Authenticating)
                    MessagingCenter.Send<BusinessManager>(this, "getPinFromUser")
                    Device.OpenUri(uri)

    // Continue an ongoing authentication.
    member this.GotPinFromUser(pin:string) =
        System.Diagnostics.Debug.WriteLine(sprintf "GotPinFromUser(%s)" pin)
        if mApplicationState = ApplicationState.Authenticating then
            if twitterService.ResumePinAuthorization(pin) then
                this.SetApplicationState(ApplicationState.Authenticated)
            else
                this.SetApplicationState(ApplicationState.LoggedOff)
                MessagingCenter.Send<BusinessManager, string>(this, "displayWarningRequest", "Authorization failed")

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

    // Handle start of twitter stream listening.
    member this.onTwitterStreamStarted() =
        this.SetApplicationState(ApplicationState.Listening)

    // Handle stop of twitter stream listening.
    member this.onTwitterStreamStopped() =
        MessagingCenter.Unsubscribe<TwitterService>(this, "twitterStreamStarted")
        MessagingCenter.Unsubscribe<TwitterService>(this, "twitterStreamStopped")
        this.SetApplicationState(ApplicationState.Authenticated)

    // Start listening to tweets.
    member this.StartListening(listener:IStrippedTweetListener) =
        if mApplicationState = ApplicationState.Authenticated then
            MessagingCenter.Subscribe<TwitterService>(this, "twitterStreamStarted", (fun _ -> this.onTwitterStreamStarted()))
            MessagingCenter.Subscribe<TwitterService>(this, "twitterStreamStopped", (fun _ -> this.onTwitterStreamStopped()))
            twitterService.StartStreaming(listener.Filter)

    // Stop listening to tweets.
    member this.StopListening() =
        if mApplicationState = ApplicationState.Listening then
            twitterService.StopStreaming()
