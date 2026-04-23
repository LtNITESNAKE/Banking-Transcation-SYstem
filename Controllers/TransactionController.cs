using Microsoft.AspNetCore.Mvc;
using Banking_Transcation_System.Models;
using System.Collections.Generic;
using System;

namespace Banking_Transcation_System.Controllers;

public class TransactionController : Controller
{
    [HttpGet]
    public IActionResult Deposit()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Deposit(TransactionViewModel model)
    {
        //todo
        return RedirectToAction("History");
    }

    [HttpGet]
    public IActionResult Withdraw()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Withdraw(TransactionViewModel model)
    {
        //todo
        return RedirectToAction("History");
    }

    [HttpGet]
    public IActionResult Transfer()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Transfer(TransactionViewModel model)
    {
        //todo
        return RedirectToAction("History");
    }

    public IActionResult History(string accountNumber)
    {
        // Mock data for UI demonstration
        var history = new List<TransactionHistoryViewModel>
        {
            new TransactionHistoryViewModel { TransactionId = 1, TransactionDate = DateTime.Now.AddDays(-5), AccountNumber = "ACC-10023", TransactionType = "Deposit", Amount = 5000.00m, Remarks = "Initial Deposit" },
            new TransactionHistoryViewModel { TransactionId = 2, TransactionDate = DateTime.Now.AddDays(-3), AccountNumber = "ACC-10024", TransactionType = "Transfer", Amount = 800.00m, Remarks = "Payment to Hadian" },
            new TransactionHistoryViewModel { TransactionId = 3, TransactionDate = DateTime.Now.AddDays(-1), AccountNumber = "ACC-10023", TransactionType = "Withdrawal", Amount = 200.00m, Remarks = "ATM Cash" }
        };

        var model = new TransactionFilterViewModel
        {
            AccountNumber = accountNumber,
            Transactions = history
        };

        return View(model);
    }
}
