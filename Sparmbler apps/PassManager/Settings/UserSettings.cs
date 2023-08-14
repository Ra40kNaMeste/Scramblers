using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PassManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Settings
{

    class UserSettings
    {
        public UserSettings(PasswordGenerator generator, ILogger logger = null)
        {
            _generator = generator;
            _logger = logger;
        }

        private PasswordGenerator _generator;
        private ILogger _logger;
        public AppTheme Theme
        {
            get => Enum.Parse<AppTheme>(Preferences.Get("Theme", AppTheme.Unspecified.ToString()));
            set
            {
                if (App.Current != null)
                {
                    App.Current.UserAppTheme = value;
                    Preferences.Set("Theme", value.ToString());
                }
            }
        }
        public int PasswordSize
        {
            get => Preferences.Get("PasswordSize", 10);
            set
            {
                _generator.Size = value;
                Preferences.Set("PasswordSize", value);
            }
        }
        public PasswordGenerateMode PasswordGenerateMode
        {
            get => Enum.Parse<PasswordGenerateMode>(Preferences.Get("PasswordGenerateMode", PasswordGenerateMode.All.ToString()));
            set
            {
                _generator.FillGenerators(value);
                Preferences.Set("PasswordGenerateMode", value.ToString());
            }
        }
        public string Language
        {
            get => Preferences.Get("Language", "en-EN");
            set
            {
                try
                {
                    System.Globalization.CultureInfo.CurrentCulture = new(value);
                    Thread.CurrentThread.CurrentCulture = new(value);
                    Thread.CurrentThread.CurrentUICulture = new(value);
                    if (value != Language && App.Current != null) 
                    { 
                        (App.Current as App).MainPage = new AppShell();
                    }
                    Preferences.Set("Language", value);
                }

                catch (Exception)
                {
                    _logger.LogWarning(string.Format($"Failed convert {0}. Unknow value {1}", nameof(Language), value));
                }
            }
        }

        public void Apply()
        {
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                property.SetValue(this, property.GetValue(this));
            }
        }
    }

}
