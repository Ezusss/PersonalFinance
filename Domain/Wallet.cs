using System;
using System.Collections.Generic;

namespace PersonalFinance.Domain;

public sealed class Wallet
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "RUB";
    public decimal InitialBalance { get; set; }
    public List<Transaction> Transactions { get; } = new();

    public decimal CurrentBalance =>
        InitialBalance
        + Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount)
        - Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
}
