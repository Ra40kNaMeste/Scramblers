using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PassManager.Commands;
using PassManager.Model;
using PassManager.Settings;
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
            //Запись при закрытии окна
            var eventManager = DependencyService.Get<AppEventManager>();
            eventManager.Destroying += (o, e) => Save();
            //загрузка открытых паролей
            Open();
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

        private Page targetPage;
        public Page TargetPage
        {
            get => targetPage;
            set
            {
                targetPage = value;
                OnPropertyChanged();
            }
        }

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


        private void Open()
        {
            var fs = Configuration.GetRequiredSection("SaveUserPathsListSettings").Get<SaveUserPathsListSettings>();
            var settings = Configuration.GetRequiredSection("Settings").Get<Settings.Settings>();
            var path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + settings.DataFolder + "\\" + fs.PassFilesXAMLPath;

            if (File.Exists(path))
            {
                try
                {
                    string str = File.ReadAllText(path);
                    var keys = JsonConvert.DeserializeObject(str, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

                    Keys.Clear();

                    if (keys is IEnumerable<PathReaderBase> temp)
                    {
                        foreach (var key in temp)
                        {
                            key.Update();
                            var t = new PathReaderVisual(key);
                            t.Request += RequesKey;
                            Keys.Add(t);
                        }
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private void Save()
        {
            var fs = Configuration.GetRequiredSection("SaveUserPathsListSettings").Get<SaveUserPathsListSettings>();
            var settings = Configuration.GetRequiredSection("Settings").Get<Settings.Settings>();
            var path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + settings.DataFolder;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path += "\\" + fs.PassFilesXAMLPath;
                if (!File.Exists(path))
                    File.Create(path);
                File.WriteAllText(path, JsonConvert.SerializeObject(Keys.Select(i => i.TargetKeyPath).ToList(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));

            }
            catch (Exception ex)
            {
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

