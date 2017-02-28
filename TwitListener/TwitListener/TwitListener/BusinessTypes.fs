namespace BusinessLogic.Types

// Application states
type ApplicationState =
    | Initial = 0
    | Authenticated = 1

// A single tweet
type Tweet = {
    Who : string
    When : System.DateTime
    What : string
} 
