using Microsoft.AspNetCore.Mvc;
using Banking_Transcation_System.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System;
using System.Data;

namespace Banking_Transcation_System.Controllers;

public class TransactionController : Controller
{
    private readonly string _connectionString;

    public TransactionController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    private int GetAccountIdByNumber(string accountNumber)
    {
        int accountId = 0;
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("SELECT AccountId FROM Accounts WHERE AccountNumber = @AccountNumber AND IsActive = 1", con))
            {
                cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                con.Open();
                var result = cmd.ExecuteScalar();
                if (result != null) accountId = Convert.ToInt32(result);
            }
        }
        return accountId;
    }

    [HttpGet]
    public IActionResult GetAccountName(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber)) return Json(new { success = false });

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT u.FullName 
                FROM Accounts a 
                JOIN Customers c ON a.CustomerId = c.CustomerId 
                JOIN Users u ON c.UserId = u.UserId 
                WHERE a.AccountNumber = @AccountNumber AND a.IsActive = 1", con))
            {
                cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                con.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Json(new { success = true, name = result.ToString() });
                }
            }
        }
        return Json(new { success = false });
    }

    [HttpGet]
    public IActionResult Deposit()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Deposit(TransactionViewModel model)
    {
        if (ModelState.IsValid)
        {
            int accountId = GetAccountIdByNumber(model.AccountNumber);
            if (accountId == 0)
            {
                ModelState.AddModelError("AccountNumber", "Invalid or inactive Account Number.");
                return View(model);
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DepositMoney", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                        cmd.Parameters.AddWithValue("@Amount", model.Amount);
                        cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "Deposit");
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = $"Successfully deposited PKR {model.Amount:N2} to {model.AccountNumber}.";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Transaction failed: " + ex.Message;
                ModelState.AddModelError("", "Transaction failed: " + ex.Message);
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Withdraw()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Withdraw(TransactionViewModel model)
    {
        if (ModelState.IsValid)
        {
            int accountId = GetAccountIdByNumber(model.AccountNumber);
            if (accountId == 0)
            {
                ModelState.AddModelError("AccountNumber", "Invalid or inactive Account Number.");
                return View(model);
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_WithdrawMoney", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                        cmd.Parameters.AddWithValue("@Amount", model.Amount);
                        cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "Withdrawal");
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = $"Successfully withdrew PKR {model.Amount:N2} from {model.AccountNumber}.";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Transaction failed: " + ex.Message;
                ModelState.AddModelError("", "Transaction failed: " + ex.Message);
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Transfer()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Transfer(TransactionViewModel model)
    {
        if (ModelState.IsValid)
        {
            int fromAccountId = GetAccountIdByNumber(model.AccountNumber);
            int toAccountId = GetAccountIdByNumber(model.ReceiverAccountNumber ?? "");

            if (fromAccountId == 0)
            {
                ModelState.AddModelError("AccountNumber", "Invalid source Account Number.");
                return View(model);
            }
            if (toAccountId == 0)
            {
                ModelState.AddModelError("ReceiverAccountNumber", "Invalid target Account Number.");
                return View(model);
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_TransferMoney", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromAccountId", fromAccountId);
                        cmd.Parameters.AddWithValue("@ToAccountId", toAccountId);
                        cmd.Parameters.AddWithValue("@Amount", model.Amount);
                        cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "Transfer");
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = $"Successfully transferred PKR {model.Amount:N2} to {model.ReceiverAccountNumber}.";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Transaction failed: " + ex.Message;
                ModelState.AddModelError("", "Transaction failed: " + ex.Message);
            }
        }
        return View(model);
    }

    public IActionResult History(string accountNumber)
    {
        var role = HttpContext.Session.GetString("Role");
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Account");

        var history = new List<TransactionHistoryViewModel>();

        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT t.TransactionId, t.TransactionDate, a.AccountNumber, t.TransactionType, t.Amount, t.Remarks, t.ReceiverAccountId 
                FROM Transactions t
                JOIN Accounts a ON t.AccountId = a.AccountId
                JOIN Customers c ON a.CustomerId = c.CustomerId
                JOIN Users u ON c.UserId = u.UserId
                WHERE 1=1";

            if (role != "Admin")
            {
                query += " AND u.Username = @Username";
            }

            if (!string.IsNullOrEmpty(accountNumber))
            {
                query += " AND a.AccountNumber LIKE @AccountNumber";
            }

            query += " ORDER BY t.TransactionDate DESC";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (role != "Admin")
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                }
                if (!string.IsNullOrEmpty(accountNumber))
                {
                    cmd.Parameters.AddWithValue("@AccountNumber", "%" + accountNumber + "%");
                }

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool isDeduction = false;
                        string type = reader["TransactionType"].ToString();
                        object receiverId = reader["ReceiverAccountId"];

                        if (type == "Withdrawal")
                        {
                            isDeduction = true;
                        }
                        else if (type == "Transfer" && receiverId != DBNull.Value)
                        {
                            isDeduction = true; // This is the sender's side of the transfer record
                        }

                        history.Add(new TransactionHistoryViewModel
                        {
                            TransactionId = Convert.ToInt32(reader["TransactionId"]),
                            TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                            AccountNumber = reader["AccountNumber"].ToString(),
                            TransactionType = type,
                            Amount = Convert.ToDecimal(reader["Amount"]),
                            IsDeduction = isDeduction,
                            Remarks = reader["Remarks"].ToString()
                        });
                    }
                }
            }
        }

        var model = new TransactionFilterViewModel
        {
            AccountNumber = accountNumber,
            Transactions = history
        };

        return View(model);
    }
}
