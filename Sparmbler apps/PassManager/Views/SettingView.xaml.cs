using PassManager.ViewModels;

namespace PassManager.Views;

public partial class SettingView : ContentPage
{
	public SettingView(SettingViewModel model)
	{
		BindingContext = model;
		InitializeComponent();
	}
}