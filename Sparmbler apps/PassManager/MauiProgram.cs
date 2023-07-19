using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PassManager.Model;
using PassManager.ViewModels;
using PassManager.Views;
using System.Reflection;

namespace PassManager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //Добавление пакета настроек

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PassManager.appsettings.json");
            var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
            builder.Configuration.AddConfiguration(config);

            RegisterModels(builder);
            RegisterViewModels(builder);
            RegisterViews(builder);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void RegisterModels(MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ScramblerManager>();
            builder.Services.AddSingleton<PasswordGenerator>(new PasswordGenerator() { 
                Size = Convert.ToInt32(builder.Configuration.GetRequiredSection("Settings").Get<Settings.Settings>().DefaultSizeKey) 
            });
        }

        private static void RegisterViewModels(MauiAppBuilder builder)
        {
            builder.Services.AddTransient<KeysViewModel>();
            builder.Services.AddTransient<PassPathsViewModel>();
            builder.Services.AddTransient<PassViewModel>();
        }

        private static void RegisterViews(MauiAppBuilder builder)
        {
            builder.Services.AddTransient<KeysView>();
            builder.Services.AddTransient<PassPathsView>();
            builder.Services.AddTransient<PassView>();
        }
    }
}