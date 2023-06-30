using PassManager.ViewModels;

namespace PassManager.Views;

public partial class KeysView : ContentPage
{
	public KeysView()
	{
		
        var vm = BindingContext ?? new KeysViewModel();
		if (vm is KeysViewModel model)
			model.TargetPage = this;
		BindingContext = vm;
        InitializeComponent();
	}
}