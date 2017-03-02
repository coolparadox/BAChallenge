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

    // Refresh toolbar.
    member this.RefreshToolbar() =
        this.ToolbarItems.Clear()
        if businessManager.IsAuthenticated() then
            this.ToolbarItems.Add(ToolbarItem("Sign Out", "", (fun _ -> this.OnSignOutOptionClicked()), ToolbarItemOrder.Default, 0))
        else
            this.ToolbarItems.Add(ToolbarItem("Sign In", "", (fun _ -> this.OnSignInOptionClicked()), ToolbarItemOrder.Default, 0))
        this.ToolbarItems.Add(ToolbarItem("About", "", (fun _ -> this.OnAboutOptionClicked()), ToolbarItemOrder.Default, 10))

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
        this.RefreshToolbar()

    // Warn about failure in authentication.
    member this.onDisplayWarningRequest(message:string) =
        this.DisplayAlert ("Warning", message, "Bummer") |> ignore

    // Handle updates in tweet list.
    member this.onTweetListChanged(tweets:Tweet list) =
        tweetsView.ItemsSource <- ((tweets |> List.toSeq) :> Collections.IEnumerable)

    // Get notified of changes in domain model.
    member this.SubscribeToModelUpdates() =

        // Subscribe to changes in application state.
        this.onApplicationStateChanged(ApplicationState.LoggedOff)
        MessagingCenter.Unsubscribe<BusinessManager, ApplicationState>(this, "onApplicationStateChanged")
        MessagingCenter.Subscribe<BusinessManager, ApplicationState>(this, "onApplicationStateChanged",
            fun _ state -> this.onApplicationStateChanged(state)
        )

        // Subscribe to authentication failed events.
        MessagingCenter.Unsubscribe<BusinessManager, string>(this, "displayWarningRequest")
        MessagingCenter.Subscribe<BusinessManager, string>(this, "displayWarningRequest",
            fun _ message -> this.onDisplayWarningRequest(message)
        )

        // Subscribe to changes in tweet list.
        this.onTweetListChanged([])
        MessagingCenter.Unsubscribe<BusinessManager, Tweet list>(this, "onTweetListChanged")
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
        businessManager.SignOut()

    // Handle window uncovering.
    override this.OnAppearing() =
        base.OnAppearing()
        this.RefreshToolbar()

    // Handle click of action button.
    member this.OnActionButtonClicked(sender : Object, args : EventArgs) = 
        this.DisplayAlert ("FIXME", "not yet implemented", "Bummer") |> ignore
