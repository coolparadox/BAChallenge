namespace Model

// A single tweet
type Tweet = {
    Who : string
    When : System.DateTime
    What : string
} 

// Model singleton
type Model private () =

    static let instance = Model()
    static member Instance = instance

    // List of tweets
    member val Tweets = List.empty<Tweet> with get,set
  