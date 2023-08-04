using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PassManager.Model;
using PassManager.Settings;
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
            PasswordGenerator generator = new();
            builder.Services.AddSingleton(generator);
            UserSettings settings = new(generator);
            builder.Services.AddSingleton(settings);
        }

        private static void RegisterViewModels(MauiAppBuilder builder)
        {
            builder.Services.AddTransient<KeysViewModel>();
            builder.Services.AddTransient<PassPathsViewModel>();
            builder.Services.AddTransient<PassViewModel>();
            builder.Services.AddTransient<SettingViewModel>();
        }

        private static void RegisterViews(MauiAppBuilder builder)
        {
            builder.Services.AddTransient<KeysView>();
            builder.Services.AddTransient<PassPathsView>();
            builder.Services.AddTransient<PassView>();
            builder.Services.AddTransient<SettingView>();
        }
    }
}