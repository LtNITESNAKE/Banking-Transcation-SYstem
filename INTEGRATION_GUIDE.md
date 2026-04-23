# Quick C# Code Examples for Stored Procedures

## Connection Setup
```csharp
using System.Data;
using System.Data.SqlClient;

public class DatabaseService
{
    private string _connectionString = "Server=localhost;Database=BankingSystemDB;Trusted_Connection=true;TrustServerCertificate=true;";
    
    // Execute procedure that returns data
    public DataTable ExecuteProcedure(string procedureName, SqlParameter[] parameters = null)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            SqlCommand cmd = new SqlCommand(procedureName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable result = new DataTable();
            adapter.Fill(result);
            return result;
        }
    }
}
```

---

## 1. Login (sp_UserLogin)
```csharp
// In AccountController.Login()
public IActionResult Login(LoginViewModel model)
{
    if (ModelState.IsValid)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@Username", model.Username),
            new SqlParameter("@Password", model.Password)
        };
        
        DataTable result = _db.ExecuteProcedure("sp_UserLogin", parameters);
        
        if (result.Rows.Count > 0)
        {
            HttpContext.Session.SetString("Username", result.Rows[0]["Username"].ToString());
            HttpContext.Session.SetString("FullName", result.Rows[0]["FullName"].ToString());
            HttpContext.Session.SetString("Role", result.Rows[0]["Role"].ToString());
            return RedirectToAction("Index", "Home");
        }
        
        ModelState.AddModelError("", "Invalid username or password.");
    }
    return View(model);
}
```

---

## 2. Sign Up (sp_CreateUser)
```csharp
// In AccountController.SignUp()
public IActionResult SignUp(SignUpViewModel model)
{
    if (ModelState.IsValid)
    {
        try
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", model.Username),
                new SqlParameter("@Password", model.Password),
                new SqlParameter("@FullName", model.FullName),
                new SqlParameter("@FirstName", model.FullName.Split(' ')[0]),
                new SqlParameter("@LastName", model.FullName.Split(' ')[1] ?? ""),
                new SqlParameter("@Email", DBNull.Value),
                new SqlParameter("@PhoneNumber", DBNull.Value),
                new SqlParameter("@Address", DBNull.Value)
            };
            
            _db.ExecuteProcedure("sp_CreateUser", parameters);
            TempData["Success"] = "Account created! Please login.";
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
    }
    return View(model);
}
```

---

## 3. Create Customer Account (sp_CreateAccount)
```csharp
// In CustomerController.Create()
public IActionResult Create(CustomerViewModel customer, AccountViewModel account)
{
    if (ModelState.IsValid)
    {
        var parameters = new SqlParameter[]
        {
            new SqlParameter("@CustomerId", customer.Id),
            new SqlParameter("@AccountNumber", account.AccountNumber),
            new SqlParameter("@AccountType", account.AccountType),
            new SqlParameter("@InitialBalance", account.Balance ?? 0)
        };
        
        _db.ExecuteProcedure("sp_CreateAccount", parameters);
        TempData["Success"] = "Account created successfully!";
        return RedirectToAction("Index");
    }
    return View();
}
```

---

## 4. Deposit Money (sp_DepositMoney)
```csharp
// In TransactionController.Deposit()
public IActionResult Deposit(TransactionViewModel model)
{
    if (ModelState.IsValid)
    {
        try
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@AccountId", GetAccountIdFromNumber(model.AccountNumber)),
                new SqlParameter("@Amount", model.Amount),
                new SqlParameter("@Remarks", model.Remarks ?? "Deposit")
            };
            
            _db.ExecuteProcedure("sp_DepositMoney", parameters);
            TempData["Success"] = $"Deposited {model.Amount} successfully!";
            return RedirectToAction("History", new { accountNumber = model.AccountNumber });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
    }
    return View(model);
}
```

---

