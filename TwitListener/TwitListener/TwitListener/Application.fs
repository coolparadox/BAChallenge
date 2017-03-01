namespace TwitListener

open BusinessLogic
open BusinessLogic.Types
open Xamarin.Forms

type App() = 

    inherit Application(MainPage = NavigationPage(MainPage()))

    let navigationPage = base.MainPage :?> NavigationPage
    let twitListenerPage = navigationPage.CurrentPage :?> MainPage
    let pinEntryPage = PinEntryPage()

    // Handle Pin request from BusinessManager
    member this.getPinFromUser() =
        navigationPage.PushAsync(pinEntryPage) |> ignore

    // Handle application state changes
    member this.OnApplicationStateChanged(state:ApplicationState) =
        if state <> ApplicationState.Authenticating then
            if navigationPage.CurrentPage.GetType() = typeof<PinEntryPage> then
                navigationPage.PopToRootAsync() |> ignore

    // Handle application start
    override this.OnStart() =
        System.Diagnostics.Debug.WriteLine("OnStart()");
        base.OnStart()

        // Customise navigation page.
        NavigationPage.SetHasNavigationBar(navigationPage, false)
        NavigationPage.SetHasBackButton(pinEntryPage, false)

        // Subscribe to Pin requests from BusinessManager
        MessagingCenter.Subscribe<BusinessManager> (this, "getPinFromUser", (fun _ -> this.getPinFromUser()))

        // Subscribe to application state changes from BusinessManager
        MessagingCenter.Subscribe<BusinessManager, ApplicationState> (this, "onApplicationStateChanged",
            fun _ state -> this.OnApplicationStateChanged(state)
        )

        // Subscribe main content page to changes in domain model.
        twitListenerPage.SubscribeToModelUpdates()

    // Handle application sleep
    override this.OnSleep() =
        System.Diagnostics.Debug.WriteLine("OnSleep()");
        BusinessManager.Instance().SaveState()
        base.OnSleep()

    // Handle application resume
    override this.OnResume() =
        System.Diagnostics.Debug.WriteLine("OnResume()");
        BusinessManager.Instance().LoadState()
        base.OnResume()
