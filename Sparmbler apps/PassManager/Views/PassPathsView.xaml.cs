using Microsoft.Extensions.Configuration;
using PassManager.ViewModels;

namespace PassManager.Views;

public partial class PassPathsView : ContentPage
{
	public PassPathsView(IConfiguration configuration)
	{
        var vm = BindingContext ?? new PassPathsViewModel(configuration);

        if (vm is PassPathsViewModel model)
        {
            model.TargetPage = this;
        }
        BindingContext = vm;
        InitializeComponent();
	}
}