using Microsoft.AspNetCore.Mvc;
using Banking_Transcation_System.Models;

namespace Banking_Transcation_System.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            //todo - Replace with database authentication later

            // Hardcoded Admin
            if (model.Username == "Mujtaba" && model.Password == "1581810")
            {
                HttpContext.Session.SetString("Username", "Mujtaba");
                HttpContext.Session.SetString("FullName", "Muhammad Mujtaba");
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("Index", "Home");
            }
            // Hardcoded Customers for demo
            else if (model.Username == "awais" && model.Password == "1234")
            {
                HttpContext.Session.SetString("Username", "awais");
                HttpContext.Session.SetString("FullName", "Sheikh Awais");
                HttpContext.Session.SetString("Role", "Customer");
                return RedirectToAction("Index", "Home");
            }
            else if (model.Username == "hadian" && model.Password == "1234")
            {
                HttpContext.Session.SetString("Username", "hadian");
                HttpContext.Session.SetString("FullName", "Hadian Arshad");
                HttpContext.Session.SetString("Role", "Customer");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignUp(SignUpViewModel model)
    {
        if (ModelState.IsValid)
        {
            //todo - Save user to database

            return RedirectToAction("Login");
        }
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
