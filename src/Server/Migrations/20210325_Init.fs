module _20210325_Init

open System
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Migrations
open Microsoft.EntityFrameworkCore.Infrastructure
open Microsoft.EntityFrameworkCore.Migrations.Operations.Builders
open Microsoft.EntityFrameworkCore.Migrations.Operations

open DataAccess

type TransactionsTable = 
    { Id: OperationBuilder<AddColumnOperation>
      UserId: OperationBuilder<AddColumnOperation>
      DateTime: OperationBuilder<AddColumnOperation>
      Pair: OperationBuilder<AddColumnOperation>
      Price: OperationBuilder<AddColumnOperation>
      Quantity: OperationBuilder<AddColumnOperation>
      Provider: OperationBuilder<AddColumnOperation>}

[<DbContext(typeof<FinanceContext>)>]
[<Migration("20210325_Init")>]
type Init() =
    inherit Migration()

    override this.Up(migrationBuilder: MigrationBuilder) =
        migrationBuilder.CreateTable(
            name = "Transactions",
            columns = 
                (fun table -> 
                    { Id = table.Column<Guid>(nullable = false)
                      UserId = table.Column<uint>(nullable = false)
                      DateTime = table.Column<DateTime>(nullable = false)
                      Pair = table.Column<string>(nullable = false)
                      Price = table.Column<double>(nullable = false)
                      Quantity = table.Column<uint>(nullable = false)
                      Provider = table.Column<string>(nullable = false) }),
            constraints = 
               fun table -> 
                   table.PrimaryKey("PK_Transaction", (fun x -> x.Id :> obj))|> ignore ) |> ignore

    override this.Down(migrationBuilder: MigrationBuilder) = 
        migrationBuilder.DropTable(name = "Transactions") |> ignore

    override this.BuildTargetModel(modelBuilder: ModelBuilder) =
        modelBuilder
            .HasAnnotation("ProductVersion", "1.0.1")
            |> ignore

        modelBuilder.Entity("Shared.Transaction", 
            fun b ->
                b.Property<Guid>("Id") |> ignore
                b.Property<uint>("UserId") |> ignore
                b.Property<DateTime>("DateTime") |> ignore
                b.Property<string>("Pair") |> ignore
                b.Property<double>("Price") |> ignore
                b.Property<uint>("Quantity") |> ignore
                b.Property<string>("Provider") |> ignore
                b.HasKey("Id") |> ignore
                b.ToTable("Transactions") |> ignore
        ) |> ignore