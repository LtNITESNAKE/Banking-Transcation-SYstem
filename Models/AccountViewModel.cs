using System.ComponentModel.DataAnnotations;

namespace Banking_Transcation_System.Models;

public class AccountViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer is required")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty; // Read-only for display

    [Required(ErrorMessage = "Account Number is required")]
    [Display(Name = "Account Number")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Account Type is required")]
    [Display(Name = "Account Type")]
    public string AccountType { get; set; } = string.Empty; // Savings, Checking

    [Required(ErrorMessage = "Initial Balance is required")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Balance must be positive")]
    public decimal Balance { get; set; }
}
