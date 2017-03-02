namespace BusinessLogic.Types

// Application states
type ApplicationState =
    | LoggedOff = 0
    | Authenticating = 1
    | Authenticated = 2
    | Listening = 3
