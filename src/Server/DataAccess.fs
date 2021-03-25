module DataAccess

open Microsoft.EntityFrameworkCore
open System.Linq

open Shared
open System

/// Postgres connection string
let connectionString = "ENTER_CONNECTION_STRING_HERE"

/// Defines the EntityFramework DbContext to handle conections with the database
type FinanceContext =
    inherit DbContext

    new() = { inherit DbContext() }
    new(options: DbContextOptions<FinanceContext>) = { inherit DbContext(options) }

    override __.OnConfiguring optionsBuilder =
        if optionsBuilder.IsConfigured <> true then
            optionsBuilder.UseNpgsql(connectionString) |> ignore

    [<DefaultValue>]
    val mutable transactions:DbSet<Transaction>
    member public this.Transactions
        with get() = this.transactions
        and set value = this.transactions <- value


/// Defines a repository layer with Add, Get and GetAll methods
/// This is used to access the database and make changes / queries to it
type FinanceRepository(context : FinanceContext) =
    let dbContext = context

    /// Adds a new item to the database
    member this.Add(transaction: Transaction) =
        dbContext.Transactions.Add(transaction) |> ignore
        dbContext.SaveChanges() |> ignore
        transaction

    /// Gets itens from the database based on the 'query' criteria
    member this.Get(query: TransactionQueryDTO) =
        let result = dbContext.Transactions.Where(fun transaction ->
            (  (query.UserId = 0 || query.UserId = transaction.UserId)
            && (query.MinQuantity = 0 || transaction.Quantity >= query.MinQuantity)
            && (query.MaxQuantity = 0 || transaction.Quantity <= query.MaxQuantity)
            && (query.MinPrice = 0.0 || transaction.Price >= query.MinPrice)
            && (query.MaxPrice = 0.0 || transaction.Price <= query.MaxPrice)
            && (query.StartDate = DateTime.MinValue || transaction.DateTime >= query.StartDate)
            && (query.EndDate = DateTime.MinValue || transaction.DateTime <= query.EndDate)
            && (query.Providers.IsEmpty || query.Providers.Contains(transaction.Provider))
            && (query.Pairs.IsEmpty || query.Pairs.Contains(transaction.Pair)) )).OrderBy(fun x -> x.DateTime)
        List.ofSeq result

    /// Gets all items from the database        
    member this.GetAll () =
        List.ofSeq (dbContext.Transactions.OrderBy(fun x -> x.DateTime))

    