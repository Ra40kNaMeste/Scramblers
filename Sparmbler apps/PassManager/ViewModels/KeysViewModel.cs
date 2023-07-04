using Microsoft.Extensions.Configuration;
using PassManager.Commands;
using PassManager.Model;
using PassManager.ViewConverters;
using PassManager.Views;
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
    class KeysViewModel : INotifyPropertyChanged
    {
        public KeysViewModel(IConfiguration configuration)
        {
            Configuration = configuration;
            Initialize();
        }

        private void Initialize()
        {
            Keys = new();
        }

        private ObservableCollection<PathReaderVisual> keys;
        public ObservableCollection<PathReaderVisual> Keys
        {
            get => keys;
            set
            {
                keys = value;
                OnPropertyChanged();
            }
        }

        private PathReaderBase selectKey;
        public PathReaderBase SelectKey
        {
            get => selectKey;
            set
            {
                selectKey = value;
                OnPropertyChanged();
            }
        }

        public Page TargetPage { get; set; }

        private OnlyEnabledCommand openKeyCommand;
        public OnlyEnabledCommand OpenKeyCommand => openKeyCommand ??= new(OpenKeyBody);

        private OnlyEnabledCommand updateCommand;
        public OnlyEnabledCommand UpdateCommand => updateCommand ??= new(UpdateBody);

        private OnlyEnabledCommand generateCommand;
        public OnlyEnabledCommand GenerateCommand => generateCommand ??= new(GenerateBodyAsync);

        public virtual async void GenerateBodyAsync(object parameter)
        {
            var page = new CreatePassView(Configuration);
            page.Unloaded += (o, e) =>
            {
                if (page.Path == null)
                    return;
                var key = new KeyPathByDrive();
                key.Path.SetPath(page.Path);
                var keyPathVisual = new PathReaderVisual(key);
                keyPathVisual.Request += RequesKey;
                Keys.Add(keyPathVisual);
                RequesKey(keyPathVisual, new(RequestedOperation.Edit));
                SelectKey = key;
            };
            await TargetPage.Navigation.PushAsync(page);
        }
        private IConfiguration Configuration { get; init; }
        private void RequesKey(object target, RequestedEventArgs e)
        {
            if (target is PathReaderVisual visual)
            {
                switch (e.Operation)
                {
                    case RequestedOperation.Edit:
                        TargetPage.Navigation.PushAsync(new PathEditView(visual.TargetKeyPath, Configuration));
                        break;
                        case RequestedOperation.Delete:
                        Keys.Remove(visual);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OpenKeyBody(object parameter)
        {
            KeyPathReaderBase key = new KeyPathByDrive();
            var keyPathVisual = new PathReaderVisual(key);
            keyPathVisual.Request += RequesKey;
            Keys.Add(keyPathVisual);
            RequesKey(keyPathVisual, new(RequestedOperation.Edit));
            SelectKey = key;
        }

        public void UpdateBody(object parameter)
        {
            foreach (var key in Keys)
            {
                key.TargetKeyPath.Update();
            }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

