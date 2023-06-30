using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModels
{
    class RootViewModel
    {
        public RootViewModel() 
        {
            Services = new();


            KeysVM = new();
            KeysVM.PropertyChanged += (sender, e) =>
            {

            };
        }

        public KeysViewModel KeysVM { get; init; }

        private ServiceCollection Services { get; init; }
    }
}
