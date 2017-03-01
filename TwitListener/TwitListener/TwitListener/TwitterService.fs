namespace ServiceAccess.Twitter

open Tweetinvi
open Xamarin.Forms

// TwitterServiceManager singleton class
type TwitterServiceManager private () =

    // Brick Abode Twitter Listener application keys
    // Registered by coolparadox
    let consumerKey = "Qz2wBeELJxvodeUe1iwJ4Z80s"
    let consumerSecret = "KrWqBOC9qU9T00lO0zgM2NhkKzJkZN4jO70Pup6djxGUPY89SI"

    // State variables
    let mutable mAuthenticationContext : Models.IAuthenticationContext = null

    // Export TwitterServiceManager.Instance
    static let instance = TwitterServiceManager()
    static member Instance() = instance

    // Save state variables.
    member this.SaveState() =
        Application.Current.Properties.Add("authenticationContext", mAuthenticationContext)

    // Load state variables.
    member this.LoadState() =
        Application.Current.Properties.TryGetValue("authenticationContext", ref (mAuthenticationContext :> obj))

    // Start new pin authorization process with Twitter
    member this.StartPinAuthorization() =
        let consumerCredentials = Auth.SetApplicationOnlyCredentials(consumerKey, consumerSecret)
        mAuthenticationContext <- AuthFlow.InitAuthentication(consumerCredentials)
        mAuthenticationContext <> null

    member this.AuthorizationURL() =
        if mAuthenticationContext <> null then
            Some(mAuthenticationContext.AuthorizationURL)
        else
            None
