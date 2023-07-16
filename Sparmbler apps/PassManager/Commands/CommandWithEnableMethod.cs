
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PassManager.Commands
{
    public class CommandWithEnableMethod : ICommand
    {

        public CommandWithEnableMethod(Action<object> action, bool isReset = true)
        {
            _action = action;
            _isReset = isReset;
        }


        private readonly Action<object> _action;
        private bool _isEnable = false;
        private bool _isReset;

        private bool IsEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;


        public void Enable() => IsEnable = true;
        public void Disable() => IsEnable = false;
        public bool CanExecute(object parameter)
        {
            return IsEnable;
        }

        public void Execute(object parameter)
        {
            _action.Invoke(parameter);
            IsEnable = !_isReset;
        }
    }
}
