using Microsoft.Extensions.Configuration;
using PassManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewConverters
{

    /// <summary>
    /// Интерфейс модели для визуализации свойств
    /// </summary>
    public interface IGeneratorVisualProperties
    {
        /// <summary>
        /// Возвращает все свойства, которые необходимо визуализировать
        /// </summary>
        /// <returns></returns>
        public List<PropertyInfo> GetVisualProeprties();
    }

    internal class VisualProperties : INotifyPropertyChanged
    {
        public VisualProperties(string name)
        {
            Name = name;
        }

        public string Name { get; init; }

        private object value;
        public object Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
