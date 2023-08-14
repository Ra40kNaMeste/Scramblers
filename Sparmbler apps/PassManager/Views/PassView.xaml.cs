using PassManager.ViewModels;

namespace PassManager.Views;

public partial class PassView : ContentPage
{
	public PassView(PassViewModel vm)
	{
		vm.SetView(this);
		BindingContext = vm;
		InitializeComponent();
	}
}