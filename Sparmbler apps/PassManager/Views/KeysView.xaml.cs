using Microsoft.Extensions.Configuration;
using PassManager.Settings;
using PassManager.ViewModels;
namespace PassManager.Views;

public partial class KeysView : ContentPage
{
	public KeysView(IConfiguration configuration)
	{
		
        var vm = BindingContext ?? new KeysViewModel(configuration);

		if (vm is PassPathsViewModel model)
		{
            model.TargetPage = this;
        }
        BindingContext = vm;
        InitializeComponent();
	}

}