## 5. Withdraw Money (sp_WithdrawMoney)
```csharp
// In TransactionController.Withdraw()
public IActionResult Withdraw(TransactionViewModel model)
{
    if (ModelState.IsValid)
    {
        try
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@AccountId", GetAccountIdFromNumber(model.AccountNumber)),
                new SqlParameter("@Amount", model.Amount),
                new SqlParameter("@Remarks", model.Remarks ?? "Withdrawal")
            };
            
            _db.ExecuteProcedure("sp_WithdrawMoney", parameters);
            TempData["Success"] = $"Withdrew {model.Amount} successfully!";
            return RedirectToAction("History", new { accountNumber = model.AccountNumber });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
    }
    return View(model);
}
```

---

## 6. Transfer Money (sp_TransferMoney)
```csharp
// In TransactionController.Transfer()
public IActionResult Transfer(TransactionViewModel model)
{
    if (ModelState.IsValid)
    {
        try
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromAccountId", GetAccountIdFromNumber(model.AccountNumber)),
                new SqlParameter("@ToAccountId", GetAccountIdFromNumber(model.ReceiverAccountNumber)),
                new SqlParameter("@Amount", model.Amount),
                new SqlParameter("@Remarks", model.Remarks ?? "Transfer")
            };
            
            _db.ExecuteProcedure("sp_TransferMoney", parameters);
            TempData["Success"] = $"Transferred {model.Amount} successfully!";
            return RedirectToAction("History");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
    }
    return View(model);
}
```

---

## 7. Transaction History (sp_GetTransactionHistory)
```csharp
// In TransactionController.History()
public IActionResult History(string accountNumber)
{
    int accountId = GetAccountIdFromNumber(accountNumber);
    
    var parameters = new SqlParameter[]
    {
        new SqlParameter("@AccountId", accountId)
    };
    
    DataTable result = _db.ExecuteProcedure("sp_GetTransactionHistory", parameters);
    
    var history = new List<TransactionHistoryViewModel>();
    foreach (DataRow row in result.Rows)
    {
        history.Add(new TransactionHistoryViewModel
        {
            TransactionId = (int)row["TransactionId"],
            TransactionDate = (DateTime)row["TransactionDate"],
            AccountNumber = row["AccountNumber"].ToString(),
            TransactionType = row["TransactionType"].ToString(),
            Amount = (decimal)row["Amount"],
            Remarks = row["Remarks"].ToString()
        });
    }
    
    var model = new TransactionFilterViewModel
    {
        AccountNumber = accountNumber,
        Transactions = history
    };
    
    return View(model);
}
```

---

## 8. Get All Customers (sp_GetAllCustomers) - Admin Only
```csharp
// In CustomerController.Index()
public IActionResult Index()
{
    if (HttpContext.Session.GetString("Role") != "Admin")
        return RedirectToAction("Index", "Home");
    
    DataTable result = _db.ExecuteProcedure("sp_GetAllCustomers");
    
    var accounts = new List<AccountViewModel>();
    foreach (DataRow row in result.Rows)
    {
        accounts.Add(new AccountViewModel
        {
            CustomerId = (int)row["CustomerId"],
            CustomerName = $"{row["FirstName"]} {row["LastName"]}",
            AccountNumber = "", // You'd need to fetch this separately
            AccountType = "",
            Balance = 0
        });
    }
    
    return View(accounts);
}
```

---

## Helper Methods
```csharp
// Convert AccountNumber to AccountId
private int GetAccountIdFromNumber(string accountNumber)
{
    string query = $"SELECT AccountId FROM Accounts WHERE AccountNumber = '{accountNumber}'";
    // Execute and return AccountId
}

// Get account balance
private decimal GetAccountBalance(int accountId)
{
    string query = $"SELECT Balance FROM Accounts WHERE AccountId = {accountId}";
    // Execute and return balance
}
```

---

## Notes
- All procedures handle transactions internally (BEGIN TRANSACTION)
- Withdrawal and Transfer check for sufficient balance automatically
- Errors are raised as SQL exceptions - catch them in C#
- Use `DBNull.Value` for optional parameters
- Remarks field is optional - use "Deposit", "Withdrawal", etc. as defaults

## Connection String Location
Update your `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BankingSystemDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```
