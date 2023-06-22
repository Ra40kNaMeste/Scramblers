using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PassManager.Commands
{
    class UniversalCommand:ICommand
    {
        public UniversalCommand(Action<object> command, Func<object, bool> condition) 
        { 
            _command = command;
            _condition = condition;
        }
        private Action<object> _command;
        private Func<object, bool> _condition;

        public void OnCommandChanged() => CanExecuteChanged?.Invoke(this, new());
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _condition != null && _condition(parameter);
        }

        public void Execute(object parameter)
        {
            _command?.Invoke(parameter);
        }
    }
}
