using Microsoft.AspNetCore.Mvc;
using Banking_Transcation_System.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Banking_Transcation_System.Controllers;

public class CustomerController : Controller
{
    private readonly string _connectionString;

    public CustomerController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        var accounts = new List<AccountViewModel>();

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT a.AccountId, c.CustomerId, u.FullName, a.AccountNumber, a.AccountType, a.Balance 
                FROM Accounts a
                JOIN Customers c ON a.CustomerId = c.CustomerId
                JOIN Users u ON c.UserId = u.UserId
                WHERE a.IsActive = 1", con))
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accounts.Add(new AccountViewModel
                        {
                            Id = Convert.ToInt32(reader["AccountId"]),
                            CustomerId = Convert.ToInt32(reader["CustomerId"]),
                            CustomerName = reader["FullName"].ToString(),
                            AccountNumber = reader["AccountNumber"].ToString(),
                            AccountType = reader["AccountType"].ToString(),
                            Balance = Convert.ToDecimal(reader["Balance"])
                        });
                    }
                }
            }
        }

        return View(accounts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public IActionResult Create(CustomerViewModel customer, AccountViewModel account)
    {
        if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Index", "Home");

        ModelState.Remove("account.AccountNumber");
        ModelState.Remove("account.CustomerId");
        ModelState.Remove("AccountNumber");
        ModelState.Remove("CustomerId");

        if (ModelState.IsValid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_CreateUser", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", customer.Username);
                        cmd.Parameters.AddWithValue("@Password", customer.Password);
                        cmd.Parameters.AddWithValue("@FullName", customer.FirstName + " " + customer.LastName);
                        cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                        cmd.Parameters.AddWithValue("@Email", customer.Email ?? "");
                        cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber ?? "");
                        cmd.Parameters.AddWithValue("@Address", customer.Address ?? "");
                        cmd.ExecuteNonQuery();
                    }

                    int customerId = 0;
                    using (SqlCommand cmd = new SqlCommand("SELECT c.CustomerId FROM Customers c JOIN Users u ON c.UserId = u.UserId WHERE u.Username = @Username", con))
                    {
                        cmd.Parameters.AddWithValue("@Username", customer.Username);
                        var result = cmd.ExecuteScalar();
                        if (result != null) customerId = Convert.ToInt32(result);
                    }

                    if (customerId > 0)
                    {
                        int newAccountId = 0;
                        using (SqlCommand cmd = new SqlCommand("sp_CreateAccount", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                            string accNumber = "ACC-" + new Random().Next(10000, 99999).ToString();
                            cmd.Parameters.AddWithValue("@AccountNumber", accNumber);
                            cmd.Parameters.AddWithValue("@AccountType", account.AccountType ?? "Savings");
                            cmd.Parameters.AddWithValue("@InitialBalance", 0);
                            
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    newAccountId = Convert.ToInt32(reader["AccountId"]);
                                }
                            }
                        }

                        if (account.Balance > 0 && newAccountId > 0)
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_DepositMoney", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@AccountId", newAccountId);
                                cmd.Parameters.AddWithValue("@Amount", account.Balance);
                                cmd.Parameters.AddWithValue("@Remarks", "Initial Deposit");
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                TempData["SuccessMessage"] = "Successfully created new customer and account.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }
        }
        return View();
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }
        
        AccountViewModel model = null;
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT a.AccountId, c.CustomerId, u.FullName, a.AccountNumber, a.AccountType, a.Balance 
                FROM Accounts a
                JOIN Customers c ON a.CustomerId = c.CustomerId
                JOIN Users u ON c.UserId = u.UserId
                WHERE a.AccountId = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new AccountViewModel
                        {
                            Id = Convert.ToInt32(reader["AccountId"]),
                            CustomerId = Convert.ToInt32(reader["CustomerId"]),
                            CustomerName = reader["FullName"].ToString(),
                            AccountNumber = reader["AccountNumber"].ToString(),
                            AccountType = reader["AccountType"].ToString(),
                            Balance = Convert.ToDecimal(reader["Balance"])
                        };
                    }
                }
            }
        }

        if (model == null) return NotFound();

        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(AccountViewModel model)
    {
        if (ModelState.IsValid)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE Accounts SET AccountType = @Type WHERE AccountId = @Id", con))
                {
                    cmd.Parameters.AddWithValue("@Type", model.AccountType);
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }
        return View(model);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Accounts SET IsActive = 0 WHERE AccountId = @Id;
                
                UPDATE Users 
                SET IsActive = 0 
                WHERE UserId IN (
                    SELECT c.UserId 
                    FROM Customers c 
                    JOIN Accounts a ON c.CustomerId = a.CustomerId 
                    WHERE a.AccountId = @Id
                );", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        return RedirectToAction("Index");
    }
}
