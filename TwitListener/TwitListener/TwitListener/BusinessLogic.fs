// Business Logic Layer
namespace BusinessLogic

open AuxFuncs
open BusinessLogic.Types
open ServiceAccess.Twitter
open ServiceAccess.Twitter.Types
open Xamarin.Forms

// BusinessManager singleton class
type BusinessManager private () =

    // State variables of our domain model.
    let mutable mApplicationState = ApplicationState.LoggedOff
    let mutable mTweets = List.empty<Tweet>
    
    // Storage keys for persisting state
    let storeKeyAppState = "applicationState"

    // Access to Twitter services
    let twitterServiceManager = TwitterServiceManager.Instance()

    // Export BusinessManager.Instance
    static let instance = BusinessManager()
    static member Instance() = instance
    
    // Save state variables.
    member this.SaveState() =
        Application.Current.Properties.Clear()
        twitterServiceManager.SaveState()
        Application.Current.Properties.Add(storeKeyAppState, mApplicationState)
        Application.Current.SavePropertiesAsync() |> ignore

    // Load state variables.
    member this.LoadState() =
        twitterServiceManager.LoadState()
        if Application.Current.Properties.ContainsKey(storeKeyAppState) then
            mApplicationState <- Application.Current.Properties.Item(storeKeyAppState) :?> ApplicationState
        System.Diagnostics.Debug.WriteLine(sprintf "--> applicationState recovered %A" mApplicationState)

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
            match twitterServiceManager.StartPinAuthorization() with
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
            if twitterServiceManager.ResumePinAuthorization(pin) then
                this.SetApplicationState(ApplicationState.Authenticated)
            else
                this.SetApplicationState(ApplicationState.LoggedOff)
                MessagingCenter.Send<BusinessManager, string>(this, "displayWarningRequest", "User authentication failed")

    // Cancel an ongoing authentication.
    member this.CancelAuthentication() =
        if mApplicationState = ApplicationState.Authenticating then
            twitterServiceManager.CancelPinAuthorization()
            this.SetApplicationState(ApplicationState.LoggedOff)

    // Are we authenticating?
    member this.IsAuthenticating() =
        mApplicationState = ApplicationState.Authenticating

    // Are we authenticated?
    member this.IsAuthenticated() =
        mApplicationState >= ApplicationState.Authenticated

    // Sign out from twitter
    member this.SignOut() =
        if this.IsAuthenticated() then
            twitterServiceManager.InvalidateUserCredentials() |> ignore
            this.SetApplicationState(ApplicationState.LoggedOff)

    // Add a new tweet.
    member this.AddTweet(tweet:Tweet) =
        if mApplicationState = ApplicationState.Listening then
            // Prepend new tweet to the list, protecting against infinite growth.
            mTweets <- tweet :: take 500 mTweets
            // Notify subscribers that tweet list was changed.
            MessagingCenter.Send<BusinessManager, Tweet list> (this, "onTweetListChanged", mTweets);
