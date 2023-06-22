
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewConverters
{
    internal interface IViewValueBuilder
    {
        public View Build(VisualProperties visual);
    }

    internal class StringValueBuilder : IViewValueBuilder
    {
        public View Build(VisualProperties visual)
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
}
