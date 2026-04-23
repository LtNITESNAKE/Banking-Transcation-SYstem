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
}
