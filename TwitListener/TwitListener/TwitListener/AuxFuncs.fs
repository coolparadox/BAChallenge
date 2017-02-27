module AuxFuncs

// Workaround for List.take error:
// "System.MissingMethodException: Method Microsoft.FSharp.Collections not found"
let rec take n list =
    match (n, list) with
    | (_, []) -> []
    | (0, _) -> []
    | (n, x::xs) -> x :: take (n-1) xs

