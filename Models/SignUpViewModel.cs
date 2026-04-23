using System.ComponentModel.DataAnnotations;

namespace Banking_Transcation_System.Models;

public class SignUpViewModel
{
    [Required(ErrorMessage = "Full Name is required")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Role")]
    public string Role { get; set; } = "Customer"; // Admin or Customer

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Account Type is required")]
    [Display(Name = "Account Type")]
    public string AccountType { get; set; } = "Savings";

    [Required(ErrorMessage = "Initial Balance is required")]
    [Display(Name = "Initial Balance")]
    [Range(0, double.MaxValue, ErrorMessage = "Balance must be non-negative")]
    public decimal InitialBalance { get; set; }
}
