namespace TwitListener

open System
open Xamarin.Forms
open Xamarin.Forms.Xaml

type TweetsViewItem = {
    Who : string
    When : DateTime
    What : string
}
 
type TwitListenerPage() = 
    inherit ContentPage()
    let _ = base.LoadFromXaml(typeof<TwitListenerPage>)
    let tweetsView = base.FindByName<ListView>("tweetsView")
    let tweetsViewItems = [
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
        { Who = "@coolparadox"; When = DateTime.Now; What = "the quick brown fox jumps over the lazy dog" }
        { Who = "@noblame"; When = DateTime.Now; What = "I have to write a lengthy tweet in order to exercise multiline text in ListView. Oh my what ever should I type @coolparadox?" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "I want to tell you an UDP joke, but I'm afraid you won't get it." }
    ]
    do tweetsView.ItemsSource <- ((tweetsViewItems |> List.toSeq) :> Collections.IEnumerable)

type App() = 
    inherit Application(MainPage = TwitListenerPage())
