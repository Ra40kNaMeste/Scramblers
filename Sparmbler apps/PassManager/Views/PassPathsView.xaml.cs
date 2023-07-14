using Microsoft.Extensions.Configuration;
using PassManager.ViewModels;

namespace PassManager.Views;

public partial class PassPathsView : ContentPage
{
    public PassPathsView(PassPathsViewModel vm)
    {
        vm.TargetPage = this;
        BindingContext = vm;
        InitializeComponent();
    }
}