namespace TwitListener

open BusinessLogic
open BusinessLogic.Types
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type MainPage() = 
    inherit ContentPage()

    let businessManager = BusinessManager.Instance()

    // Setup and reference UI components
    let _ = base.LoadFromXaml(typeof<MainPage>)
    let filterEntry = base.FindByName<Entry>("filterEntry")
    let actionButton = base.FindByName<Button>("actionButton")
    let tweetsView = base.FindByName<ListView>("tweetsView")

    // Propagate changes in application state to UI components.
    member this.onApplicationStateChanged(state:ApplicationState) =
        match state with
            | ApplicationState.Authenticating ->
                filterEntry.IsEnabled <- false
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- false
                tweetsView.IsEnabled <- false
            | ApplicationState.Authenticated ->
                filterEntry.IsEnabled <- true
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- true
                tweetsView.IsEnabled <- false
            | _ ->
                filterEntry.IsEnabled <- false
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- false
                tweetsView.IsEnabled <- false

    // Warn about failure in authentication.
    member this.onAuthenticationFailed() =
        this.DisplayAlert ("Warning", "Authentication failed", "Ok") |> ignore

    // Handle updates in tweet list.
    member this.onTweetListChanged(tweets:Tweet list) =
        tweetsView.ItemsSource <- ((tweets |> List.toSeq) :> Collections.IEnumerable)

    // Get notified of changes in domain model.
    member this.SubscribeToModelUpdates() =

        // Subscribe to changes in application state.
        this.onApplicationStateChanged(ApplicationState.LoggedOff)
        MessagingCenter.Subscribe<BusinessManager, ApplicationState> (this, "onApplicationStateChanged",
            fun _ state -> this.onApplicationStateChanged(state)
        )

        // Subscribe to authentication failed events.
        MessagingCenter.Subscribe<BusinessManager> (this, "authenticationFailed",
            fun _ -> this.onAuthenticationFailed()
        )

        // Subscribe to changes in tweet list.
        this.onTweetListChanged([])
        MessagingCenter.Subscribe<BusinessManager, Tweet list> (this, "onTweetListChanged",
            fun _ tweets -> this.onTweetListChanged(tweets)
        )

    // Handle click of 'About' menu option.
    member this.OnAboutOptionClicked() =
        let message = "This is a simple Twitter stream API exerciser by coolparadox@gmail.com"
        this.DisplayAlert ("Twitter Listener", message, "OK") |> ignore

    // Handle click of 'Sign In' menu option.
    member this.OnSignInOptionClicked() =
        businessManager.StartSignIn()

    // Handle click of 'Sign Out' menu option.
    member this.OnSignOutOptionClicked() =
        this.DisplayAlert ("FIXME", "not yet implemented", "Bummer") |> ignore

    // Handle window uncovering.
    override this.OnAppearing() =
        base.OnAppearing()

        // Build toolbar.
        this.ToolbarItems.Clear()
        if businessManager.IsAuthenticated() then
            this.ToolbarItems.Add(ToolbarItem("Sign Out", "", (fun _ -> this.OnSignOutOptionClicked()), ToolbarItemOrder.Default, 0))
        else
            this.ToolbarItems.Add(ToolbarItem("Sign In", "", (fun _ -> this.OnSignInOptionClicked()), ToolbarItemOrder.Default, 0))
        this.ToolbarItems.Add(ToolbarItem("About", "", (fun _ -> this.OnAboutOptionClicked()), ToolbarItemOrder.Default, 10))

        // Check if pin entry dialog has been cancelled by OS back button.
        if businessManager.IsAuthenticating() then
            businessManager.CancelAuthentication()

    // Handle click of action button.
    member this.OnActionButtonClicked(sender : Object, args : EventArgs) = 
        //modelManager.addTweet({Who="@coolparadox"; When=DateTime.Now; What="the quick brown fox jumps over the lazy dog"})
        //modelManager.SetApplicationState(ModelTypes.State.Authenticated)
        this.DisplayAlert("FIXME", "Authenticate and start streaming dude!", "Yeah...") |> ignore
