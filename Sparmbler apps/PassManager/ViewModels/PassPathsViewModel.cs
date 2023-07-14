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

    public class PassPathsViewModel: PathViewModelBase
    {
        public PassPathsViewModel(IConfiguration configuration, ScramblerManager manager):base(configuration)
        {
            //Добавление связи с менеджером
            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(SelectPath))
                    manager.PassReader = (PassPathReaderBase)SelectPath?.TargetKeyPath;
            };
        }


        #region OverrideMethods

        public override async Task GenerateBody()
        {
            using var stream = new MemoryStream();
            try
            {
                var result = await FileSaver.Default.SaveAsync(Properties.Resources.SavePathDefaultName, stream, new CancellationToken());
                if (result != null)
                {
                    var path = new PassPathByDrive();
                    path.Path.SetPath(result.FilePath);
                    AddPath(path);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public override void OpenPathBody(object parameter)
        {
            AddPath(new PassPathByDrive());
        }

        protected override CreatedValueOptions GetViewModelOptions() => new() { FileType = TargetFileType.Pass };

        #endregion //OverrideMethods
    }

}
