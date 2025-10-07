using PersonalFinance.Domain;

namespace PersonalFinance.Application;

public sealed record TransactionGroup(
    TransactionType Type,
    decimal Total,
    IReadOnlyList<Transaction> Items
);
