# Bank System

A simple, straightforward ASP.NET Core MVC Bank Management System built on .NET 10.

## Tech Stack
- **Framework:** ASP.NET Core MVC (.NET 10 LTS)
- **Database:** Entity Framework Core 10 (Code-First) & SQL Server (LocalDB)
- **Authentication:** ASP.NET Core Identity
- **Architecture:** Controller-heavy minimal architecture, no repository/service layers.

## Features
- **Customers & Accounts:** Create user profiles and manage bank accounts.
- **Transactions:** Deposit, Withdraw, and Transfer money between accounts securely (using DB transactions).
- **Loans & Cards:** Apply for loans, manage card details.
- **Admin Dashboard:** Role-based access control for administrative tasks.
- **Audit Logging:** Secure logging of all sensitive actions.

## Setup Instructions
1. Install [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Open the solution in Visual Studio or your preferred IDE.
3. Open a terminal in the `BankSystem.Web` directory and install dependencies:
   ```bash
   dotnet restore
   ```
4. Apply Entity Framework Migrations to create the LocalDB database:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

*(Roles "Admin" and "Customer" are automatically seeded on startup)*
