using System;
using System.Windows.Input;

namespace WPF
{
    class Command: ICommand
    {
        //метод, который будет выполнен при выполнении команды
        private Action _execute;
        //хранит ссылку на метод, который определяет, может ли команда выполняться в данный момент
        private Func<bool> _canExecute;

        public Command(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        //добавляет и удаляет обработчики событий
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        //возвращает результат выполнения делегата _canExecute или true
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
