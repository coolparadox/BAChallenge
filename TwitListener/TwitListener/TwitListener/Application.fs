namespace TwitListener

open BusinessLogic
open BusinessLogic.Types
open Xamarin.Forms

type App() = 

    inherit Application(MainPage = NavigationPage(MainPage()))

    let appManager = ApplicationManager.Instance()

    let myNav = base.MainPage :?> NavigationPage
    let mainPage = myNav.CurrentPage :?> MainPage
    let pinEntryPage = PinEntryPage()

    // Handle Pin request from ApplicationManager
    member this.getPinFromUser() =
        myNav.PushAsync(pinEntryPage) |> ignore

    // Handle application state changes
    member this.OnApplicationStateChanged(state:ApplicationState) =
        if state <> ApplicationState.Authenticating then
            if myNav.CurrentPage.GetType() = typeof<PinEntryPage> then
                myNav.PopToRootAsync() |> ignore

    // Handle removal of pages from navigation stack.
    member this.OnPopped(nav:NavigationPage, args:NavigationEventArgs) =
        System.Diagnostics.Debug.WriteLine("onPopped()")
        if args.Page.GetType() = typeof<PinEntryPage> then
            // Pin entry dialog has just returned.
            let pinEntryPage = args.Page :?> PinEntryPage
            match pinEntryPage.GetPin() with
                | None ->
                    // No pin was provided.
                    appManager.CancelAuthentication()
                | Some pin ->
                    // User entered a pin.
                    appManager.GotPinFromUser(pin)

    // Configure app and navigation page
    member this.SetupApp() =

        // Subscribe to Pin requests from ApplicationManager
        MessagingCenter.Unsubscribe<ApplicationManager>(this, "getPinFromUser")
        MessagingCenter.Subscribe<ApplicationManager>(this, "getPinFromUser", (fun _ -> this.getPinFromUser()))

        // Subscribe to application state changes from ApplicationManager
        MessagingCenter.Unsubscribe<ApplicationManager, ApplicationState>(this, "onApplicationStateChanged")
        MessagingCenter.Subscribe<ApplicationManager, ApplicationState>(this, "onApplicationStateChanged",
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
        appManager.ApplicationRecover()
        if appManager.IsAuthenticating() then
            appManager.CancelAuthentication()

    // Handle application sleep
    override this.OnSleep() =
        System.Diagnostics.Debug.WriteLine("OnSleep()");
        base.OnSleep()
        appManager.ApplicationSleep()

    // Handle application resume
    override this.OnResume() =
        System.Diagnostics.Debug.WriteLine("OnResume()");
        base.OnResume()
        this.SetupApp()
        appManager.ApplicationRecover()
        if appManager.IsAuthenticating() then
            System.Diagnostics.Debug.WriteLine(sprintf "--> we were authenticating; request PIN again")
            this.getPinFromUser()
