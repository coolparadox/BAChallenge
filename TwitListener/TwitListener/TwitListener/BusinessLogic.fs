// Business Logic Layer
namespace BusinessLogic

open AuxFuncs
open BusinessLogic.Types
open Xamarin.Forms

// BusinessManager singleton class
type BusinessManager private () =

    // State variables of our domain model.
    let mutable mState = ApplicationState.Initial
    let mutable mTweets = List.empty<Tweet>

    // Export BusinessManager.Instance
    static let instance = BusinessManager()
    static member Instance = instance

    // Change application state.
    member private this.setApplicationState(state:ApplicationState) =
        if state <> mState then
            mState <- state
            // Notify subscribers that application state was changed.
            MessagingCenter.Send<BusinessManager, ApplicationState> (this, "onApplicationStateChanged", mState);

    // Add a new tweet.
    member this.addTweet(tweet:Tweet) =
        // Prepend new tweet to the list, protecting against infinite growth.
        mTweets <- tweet :: take 500 mTweets
        // Notify subscribers that tweet list was changed.
        MessagingCenter.Send<BusinessManager, Tweet list> (this, "onTweetListChanged", mTweets);
