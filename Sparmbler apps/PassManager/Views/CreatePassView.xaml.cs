using Microsoft.Extensions.Configuration;
using PassManager.ViewModels;

namespace PassManager.Views;

public partial class CreatePassView : ContentPage
{
	public CreatePassView(IConfiguration configuration)
	{
		var vm = new CreatePassViewModel(configuration, this);
		vm.PropertyChanged += (o, e) => Path = vm.SavePath;
		BindingContext = vm;
		InitializeComponent();
	}
	private string path;
	public string Path 
	{
		get => path;
		private set
		{
			path = value;
			OnPropertyChanged();
		}
	}
}