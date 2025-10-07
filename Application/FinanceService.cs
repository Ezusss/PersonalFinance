using PersonalFinance.Domain;

namespace PersonalFinance.Application;

public sealed class FinanceService : IFinanceService
{
    public void AddTransaction(Wallet wallet, Transaction tx)
    {
        if (tx.Amount <= 0m)
            throw new ArgumentOutOfRangeException(nameof(tx.Amount), "Amount must be > 0.");
        if (tx.Type == TransactionType.Expense && tx.Amount > wallet.CurrentBalance)
            throw new InvalidOperationException("Недостаточно средств для данной траты.");

        wallet.Transactions.Add(tx);
    }

    // ---------- МЕСЯЧНЫЕ ОБЁРТКИ ----------
    public (decimal income, decimal expense) GetMonthlyTotals(Wallet wallet, int year, int month) =>
        GetTotals(wallet, new DateTime(year, month, 1), EndOfMonth(year, month));

    public IReadOnlyList<TransactionGroup> GetMonthlyGroups(Wallet wallet, int year, int month) =>
        GetGroups(wallet, new DateTime(year, month, 1), EndOfMonth(year, month));

    public IReadOnlyList<Transaction> GetTop3Expenses(Wallet wallet, int year, int month) =>
        GetTop3Expenses(wallet, new DateTime(year, month, 1), EndOfMonth(year, month));

    private static DateTime EndOfMonth(int y, int m) =>
        new DateTime(y, m, DateTime.DaysInMonth(y, m));

    // ---------- ДИАПАЗОННЫЕ МЕТОДЫ ----------
    public (decimal income, decimal expense) GetTotals(Wallet wallet, DateTime from, DateTime to)
    {
        var q = Filter(wallet, from, to);
        return (q.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                q.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount));
    }

    public IReadOnlyList<TransactionGroup> GetGroups(Wallet wallet, DateTime from, DateTime to) =>
        Filter(wallet, from, to)
            .GroupBy(t => t.Type)
            .Select(g => new TransactionGroup(
                g.Key,
                g.Sum(t => t.Amount),
                g.OrderBy(t => t.Date).ToList()))
            .OrderByDescending(g => g.Total)
            .ToList();

    public IReadOnlyList<Transaction> GetTop3Expenses(Wallet wallet, DateTime from, DateTime to) =>
        Filter(wallet, from, to)
            .Where(t => t.Type == TransactionType.Expense)
            .OrderByDescending(t => t.Amount)
            .Take(3)
            .ToList();

    // включительно по датам
    private static IEnumerable<Transaction> Filter(Wallet wallet, DateTime from, DateTime to)
    {
        var f = from.Date; var t = to.Date;
        return wallet.Transactions.Where(x => x.Date.Date >= f && x.Date.Date <= t);
    }
}
