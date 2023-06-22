using PassManager.ViewModels;

namespace PassManager.Views;

public partial class KeysView : ContentPage
{
	public KeysView()
	{
        BindingContext = new KeysViewModel() { TargetPage = this };

        InitializeComponent();
	}
}