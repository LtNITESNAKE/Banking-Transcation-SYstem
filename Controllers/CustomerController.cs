using Microsoft.AspNetCore.Mvc;
using Banking_Transcation_System.Models;
using System.Collections.Generic;

namespace Banking_Transcation_System.Controllers;

public class CustomerController : Controller
{
    public IActionResult Index()
    {
        // Only admin can access this page
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        // Mock data for UI demonstration
        var accounts = new List<AccountViewModel>
        {
            new AccountViewModel { Id = 1, CustomerId = 1, CustomerName = "Sheikh Awais", AccountNumber = "ACC-10023", AccountType = "Savings", Balance = 5000.00m },
            new AccountViewModel { Id = 2, CustomerId = 2, CustomerName = "Hadian Arshad", AccountNumber = "ACC-10024", AccountType = "Checking", Balance = 1200.50m },
            new AccountViewModel { Id = 3, CustomerId = 3, CustomerName = "Ali Khan", AccountNumber = "ACC-10025", AccountType = "Savings", Balance = 8750.00m }
        };
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
        //todo
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }
        // Mock data for UI demonstration
        var model = new AccountViewModel { Id = id, CustomerId = 1, CustomerName = "Sheikh Awais", AccountNumber = "ACC-10023", AccountType = "Savings", Balance = 5000.00m };
        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(AccountViewModel model)
    {
        //todo
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        //todo
        return RedirectToAction("Index");
    }
}
