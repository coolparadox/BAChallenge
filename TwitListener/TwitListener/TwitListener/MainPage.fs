﻿namespace TwitListener

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

    // Handle an incoming tweet.
    member this.OnTweetReceived(tweet:StrippedTweet) =
        System.Diagnostics.Debug.WriteLine(sprintf "--> %A tweeted at %A:\n%A" tweet.Who tweet.When tweet.What)
        // Add tweet to list.
        // Safeguard against infinite list expansion.
        mTweets <- tweet :: take 100 mTweets
        // Update view.
        tweetsView.ItemsSource <- ((mTweets |> List.toSeq) :> Collections.IEnumerable)

    // Refresh toolbar.
    member this.RefreshToolbar() =
        this.ToolbarItems.Clear()
        if appManager.IsAuthenticated() then
            this.ToolbarItems.Add(ToolbarItem("Sign Out", "", (fun _ -> this.OnSignOutOptionClicked()), ToolbarItemOrder.Default, 0))
        else
            this.ToolbarItems.Add(ToolbarItem("Sign In", "", (fun _ -> this.OnSignInOptionClicked()), ToolbarItemOrder.Default, 0))
        this.ToolbarItems.Add(ToolbarItem("About", "", (fun _ -> this.OnAboutOptionClicked()), ToolbarItemOrder.Default, 10))

    // Update visibility of UI components
    member this.UpdateVisibilityOfComponents(state:ApplicationState) =
        match state with
            | ApplicationState.Authenticating ->
                filterEntry.Placeholder <- "Please sign in"
                filterEntry.IsEnabled <- false
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- false
                tweetsView.IsEnabled <- false
            | ApplicationState.Authenticated ->
                filterEntry.Placeholder <- "enter filter text"
                filterEntry.IsEnabled <- true
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- not (String.IsNullOrWhiteSpace(filterEntry.Text))
                tweetsView.IsEnabled <- true
            | ApplicationState.Listening ->
                filterEntry.Placeholder <- "enter filter text"
                filterEntry.IsEnabled <- false
                actionButton.Text <- "Stop"
                actionButton.IsEnabled <- true
                tweetsView.IsEnabled <- true
            | _ ->
                filterEntry.Placeholder <- "Please sign in"
                filterEntry.IsEnabled <- false
                actionButton.Text <- "Start"
                actionButton.IsEnabled <- false
                tweetsView.IsEnabled <- false
        this.RefreshToolbar()

    // Propagate changes in application state to UI components.
    member this.onApplicationStateChanged(state:ApplicationState) =
        this.UpdateVisibilityOfComponents(state)
        if state = ApplicationState.Listening then
            MessagingCenter.Subscribe<ApplicationManager, StrippedTweet>(this, "tweetReceived", (fun _ tweet ->
                this.OnTweetReceived(tweet)
            ))
        else
            MessagingCenter.Unsubscribe<ApplicationManager, StrippedTweet>(this, "tweetReceived")            

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
        this.UpdateVisibilityOfComponents(appManager.CurrentState)

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
