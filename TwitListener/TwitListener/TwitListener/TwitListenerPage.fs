namespace TwitListener

open BusinessLogic
open BusinessLogic.Types
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type TwitListenerPage() = 
    inherit ContentPage()

    let businessManager = BusinessManager.Instance()

    // Setup and reference UI components
    let _ = base.LoadFromXaml(typeof<TwitListenerPage>)
    let filterEntry = base.FindByName<Entry>("filterEntry")
    let actionButton = base.FindByName<Button>("actionButton")
    let tweetsView = base.FindByName<ListView>("tweetsView")

    // Propagate changes in application state to UI components.
    member this.OnApplicationStateChanged(state:ApplicationState) =
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

    // Handle updates in tweet list.
    member this.OnTweetListChanged(tweets:Tweet list) =
        tweetsView.ItemsSource <- ((tweets |> List.toSeq) :> Collections.IEnumerable)

    // Get notified of changes in domain model.
    member this.SubscribeToModelUpdates() =

        // Subscribe to changes in application state.
        this.OnApplicationStateChanged(ApplicationState.LoggedOff)
        MessagingCenter.Subscribe<BusinessManager, ApplicationState> (this, "onApplicationStateChanged",
            fun _ state -> this.OnApplicationStateChanged(state)
        )

        // Subscribe to changes in tweet list.
        this.OnTweetListChanged([])
        MessagingCenter.Subscribe<BusinessManager, Tweet list> (this, "onTweetListChanged",
            fun _ tweets -> this.OnTweetListChanged(tweets)
        )

    // Handle click of 'About' menu option.
    member this.OnAboutOptionClicked() =
        let message = "This is a simple Twitter stream API exerciser by coolparadox@gmail.com"
        this.DisplayAlert ("Twitter Listener", message, "OK") |> ignore

    // Handle click of 'Sign In' menu option.
    member this.OnSignInOptionClicked() =
        businessManager.signIn(this)

    // Handle window uncovering.
    override this.OnAppearing() =
        base.OnAppearing()
        // Build toolbar.
        this.ToolbarItems.Clear()
        this.ToolbarItems.Add(ToolbarItem("Sign In", "", (fun _ -> this.OnSignInOptionClicked()), ToolbarItemOrder.Default, 0))
        this.ToolbarItems.Add(ToolbarItem("About", "", (fun _ -> this.OnAboutOptionClicked()), ToolbarItemOrder.Default, 10))

    // Handle click of action button.
    member this.OnActionButtonClicked(sender : Object, args : EventArgs) = 
        //modelManager.addTweet({Who="@coolparadox"; When=DateTime.Now; What="the quick brown fox jumps over the lazy dog"})
        //modelManager.SetApplicationState(ModelTypes.State.Authenticated)
        this.DisplayAlert("FIXME", "Authenticate and start streaming dude!", "Yeah...") |> ignore

type App() = 

    inherit Application(MainPage = NavigationPage(TwitListenerPage()))

    let navigationPage = base.MainPage :?> NavigationPage

    // Handle Pin request from BusinessManager
    member this.getPinFromUser() =
        navigationPage.PushAsync(PinEntryPage(), true) |> ignore

    // Handle application start
    override this.OnStart() =
        base.OnStart()

        // Disable navigation bar.
        let navigationPage = this.MainPage :?> NavigationPage
        NavigationPage.SetHasNavigationBar(navigationPage, false)

        // Subscribe navigation page to Pin requests from BusinessManager
        MessagingCenter.Subscribe<BusinessManager> (this, "getPinFromUser", (fun _ -> this.getPinFromUser()))

        // Subscribe main content page to changes in domain model.
        let twitListenerPage = navigationPage.CurrentPage :?> TwitListenerPage
        twitListenerPage.SubscribeToModelUpdates()
