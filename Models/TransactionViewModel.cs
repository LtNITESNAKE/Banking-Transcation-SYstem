using System.ComponentModel.DataAnnotations;

namespace Banking_Transcation_System.Models;

public class TransactionViewModel
{
    [Required(ErrorMessage = "Account Number is required")]
    [Display(Name = "Account Number")]
    public string AccountNumber { get; set; } = string.Empty;

    [Display(Name = "Receiver Account Number (For Transfer)")]
    public string? ReceiverAccountNumber { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [DataType(DataType.Currency)]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }
    
    public string? Remarks { get; set; }
}
