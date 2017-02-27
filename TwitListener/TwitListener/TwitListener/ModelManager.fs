namespace ModelManager

open Model
open Xamarin.Forms

// ModelManager singleton
type ModelManager private () =

    // Export this class as ModelManager.Instance
    static let instance = ModelManager()
    static member Instance = instance

    // Add a new tweet to the Model and notify listeners.
    member this.AddTweet(tweet:Model.Tweet) =
        Model.Instance.Tweets <- tweet :: Model.Instance.Tweets
        MessagingCenter.Send<ModelManager, Model.Tweet list> (this, "onTweetListChanged", Model.Instance.Tweets);
