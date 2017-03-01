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

    // Storage keys for persisting state
    let storeKeyAuthCredential = "authorizationCredential"

    // Export TwitterServiceManager.Instance
    static let instance = TwitterServiceManager()
    static member Instance() = instance

    // Save state variables.
    member this.SaveState() =
        //System.Diagnostics.Debug.WriteLine(sprintf "--> authorizationCredential state is %A" mAuthorizationCredential)
        Application.Current.Properties.Add(storeKeyAuthCredential, mAuthorizationCredential)
        //System.Diagnostics.Debug.WriteLine(sprintf "--> application property keys %A" (Application.Current.Properties.Keys))
        //System.Diagnostics.Debug.WriteLine(sprintf "--> application property values %A" (Application.Current.Properties.Values))

    // Load state variables.
    member this.LoadState() =
        //mAuthorizationCredential <- None
        //System.Diagnostics.Debug.WriteLine(sprintf "--> application property keys %A" (Application.Current.Properties.Keys))
        //System.Diagnostics.Debug.WriteLine(sprintf "--> application property values %A" (Application.Current.Properties.Values))
        if Application.Current.Properties.ContainsKey(storeKeyAuthCredential) then
            mAuthorizationCredential <- Application.Current.Properties.Item(storeKeyAuthCredential) :?> Credential option
        //else
        //    System.Diagnostics.Debug.WriteLine(sprintf "--> application property key %s not found" "authorizationCredential")
        //System.Diagnostics.Debug.WriteLine(sprintf "--> authorizationCredential recovered %A" mAuthorizationCredential)

    // Start new pin authorization process with Twitter
    member this.StartPinAuthorization() =
        let applicationCredential = Auth.SetApplicationOnlyCredentials(consumerCredential.key, consumerCredential.secret)
        let context = AuthFlow.InitAuthentication applicationCredential
        mAuthorizationCredential <- 
            match context with
                | null -> None
                | _ ->
                    let token = context.Token
                    Some { key = token.AuthorizationKey; secret = token.AuthorizationSecret }
        match context with
            | null -> None
            | _ -> Some (System.Uri(context.AuthorizationURL))
