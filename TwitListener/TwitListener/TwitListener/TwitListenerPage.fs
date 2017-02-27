namespace TwitListener

open ModelManager
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type TwitListenerPage() = 

    inherit ContentPage()

    let modelManager = ModelManager.Instance
    let _ = base.LoadFromXaml(typeof<TwitListenerPage>)
    let tweetsView = base.FindByName<ListView>("tweetsView")

    // Get notified of changes in domain model.
    // This only works if model is updated through ModelManager.
    member this.SubscribeToModelUpdates =
        // Refresh tweet list
        MessagingCenter.Subscribe<ModelManager, ModelTypes.Tweet list> (this, "onTweetListChanged",
            fun _ arg -> tweetsView.ItemsSource <- ((arg |> List.toSeq) :> Collections.IEnumerable)
        )

    member this.OnAboutClicked() =
        let message = "This is a simple Twitter stream API exerciser by coolparadox@gmail.com"
        this.DisplayAlert ("Twitter Listener", message, "OK") |> ignore

    override this.OnAppearing() =
        base.OnAppearing()
        this.ToolbarItems.Clear()
        this.ToolbarItems.Add(ToolbarItem("About", "", (fun _ -> this.OnAboutClicked()), ToolbarItemOrder.Default, 0))

    member this.OnButtonClicked(sender : Object, args : EventArgs) = 
        modelManager.addTweet({Who="@coolparadox"; When=DateTime.Now; What="the quick brown fox jumps over the lazy dog"})

type App() = 

    inherit Application(MainPage = NavigationPage(TwitListenerPage()))

    override this.OnStart() =
        base.OnStart()

        // Disable navigation bar.
        let navigationPage = this.MainPage :?> NavigationPage
        NavigationPage.SetHasNavigationBar(navigationPage, false)

        // Subscribe content page to events from domain model.
        let twitListenerPage = navigationPage.CurrentPage :?> TwitListenerPage
        twitListenerPage.SubscribeToModelUpdates
