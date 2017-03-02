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
