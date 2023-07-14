using PassManager.ViewModels;

namespace PassManager.Views;

public partial class PassView : ContentPage
{
	public PassView(PassViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}