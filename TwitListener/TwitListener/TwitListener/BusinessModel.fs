module Model

open BusinessLogic.Types

// State variables of this application.
// Don't access them directly; use BusinessManager instead.

// Application state
let mutable applicationState = ApplicationState.Initial

// List of tweets
let mutable tweets = List.empty<Tweet>
