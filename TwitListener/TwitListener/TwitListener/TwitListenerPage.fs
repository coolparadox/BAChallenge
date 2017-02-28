namespace TwitListener

open BusinessLogic
open BusinessLogic.Types
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type TwitListenerPage() = 

    inherit ContentPage()

    //let businessManager = ModelManager.Instance

    let _ = base.LoadFromXaml(typeof<TwitListenerPage>)
    let filterEntry = base.FindByName<Entry>("filterEntry")
    let actionButton = base.FindByName<Button>("actionButton")
    let tweetsView = base.FindByName<ListView>("tweetsView")

    // Propagate changes in application state to UI components.
    member this.OnApplicationStateChanged(state:ApplicationState) =
        match state with
            | ApplicationState.Authenticated ->
                filterEntry.IsEnabled <- true
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- true
                tweetsView.IsEnabled <- false
            | _ ->
                filterEntry.IsEnabled <- true
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- true
                tweetsView.IsEnabled <- false

    // Handle updates in tweet list.
    member this.OnTweetListChanged(tweets:Tweet list) =
        tweetsView.ItemsSource <- ((tweets |> List.toSeq) :> Collections.IEnumerable)

    // Get notified by ModelManager of changes in domain model.
    member this.SubscribeToModelUpdates() =

        // Subscribe to changes in application state.
        this.OnApplicationStateChanged(ApplicationState.Initial)
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

    // Handle window uncovering.
    override this.OnAppearing() =
        base.OnAppearing()
        // Build toolbar.
        this.ToolbarItems.Clear()
        this.ToolbarItems.Add(ToolbarItem("About", "", (fun _ -> this.OnAboutOptionClicked()), ToolbarItemOrder.Secondary, 1))

    // Handle click of action button.
    member this.OnActionButtonClicked(sender : Object, args : EventArgs) = 
        //modelManager.addTweet({Who="@coolparadox"; When=DateTime.Now; What="the quick brown fox jumps over the lazy dog"})
        //modelManager.SetApplicationState(ModelTypes.State.Authenticated)
        this.DisplayAlert("FIXME", "Authenticate and start streaming dude!", "Yeah...") |> ignore

type App() = 

    inherit Application(MainPage = NavigationPage(TwitListenerPage()))

    override this.OnStart() =
        base.OnStart()

        // Disable navigation bar.
        let navigationPage = this.MainPage :?> NavigationPage
        NavigationPage.SetHasNavigationBar(navigationPage, false)

        // Subscribe content page to events from domain model.
        let twitListenerPage = navigationPage.CurrentPage :?> TwitListenerPage
        twitListenerPage.SubscribeToModelUpdates()
