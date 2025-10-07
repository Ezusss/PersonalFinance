using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PersonalFinance.Application;
using PersonalFinance.Domain;
using PersonalFinance.Wpf.Mvvm;

namespace PersonalFinance.Wpf.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly IFinanceService _finance;

    public MainViewModel(IFinanceService finance)
    {
        _finance = finance;
        SeedCommand = new RelayCommand(SeedData);
        AddTransactionCommand = new RelayCommand(AddTransaction, CanAddTransaction);

        // текущий месяц по умолчанию (первое число)
        SelectedMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    }

    public ObservableCollection<WalletViewModel> Wallets { get; } = new();

    private WalletViewModel? _selectedWallet;
    public WalletViewModel? SelectedWallet
    {
        get => _selectedWallet;
        set
        {
            _selectedWallet = value;
            Raise();
            Raise(nameof(MonthlyGroups));
            Raise(nameof(Top3Expenses));
            AddTransactionCommand.RaiseCanExecuteChanged();
        }
    }

    private DateTime _selectedMonth;
    public DateTime SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            _selectedMonth = new DateTime(value.Year, value.Month, 1); // нормализуем к 1-му числу
            Raise();
            Raise(nameof(MonthlyGroups));
            Raise(nameof(Top3Expenses));
        }
    }

    // поля ввода новой транзакции
    private DateTime _newDate = DateTime.Today;
    public DateTime NewDate { get => _newDate; set { _newDate = value; Raise(); AddTransactionCommand.RaiseCanExecuteChanged(); } }

    private decimal _newAmount;
    public decimal NewAmount { get => _newAmount; set { _newAmount = value; Raise(); AddTransactionCommand.RaiseCanExecuteChanged(); } }

    private TransactionType _newType = TransactionType.Expense;
    public TransactionType NewType { get => _newType; set { _newType = value; Raise(); AddTransactionCommand.RaiseCanExecuteChanged(); } }

    private string _newDescription = string.Empty;
    public string NewDescription { get => _newDescription; set { _newDescription = value; Raise(); } }

    public RelayCommand SeedCommand { get; }
    public RelayCommand AddTransactionCommand { get; }

    // ОТЧЁТЫ ЗА ВЫБРАННЫЙ МЕСЯЦ
    public IReadOnlyList<TransactionGroup> MonthlyGroups =>
        SelectedWallet is null
            ? Array.Empty<TransactionGroup>()
            : _finance.GetMonthlyGroups(SelectedWallet.Model, SelectedMonth.Year, SelectedMonth.Month);

    public IReadOnlyList<Transaction> Top3Expenses =>
        SelectedWallet is null
            ? Array.Empty<Transaction>()
            : _finance.GetTop3Expenses(SelectedWallet.Model, SelectedMonth.Year, SelectedMonth.Month);

    private void SeedData()
    {
        Wallets.Clear();
        foreach (var w in Seed.Generate()) Wallets.Add(new WalletViewModel(w));
        SelectedWallet = Wallets.FirstOrDefault();
        AddTransactionCommand.RaiseCanExecuteChanged();
        Raise(nameof(MonthlyGroups));
        Raise(nameof(Top3Expenses));
    }

    private bool CanAddTransaction()
    {
        if (SelectedWallet is null) return false;
        if (NewAmount <= 0m) return false;
        if (NewType == TransactionType.Expense && NewAmount > SelectedWallet.CurrentBalance) return false;
        return true;
    }

    private void AddTransaction()
    {
        if (SelectedWallet is null) return;

        var tx = new Transaction
        {
            Date = NewDate,
            Amount = NewAmount,
            Type = NewType,
            Description = NewDescription?.Trim() ?? string.Empty
        };

        try
        {
            _finance.AddTransaction(SelectedWallet.Model, tx);
            SelectedWallet.SyncFromModel();
            Raise(nameof(MonthlyGroups));
            Raise(nameof(Top3Expenses));

            NewAmount = 0m;
            NewDescription = string.Empty;
            AddTransactionCommand.RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}
