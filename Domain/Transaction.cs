using System;

namespace PersonalFinance.Domain;

public sealed class Transaction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime Date { get; init; }
    public decimal Amount { get; init; }           // > 0
    public TransactionType Type { get; init; }
    public string Description { get; init; } = string.Empty;
}
