namespace TwitListener

open System
open Xamarin.Forms
open Xamarin.Forms.Xaml

// FIXME: move to Model layer
type TweetsViewItem(user:string, timestamp:DateTime, message:string) = 
    member this.Who = user
    member this.When = timestamp
    member this.What = message
 
type TwitListenerPage() = 

    inherit ContentPage()

    let _ = base.LoadFromXaml(typeof<TwitListenerPage>)
    let tweetsView = base.FindByName<ListView>("tweetsView")

    // FIXME: remove bogus data
    let mutable tweetsViewItems = [
        TweetsViewItem("@coolparadox", DateTime.Now, "the quick brown fox jumps over the lazy dog")
    ]
    do tweetsView.ItemsSource <- ((tweetsViewItems |> List.toSeq) :> Collections.IEnumerable)

    member this.OnButtonClicked(sender : Object, args : EventArgs) = 
        let newTweets = TweetsViewItem("@starcrusher", DateTime.Now, "yo bro kkk") :: tweetsViewItems
        tweetsViewItems <- newTweets
        do tweetsView.ItemsSource <- ((tweetsViewItems |> List.toSeq) :> Collections.IEnumerable)

type App() = 
    inherit Application(MainPage = TwitListenerPage())
