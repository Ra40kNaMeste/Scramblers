﻿using PassManager.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewConverters
{
    public class AppThemeConverter : IValueConverter
    {
        private Dictionary<AppTheme, string> _themes = GetDictionaryThemes();

        private static Dictionary<AppTheme, string> GetDictionaryThemes()=> new()
        {
            { AppTheme.Unspecified, Properties.Resources.UnspecifiedThemeName },
            { AppTheme.Light, Properties.Resources.LightThemeName },
            { AppTheme.Dark, Properties.Resources.DarkThemeName },
        };

        internal static IEnumerable<AppTheme> GetThemes() => GetDictionaryThemes().Keys;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _themes[(AppTheme)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _themes.Where(i => i.Value == value.ToString()).First();
        }
    }
    public class LanguageConverter : IValueConverter
    {
        private static Dictionary<string, string> _languages = new()
        {
            {"en-EN", Properties.Resources.EnglishName },
            {"ru-RU", Properties.Resources.RussianName }
        };

        internal static IEnumerable<string> GetLanguages() => _languages.Keys;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _languages[value.ToString()];  
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _languages.Where(i=>i.Value == value.ToString()).First();
        }
    }

    public class GenerateModeConverter : IValueConverter
    {
        private Dictionary<PasswordGenerateMode, string> _modes = new()
        {
            { PasswordGenerateMode.NumberChars, Properties.Resources.GenerateModeNumberCharsName },
            { PasswordGenerateMode.UppercaseChars, Properties.Resources.GenerateModeUppercaseCharsName },
            { PasswordGenerateMode.All, Properties.Resources.GenerateModeAllName },
            { PasswordGenerateMode.CapitalChars, Properties.Resources.GenerateModeCapitalCharsName },
            { PasswordGenerateMode.SpecialSymbol, Properties.Resources.GenerateModeSpecialSymbolsName }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _modes[(PasswordGenerateMode)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _modes.Where(i=>i.Value == value.ToString()).First();
        }
    }
}
