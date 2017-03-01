namespace TwitListener

open BusinessLogic
open BusinessLogic.Types
open Xamarin.Forms

type App() = 

    inherit Application(MainPage = NavigationPage(MainPage()))

    let myNav = base.MainPage :?> NavigationPage
    let mainPage = myNav.CurrentPage :?> MainPage
    let pinEntryPage = PinEntryPage()

    // Handle Pin request from BusinessManager
    member this.getPinFromUser() =
        myNav.PushAsync(pinEntryPage) |> ignore

    // Handle application state changes
    member this.OnApplicationStateChanged(state:ApplicationState) =
        if state <> ApplicationState.Authenticating then
            if myNav.CurrentPage.GetType() = typeof<PinEntryPage> then
                myNav.PopToRootAsync() |> ignore

    member this.OnPopped(nav:NavigationPage, args:NavigationEventArgs) =
        System.Diagnostics.Debug.WriteLine("onPopped()")
        if args.Page.GetType() = typeof<PinEntryPage> then
            let pinEntryPage = args.Page :?> PinEntryPage
            match pinEntryPage.GetPin() with
                | None ->
                    BusinessManager.Instance().CancelAuthentication()
                | Some pin ->
                    BusinessManager.Instance().GotPinFromUser(pin)

    // Configure app and navigation page
    member this.SetupApp() =

        //// Customise navigation page.
        //MyNavPage.SetHasNavigationBar(myNav, false)
        //MyNavPage.SetHasBackButton(pinEntryPage, false)

        // Subscribe to Pin requests from BusinessManager
        MessagingCenter.Unsubscribe<BusinessManager>(this, "getPinFromUser")
        MessagingCenter.Subscribe<BusinessManager>(this, "getPinFromUser", (fun _ -> this.getPinFromUser()))

        // Subscribe to application state changes from BusinessManager
        MessagingCenter.Unsubscribe<BusinessManager, ApplicationState>(this, "onApplicationStateChanged")
        MessagingCenter.Subscribe<BusinessManager, ApplicationState>(this, "onApplicationStateChanged",
            fun _ state -> this.OnApplicationStateChanged(state)
        )

        // Subscribe main content page to changes in domain model.
        mainPage.SubscribeToModelUpdates()

    // Handle application start
    override this.OnStart() =
        System.Diagnostics.Debug.WriteLine("OnStart()");
        base.OnStart()
        myNav.Popped.Add(fun args -> this.OnPopped(myNav, args))
        this.SetupApp()

    // Handle application sleep
    override this.OnSleep() =
        System.Diagnostics.Debug.WriteLine("OnSleep()");
        base.OnSleep()
        BusinessManager.Instance().SaveState()

    // Handle application resume
    override this.OnResume() =
        System.Diagnostics.Debug.WriteLine("OnResume()");
        base.OnResume()
        this.SetupApp()
        BusinessManager.Instance().LoadState()
        if BusinessManager.Instance().IsAuthenticating() then
            System.Diagnostics.Debug.WriteLine(sprintf "--> we were authenticating; request PIN again")
            this.getPinFromUser()
