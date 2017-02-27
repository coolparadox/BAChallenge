namespace TwitListener

open ModelManager
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type TwitListenerPage() = 

    inherit ContentPage()

    let _ = base.LoadFromXaml(typeof<TwitListenerPage>)
    let tweetsView = base.FindByName<ListView>("tweetsView")

    member this.OnButtonClicked(sender : Object, args : EventArgs) = 
        ModelManager.Instance.AddTweet({Who="@coolparadox"; When=DateTime.Now; What="the quick brown fox jumps over the lazy dog"})

    member this.SubscribeToEvents =
        MessagingCenter.Subscribe<ModelManager, Model.Tweet list> (this, "onTweetListChanged",
            fun _ arg -> tweetsView.ItemsSource <- ((arg |> List.toSeq) :> Collections.IEnumerable)
        )

type App() = 

    inherit Application(MainPage = TwitListenerPage())

    override this.OnStart() =
        base.OnStart()
        (this.MainPage :?> TwitListenerPage).SubscribeToEvents
