/// This program is used to test the API
/// To execute, from the 'Demo' folder, type 'dotnet run'

open System.Net.Http
open Newtonsoft.Json

open Shared
open System.Text
open Microsoft.FSharp.Collections
open System
open System.Collections.Generic

/// API's base URL and routes
let apiUrl = "http://127.0.0.1:8085/api/IFinanceApi"
let getAllTransactionsUrl = sprintf "%s/GetAllTransactions" apiUrl
let getTransactionsUrl = sprintf "%s/GetTransactions" apiUrl
let createTransactionUrl = sprintf "%s/CreateTransaction" apiUrl

let httpClient = new HttpClient()
let random = Random()

/// Used for the random generated data
let pairs = ["GBP/USD"; "GBP/EUR"; "USD/EUR"; "USD/GBP"; "EUR/USD"; "EUR/GBP"]
let providers = ["CBOE feed"; "CBOE vwap"; "UBS"]

/// Prints a Transaction item to the console
let printTransaction(transaction: Transaction) =
    printfn "{ Id= %s; UserId= %d; DateTime = %s; Pair= %s; Price= %f; Quantity= %d; Provider= %s }"
        (transaction.Id.ToString())
        transaction.UserId
        (transaction.DateTime.ToString())
        transaction.Pair
        transaction.Price
        transaction.Quantity
        transaction.Provider

/// Utility function to convert a string to date OR return MinValue if input string is empty
let getDateFromString(dateString: string) =
    if String.IsNullOrWhiteSpace dateString then
        DateTime.MinValue
    else
        DateTime.Parse(dateString)

/// Reads the query input to get data from the API (any value can be ignored)
/// This function doesn't perform validations, so invalid values should halt the program
let getQueryInput() =
    printf "Enter UserId [1] (0 to ignore): "
    let userId = Console.ReadLine() |> int
    printf "Enter StartDate [2020-02-04 21:00:00] (empty string to ignore): "
    let startDateSrt = Console.ReadLine()
    printf "Enter EndDate [2020-02-04 21:00:00] (empty string to ignore): "
    let endDateSrt = Console.ReadLine()
    printf "Enter MinPrice [1.0] (0 to ignore): "
    let minPrice = Console.ReadLine() |> double
    printf "Enter MaxPrice [100.0] (0 to ignore): "
    let maxPrice = Console.ReadLine() |> double
    printf "Enter MinQuantity [100] (0 to ignore): "
    let minQuantity = Console.ReadLine() |> int
    printf "Enter MaxQuantity [1000] (0 to ignore): "
    let maxQuantity = Console.ReadLine() |> int
        
    let providerList = new List<string>()
    let mutable isReadingProviders = true
    while isReadingProviders do
        printf "Enter Providers [CBOE] (empty string to ignore): "
        let provider = Console.ReadLine()
        if String.IsNullOrWhiteSpace provider then
            isReadingProviders <- false
        else
            providerList.Add(provider)

    let pairList = new List<string>()
    let mutable isReadingPairs = true
    while isReadingPairs do
        printf "Enter Pairs [GBP/USD] (empty string to ignore): "
        let pair = Console.ReadLine()
        if String.IsNullOrWhiteSpace pair then
            isReadingPairs <- false
        else
            providerList.Add(pair)

    let query =
        { UserId = userId;
          Pairs = List.ofSeq pairList;
          StartDate = getDateFromString(startDateSrt);
          EndDate = getDateFromString(endDateSrt);
          MinPrice = minPrice;
          MaxPrice = maxPrice;
          MinQuantity = minQuantity;
          MaxQuantity = maxQuantity;
          Providers = List.ofSeq providerList }
    query

/// Runs a GET request to the specified 'url' and returns the result
let getAsync(url: string) = 
    async {
        let! response = httpClient.GetAsync(url) |> Async.AwaitTask
        response.EnsureSuccessStatusCode () |> ignore
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content
    }

/// Runs a POST request to the specified 'url' with the json paramenter and returns the result
let postAsync(url: string, json: string) =
    async {
        let json = sprintf "[%s]" json
        printf "%s" json
        use content = new StringContent(json, Encoding.UTF8, "application/json")
        let! response = httpClient.PostAsync(url, content) |> Async.AwaitTask
        let! body = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return body
    }

/// Gets all the Transaction items from the database through the API and prints each to the console
let getAllTransactions() =
    async {        
        let! result =  getAsync(getAllTransactionsUrl)
        printfn "All transactions:"        
        let transactionsList = JsonConvert.DeserializeObject<List<Transaction>> (result)
        for transaction in transactionsList do
            printTransaction(transaction)
    }

/// Gets all the Transaction items from the database through the API that matches the query criteria and prints each to the console
let getTransactions (query: TransactionQueryDTO) =
    async {
        let! result = postAsync(getTransactionsUrl, JsonConvert.SerializeObject(query))
        printfn "Transactions:"  
        let transactionsList = JsonConvert.DeserializeObject<List<Transaction>> (result)
        for transaction in transactionsList do
            printTransaction(transaction)
    }

/// Adds a new Transaction item to the database
let createTransaction (transaction: TransactionCreateDTO) =
    async {        
        let! result = postAsync(createTransactionUrl, JsonConvert.SerializeObject(transaction))
        printfn "Created: %s" result
    }

/// Runs the getAllTransactions task synchronously
let queryAllTransactions() =
    getAllTransactions()
    |> Async.RunSynchronously

/// Reads user input for query criteria and runs the getTransactions task synchronously
let queryTransactions() =    
    let query = getQueryInput()
    getTransactions(query)
    |> Async.RunSynchronously  

/// Populates the database with 10 random Transaction items
let populateDatabase() =
    for i in 1 .. 10 do
        let userId = random.Next(1, 10)
        let price = random.NextDouble() * 10.0
        let quantity = random.Next(100, 10000)
        let dateTime = DateTime.Now
        let pair = pairs.Item (random.Next(pairs.Length))
        let provider = providers.Item (random.Next(providers.Length))
        let query = { UserId = userId; DateTime = dateTime; Pair = pair; Price = price; Quantity = quantity; Provider = provider }
        createTransaction query
        |> Async.RunSynchronously    

[<EntryPoint>]
let main argv =
    let mutable isRunning = true
    while isRunning do 
        printfn "\n------------------------------"
        printfn "1 - Populate database (add 10 random records)"
        printfn "2 - Query transactions (with filters)"
        printfn "3 - List all transactions"
        printfn "Anything else - Quit"
        printf "Select: "
        let input = Console.ReadLine()

        match input with
        | "1" -> populateDatabase()
        | "2" -> queryTransactions()
        | "3" -> queryAllTransactions()
        | _ -> isRunning <- false
    0
