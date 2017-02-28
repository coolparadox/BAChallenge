namespace TwitListener

open BusinessLogic
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type PinEntryPage() = 
    inherit ContentPage()

    let businessManager = BusinessManager.Instance()

    // Setup and reference UI components
    (*
    let baseContent = StackLayout(
                          Orientation = StackOrientation.Vertical, 
                          VerticalOptions = LayoutOptions.FillAndExpand
                      )
    do
        baseContent.Children.Add(
            Label(
                Text = "Twitter Sign In",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof<Label>)
            )
        )
        baseContent.Children.Add(
            EntryCell(
                Keyboard = Keyboard.Numeric,

            )
        )
        *)
    let _ = base.LoadFromXaml(typeof<PinEntryPage>)
    //let pinEntry = base.FindByName<Entry>("pinEntry")

    // Handle click of 'Ok' button.
    member this.onOkButtonClicked(sender : Object, args : EventArgs) = 
        true |> ignore

    // Handle click of 'Cancel' button.
    member this.onCancelButtonClicked(sender : Object, args : EventArgs) = 
        true |> ignore
