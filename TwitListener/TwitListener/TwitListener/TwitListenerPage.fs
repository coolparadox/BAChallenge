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
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
        { Who = "@coolparadox"; When = DateTime.Now; What = "tweet this dude #partiu" }
        { Who = "@starcrusher"; When = DateTime.Now; What = "naah I use messenger duh! Twitter SMS verification conde is soooooo slow it never reaches destiny argh" }
    ]
    do tweetsView.ItemsSource <- ((tweetsViewItems |> List.toSeq) :> Collections.IEnumerable)

type App() = 
    inherit Application(MainPage = TwitListenerPage())
