namespace TwitListener

open AuxFuncs
open BusinessLogic
open BusinessLogic.Types
open ServiceAccess.Twitter.Types
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type MainPage() = 
    inherit ContentPage()

    let appManager = ApplicationManager.Instance()
    let mutable mTweets = List.empty<StrippedTweet>

    // Setup and reference UI components
    let _ = base.LoadFromXaml(typeof<MainPage>)
    let filterEntry = base.FindByName<Entry>("filterEntry")
    let actionButton = base.FindByName<Button>("actionButton")
    let tweetsView = base.FindByName<ListView>("tweetsView")

    (*
    // This page can listen to tweets.
    interface IStrippedTweetListener with

        member this.Filter =
            filterEntry.Text

        member this.OnTweetReceived(tweet:StrippedTweet) =
            // Add tweet to list.
            // Safeguard against infinite list expansion.
            mTweets <- tweet :: take 100 mTweets
            // Update view.
            tweetsView.ItemsSource <- ((mTweets |> List.toSeq) :> Collections.IEnumerable)

        member this.OnStreamStopped(reason:string) =
            this.DisplayAlert ("Tweet stream stopped", reason, "Ok") |> ignore
    *)

    // Refresh toolbar.
    member this.RefreshToolbar() =
        this.ToolbarItems.Clear()
        if appManager.IsAuthenticated() then
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
                actionButton.IsEnabled <- not (String.IsNullOrWhiteSpace(filterEntry.Text))
                tweetsView.IsEnabled <- true
            | ApplicationState.Listening ->
                filterEntry.IsEnabled <- false
                actionButton.Text <- "Stop"
                actionButton.IsEnabled <- true
                tweetsView.IsEnabled <- true
            | _ ->
                filterEntry.IsEnabled <- false
                filterEntry.Text <- ""
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- false
                tweetsView.IsEnabled <- false
        this.RefreshToolbar()

    // Warn about failure in authentication.
    member this.onDisplayWarningRequest(message:string) =
        this.DisplayAlert ("Warning", message, "Bummer") |> ignore

    // Get notified of changes in domain model.
    member this.SubscribeToModelUpdates() =

        // Subscribe to changes in application state.
        this.onApplicationStateChanged(ApplicationState.LoggedOff)
        MessagingCenter.Unsubscribe<ApplicationManager, ApplicationState>(this, "onApplicationStateChanged")
        MessagingCenter.Subscribe<ApplicationManager, ApplicationState>(this, "onApplicationStateChanged",
            fun _ state -> this.onApplicationStateChanged(state)
        )

        // Subscribe to authentication failed events.
        MessagingCenter.Unsubscribe<ApplicationManager, string>(this, "displayWarningRequest")
        MessagingCenter.Subscribe<ApplicationManager, string>(this, "displayWarningRequest",
            fun _ message -> this.onDisplayWarningRequest(message)
        )

    // Handle click of 'About' menu option.
    member this.OnAboutOptionClicked() =
        let message = "This is a simple Twitter stream API exerciser by coolparadox@gmail.com"
        this.DisplayAlert ("Twitter Listener", message, "OK") |> ignore

    // Handle click of 'Sign In' menu option.
    member this.OnSignInOptionClicked() =
        appManager.StartSignIn()

    // Handle click of 'Sign Out' menu option.
    member this.OnSignOutOptionClicked() =
        appManager.SignOut()

    // Handle window uncovering.
    override this.OnAppearing() =
        base.OnAppearing()
        this.RefreshToolbar()

    // Handle change of filter entry content.
    member this.OnFilterEntryTextChanged(sender:Object, args:EventArgs) = 
        actionButton.IsEnabled <- not (String.IsNullOrWhiteSpace(filterEntry.Text))

    // Handle click of action button.
    member this.OnActionButtonClicked(sender: Object, args: EventArgs) = 
        match appManager.CurrentState with
            | ApplicationState.Authenticated ->
                appManager.StartListening(filterEntry.Text)
            | ApplicationState.Listening ->
                appManager.StopListening()
            | _ -> ()
