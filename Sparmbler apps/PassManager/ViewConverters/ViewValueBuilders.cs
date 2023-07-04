
using Microsoft.Extensions.Configuration;
using PassManager.Model;
using PassManager.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewConverters
{
    internal interface IViewValueBuilder
    {
        public View Build(VisualProperties visual, IConfiguration configuration);
    }

    internal class StringValueBuilder : IViewValueBuilder
    {
        public View Build(VisualProperties visual, IConfiguration configuration)
        {
            Editor editor = new();
            Binding binding = new Binding();
            binding.Mode = BindingMode.TwoWay;
            binding.Source = visual;
            binding.Path = nameof(VisualProperties.Value);

            editor.SetBinding(Editor.TextProperty, binding);

            return editor;
        }
    }
    internal class PathValueBuilder : IViewValueBuilder
    {
        public View Build(VisualProperties visual, IConfiguration configuration)
        {
            Grid res = new();
            res.ColumnDefinitions.Add(new ColumnDefinition());
            res.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            //Создание окна с путём
            Label label = new Label();
            Binding binding = new Binding();
            binding.Source = visual.Value;
            binding.Path = nameof(PathByDrive.Path);
            label.SetBinding(Label.TextProperty, binding);
            res.Add(label);

            Button button = new Button();
            button.Text = "...";
            button.Clicked += async (s, e) =>
            {
                try
                {
                    PickOptions options = new();
                    options.FileTypes = new(VisualOperations.CreatePassFileTypeDictionary(configuration));
                    options.PickerTitle = Properties.Resources.SelectFileTitle;
                    var result = await FilePicker.Default.PickAsync(options);
                    if (result != null)
                        ((PathByDrive)visual.Value).SetPath(result.FullPath);
                }
                catch (Exception)
                {

                }
            };
            Grid.SetColumn(button, 1);
            res.Children.Add(button);
            return res;
        }

    }
}
