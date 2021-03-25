namespace Shared

open System

/// DTO for requesting data with a filter
type TransactionQueryDTO =
    { UserId: int
      Pairs: List<string>
      StartDate: DateTime
      EndDate: DateTime
      MinPrice: double
      MaxPrice: double
      MinQuantity: int
      MaxQuantity: int
      Providers: List<string> }

/// DTO for adding data to the database
type TransactionCreateDTO =
    { UserId: int
      DateTime: DateTime
      Pair: string 
      Price: double
      Quantity: int
      Provider: string }

/// Transaction object that is persisted to the database
[<CLIMutable>]
type Transaction =
    { Id: Guid
      UserId: int
      DateTime: DateTime
      Pair: string 
      Price: double
      Quantity: int
      Provider: string }

/// Module for validating and creating new Transaction objects
module Transaction =
    let isValid (dto: TransactionCreateDTO) =
        dto.Quantity >= 1
        && dto.Price > 0.0
        && String.IsNullOrEmpty(dto.Pair) |> not
        && String.IsNullOrEmpty(dto.Provider) |> not

    let create (transactionDTO: TransactionCreateDTO) =
        { Id = Guid.NewGuid()
          UserId = transactionDTO.UserId
          DateTime = transactionDTO.DateTime
          Pair = transactionDTO.Pair
          Price = transactionDTO.Price
          Quantity = transactionDTO.Quantity
          Provider = transactionDTO.Provider }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

/// Finance API interface
type IFinanceApi =
    { CreateTransaction: TransactionCreateDTO -> Transaction Async
      GetTransactions: TransactionQueryDTO -> Transaction list Async
      GetAllTransactions: unit -> Transaction list Async }