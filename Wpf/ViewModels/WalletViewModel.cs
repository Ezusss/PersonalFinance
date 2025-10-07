using PersonalFinance.Domain;
using PersonalFinance.Wpf.Mvvm;
using System.Collections.ObjectModel;

namespace PersonalFinance.Wpf.ViewModels;

public sealed class WalletViewModel : ViewModelBase
{
    public Wallet Model { get; }
    public ObservableCollection<Transaction> Transactions { get; }

    public WalletViewModel(Wallet model)
    {
        Model = model;
        Transactions = new ObservableCollection<Transaction>(Model.Transactions);
    }

    public string Name
    {
        get => Model.Name;
        set { Model.Name = value; Raise(); }
    }

    public string Currency
    {
        get => Model.Currency;
        set { Model.Currency = value; Raise(); }
    }

    public decimal InitialBalance
    {
        get => Model.InitialBalance;
        set { Model.InitialBalance = value; Raise(); Raise(nameof(CurrentBalance)); }
    }

    public decimal CurrentBalance => Model.CurrentBalance;

    public void SyncFromModel()
    {
        Transactions.Clear();
        foreach (var t in Model.Transactions) Transactions.Add(t);
        Raise(nameof(CurrentBalance));
    }
}
