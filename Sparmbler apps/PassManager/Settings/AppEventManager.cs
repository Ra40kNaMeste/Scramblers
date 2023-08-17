using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Settings
{
    /// <summary>
    /// Класс для связи с событиями приложения
    /// </summary>
    public class AppEventManager
    {
        public AppEventManager(Window window) 
        {
            _window= window;
        }
        private Window _window;

        public event EventHandler Destroying
        {
            add { _window.Destroying += value; }
            remove { _window.Destroying -= value;}
        }


        public event EventHandler Created
        {
            add { _window.Created += value; }
            remove { _window.Created -= value; }
        }
        public event EventHandler Stopped
        {
            add { _window.Stopped += value; }
            remove { _window.Stopped -= value; }
        }

    }
}
