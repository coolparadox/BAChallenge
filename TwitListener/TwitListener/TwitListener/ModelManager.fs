// Manager of the domain model.
// This is the interface to this application domain model.
namespace ModelManager

open Xamarin.Forms

// ModelManager singleton class
type ModelManager private () =

    // Export this class as ModelManager.Instance
    static let instance = ModelManager()
    static member Instance = instance

    // Change application state.
    member this.SetApplicationState(state:ModelTypes.State) =
        if state <> Model.state then
            Model.state <- state
            // Notify subscribers that application state was changed.
            MessagingCenter.Send<ModelManager, ModelTypes.State> (this, "onApplicationStateChanged", Model.state);

    // Add a new tweet.
    member this.addTweet(tweet:ModelTypes.Tweet) =
        // Prepend new tweet to the list, protecting against infinite growth.
        Model.tweets <- tweet :: AuxFuncs.take 500 Model.tweets
        // Notify subscribers that tweet list was changed.
        MessagingCenter.Send<ModelManager, ModelTypes.Tweet list> (this, "onTweetListChanged", Model.tweets);
