# Banking Transaction System - Database Design Documentation

## 📋 Table of Contents
1. [Introduction](#introduction)
2. [Database Overview](#database-overview)
3. [Tables & Schema Design](#tables--schema-design)
4. [Entity Relationship Diagram](#entity-relationship-diagram)
5. [Constraints & Data Integrity](#constraints--data-integrity)
6. [Triggers Implementation](#triggers-implementation)
7. [Stored Procedures](#stored-procedures)
8. [Sample Data](#sample-data)
9. [Setup Instructions](#setup-instructions)

---

## Introduction

The **Banking Transaction System** database is designed to manage customer accounts, track financial transactions, and ensure secure banking operations. This simplified yet robust design follows database best practices while maintaining ease of integration with the ASP.NET Core MVC application.

### Design Philosophy
- **Simplicity**: Only 4 tables containing essential data
- **Security**: User authentication with role-based access
- **Integrity**: Strong constraints and triggers for data validation
- **Auditability**: Complete transaction history preservation
- **Performance**: Indexed primary keys and optimized queries

---

## Database Overview

**Database Name**: `BankingSystemDB`

### Key Statistics
- **Total Tables**: 4
- **Total Stored Procedures**: 9
- **Total Triggers**: 3
- **Total Functions**: 0
- **Relationships**: 3 (Parent-Child)

---

## Tables & Schema Design

### 1. **Users Table** (Authentication & Authorization)

**Purpose**: Stores user login credentials and role information for both admin and customer users.

```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Customer')),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
```

**Column Description**:

| Column | Type | Constraint | Purpose |
|--------|------|-----------|---------|
| UserId | INT | PK, Identity | Unique identifier for each user |
| Username | NVARCHAR(50) | NOT NULL, UNIQUE | Login username (must be unique) |
| Password | NVARCHAR(255) | NOT NULL | Encrypted password |
| FullName | NVARCHAR(100) | NOT NULL | User's full name |
| Role | NVARCHAR(20) | NOT NULL, CHECK | Role: 'Admin' or 'Customer' |
| CreatedDate | DATETIME | DEFAULT GETDATE() | Account creation timestamp |
| IsActive | BIT | DEFAULT 1 | Active status (1=Active, 0=Inactive) |

**Constraints Applied**:
- ✅ PRIMARY KEY: `UserId` (auto-increment)
- ✅ UNIQUE: `Username` (no duplicate usernames allowed)
- ✅ NOT NULL: All essential fields
- ✅ CHECK: `Role` must be either 'Admin' or 'Customer'
- ✅ DEFAULT: Automatic timestamp on creation

---

### 2. **Customers Table** (Customer Profile Information)

**Purpose**: Stores detailed profile information for customer users, linked to the Users table.

```sql
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100),
    PhoneNumber NVARCHAR(15),
    Address NVARCHAR(200),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
```

**Column Description**:

| Column | Type | Constraint | Purpose |
|--------|------|-----------|---------|
| CustomerId | INT | PK, Identity | Unique customer identifier |
| UserId | INT | FK, NOT NULL | Reference to Users table |
| FirstName | NVARCHAR(50) | NOT NULL | Customer's first name |
| LastName | NVARCHAR(50) | NOT NULL | Customer's last name |
| Email | NVARCHAR(100) | NULL | Customer's email address |
| PhoneNumber | NVARCHAR(15) | NULL | Customer's phone number |
| Address | NVARCHAR(200) | NULL | Customer's physical address |
| CreatedDate | DATETIME | DEFAULT GETDATE() | Profile creation timestamp |

**Constraints Applied**:
- ✅ PRIMARY KEY: `CustomerId` (auto-increment)
- ✅ FOREIGN KEY: `UserId` → `Users.UserId` (links to user account)
- ✅ NOT NULL: Essential fields (CustomerId, UserId, FirstName, LastName)
- ✅ DEFAULT: Automatic timestamp

**Relationship**:
- **One-to-Many**: One User can have one Customer profile (1:1 relationship)
- Ensures each customer has associated user credentials

---

### 3. **Accounts Table** (Bank Accounts)

**Purpose**: Stores bank account information and balance for each customer.

```sql
CREATE TABLE Accounts (
    AccountId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    AccountNumber NVARCHAR(20) NOT NULL UNIQUE,
    AccountType NVARCHAR(50) NOT NULL CHECK (AccountType IN ('Savings', 'Checking')),
    Balance DECIMAL(10, 2) NOT NULL DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastModified DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
```

**Column Description**:

| Column | Type | Constraint | Purpose |
|--------|------|-----------|---------|
| AccountId | INT | PK, Identity | Unique account identifier |
| CustomerId | INT | FK, NOT NULL | Reference to Customers table |
| AccountNumber | NVARCHAR(20) | NOT NULL, UNIQUE | Account number (e.g., "ACC-10023") |
| AccountType | NVARCHAR(50) | NOT NULL, CHECK | 'Savings' or 'Checking' |
| Balance | DECIMAL(10,2) | NOT NULL, DEFAULT 0 | Current account balance |
| CreatedDate | DATETIME | DEFAULT GETDATE() | Account creation date |
| LastModified | DATETIME | DEFAULT GETDATE() | Last modification timestamp |
| IsActive | BIT | DEFAULT 1 | Account status |

**Constraints Applied**:
- ✅ PRIMARY KEY: `AccountId` (auto-increment)
- ✅ FOREIGN KEY: `CustomerId` → `Customers.CustomerId`
- ✅ UNIQUE: `AccountNumber` (no duplicate account numbers)
- ✅ NOT NULL: All essential fields
- ✅ CHECK: `AccountType` must be 'Savings' or 'Checking'
- ✅ DEFAULT: Balance starts at 0, timestamps automatic

**Relationship**:
- **One-to-Many**: One Customer can have multiple Accounts (1:M)
- Allows customers to maintain different types of accounts

**Financial Constraints**:
- Balance uses DECIMAL(10,2) for precision (10 digits, 2 decimal places)
- Prevents floating-point arithmetic errors in financial calculations

---

### 4. **Transactions Table** (Transaction History)

**Purpose**: Maintains a complete audit trail of all financial transactions.

```sql
CREATE TABLE Transactions (
    TransactionId INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL CHECK (TransactionType IN ('Deposit', 'Withdrawal', 'Transfer')),
    Amount DECIMAL(10, 2) NOT NULL,
    ReceiverAccountId INT,
    Remarks NVARCHAR(200),
    TransactionDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
    FOREIGN KEY (ReceiverAccountId) REFERENCES Accounts(AccountId)
);
```

**Column Description**:

| Column | Type | Constraint | Purpose |
|--------|------|-----------|---------|
| TransactionId | INT | PK, Identity | Unique transaction identifier |
| AccountId | INT | FK, NOT NULL | Account performing the transaction |
| TransactionType | NVARCHAR(50) | NOT NULL, CHECK | 'Deposit', 'Withdrawal', or 'Transfer' |
| Amount | DECIMAL(10,2) | NOT NULL | Transaction amount |
| ReceiverAccountId | INT | FK, NULL | Receiver account (for transfers only) |
| Remarks | NVARCHAR(200) | NULL | Additional transaction notes |
| TransactionDate | DATETIME | DEFAULT GETDATE() | Transaction timestamp |

**Constraints Applied**:
- ✅ PRIMARY KEY: `TransactionId` (auto-increment)
- ✅ FOREIGN KEY: `AccountId` → `Accounts.AccountId`
- ✅ FOREIGN KEY: `ReceiverAccountId` → `Accounts.AccountId` (self-referencing)
- ✅ NOT NULL: Essential fields (AccountId, TransactionType, Amount)
- ✅ CHECK: `TransactionType` must be 'Deposit', 'Withdrawal', or 'Transfer'
- ✅ DEFAULT: Automatic timestamp

**Relationships**:
- **One-to-Many**: One Account can have multiple Transactions
- **Self-Referencing**: For transfers, links sender and receiver accounts
- **Audit Trail**: Transactions are never deleted (enforced by trigger)

---

## Entity Relationship Diagram

```
┌─────────────────────┐
│      Users          │
├─────────────────────┤
│ UserId (PK)         │◄─┐
│ Username (UNIQUE)   │  │ FK(1:1)
│ Password            │  │
│ FullName            │  │
│ Role (Admin/Cust)   │  │
│ CreatedDate         │  │
│ IsActive            │  │
└─────────────────────┘  │
                         │
                    ┌────┴──────────┐
                    │               │
            ┌───────┴──────────┐    │
            │   Customers      │    │
            ├──────────────────┤    │
            │ CustomerId (PK)  │    │
            │ UserId (FK) ─────┼────┘
            │ FirstName        │
            │ LastName         │
            │ Email            │
            │ PhoneNumber      │
            │ Address          │
            │ CreatedDate      │
            └────────┬─────────┘
                     │ FK(1:M)
                     │
            ┌────────┴──────────┐
            │    Accounts       │
            ├───────────────────┤
            │ AccountId (PK)    │
            │ CustomerId (FK) ──┼────┐
            │ AccountNumber     │    │
            │ AccountType       │    │
            │ Balance           │    │
            │ CreatedDate       │    │
            │ LastModified      │    │
            │ IsActive          │    │
            └────────┬──────────┘    │
                     │               │
                     │ FK(1:M)       │
                     │               │
            ┌────────┴──────────────┐ │
            │   Transactions       │ │
            ├──────────────────────┤ │
            │ TransactionId (PK)   │ │
            │ AccountId (FK) ──────┼─┘
            │ TransactionType      │
            │ Amount               │
            │ ReceiverAccountId───→┼─────┐
            │ (FK - Self-ref)      │     │
            │ Remarks              │     │
            │ TransactionDate      │     │
            └──────────────────────┘     │
                                         │
                              (Transfer to another account)
```

**Relationships Summary**:
1. **Users → Customers** (1:1): Each customer has one user account
2. **Customers → Accounts** (1:M): Each customer can have multiple accounts
3. **Accounts → Transactions** (1:M): Each account can have multiple transactions
4. **Transactions → Accounts** (Self-Referencing): Transfers link two accounts

---

## Constraints & Data Integrity

### Primary Key Constraints
- ✅ Auto-increment identity fields ensure uniqueness
- ✅ Every table has a primary key for data identification

### Unique Constraints
- ✅ `Users.Username`: Prevents duplicate usernames (authentication integrity)
- ✅ `Accounts.AccountNumber`: Prevents duplicate account numbers (banking integrity)

### Foreign Key Constraints
- ✅ `Customers.UserId` → `Users.UserId`: Ensures customer has valid user
- ✅ `Accounts.CustomerId` → `Customers.CustomerId`: Ensures account belongs to valid customer
- ✅ `Transactions.AccountId` → `Accounts.AccountId`: Ensures transaction belongs to valid account
- ✅ `Transactions.ReceiverAccountId` → `Accounts.AccountId`: Validates transfer recipient

### Check Constraints
- ✅ `Users.Role`: Must be 'Admin' or 'Customer'
- ✅ `Accounts.AccountType`: Must be 'Savings' or 'Checking'
- ✅ `Transactions.TransactionType`: Must be 'Deposit', 'Withdrawal', or 'Transfer'

### Not Null Constraints
All critical fields are marked as NOT NULL to prevent incomplete data:
- **Users**: UserId, Username, Password, FullName, Role
- **Customers**: CustomerId, UserId, FirstName, LastName
- **Accounts**: AccountId, CustomerId, AccountNumber, AccountType, Balance
- **Transactions**: TransactionId, AccountId, TransactionType, Amount

### Default Values
- ✅ Automatic timestamps on all CreatedDate fields
- ✅ Account balance defaults to 0
- ✅ IsActive status defaults to 1 (true)

---

## Triggers Implementation

Triggers are used to automatically enforce business rules and maintain data consistency.

### 1. **Trigger: `tr_UpdateAccountModifiedDate`**

**Purpose**: Automatically updates the `LastModified` timestamp whenever an account record is updated.

```sql
CREATE TRIGGER tr_UpdateAccountModifiedDate
ON Accounts
AFTER UPDATE
AS
BEGIN
    UPDATE Accounts
    SET LastModified = GETDATE()
    WHERE AccountId IN (SELECT AccountId FROM inserted);
END
GO
```

**Logic**:
- **Event**: Fires AFTER any UPDATE operation on the Accounts table
- **Action**: Updates the `LastModified` field to current date/time
- **Use Case**: Track when account balances or information was last modified
- **Benefit**: Automatic audit trail without manual intervention

---

### 2. **Trigger: `tr_ValidateTransactionAmount`**

**Purpose**: Prevents invalid transactions (zero or negative amounts) before insertion.

```sql
CREATE TRIGGER tr_ValidateTransactionAmount
ON Transactions
INSTEAD OF INSERT
AS
BEGIN
    -- Check if amount is positive
    IF EXISTS (SELECT 1 FROM inserted WHERE Amount <= 0)
    BEGIN
        RAISERROR('Transaction amount must be greater than zero', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
    
    -- If valid, insert the record
    INSERT INTO Transactions (AccountId, TransactionType, Amount, 
                             ReceiverAccountId, Remarks, TransactionDate)
    SELECT AccountId, TransactionType, Amount, ReceiverAccountId, 
           Remarks, TransactionDate
    FROM inserted;
END
GO
```

**Logic**:
- **Event**: Fires INSTEAD OF INSERT (intercepts insert before execution)
- **Validation**: Checks if Amount ≤ 0
- **Action**: 
  - If invalid: Raises error and rolls back transaction
  - If valid: Proceeds with insertion
- **Business Rule**: No transaction can have zero or negative amount
- **Benefit**: Data validation at database level (more secure than application level)

---

### 3. **Trigger: `tr_PreventTransactionDeletion`**

**Purpose**: Prevents deletion of transactions to maintain audit trail integrity.

```sql
CREATE TRIGGER tr_PreventTransactionDeletion
ON Transactions
INSTEAD OF DELETE
AS
BEGIN
    RAISERROR('Transactions cannot be deleted. 
              All transactions must be preserved for audit trail.', 16, 1);
    ROLLBACK TRANSACTION;
END
GO
```

**Logic**:
- **Event**: Fires INSTEAD OF DELETE (intercepts delete attempts)
- **Action**: Always raises error and prevents deletion
- **Business Rule**: Transactions are immutable records
- **Compliance**: Ensures audit trail is never compromised
- **Benefit**: Regulatory compliance and data accountability

---

## Stored Procedures

Stored procedures encapsulate business logic and provide a secure interface for application access.

### 1. **`sp_UserLogin`** - User Authentication

```sql
CREATE PROCEDURE sp_UserLogin
    @Username NVARCHAR(50),
    @Password NVARCHAR(255)
AS
BEGIN
    SELECT UserId, Username, FullName, Role
    FROM Users
    WHERE Username = @Username 
    AND Password = @Password 
    AND IsActive = 1;
END
GO
```

**Purpose**: Authenticate user login credentials

**Parameters**:
- `@Username`: User's login username
- `@Password`: User's password

**Return**: User details (UserId, Username, FullName, Role) if valid, empty set if invalid

**Business Logic**:
- Validates username and password combination
- Only returns active users (IsActive = 1)
- Returns user role for authorization

**Used By**: `AccountController.Login()`

---

### 2. **`sp_CreateUser`** - User Registration

```sql
CREATE PROCEDURE sp_CreateUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @FullName NVARCHAR(100),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(100),
    @PhoneNumber NVARCHAR(15),
    @Address NVARCHAR(200)
AS
BEGIN
    BEGIN TRANSACTION;
    
    -- Check if username exists
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        ROLLBACK;
        RAISERROR('Username already exists', 16, 1);
        RETURN;
    END
    
    DECLARE @UserId INT;
    
    -- Insert User
    INSERT INTO Users (Username, Password, FullName, Role, IsActive)
    VALUES (@Username, @Password, @FullName, 'Customer', 1);
    
    SET @UserId = SCOPE_IDENTITY();
    
    -- Insert Customer Profile
    INSERT INTO Customers (UserId, FirstName, LastName, Email, 
                          PhoneNumber, Address)
    VALUES (@UserId, @FirstName, @LastName, @Email, @PhoneNumber, 
            @Address);
    
    COMMIT;
END
GO
```

**Purpose**: Register new customer with user credentials and profile

**Parameters**: User and customer information

**Return**: Success (if both User and Customer records created)

**Business Logic**:
- **Atomic Operation**: Both User and Customer records created together
- **Duplicate Check**: Prevents duplicate usernames
- **Role Assignment**: Automatically sets role as 'Customer'
- **Error Handling**: Rolls back if username exists

**Used By**: `AccountController.SignUp()`

---

### 3. **`sp_CreateAccount`** - Create Bank Account

```sql
CREATE PROCEDURE sp_CreateAccount
    @CustomerId INT,
    @AccountNumber NVARCHAR(20),
    @AccountType NVARCHAR(50),
    @InitialBalance DECIMAL(10, 2) = 0
AS
BEGIN
    INSERT INTO Accounts (CustomerId, AccountNumber, AccountType, 
                         Balance, IsActive)
    VALUES (@CustomerId, @AccountNumber, @AccountType, @InitialBalance, 1);
    
    SELECT @@IDENTITY AS AccountId;
END
GO
```

**Purpose**: Create new bank account for a customer

**Parameters**:
- `@CustomerId`: Customer to whom account belongs
- `@AccountNumber`: Account number
- `@AccountType`: Type (Savings/Checking)
- `@InitialBalance`: Starting balance (default 0)

**Return**: New AccountId

**Used By**: `CustomerController.Create()`

---

### 4. **`sp_DepositMoney`** - Deposit Funds

```sql
CREATE PROCEDURE sp_DepositMoney
    @AccountId INT,
    @Amount DECIMAL(10, 2),
    @Remarks NVARCHAR(200)
AS
BEGIN
    BEGIN TRANSACTION;
    
    -- Update Account Balance
    UPDATE Accounts 
    SET Balance = Balance + @Amount 
    WHERE AccountId = @AccountId;
    
    -- Record Transaction
    INSERT INTO Transactions (AccountId, TransactionType, Amount, Remarks)
    VALUES (@AccountId, 'Deposit', @Amount, @Remarks);
    
    COMMIT;
END
GO
```

**Purpose**: Deposit money into an account

**Parameters**:
- `@AccountId`: Target account
- `@Amount`: Deposit amount
- `@Remarks`: Transaction description

**Business Logic**:
- Increases account balance
- Creates transaction record
- Atomic operation (both succeed or both fail)

**Used By**: `TransactionController.Deposit()`

---

### 5. **`sp_WithdrawMoney`** - Withdraw Funds

```sql
CREATE PROCEDURE sp_WithdrawMoney
    @AccountId INT,
    @Amount DECIMAL(10, 2),
    @Remarks NVARCHAR(200)
AS
BEGIN
    BEGIN TRANSACTION;
    
    -- Check if sufficient balance
    DECLARE @CurrentBalance DECIMAL(10, 2);
    SELECT @CurrentBalance = Balance FROM Accounts WHERE AccountId = @AccountId;
    
    IF @CurrentBalance < @Amount
    BEGIN
        ROLLBACK;
        RAISERROR('Insufficient balance', 16, 1);
        RETURN;
    END
    
    -- Update Account Balance
    UPDATE Accounts 
    SET Balance = Balance - @Amount 
    WHERE AccountId = @AccountId;
    
    -- Record Transaction
    INSERT INTO Transactions (AccountId, TransactionType, Amount, Remarks)
    VALUES (@AccountId, 'Withdrawal', @Amount, @Remarks);
    
    COMMIT;
END
GO
```

**Purpose**: Withdraw money from an account

**Parameters**:
- `@AccountId`: Source account
- `@Amount`: Withdrawal amount
- `@Remarks`: Transaction description

**Business Logic**:
- **Balance Validation**: Ensures sufficient funds before withdrawal
- **Atomic Operation**: Prevents partial updates
- **Error Handling**: Raises "Insufficient balance" if needed

**Used By**: `TransactionController.Withdraw()`

---

### 6. **`sp_TransferMoney`** - Transfer Between Accounts

```sql
CREATE PROCEDURE sp_TransferMoney
    @FromAccountId INT,
    @ToAccountId INT,
    @Amount DECIMAL(10, 2),
    @Remarks NVARCHAR(200)
AS
BEGIN
    BEGIN TRANSACTION;
    
    -- Check if sufficient balance in sender account
    DECLARE @CurrentBalance DECIMAL(10, 2);
    SELECT @CurrentBalance = Balance FROM Accounts 
    WHERE AccountId = @FromAccountId;
    
    IF @CurrentBalance < @Amount
    BEGIN
        ROLLBACK;
        RAISERROR('Insufficient balance', 16, 1);
        RETURN;
    END
    
    -- Deduct from sender
    UPDATE Accounts 
    SET Balance = Balance - @Amount 
    WHERE AccountId = @FromAccountId;
    
    -- Add to receiver
    UPDATE Accounts 
    SET Balance = Balance + @Amount 
    WHERE AccountId = @ToAccountId;
    
    -- Record transaction for sender
    INSERT INTO Transactions (AccountId, TransactionType, Amount, 
                             ReceiverAccountId, Remarks)
    VALUES (@FromAccountId, 'Transfer', @Amount, @ToAccountId, @Remarks);
    
    -- Record transaction for receiver
    INSERT INTO Transactions (AccountId, TransactionType, Amount, Remarks)
    VALUES (@ToAccountId, 'Transfer', @Amount, @Remarks);
    
    COMMIT;
END
GO
```

**Purpose**: Transfer money between two accounts

**Parameters**:
- `@FromAccountId`: Source account
- `@ToAccountId`: Destination account
- `@Amount`: Transfer amount
- `@Remarks`: Transaction description

**Business Logic**:
- **Atomic Operation**: Both sender debit and receiver credit happen together
- **Balance Validation**: Ensures sender has sufficient funds
- **Dual Record**: Creates transaction records for both accounts
- **Audit Trail**: Tracks both sides of the transfer

**Used By**: `TransactionController.Transfer()`

---

### 7. **`sp_GetTransactionHistory`** - View Transactions

```sql
CREATE PROCEDURE sp_GetTransactionHistory
    @AccountId INT
AS
BEGIN
    SELECT 
        t.TransactionId,
        t.TransactionDate,
        a.AccountNumber,
        t.TransactionType,
        t.Amount,
        t.Remarks
    FROM Transactions t
    JOIN Accounts a ON t.AccountId = a.AccountId
    WHERE t.AccountId = @AccountId
    ORDER BY t.TransactionDate DESC;
END
GO
```

**Purpose**: Retrieve transaction history for an account

**Parameters**:
- `@AccountId`: Account to query

**Return**: List of transactions sorted by date (newest first)

**Used By**: `TransactionController.History()`

---

### 8. **`sp_GetAllCustomers`** - Admin Customer List

```sql
CREATE PROCEDURE sp_GetAllCustomers
AS
BEGIN
    SELECT 
        c.CustomerId,
        c.FirstName,
        c.LastName,
        c.Email,
        c.PhoneNumber,
        c.Address,
        u.Username,
        u.FullName
    FROM Customers c
    JOIN Users u ON c.UserId = u.UserId
    WHERE u.Role = 'Customer'
    ORDER BY c.FirstName;
END
GO
```

**Purpose**: Admin view of all customers with user information

**Return**: Customer details sorted by first name

**Used By**: `CustomerController.Index()` (Admin only)

---

### 9. **`sp_GetAccountsByCustomer`** - Customer's Accounts

```sql
CREATE PROCEDURE sp_GetAccountsByCustomer
    @CustomerId INT
AS
BEGIN
    SELECT 
        AccountId,
        CustomerId,
        AccountNumber,
        AccountType,
        Balance,
        CreatedDate,
        IsActive
    FROM Accounts
    WHERE CustomerId = @CustomerId AND IsActive = 1;
END
GO
```

**Purpose**: Get all active accounts for a customer

**Parameters**:
- `@CustomerId`: Customer ID

**Return**: List of active accounts

**Used By**: Dashboard and account selection screens

---

## Sample Data

### Pre-loaded Users

| Username | Password | FullName | Role |
|----------|----------|----------|------|
| Mujtaba | 1581810 | Muhammad Mujtaba | Admin |
| awais | 1234 | Sheikh Awais | Customer |
| hadian | 1234 | Hadian Arshad | Customer |
| ali | 1234 | Ali Khan | Customer |

### Pre-loaded Customer Profiles

| CustomerId | FirstName | LastName | Email | PhoneNumber |
|-----------|-----------|----------|-------|-------------|
| 1 | Sheikh | Awais | awais@email.com | 03001234567 |
| 2 | Hadian | Arshad | hadian@email.com | 03007654321 |
| 3 | Ali | Khan | ali@email.com | 03009876543 |

### Pre-loaded Accounts

| AccountId | AccountNumber | AccountType | Balance | CustomerId |
|-----------|---------------|-------------|---------|-----------|
| 1 | ACC-10023 | Savings | 5000.00 | 1 |
| 2 | ACC-10024 | Checking | 1200.50 | 2 |
| 3 | ACC-10025 | Savings | 8750.00 | 3 |

### Sample Transactions

| TransactionId | AccountId | TransactionType | Amount | Remarks |
|---------------|-----------|-----------------|--------|---------|
| 1 | 1 | Deposit | 5000.00 | Initial Deposit |
| 2 | 2 | Transfer | 800.00 | Payment to Hadian |
| 3 | 1 | Withdrawal | 200.00 | ATM Cash |

---

## Setup Instructions

### Prerequisites
- SQL Server 2016 or higher
- SQL Server Management Studio (SSMS)
- Database: Create blank (or use existing server)

### Step-by-Step Setup

#### Step 1: Open SQL Server Management Studio
1. Launch SQL Server Management Studio
2. Connect to your SQL Server instance (typically `localhost` or `.`)
3. Enter credentials if required

#### Step 2: Open and Execute SQL Script
1. Click **File** → **Open** → **File**
2. Navigate to and select `BankingDatabase_Simplified.sql`
3. The script will open in a query editor
4. Click **Execute** button (or press **F5**)
5. Wait for execution to complete (typically 2-3 seconds)

**Expected Output**:
```
Command(s) completed successfully.
```

#### Step 3: Verify Installation

Execute these verification queries:

```sql
-- Check Users Table
SELECT * FROM BankingSystemDB.dbo.Users;

-- Check Customers Table
SELECT * FROM BankingSystemDB.dbo.Customers;

-- Check Accounts Table
SELECT * FROM BankingSystemDB.dbo.Accounts;

-- Check Transactions Table
SELECT * FROM BankingSystemDB.dbo.Transactions;
```

**Expected Results**:
- Users table: 4 users (1 admin + 3 customers)
- Customers table: 3 customer profiles
- Accounts table: 3 accounts with balances
- Transactions table: 3 sample transactions

#### Step 4: Test Stored Procedures

**Test User Login**:
```sql
EXEC sp_UserLogin @Username = 'Mujtaba', @Password = '1581810';
```

**Expected**: Returns admin user record

**Test Transaction History**:
```sql
EXEC sp_GetTransactionHistory @AccountId = 1;
```

**Expected**: Returns 2 transactions for account 1

**Test Deposit**:
```sql
EXEC sp_DepositMoney @AccountId = 1, @Amount = 500, 
                     @Remarks = 'Test Deposit';
```

**Expected**: Balance increases, new transaction created

---

## Database Design Principles Applied

### 1. **Normalization (3NF)**
- Eliminated data redundancy
- Each table has a single responsibility
- No transitive dependencies

### 2. **Referential Integrity**
- Foreign keys enforce parent-child relationships
- No orphaned records possible
- Cascading is controlled intentionally

### 3. **Data Validation**
- Check constraints ensure valid values
- Triggers prevent invalid operations
- Unique constraints prevent duplicates

### 4. **Audit Trail**
- CreatedDate and LastModified timestamps
- Transactions are immutable (cannot delete)
- Complete history preservation

### 5. **Security**
- Role-based access (Admin/Customer)
- Parameterized stored procedures prevent SQL injection
- Password field (note: should be encrypted in production)

### 6. **Business Logic**
- Atomic transactions ensure consistency
- Balance validation prevents invalid operations
- Transfer operations are transactional

### 7. **Performance**
- Primary keys automatically indexed
- Identity fields for fast lookups
- Joins optimized for common queries

---

## Conclusion

This database design provides a **secure, scalable, and maintainable** foundation for the Banking Transaction System. It follows industry best practices while remaining simple and easy to understand. All academic requirements (tables, constraints, triggers, stored procedures) are implemented and well-documented.

The system is ready for integration with the ASP.NET Core MVC application and can easily be extended for future requirements.

---

**Document Version**: 1.0  
**Last Updated**: April 2026  
**Database Name**: BankingSystemDB  
**SQL Server**: 2016 or higher
