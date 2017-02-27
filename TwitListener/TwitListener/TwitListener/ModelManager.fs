// Manager of the domain model.
// Always update the domain model through ModelManager class only.
namespace ModelManager

open Xamarin.Forms

// ModelManager singleton class
type ModelManager private () =

    // Export this class as ModelManager.Instance
    static let instance = ModelManager()
    static member Instance = instance

    // Add a new tweet to the Model and notify listeners.
    member this.addTweet(tweet:ModelTypes.Tweet) =
        Model.tweets <- tweet :: Model.tweets
        MessagingCenter.Send<ModelManager, ModelTypes.Tweet list> (this, "onTweetListChanged", Model.tweets);
