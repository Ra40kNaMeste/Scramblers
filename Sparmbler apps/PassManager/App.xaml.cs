using PassManager.Settings;
using PassManager.ViewModels;

namespace PassManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserSettings settings = new(new());
            settings.Apply();
            MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var res = base.CreateWindow(activationState);
            AppEventManager eventManager = new(res);
            DependencyService.RegisterSingleton(eventManager);
            return res;
        }
    }
}