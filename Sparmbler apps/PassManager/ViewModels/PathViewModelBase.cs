using CommunityToolkit.Maui.Storage;
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModels
{
    public abstract class PathViewModelBase
    {
        #region Constructions
        public PathViewModelBase(IConfiguration configuration)
        {
            Configuration = configuration;
            Initialize();
        }

        private void Initialize()
        {
            Paths = new();
            //Запись при закрытии окна
            var eventManager = DependencyService.Get<AppEventManager>();
            eventManager.Destroying += (o, e) => Save();
            //загрузка открытых паролей
            Open();
        }
        #endregion//Constructions

        #region VisualProperties

        private ObservableCollection<PathReaderVisual> paths;
        public ObservableCollection<PathReaderVisual> Paths
        {
            get => paths;
            set
            {
                paths = value;
                OnPropertyChanged();
            }
        }

        private PathReaderVisual selectPath;
        public PathReaderVisual SelectPath
        {
            get => selectPath;
            set
            {
                selectPath = value;
                foreach (var item in paths)
                {
                    item.IsSelect = false;
                }
                if (selectPath != null)
                    selectPath.IsSelect = true;
                OnPropertyChanged();
            }
        }

        #endregion //VisualProperties

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

        protected IConfiguration Configuration { get; init; }

        #region Commands

        #region HeadCommands
        private OnlyEnabledCommand openPathCommand;
        public OnlyEnabledCommand OpenPathCommand => openPathCommand ??= new(OpenPathBody);

        private OnlyEnabledCommand updateCommand;
        public OnlyEnabledCommand UpdateCommand => updateCommand ??= new(UpdateBody);

        private OnlyEnabledCommand generateCommand;
        public OnlyEnabledCommand GenerateCommand => generateCommand ??= new(GenerateBodyAsync);

        #endregion //HeadCommands

        #region BodyCommands

        protected void RequesKey(object target, RequestedEventArgs e)
        {
            if (target is PathReaderVisual visual)
            {
                switch (e.Operation)
                {
                    case RequestedOperation.Edit:
                        TargetPage?.Navigation.PushAsync(new PathEditView(visual.TargetKeyPath, Configuration, GetViewModelOptions()));
                        break;
                    case RequestedOperation.Delete:
                        Paths.Remove(visual);
                        break;
                    default:
                        break;
                }
            }
        }

        public void UpdateBody(object parameter)
        {
            foreach (var key in Paths)
            {
                key.TargetKeyPath.Update();
            }
        }

        private void Open()
        {
            var settings = Configuration.GetRequiredSection("Settings").Get<Settings.Settings>();

            var path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + settings.DataFolder + "\\";

            path += GetPathXAMLBySaveKeys();

            if (File.Exists(path))
            {
                try
                {
                    string str = File.ReadAllText(path);
                    var keys = JsonConvert.DeserializeObject(str, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

                    Paths.Clear();

                    if (keys is IEnumerable<PathReaderBase> temp)
                    {
                        foreach (var key in temp)
                        {
                            key.Update();
                            AddPath(key);
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

        private async void Save()
        {
            var settings = Configuration.GetRequiredSection("Settings").Get<Settings.Settings>();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + settings.DataFolder;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path += "\\";

                path += GetPathXAMLBySaveKeys();


                if (!File.Exists(path))
                    File.Create(path);
                await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(Paths.Select(i => i.TargetKeyPath).ToList(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));

            }
            catch (Exception ex)

            {
            }
        }

        public async void GenerateBodyAsync(object parameter) => await GenerateBody();

        #endregion //BodyCommand

        #endregion //Commands

        #region VirtualMethods

        /// <summary>
        /// Возвращает опции для открытия файла
        /// </summary>
        /// <returns></returns>
        protected abstract CreatedValueOptions GetViewModelOptions();

        /// <summary>
        /// Реализация команды OpenPathCommand. Следует добавить через AddPath элемент
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void OpenPathBody(object parameter);

        /// <summary>
        /// Генерирует новый элемент и добавляет его через AddPath
        /// </summary>
        /// <returns></returns>
        public abstract Task GenerateBody();

        #endregion //VirtualMethods

        #region ProtectedMethods

        /// <summary>
        /// Добавляет с обёрткой новый элемент
        /// </summary>
        /// <param name="target"></param>
        protected void AddPath(PathReaderBase target)
        {
            var pathPathVisual = new PathReaderVisual(target);
            pathPathVisual.Request += RequesKey;
            Paths.Add(pathPathVisual);
            RequesKey(pathPathVisual, new(RequestedOperation.Edit));
            SelectPath = pathPathVisual;
        }
        #endregion //ProtectedMethods

        #region PrivateMethods

        private string GetPathXAMLBySaveKeys()
        {
            var options = GetViewModelOptions();
            var fs = Configuration.GetRequiredSection("SaveUserPathsListSettings").Get<SaveUserPathsListSettings>();
            return options.FileType switch
            {
                TargetFileType.Pass => fs.PassFilesXAMLPath,
                TargetFileType.Key => fs.KeyFilesXAMLPath,
                _ => string.Empty
            };
        }

        #endregion

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
