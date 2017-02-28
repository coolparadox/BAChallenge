// Types used by our domain model.
module ModelTypes

// Application states
type State =
    | Initial = 0
    | Authenticated = 1

// A single tweet
type Tweet = {
    Who : string
    When : System.DateTime
    What : string
} 
