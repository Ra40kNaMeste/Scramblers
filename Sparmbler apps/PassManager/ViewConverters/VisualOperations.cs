using Microsoft.Extensions.Configuration;
using PassManager.Model;
using PassManager.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewConverters
{
    internal static class VisualOperations
    {
        /// <summary>
        /// Создаёт сетку со свойствами
        /// </summary>
        /// <param name="generator">Генератор свойств</param>
        /// <returns></returns>
        public static StackLayout CreateGridProperties(IGeneratorVisualProperties generator, IConfiguration configuration)
        {
            //Создание сетки
            var props = generator.GetVisualProeprties();
            StackLayout result = new StackLayout();

            Grid grid = new();
            result.Add(grid);

            int row = 0;
            grid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new());
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

                grid.RowDefinitions.Add(new RowDefinition());
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, row);
                grid.Add(label);

                //Генерация поля для выбора
                var valueBox = GetViewValueBuilderByType(prop.GetValue(generator).GetType()).Build(visual, configuration);
                Grid.SetColumn(valueBox, 1);
                Grid.SetRow(valueBox, row++);
                grid.Add(valueBox);
            }
            return result;
        }

        private static Dictionary<Type, IViewValueBuilder> viewValueBuidlders = new()
        {
            {typeof(string), new StringValueBuilder() },
            {typeof(PathByDrive), new PathValueBuilder() }
        };
        public static IViewValueBuilder GetViewValueBuilderByType(Type type) => viewValueBuidlders[type];

        private static Dictionary<string, IValueConverter> namePropertyConverters = new()
        {
            { nameof(KeyPathByDrive.Name), new NamePropertyConverter() },
            { nameof(KeyPathByDrive.Path), new PathPropertyConverter() },
        };
        public static IValueConverter GetNamePropertyConverter(string name) => namePropertyConverters[name];

        private static List<DevicePlatform> platforms = new()
        {
            DevicePlatform.Android, DevicePlatform.iOS, DevicePlatform.macOS, DevicePlatform.Tizen, DevicePlatform.WinUI, DevicePlatform.Unknown
        };

  
        public static Dictionary<DevicePlatform, IEnumerable<string>> CreatePassTypeDictionary(IConfiguration configuration)
        {
            var fileType = configuration.GetRequiredSection("FileSettings").Get<FileSettings>();
            Dictionary<DevicePlatform, IEnumerable<string>> res = new();
            foreach (var platform in platforms)
                if (platform == DevicePlatform.WinUI)
                    res.Add(platform, fileType.PassTypes.Select(i => "." + i));
                else
                    res.Add(platform, fileType.PassTypes);

            return res;
        }

        public static Dictionary<DevicePlatform, IEnumerable<string>> CreatePassFileTypeDictionary(IConfiguration configuration)
        {
            var fileType = configuration.GetRequiredSection("FileSettings").Get<FileSettings>();
            Dictionary<DevicePlatform, IEnumerable<string>> res = new();
            foreach (var platform in platforms)
                if (platform == DevicePlatform.WinUI)
                    res.Add(platform, fileType.PassFileTypes.Select(i => "." + i));
                else
                    res.Add(platform, fileType.PassFileTypes);

            return res;
        }
    }

}
