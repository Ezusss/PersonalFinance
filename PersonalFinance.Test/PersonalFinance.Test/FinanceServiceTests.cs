using PersonalFinance.Application;
using PersonalFinance.Domain;
using System;
using System.Linq;
using Xunit;

namespace PersonalFinance.Tests;

public sealed class FinanceServiceTests
{
    // ’елперы
    private static Wallet W(decimal initial = 0m)
        => new() { Name = "W", Currency = "RUB", InitialBalance = initial };

    private static Transaction Tx(DateTime d, decimal a, TransactionType t, string? desc = null)
        => new() { Date = d, Amount = a, Type = t, Description = desc ?? string.Empty };

    [Fact]
    public void CurrentBalance_ComputedCorrectly()
    {
        var s = new FinanceService();
        var w = W(1000m);

        s.AddTransaction(w, Tx(new(2025, 10, 1), 500m, TransactionType.Income));
        s.AddTransaction(w, Tx(new(2025, 10, 2), 200m, TransactionType.Expense));

        Assert.Equal(1300m, w.CurrentBalance);
    }

    [Fact]
    public void AddExpense_Throws_When_InsufficientFunds()
    {
        var s = new FinanceService();
        var w = W(100m);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            s.AddTransaction(w, Tx(new(2025, 10, 1), 150m, TransactionType.Expense)));

        Assert.Contains("Ќедостаточно средств", ex.Message);
    }

    [Fact]
    public void AddTransaction_Throws_When_Amount_NonPositive()
    {
        var s = new FinanceService();
        var w = W(0m);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            s.AddTransaction(w, Tx(new(2025, 10, 1), 0m, TransactionType.Income)));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            s.AddTransaction(w, Tx(new(2025, 10, 1), -10m, TransactionType.Expense)));
    }

    [Fact]
    public void MonthlyTotals_Correct()
    {
        var s = new FinanceService();
        var w = W();

        // ¬ окт€бре
        s.AddTransaction(w, Tx(new(2025, 10, 5), 100m, TransactionType.Income));
        s.AddTransaction(w, Tx(new(2025, 10, 6), 40m, TransactionType.Expense));
        // ¬не мес€ца (сент€брь) Ч не должен учитыватьс€
        s.AddTransaction(w, Tx(new(2025, 9, 30), 500m, TransactionType.Income));

        var (inc, exp) = s.GetMonthlyTotals(w, 2025, 10);

        Assert.Equal(100m, inc);
        Assert.Equal(40m, exp);
    }

    [Fact]
    public void MonthlyGroups_Sorted_By_TotalDesc_And_ItemsByDateAsc()
    {
        var s = new FinanceService();
        var w = new Wallet { Name = "W", Currency = "RUB", InitialBalance = 0m };

        // —Ќј„јЋј доход, затем расходы Ч чтобы не было овердрафта
        s.AddTransaction(w, new Transaction { Date = new(2025, 10, 1), Amount = 1000m, Type = TransactionType.Income });
        s.AddTransaction(w, new Transaction { Date = new(2025, 10, 3), Amount = 50m, Type = TransactionType.Expense });
        s.AddTransaction(w, new Transaction { Date = new(2025, 10, 5), Amount = 200m, Type = TransactionType.Expense });
        // Ќо€брь Ч игнор
        s.AddTransaction(w, new Transaction { Date = new(2025, 11, 1), Amount = 10m, Type = TransactionType.Income });

        var groups = s.GetMonthlyGroups(w, 2025, 10);

        // ѕор€док групп по общей сумме: Income(1000) > Expense(250)
        Assert.Equal(TransactionType.Income, groups[0].Type);
        Assert.Equal(1000m, groups[0].Total);
        Assert.Equal(TransactionType.Expense, groups[1].Type);
        Assert.Equal(250m, groups[1].Total);

        // ¬нутри Expense Ч по дате возрастани€: 3-е, затем 5-е
        var expense = groups[1].Items;
        Assert.Equal(new DateTime(2025, 10, 3), expense[0].Date);
        Assert.Equal(new DateTime(2025, 10, 5), expense[1].Date);
    }

    [Fact]
    public void MonthlyTop3Expenses_Correct_Order_And_Filtering()
    {
        var s = new FinanceService();
        var w = W(10_000m);

        // ќкт€брь
        s.AddTransaction(w, Tx(new(2025, 10, 1), 300m, TransactionType.Expense));
        s.AddTransaction(w, Tx(new(2025, 10, 2), 700m, TransactionType.Expense));
        s.AddTransaction(w, Tx(new(2025, 10, 3), 100m, TransactionType.Expense));
        s.AddTransaction(w, Tx(new(2025, 10, 4), 900m, TransactionType.Expense));
        // —ент€брь Ч игнор
        s.AddTransaction(w, Tx(new(2025, 9, 30), 500m, TransactionType.Expense));

        var top = s.GetTop3Expenses(w, 2025, 10)
                   .Select(t => t.Amount)
                   .ToArray();

        Assert.Equal(new[] { 900m, 700m, 300m }, top);
    }
}
