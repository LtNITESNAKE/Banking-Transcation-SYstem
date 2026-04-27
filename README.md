<div align="center">
  
# 🏦 Banking Transaction System
  
**A Full-Stack Enterprise-Level Web Application built with ASP.NET Core MVC & Microsoft SQL Server.**

[![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![Status](https://img.shields.io/badge/Status-Live_on_Somee.com-23C16B?style=for-the-badge)](#)

[**Live Demo (Mobile Responsive)**](http://Banking-Transcation-System.somee.com) • [**Database Documentation**](Week2_Database_Design.md)

</div>

---

## 📖 About the Project
This project was developed for the **Advanced Database Management Systems (ADBMS)** course. It simulates a highly secure, real-world banking ledger. 

Unlike standard CRUD applications, this system enforces **Double-Entry Bookkeeping**, **Soft-Deletions**, and an **Immutable Audit Trail** strictly at the database level using T-SQL Triggers and Stored Procedures. The front-end is crafted with a modern, glass-morphism aesthetic ensuring a premium user experience across all devices.

## ✨ Key Features

### 👨‍💻 For Customers
- **Live Interactive Dashboard**: View real-time account balances and total transaction counts.
- **Secure Fund Transfers**: Send money using live AJAX account-validation (prevents sending to invalid/incorrect names).
- **Transaction History**: View personal immutable ledger logs (Deposits, Withdrawals, Transfers).
- **Responsive Design**: Flawless UI scaling from desktop laptops to mobile phones.

### 🛡️ For Administrators
- **Total Visibility**: Track all capital moving through the bank.
- **Account Management**: Create new customers securely.
- **Soft-Deletion Protocol**: Deleting a user safely revokes their login access without hard-deleting legal financial records.

### 💾 Advanced Database Architecture
- **Stored Procedures**: No inline SQL. Every action (`sp_TransferMoney`, `sp_CreateUser`) runs safely via atomic transactions.
- **SQL Security Triggers**: 
  - `tr_PreventTransactionDeletion`: System actively blocks any `DELETE` attempts on the transaction history.
  - `tr_ValidateTransactionAmount`: Prevents zero or negative transaction attempts.
- **Data Normalization**: 4 tightly coupled tables using Foreign Keys and Cascading safeguards.

---

## 🚀 Live Deployment

The project is actively hosted in the cloud. You can try it out here:  
👉 **[http://Banking-Transcation-System.somee.com](http://Banking-Transcation-System.somee.com)**

*(Note: Hosted on a free tier, so it may take 5 seconds to wake up on the first load!)*

---

## 🛠️ Local Setup Guide

If you wish to run this system locally on your own machine:

### 1. Database Setup
1. Open SQL Server Management Studio (SSMS).
2. Connect to your local SQL server (e.g., `YOUR-PC\SQLEXPRESS`).
3. Open and execute the `BankingDatabase_Simplified.sql` script provided in this repository.
   - *This will automatically create the database, tables, triggers, and stored procedures.*

### 2. Application Setup
1. Open the project in Visual Studio or VS Code.
2. Open `appsettings.json`.
3. Update the `DefaultConnection` string to point to your local SQL Server instance.
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=BankingSystem;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 3. Run the App
1. Open your terminal in the project directory.
2. Run `dotnet restore` to install dependencies.
3. Run `dotnet run` to launch the application.

---

## 📚 Academic Submissions
- **Week 2 Database Design & Logic**: [View the Document](Week2_Database_Design.md)

---
<div align="center">
  <i>Developed with ❤️ by Muhammad Mujtaba</i>
</div>
