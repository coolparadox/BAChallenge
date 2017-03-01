namespace TwitListener

open BusinessLogic
open System
open Xamarin.Forms
open Xamarin.Forms.Xaml
 
type PinEntryPage() = 
    inherit ContentPage()

    let businessManager = BusinessManager.Instance()

    // Setup and reference UI components
    let _ = base.LoadFromXaml(typeof<PinEntryPage>)
    let pinEntry = base.FindByName<Entry>("pinEntry")
    let okButton = base.FindByName<Button>("okButton")

    // Handle change of pin entry content.
    member this.onPinEntryTextChanged(sender:Object, args:EventArgs) = 
        okButton.IsEnabled <- String.length(pinEntry.Text) > 0

    // Handle click of 'Ok' button.
    member this.onOkButtonClicked(sender:Object, args:EventArgs) = 
        businessManager.gotPinFromUser(pinEntry.Text)

    // Handle click of 'Cancel' button.
    member this.onCancelButtonClicked(sender:Object, args:EventArgs) = 
        businessManager.cancelAuthentication()
