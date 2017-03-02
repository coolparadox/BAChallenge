namespace ServiceAccess.Twitter.Types

// OAuth tokens
[<CLIMutable>]
type Credential = {
    key : string
    secret : string
}

// A single tweet
type StrippedTweet = {

    Who : string
    When : System.DateTime
    What : string

}

(*
// Tweet listener interface.
type IStrippedTweetListener =

    // Stream API "track".
    abstract member Filter : string with get

    // Handler for notification of received tweets.
    abstract member OnTweetReceived : StrippedTweet -> unit

    // Handler for notification of stopping of listening.
    // A textual description about the event is sent ti the handler.
    abstract member OnStreamStopped : string -> unit
*)
