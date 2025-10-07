using System.Windows;
using PersonalFinance.Application;           
using PersonalFinance.Wpf.ViewModels;

namespace PersonalFinance;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(new FinanceService());
    }
}
