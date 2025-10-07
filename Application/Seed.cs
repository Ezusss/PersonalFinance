using PersonalFinance.Domain;

namespace PersonalFinance.Application;

public static class Seed
{
    public static IReadOnlyList<Wallet> Generate()
    {
        var w1 = new Wallet { Name = "Основной", Currency = "RUB", InitialBalance = 10_000m };
        var w2 = new Wallet { Name = "USD счёт", Currency = "USD", InitialBalance = 300m };
        var s = new FinanceService();

        s.AddTransaction(w1, new Transaction { Date = DateTime.Today.AddDays(-28), Amount = 25000m, Type = TransactionType.Income, Description = "Зарплата" });
        s.AddTransaction(w1, new Transaction { Date = DateTime.Today.AddDays(-20), Amount = 12000m, Type = TransactionType.Expense, Description = "Аренда" });
        s.AddTransaction(w1, new Transaction { Date = DateTime.Today.AddDays(-18), Amount = 3000m, Type = TransactionType.Expense, Description = "Продукты" });

        s.AddTransaction(w2, new Transaction { Date = DateTime.Today.AddDays(-10), Amount = 150m, Type = TransactionType.Income, Description = "Кэшбэк" });
        s.AddTransaction(w2, new Transaction { Date = DateTime.Today.AddDays(-9), Amount = 100m, Type = TransactionType.Expense, Description = "Подписки" });

        return new[] { w1, w2 };
    }
}
