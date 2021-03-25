# Database Integration
This program demonstrates the usage of a Repository layer to insert and read data from a finance context.

It was developed in F# using:
- The SAFE stack for the backend (the web frontend is not used) 
- EntityFramework to handle database access and Migrations
- PostgreSQL database

## How the project is organized

Following the SAFE stack, it has three projects (Client, Server and Shared), but the Client is not being used here. I kept only the minimum code to keep it running. A console App is also provided to help test the application. 

Code is organized as follows:
- The Shared project contains the API's definition and  data types
- The Server project contains:
	- The API's implementation (for create, get and getAll endpoints)
	- The Repository that handles access to the database
	- The migrations file to init the database
- The Demo project contains a script to insert random items into the database and query items (all items or the items that match a criteria defined by the user)

## How to run the server

- Clone this repository to your machine
- Set the database connection string:
	- For the sake of simplicity, it's defined as a string at **DataAccess.fs**
	- Fill the **connectionString** with your database info (it could be from a free provider, like www.elephantsql.com)
- Navigate to **src/Server** and run **dotnet ef database update** to run migrations and initialize the database
	- If this doesn't work, maybe you need to install the EntityFramework tools **dotnet tool install --global dotnet-ef**
- Before running the project for the first time, run **dotnet tool restore** from the project's root directory
- From the project's root directory, run **dotnet fake build --target run** to start the server
- To ensure that everything is running, go to **http://localhost:8080** on a web browser. It should display an empty page with a random image as background

## How to run the demo App
- Navigate to **src/Demo** and run **dotnet run** to start the demo console App
- From the console App, input the desired option:
	- 1 - To populate the database with 10 random rows (you can do this multiple times)
	- 2 - To query the database with a given criteria, as follows:
		- Input the UserId, StartDate, EndDate, MinPrice, MaxPrice, MinQuantity, MaxQuantity, Provider list and Pair list
		- There's an example in brackets **[like this]** of the expect input for each field
		- Each of these values will be used as a filter on the database query
		- To ignore a filter (for example, to get rows for all users), just type 0 or an empty string as indicated in the input prompt
		- **Important: there's not much validations being performed here, so if an invalid input is inserted, the application will stop**
	- 3 - To list all rows currently in the database
	- Anything else to quit