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

    member this.OnButtonClicked(sender : Object, args : EventArgs) = 
        modelManager.addTweet({Who="@coolparadox"; When=DateTime.Now; What="the quick brown fox jumps over the lazy dog"})

type App() = 

    inherit Application(MainPage = TwitListenerPage())

    override this.OnStart() =
        base.OnStart()
        // Subscribe to changes in domain model.
        (this.MainPage :?> TwitListenerPage).SubscribeToModelUpdates
