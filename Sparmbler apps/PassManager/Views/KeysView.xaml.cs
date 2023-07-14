using Microsoft.Extensions.Configuration;
using PassManager.Model;
using PassManager.Settings;
using PassManager.ViewModels;
namespace PassManager.Views;

public partial class KeysView : ContentPage
{
	public KeysView(KeysViewModel keyVM)
	{
        BindingContext = keyVM;
        keyVM.TargetPage = this;

        InitializeComponent();
    }

}