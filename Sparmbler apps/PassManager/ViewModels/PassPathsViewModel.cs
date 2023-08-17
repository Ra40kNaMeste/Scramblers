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

    public class PassPathsViewModel : PathViewModelBase, IDisposable
    {
        public PassPathsViewModel(IConfiguration configuration, ScramblerManager manager) : base(configuration)
        {
            //Добавление связи с менеджером
            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(SelectPath))
                    manager.PassReader = (PassPathReaderBase)SelectPath?.TargetKeyPath;
            };

            //Добавление удаления задействованных ресурсов
            Paths.CollectionChanged += async (o, e) =>
            {
                if (e.OldItems != null)
                    foreach (var item in e.OldItems)
                    {
                        if (item is IDisposable dis)
                            dis.Dispose();
                        if(item is PathReaderVisual visual)
                            if(visual.TargetKeyPath is PassPathBySqlite lite)
                                await lite.RemoveDataBaseAsync();

                    }
            };

            DependencyService.Get<AppEventManager>().Destroying += (o, e) => Dispose();
        }

        #region OverrideMethods

        public override async Task GenerateBody()
        {
            try
            {
                if (TargetPage == null)
                    return;
                var path = await TargetPage.DisplayPromptAsync(Properties.Resources.CreateDBTitle,
                    Properties.Resources.CreateDBMessage, Properties.Resources.OkButton,
                    Properties.Resources.CancelButton, null, 100, Keyboard.Text,
                    Properties.Resources.SavePathDefaultName);

                if (path == null)
                    return;
                var Database = new PassPathBySqlite();
                Database.DBPath = path;
                AddPath(Database);
            }
            catch (Exception ex)
            {

            }
        }

        public override void OpenPathBody(object parameter)
        {
            AddPath(new PassPathBySqlite());
        }

        protected override CreatedValueOptions GetViewModelOptions() => new() { FileType = TargetFileType.Pass };

        #endregion //OverrideMethods

        public void Dispose()
        {
            foreach (var item in Paths)
            {
                if (item is IDisposable dis)
                    dis.Dispose();
            }
        }

    }

}
