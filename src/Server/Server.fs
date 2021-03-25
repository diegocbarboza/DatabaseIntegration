module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Microsoft.EntityFrameworkCore

open Shared
open DataAccess

/// EntityFramework DbContext used to access the database
let financeContext = new FinanceContext(new DbContextOptions<FinanceContext>())
/// Repository layer 
let financeRepository = FinanceRepository(financeContext)

// Finance API implementation to add a new Transaction to the database from a DTO
let createTransaction transactionDTO =
    async {
        if not (Transaction.isValid transactionDTO) then failwith "Invalid Transaction input."
        let transaction = Transaction.create transactionDTO
        return financeRepository.Add transaction }

// Finance API implementation to get a list of Transactions from the database using the DTO as a query filter
let getTransactions queryDTO =
    async {
        let transactions = financeRepository.Get(queryDTO)
        return transactions }

// Finance API implementation to get a list of all Transactions from the database
let getAllTransactions _ =
    async {
        let transactions = financeRepository.GetAll()
        return transactions }

/// Define the methods that will respond to each API call
let financeApi =
    { CreateTransaction = createTransaction
      GetTransactions = getTransactions
      GetAllTransactions = getAllTransactions }

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder    
    |> Remoting.fromValue financeApi
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip }

run app
