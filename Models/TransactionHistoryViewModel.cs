using System;
using System.ComponentModel.DataAnnotations;

namespace Banking_Transcation_System.Models;

public class TransactionHistoryViewModel
{
    public int TransactionId { get; set; }

    [Display(Name = "Date")]
    public DateTime TransactionDate { get; set; }

    [Display(Name = "Account")]
    public string AccountNumber { get; set; } = string.Empty;

    [Display(Name = "Type")]
    public string TransactionType { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer

    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    public string? Remarks { get; set; }
}

public class TransactionFilterViewModel
{
    [Display(Name = "Filter by Account Number")]
    public string? AccountNumber { get; set; }
    
    public System.Collections.Generic.List<TransactionHistoryViewModel> Transactions { get; set; } = new System.Collections.Generic.List<TransactionHistoryViewModel>();
}
