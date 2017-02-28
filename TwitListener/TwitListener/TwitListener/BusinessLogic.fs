// Business Logic Layer
namespace BusinessLogic

open AuxFuncs
open BusinessLogic.Types
open Xamarin.Forms

// ModelManager singleton class
type BusinessManager private () =

    // Export this class as ModelManager.Instance
    static let instance = BusinessManager()
    static member Instance = instance

    // Change application state.
    member private this.setApplicationState(state:ApplicationState) =
        if state <> Model.applicationState then
            Model.applicationState <- state
            // Notify subscribers that application state was changed.
            MessagingCenter.Send<BusinessManager, ApplicationState> (this, "onApplicationStateChanged", Model.applicationState);

    // Add a new tweet.
    member this.addTweet(tweet:Tweet) =
        // Prepend new tweet to the list, protecting against infinite growth.
        Model.tweets <- tweet :: take 500 Model.tweets
        // Notify subscribers that tweet list was changed.
        MessagingCenter.Send<BusinessManager, Tweet list> (this, "onTweetListChanged", Model.tweets);
