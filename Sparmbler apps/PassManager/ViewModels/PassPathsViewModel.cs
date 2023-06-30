using PassManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModels
{
    class PassPathsViewModel:PathsViewModel
    {
        public PassPathsViewModel():base() { }
        private OnlyEnabledCommand createKeyCommand;
        public OnlyEnabledCommand CreateKeyCommand => createKeyCommand ??= new(CreateKeyBody);

        public void CreateKeyBody(object parameter)
        {

        }
    }
}
