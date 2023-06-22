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

    internal static class VisualOperations
    {
        /// <summary>
        /// Создаёт сетку со свойствами
        /// </summary>
        /// <param name="generator">Генератор свойств</param>
        /// <returns></returns>
        public static Grid CreateGridProperties(IGeneratorVisualProperties generator)
        {
            //Создание сетки
            var props = generator.GetVisualProeprties();
            Grid res = new();
            int row = 0;
            res.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            res.ColumnDefinitions.Add(new());
            //Заполнение сетки
            foreach (PropertyInfo prop in props)
            {
                //Связывание визуала и модели
                VisualProperties visual = new(prop.Name);
                visual.PropertyChanged += (sender, e) =>
                {
                    VisualProperties temp = (VisualProperties)sender;
                    if (temp.Value != prop.GetValue(generator))
                        prop.SetValue(generator, temp.Value);
                };

                //Установка свойства
                object temp;
                if ((temp = prop.GetValue(generator)) != visual.Value)
                    visual.Value = temp;

                //Связывание со свойством по средством INotifyPropertyChanged если его реализует IGeneratorVisualProperties
                if (generator is INotifyPropertyChanged changed)
                    changed.PropertyChanged += (sender, e) =>
                    {
                        object temp;
                        if ((temp = prop.GetValue(generator)) != visual.Value)
                            visual.Value = temp;
                    };

                //Генерация поля: имя свойства
                Label label = new Label();
                Binding binding = new Binding();
                binding.Source = generator;
                binding.Converter = GetNamePropertyConverter(prop.Name);
                binding.Path = nameof(VisualProperties.Name);
                label.SetBinding(Label.TextProperty, binding);

                res.RowDefinitions.Add(new RowDefinition());
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, row);
                res.Add(label);

                //Генерация поля для выбора
                var valueBox = GetViewValueBuilderByType(prop.GetValue(generator).GetType()).Build(visual);
                Grid.SetColumn(valueBox, 1);
                Grid.SetRow(valueBox, row++);
                res.Add(valueBox);
            }
            return res;
        }

        private static Dictionary<Type, IViewValueBuilder> viewValueBuidlders = new()
        {
            {typeof(string), new StringValueBuilder() }
        };
        public static IViewValueBuilder GetViewValueBuilderByType(Type type) => viewValueBuidlders[type];

        private static Dictionary<string, IValueConverter> namePropertyConverters = new()
        {
            { nameof(KeyPathByDrive.Name), new NamePropertyConverter() },
            { nameof(KeyPathByDrive.DriveLabel), new DriveLabelPropertyConverter() },
            { nameof(KeyPathByDrive.DriveName), new DriveNamePropertyConverter() },
            { nameof(KeyPathByDrive.Path), new PathPropertyConverter() }
        };
        public static IValueConverter GetNamePropertyConverter(string name) => namePropertyConverters[name];
    }
}
