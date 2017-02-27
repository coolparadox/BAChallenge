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
        // Prepend new tweet to the list, protecting against infinite growth.
        Model.tweets <- tweet :: AuxFuncs.take 500 Model.tweets
        // Notify subscribers that tweet list was changed.
        MessagingCenter.Send<ModelManager, ModelTypes.Tweet list> (this, "onTweetListChanged", Model.tweets);
