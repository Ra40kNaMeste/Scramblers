using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

            builder.Services.AddTransient<KeysView>();
            builder.Services.AddTransient<PassPathsView>();

#if DEBUG
		    builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}