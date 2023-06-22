using PassManager.ViewConverters;

namespace PassManager.Views;

public class KeyEditView : ContentPage
{
    public KeyEditView(IGeneratorVisualProperties generator)
    {
        Content = VisualOperations.CreateGridProperties(generator);
    }
}