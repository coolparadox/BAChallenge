namespace Model

open System

// A single tweet
type Tweet = {
    Who : string
    When : System.DateTime
    What : string
} 

// The Model
type Model private () =

    // State variables 
    // List of tweets
    let mutable tweets = List.empty<Tweet>

    // Export model as a singleton; access it by Model.Instance  
    static let instance = Model()
    static member Instance = instance

    // List of tweets
    member this.Tweets = tweets

    // Prepend a new tweet to the list of tweets
    member this.AddTweet(tweet:Tweet) = 
        tweets <- tweet :: tweets
  