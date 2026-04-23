using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Banking_Transcation_System.Models;
using Microsoft.Data.SqlClient;

namespace Banking_Transcation_System.Controllers;

public class HomeController : Controller
{
    private readonly string _connectionString;

    public HomeController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("Role");
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(role))
        {
            return RedirectToAction("Login", "Account");
        }

        int totalCustomers = 0;
        decimal totalBalance = 0;
        int totalTransactions = 0;

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            con.Open();
            if (role == "Admin")
            {
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(DISTINCT u.UserId) 
                    FROM Users u 
                    JOIN Customers c ON u.UserId = c.UserId 
                    JOIN Accounts a ON c.CustomerId = a.CustomerId 
                    WHERE u.Role = 'Customer' AND u.IsActive = 1 AND a.IsActive = 1", con))
                {
                    totalCustomers = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Balance), 0) FROM Accounts WHERE IsActive = 1", con))
                {
                    totalBalance = (decimal)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Transactions t
                    JOIN Accounts a ON t.AccountId = a.AccountId
                    WHERE a.IsActive = 1", con))
                {
                    totalTransactions = (int)cmd.ExecuteScalar();
                }
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(a.Balance), 0) 
                    FROM Accounts a
                    JOIN Customers c ON a.CustomerId = c.CustomerId
                    JOIN Users u ON c.UserId = u.UserId
                    WHERE u.Username = @Username AND a.IsActive = 1", con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    totalBalance = (decimal)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Transactions t
                    JOIN Accounts a ON t.AccountId = a.AccountId
                    JOIN Customers c ON a.CustomerId = c.CustomerId
                    JOIN Users u ON c.UserId = u.UserId
                    WHERE u.Username = @Username", con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    totalTransactions = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT a.AccountNumber, a.AccountType
                    FROM Accounts a
                    JOIN Customers c ON a.CustomerId = c.CustomerId
                    JOIN Users u ON c.UserId = u.UserId
                    WHERE u.Username = @Username AND a.IsActive = 1", con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ViewBag.AccountNumber = reader["AccountNumber"].ToString();
                            ViewBag.AccountType = reader["AccountType"].ToString();
                        }
                    }
                }
            }
        }

        ViewBag.TotalCustomers = totalCustomers;
        ViewBag.TotalBalance = totalBalance;
        ViewBag.TotalTransactions = totalTransactions;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
