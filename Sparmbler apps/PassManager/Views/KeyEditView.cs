using PassManager.ViewConverters;

namespace PassManager.Views;

public class PathEditView : ContentPage
{
    public PathEditView(IGeneratorVisualProperties generator)
    {
        Content = VisualOperations.CreateGridProperties(generator);
    }
}
