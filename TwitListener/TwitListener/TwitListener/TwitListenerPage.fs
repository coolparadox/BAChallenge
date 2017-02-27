namespace TwitListener

open Model
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type TwitListenerPage() = 

    inherit ContentPage()

    let _ = base.LoadFromXaml(typeof<TwitListenerPage>)
    let tweetsView = base.FindByName<ListView>("tweetsView")

    do tweetsView.ItemsSource <- ((Model.Instance.Tweets |> List.toSeq) :> Collections.IEnumerable)

    member this.OnButtonClicked(sender : Object, args : EventArgs) = 
        Model.Instance.AddTweet({Who="@coolparadox"; When=DateTime.Now; What="the quick brown fox jumps over the lazy dog"})
        tweetsView.ItemsSource <- ((Model.Instance.Tweets |> List.toSeq) :> Collections.IEnumerable)

type App() = 
    inherit Application(MainPage = TwitListenerPage())
