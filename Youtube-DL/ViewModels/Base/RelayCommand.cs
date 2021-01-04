using System;
using System.Windows.Input;

namespace Youtube_DL.ViewModels.Base
{
    class RelayCommand : ICommand
    {
        public virtual Action<object> ExecuteCommand { get; init; }
        public virtual Predicate<object> CanExecuteCommand { get; init; }

        public RelayCommand(Action<object> executeCommand)
        {
            ExecuteCommand = executeCommand;
            CanExecuteCommand = e => true;
        }

        public RelayCommand(Action<Object> action, Predicate<object> predicate)
        {
            ExecuteCommand = action;
            CanExecuteCommand = predicate;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => CanExecuteCommand(parameter);

        public void Execute(object parameter) => ExecuteCommand(parameter);
    }

}
