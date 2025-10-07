using PersonalFinance.Domain;

namespace PersonalFinance.Application;

public interface IFinanceService
{
    void AddTransaction(Wallet wallet, Transaction tx);

    (decimal income, decimal expense) GetMonthlyTotals(Wallet wallet, int year, int month);
    IReadOnlyList<TransactionGroup> GetMonthlyGroups(Wallet wallet, int year, int month);
    IReadOnlyList<Transaction> GetTop3Expenses(Wallet wallet, int year, int month);

    (decimal income, decimal expense) GetTotals(Wallet wallet, DateTime from, DateTime to);
    IReadOnlyList<TransactionGroup> GetGroups(Wallet wallet, DateTime from, DateTime to);
    IReadOnlyList<Transaction> GetTop3Expenses(Wallet wallet, DateTime from, DateTime to);
}
