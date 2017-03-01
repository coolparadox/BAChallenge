namespace TwitListener

open BusinessLogic
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type PinEntryPage() = 
    inherit ContentPage()

    let mutable pin : string option = None

    // Setup and reference UI components
    let _ = base.LoadFromXaml(typeof<PinEntryPage>)
    let pinEntry = base.FindByName<Entry>("pinEntry")
    let okButton = base.FindByName<Button>("okButton")

    member this.GetPin() =
        pin

    override this.OnAppearing() =
        base.OnAppearing()
        pin <- None

    // Handle change of pin entry content.
    member this.onPinEntryTextChanged(sender:Object, args:EventArgs) = 
        okButton.IsEnabled <- String.length(pinEntry.Text) > 0

    // Handle click of 'Ok' button.
    member this.onOkButtonClicked(sender:Object, args:EventArgs) = 
        pin <- Some pinEntry.Text
        this.Navigation.RemovePage(this)
