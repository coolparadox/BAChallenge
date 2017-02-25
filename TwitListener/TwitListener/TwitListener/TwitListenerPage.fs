namespace TwitListener

open System
open Xamarin.Forms
open Xamarin.Forms.Xaml

type TwitListenerPage() = 
    inherit ContentPage()
    let _ = base.LoadFromXaml(typeof<TwitListenerPage>)

type App() = 
    inherit Application(MainPage = TwitListenerPage())
