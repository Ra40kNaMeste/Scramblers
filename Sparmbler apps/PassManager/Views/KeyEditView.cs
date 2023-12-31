using Microsoft.Extensions.Configuration;
using PassManager.Settings;
using PassManager.ViewConverters;

namespace PassManager.Views;

public class PathEditView : ContentPage
{
    public PathEditView(IGeneratorVisualProperties generator, IConfiguration configuration, CreatedValueOptions options)
    {
        Content = VisualOperations.CreateGridProperties(generator, configuration, options);
    }
}
