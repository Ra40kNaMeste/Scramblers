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
        public AppEventManager() 
        {
            
        }

        public void OnDestroying(object sender, EventArgs e)=>Destroying?.Invoke(sender, e);
        public void OnCreated(object sender, EventArgs e) => Created?.Invoke(sender, e);


        public event EventHandler Destroying;
        public event EventHandler Created;

    }
}
