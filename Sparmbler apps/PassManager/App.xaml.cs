using PassManager.Settings;

namespace PassManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var res = base.CreateWindow(activationState);
            AppEventManager eventManager = new();
            res.Created += eventManager.OnCreated;
            res.Destroying += eventManager.OnDestroying;

            DependencyService.RegisterSingleton(eventManager);
            return res;
        }
    }
}