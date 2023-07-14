using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PassManager.Commands
{
    public class OnlyEnabledCommand : ICommand
    {
        public OnlyEnabledCommand(Action<object> command) => _command = command;

        private Action<object> _command;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _command?.Invoke(parameter);
        }
    }
}
