// Business Logic Layer
namespace BusinessLogic

open AuxFuncs
open BusinessLogic.Types
open Xamarin.Forms

// BusinessManager singleton class
type BusinessManager private () =

    // State variables of our domain model.
    let mutable mState = ApplicationState.LoggedOff
    let mutable mTweets = List.empty<Tweet>
    let mutable authenticationContext : Tweetinvi.Models.IAuthenticationContext option = None

    // Export BusinessManager.Instance
    static let instance = BusinessManager()
    static member Instance() = instance

    // Change application state.
    member private this.setApplicationState(state:ApplicationState) =
        if state <> mState then
            mState <- state
            // Notify subscribers that application state was changed.
            MessagingCenter.Send<BusinessManager, ApplicationState> (this, "onApplicationStateChanged", mState);

    // Authenticate to Twitter service.
    member this.signIn(page:Page) =
        if mState = ApplicationState.LoggedOff then
            this.setApplicationState(ApplicationState.Authenticating)
            let consumerKey = "Qz2wBeELJxvodeUe1iwJ4Z80s"
            let consumerSecret = "KrWqBOC9qU9T00lO0zgM2NhkKzJkZN4jO70Pup6djxGUPY89SI"
            let consumerCredentials = Tweetinvi.Auth.SetApplicationOnlyCredentials(consumerKey, consumerSecret)
            authenticationContext <- Some(Tweetinvi.AuthFlow.InitAuthentication(consumerCredentials))
            //Device.OpenUri(System.Uri(authenticationContext.Value.AuthorizationURL))
            MessagingCenter.Send<BusinessManager>(this, "getPinFromUser")

    // Continue an ongoing (Pin) authentication.
    member this.gotPinFromUser(pin:string) =
        if mState = ApplicationState.Authenticating then
            let userCredentials = Tweetinvi.AuthFlow.CreateCredentialsFromVerifierCode(pin, authenticationContext.Value)
            match userCredentials with
            | null ->
                this.setApplicationState(ApplicationState.LoggedOff)
                MessagingCenter.Send<BusinessManager>(this, "authenticationFailed")
            | _ ->
            //Tweetinvi.Auth.SetCredentials(userCredentials)
            //let authenticatedUser = Tweetinvi.User.GetAuthenticatedUser()
            this.setApplicationState(ApplicationState.Authenticated)

    // Cancel an ongoing (Pin) authentication.
    member this.cancelAuthentication() =
        if mState = ApplicationState.Authenticating then
            this.setApplicationState(ApplicationState.LoggedOff)

    // Are we authenticated?
    member this.isAuthenticated() =
        mState >= ApplicationState.Authenticated

    // Add a new tweet.
    member this.addTweet(tweet:Tweet) =
        if mState = ApplicationState.Listening then
            // Prepend new tweet to the list, protecting against infinite growth.
            mTweets <- tweet :: take 500 mTweets
            // Notify subscribers that tweet list was changed.
            MessagingCenter.Send<BusinessManager, Tweet list> (this, "onTweetListChanged", mTweets);
