# Banking Transaction System

A full-stack, production-ready Banking Transaction System built with ASP.NET Core MVC (C#) and SQL Server. This project is a comprehensive educational resource demonstrating essential web development and database management principles.

## Features
- **User Authentication**: Secure Login and Sign Up using ADO.NET and Stored Procedures.
- **Role-Based Access**: Specialized views for 'Admin' and 'Customer' users.
- **Database Integration**: Fully connected UI that interacts with an underlying SQL Server database for authentic persistence.
- **Premium UI**: Polished, modern web design for an elevated user experience.

## Database Setup
1. Open SQL Server Management Studio (SSMS).
2. Connect to your local server.
3. Open and execute the `BankingDatabase_Simplified.sql` script provided in the repository to create the database, tables, triggers, and stored procedures, and insert sample data.

## Project Configuration
Make sure to check the `appsettings.json` file to ensure the `DefaultConnection` string correctly points to your local SQL Server instance.



## How to Run
1. Navigate to the project directory.
2. Run `dotnet restore` to install missing packages (e.g., `Microsoft.Data.SqlClient`).
3. Run `dotnet run` to start the application.
4. Access the web interface via the browser to test the newly integrated database connections.

## Week 2 Task Completion
- Complete SQL script for tables, primary/foreign keys, and constraints included.
- Stored Procedures and Triggers implemented.
- Database document attached: `Week2_Database_Design.md`.
- Database properly connected to UI login and signup screens.
- Rebranded login/signup screens with the main project title.