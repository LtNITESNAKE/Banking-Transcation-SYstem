using Microsoft.AspNetCore.Mvc;
using Banking_Transcation_System.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Banking_Transcation_System.Controllers;

public class AccountController : Controller
{
    private readonly string _connectionString;

    public AccountController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

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
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UserLogin", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", model.Username);
                        cmd.Parameters.AddWithValue("@Password", model.Password);

                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string role = reader["Role"].ToString();
                                string fullName = reader["FullName"].ToString();

                                HttpContext.Session.SetString("Username", model.Username);
                                HttpContext.Session.SetString("FullName", fullName);
                                HttpContext.Session.SetString("Role", role);

                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid username or password.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Database connection error: " + ex.Message);
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
            try
            {
                string[] nameParts = model.FullName.Split(' ', 2);
                string firstName = nameParts[0];
                string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            int userId = 0;
                            using (SqlCommand cmd = new SqlCommand("sp_CreateUser", con, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Username", model.Username);
                                cmd.Parameters.AddWithValue("@Password", model.Password);
                                cmd.Parameters.AddWithValue("@FullName", model.FullName);
                                cmd.Parameters.AddWithValue("@FirstName", firstName);
                                cmd.Parameters.AddWithValue("@LastName", lastName);
                                cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                                cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber ?? "");
                                cmd.Parameters.AddWithValue("@Address", model.Address ?? "");
                                cmd.ExecuteNonQuery();
                            }

                            // Get the UserId just created
                            using (SqlCommand cmd = new SqlCommand("SELECT UserId FROM Users WHERE Username = @Username", con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Username", model.Username);
                                userId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            int customerId = 0;
                            using (SqlCommand cmd = new SqlCommand("SELECT CustomerId FROM Customers WHERE UserId = @UserId", con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                customerId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            if (customerId > 0)
                            {
                                int accountId = 0;
                                using (SqlCommand cmd = new SqlCommand("sp_CreateAccount", con, transaction))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                                    string accNumber = "ACC-" + new Random().Next(10000, 99999).ToString();
                                    cmd.Parameters.AddWithValue("@AccountNumber", accNumber);
                                    cmd.Parameters.AddWithValue("@AccountType", model.AccountType);
                                    cmd.Parameters.AddWithValue("@InitialBalance", 0); // Open with 0, then deposit
                                    
                                    object result = cmd.ExecuteScalar();
                                    if (result != null) accountId = Convert.ToInt32(result);
                                }

                                if (model.InitialBalance > 0 && accountId > 0)
                                {
                                    using (SqlCommand cmd = new SqlCommand("sp_DepositMoney", con, transaction))
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                                        cmd.Parameters.AddWithValue("@Amount", model.InitialBalance);
                                        cmd.Parameters.AddWithValue("@Remarks", "Initial Deposit");
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                return RedirectToAction("Login");
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Username already exists"))
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Database error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }
        }
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
