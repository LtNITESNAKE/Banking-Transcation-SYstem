# Banking Transaction System - Simplified Database Setup Guide

## Overview
This is a **simplified, clean database** design that uses only what your ASP.NET Core MVC application actually needs. No overkill, no unnecessary complexity.

## Database Structure

### 4 Tables (That's it!)

#### 1. **Users** - Authentication & Account Info
```
- UserId (Primary Key)
- Username (Unique, Required)
- Password (Required)
- FullName (Required)
- Role (Admin or Customer)
- CreatedDate (Auto-timestamp)
- IsActive (1 or 0)
```

#### 2. **Customers** - Customer Profiles
```
- CustomerId (Primary Key)
- UserId (Foreign Key → Users)
- FirstName, LastName (Required)
- Email, PhoneNumber, Address (Optional)
- CreatedDate (Auto-timestamp)
```

#### 3. **Accounts** - Bank Accounts
```
- AccountId (Primary Key)
- CustomerId (Foreign Key → Customers)
- AccountNumber (Unique, e.g., "ACC-10023")
- AccountType (Savings or Checking)
- Balance (Decimal, tracks money)
- CreatedDate (Auto-timestamp)
- IsActive (1 or 0)
```

#### 4. **Transactions** - Transaction History
```
- TransactionId (Primary Key)
- AccountId (Foreign Key → Accounts)
- TransactionType (Deposit, Withdrawal, or Transfer)
- Amount (Decimal)
- ReceiverAccountId (Optional, for transfers)
- Remarks (Description)
- TransactionDate (Auto-timestamp)
```

## Stored Procedures (9 Essential Only)

| Procedure Name | Purpose | Used By |
|---|---|---|
| `sp_UserLogin` | Authenticate user | AccountController.Login() |
| `sp_CreateUser` | Register new customer | AccountController.SignUp() |
| `sp_CreateAccount` | Create new bank account | CustomerController.Create() |
| `sp_DepositMoney` | Deposit to account | TransactionController.Deposit() |
| `sp_WithdrawMoney` | Withdraw from account | TransactionController.Withdraw() |
| `sp_TransferMoney` | Transfer between accounts | TransactionController.Transfer() |
| `sp_GetTransactionHistory` | View transactions | TransactionController.History() |
| `sp_GetAllCustomers` | Admin view all customers | CustomerController.Index() |
| `sp_GetAccountsByCustomer` | Get customer's accounts | Dashboard |

## Setup Instructions

### Step 1: Open SQL Server Management Studio
1. Open SSMS
2. Connect to your SQL Server (usually `localhost` or `.`)

### Step 2: Run the SQL Script
1. File → Open → Select `BankingDatabase_Simplified.sql`
2. Click **Execute** or press F5
3. Wait for completion (should take 2-3 seconds)

### Step 3: Verify Installation
```sql
-- Run this to verify
SELECT * FROM BankingSystemDB.dbo.Users;
SELECT * FROM BankingSystemDB.dbo.Customers;
SELECT * FROM BankingSystemDB.dbo.Accounts;
SELECT * FROM BankingSystemDB.dbo.Transactions;
```

### Step 4: Test a Stored Procedure
```sql
-- Test login
EXEC sp_UserLogin @Username = 'Mujtaba', @Password = '1581810';

-- Test transaction history
EXEC sp_GetTransactionHistory @AccountId = 1;
```

## Sample Data Included

### Users (3 Customers + 1 Admin)
- **Admin**: Username `Mujtaba`, Password `1581810`
- **Customer 1**: Username `awais`, Password `1234` (Sheikh Awais)
- **Customer 2**: Username `hadian`, Password `1234` (Hadian Arshad)
- **Customer 3**: Username `ali`, Password `1234` (Ali Khan)

### Accounts
- 3 bank accounts with different balances

### Transactions
- 3 sample transactions (Deposit, Transfer, Withdrawal)

## Triggers Included (Academic Requirement)

1. **tr_UpdateAccountModifiedDate** - Automatically updates the `LastModified` timestamp whenever an account is modified
2. **tr_ValidateTransactionAmount** - Prevents invalid transactions by rejecting zero or negative amounts
3. **tr_PreventTransactionDeletion** - Protects audit trail by preventing deletion of any transaction records

## What Was REMOVED (vs. Complex Design)
❌ AuditLog table (unnecessary for coursework)
❌ Complex functions (balance calculations)
❌ Overdraft prevention triggers
❌ 13 procedures → simplified to 9 essential ones
❌ Admin user management procedures

## Total Complexity
- **Lines of Code**: ~450 (includes triggers)
- **Tables**: 4 (vs. 5 in complex)
- **Procedures**: 9
- **Functions**: 0
- **Triggers**: 3 ✅ (for academic requirement)

## Connection String (Already in appsettings.json)
```
Server=localhost;Database=BankingSystemDB;Trusted_Connection=true;TrustServerCertificate=true;
```

## Next Steps

### 1. Run the SQL Script
Execute `BankingDatabase_Simplified.sql` in SQL Server

### 2. Update Controllers to Use Database
Replace hardcoded logic with stored procedure calls

### 3. Add Data Access Layer
Create a simple data access class to call procedures

### 4. Test Everything
Run the application and verify login, signup, transactions work

## Troubleshooting

### Error: "Database 'BankingSystemDB' already exists"
The script drops and recreates the database automatically. If you still get errors:
```sql
-- Manually drop it first
DROP DATABASE BankingSystemDB;
```

### Error: "Login failed for user"
Make sure:
1. Connection string is correct in `appsettings.json`
2. SQL Server is running
3. Use `Trusted_Connection=true` for Windows authentication

### No sample data appearing
Scroll down in the script output - the `SELECT` statements at the end show all data was inserted correctly.

## Questions?
All procedures have clear names and parameters. Hover over any procedure name in SQL to see its definition.

---

**Created**: Simplified version focusing on essential functionality only  
**Target**: ASP.NET Core MVC Banking System  
**Database**: Microsoft SQL Server
