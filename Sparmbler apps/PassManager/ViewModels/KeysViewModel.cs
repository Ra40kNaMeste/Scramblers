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
        public KeysViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            Keys = new();
        }

        private ObservableCollection<KeyPathVisual> keys;
        public ObservableCollection<KeyPathVisual> Keys
        {
            get => keys;
            set
            {
                keys = value;
                OnPropertyChanged();
            }
        }

        public Page TargetPage { get; set; }

        private OnlyEnabledCommand addKeyCommand;
        public OnlyEnabledCommand AddKeyCommand => addKeyCommand ??= new(AddKeyBody);

        private OnlyEnabledCommand updateCommand;
        public OnlyEnabledCommand UpdateCommand => updateCommand ??= new(UpdateBody);

        private void RequesKey(object target, RequestedEventArgs e)
        {
            if (target is KeyPathVisual visual)
            {
                switch (e.Operation)
                {
                    case RequestedOperation.Edit:
                        TargetPage.Navigation.PushAsync(new KeyEditView(visual.TargetKeyPath));
                        break;
                    case RequestedOperation.Select:
                        break;
                    default:
                        break;
                }
            }
        }

        public void AddKeyBody(object parameter)
        {
            KeyPathReaderBase key = new KeyPathByDrive();
            var keyPathVisual = new KeyPathVisual(key);
            keyPathVisual.Request += RequesKey;
            Keys.Add(keyPathVisual);
            RequesKey(keyPathVisual, new(RequestedOperation.Edit));
        }

        public void UpdateBody(object parameter)
        {
            foreach (var key in Keys)
            {
                key.SelectCommand.OnCommandChanged();
            }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
