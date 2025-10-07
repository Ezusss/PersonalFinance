using System.Windows.Input;

namespace PersonalFinance.Wpf.Mvvm;

public sealed class RelayCommand : ICommand
{
    private readonly Action _exec;
    private readonly Func<bool>? _canExec;
    public RelayCommand(Action exec, Func<bool>? canExec = null) { _exec = exec; _canExec = canExec; }
    public bool CanExecute(object? parameter) => _canExec?.Invoke() ?? true;
    public void Execute(object? parameter) => _exec();
    public event EventHandler? CanExecuteChanged;
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
