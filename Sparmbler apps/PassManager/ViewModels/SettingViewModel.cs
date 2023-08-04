using PassManager.Model;
using PassManager.Settings;
using PassManager.ViewConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModels
{
    public class SettingViewModel : INotifyPropertyChanged
    {
        public SettingViewModel(PasswordGenerator generator)
        {
            Settings = new(generator);
            FillModes();
            AppThemes = AppThemeConverter.GetThemes().ToList();
            Languages = LanguageConverter.GetLanguages().ToList();
        }

        private void FillModes()
        {
            var modes = Enum.GetValues(typeof(PasswordGenerateMode));
            GenerateModes = new();
            foreach (PasswordGenerateMode mode in modes)
            {
                if (mode != PasswordGenerateMode.None && mode != PasswordGenerateMode.All)
                {
                    var item = new GenerateModeItem(mode, (mode & Settings.PasswordGenerateMode) != 0);
                    item.PropertyChanged += (o, e) =>
                    {
                        var target = (GenerateModeItem)o;
                        if (target.IsEnable)
                            Settings.PasswordGenerateMode |= target.Mode;
                        else
                            Settings.PasswordGenerateMode &= ~target.Mode;
                    };
                    GenerateModes.Add(item);
                }
            }
        }

        public IList<AppTheme> AppThemes { get; init; }
        public AppTheme AppTheme
        {
            get => Settings.Theme;
            set
            {
                Settings.Theme = value;
                OnPropertyChanged();
            }
        }

        public int PasswordGenerationSize
        {
            get => Settings.PasswordSize;
            set 
            {
                Settings.PasswordSize = value;
                OnPropertyChanged();
            }
        }

        public IList<string> Languages { get; init; }
        public string Language
        {
            get => Settings.Language;
            set
            {
                Settings.Language = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<GenerateModeItem> GenerateModes { get; private set; }

        private UserSettings Settings { get; init; }

        private void OnPropertyChanged([CallerMemberName] string property = null) => PropertyChanged?.Invoke(this, new(property));
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class GenerateModeItem : INotifyPropertyChanged
    {
        public GenerateModeItem(PasswordGenerateMode mode, bool isEnable)
        {
            Mode = mode;
            IsEnable = isEnable;
        }

        public PasswordGenerateMode Mode { get; init; }

        private bool isEnable;
        public bool IsEnable
        {
            get => isEnable;
            set
            {
                isEnable = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string property = null) => PropertyChanged?.Invoke(this, new(property));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
