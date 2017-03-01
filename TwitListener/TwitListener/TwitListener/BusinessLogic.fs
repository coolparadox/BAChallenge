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

    // Access to Twitter services
    let twitterServiceManager = TwitterServiceManager.Instance()

    // Export BusinessManager.Instance
    static let instance = BusinessManager()
    static member Instance() = instance
    
    // Save state variables.
    member this.SaveState() =
        Application.Current.Properties.Clear()
        twitterServiceManager.SaveState()
        Application.Current.SavePropertiesAsync() |> ignore

    // Load state variables.
    member this.LoadState() =
        if not (twitterServiceManager.LoadState()) then
            System.Diagnostics.Debug.WriteLine("LoadState ERROR")

    // Change application state.
    member private this.SetApplicationState(state:ApplicationState) =
        if state <> mApplicationState then
            mApplicationState <- state
            // Notify subscribers that application state was changed.
            MessagingCenter.Send<BusinessManager, ApplicationState> (this, "onApplicationStateChanged", mApplicationState);

    // Start authentication to Twitter service.
    member this.StartSignIn() =
        if mApplicationState = ApplicationState.LoggedOff then
            this.SetApplicationState(ApplicationState.Authenticating)
            if twitterServiceManager.StartPinAuthorization() then
                MessagingCenter.Send<BusinessManager>(this, "getPinFromUser")
                match twitterServiceManager.AuthorizationURL() with
                    | Some url ->
                        Device.OpenUri(System.Uri(url))
                    | None ->
                        Device.OpenUri(System.Uri("http://dilbert.com/assets/error-strip-c6fc9d5e2ea0ade7187aa3deacdf4a3d.jpg"))
            else
                this.SetApplicationState(ApplicationState.LoggedOff)
                MessagingCenter.Send<BusinessManager>(this, "authenticationFailed")

    // Continue an ongoing (Pin) authentication.
    member this.GotPinFromUser(pin:string) =
        true |> ignore
        (*
        if mApplicationState = ApplicationState.Authenticating then
            let userCredentials = Tweetinvi.AuthFlow.CreateCredentialsFromVerifierCode(pin, authenticationContext.Value)
            match userCredentials with
            | null ->
                this.setApplicationState(ApplicationState.LoggedOff)
                MessagingCenter.Send<BusinessManager>(this, "authenticationFailed")
            | _ ->
            //Tweetinvi.Auth.SetCredentials(userCredentials)
            //let authenticatedUser = Tweetinvi.User.GetAuthenticatedUser()
            this.setApplicationState(ApplicationState.Authenticated)
        *)

    // Cancel an ongoing (Pin) authentication.
    member this.CancelAuthentication() =
        if mApplicationState = ApplicationState.Authenticating then
            this.SetApplicationState(ApplicationState.LoggedOff)

    // Are we authenticating?
    member this.IsAuthenticating() =
        mApplicationState = ApplicationState.Authenticating

    // Are we authenticated?
    member this.IsAuthenticated() =
        mApplicationState >= ApplicationState.Authenticated

    // Add a new tweet.
    member this.AddTweet(tweet:Tweet) =
        if mApplicationState = ApplicationState.Listening then
            // Prepend new tweet to the list, protecting against infinite growth.
            mTweets <- tweet :: take 500 mTweets
            // Notify subscribers that tweet list was changed.
            MessagingCenter.Send<BusinessManager, Tweet list> (this, "onTweetListChanged", mTweets);
