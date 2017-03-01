namespace ServiceAccess.Twitter.Types

// OAuth tokens
[<CLIMutable>]
type Credential = {
    key : string
    secret : string
}
