// State variables of this application.
// Please change them through ModelManager only.
module Model

// Application state
let mutable state = ModelTypes.State.Initial

// List of tweets
let mutable tweets = List.empty<ModelTypes.Tweet>
