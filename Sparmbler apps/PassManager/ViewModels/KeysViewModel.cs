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
    public class KeysViewModel : PathViewModelBase
    {
        public KeysViewModel(IConfiguration configuration, ScramblerManager manager):base(configuration)
        {
            //Добавление связи с менеджером
            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(SelectPath))
                    manager.KeyReader = (IKeyPathReader)SelectPath?.TargetKeyPath;
            };
        }
        #region OverrideMethods
        public override async Task GenerateBody()
        {
            var page = new CreatePassView(Configuration);
            page.Unloaded += (o, e) =>
            {
                if (page.Path == null)
                    return;
                var key = new KeyPathByDrive();
                key.Path.SetPath(page.Path);
                AddPath(key);
            };
            await TargetPage.Navigation.PushAsync(page);
        }

        public override void OpenPathBody(object parameter)
        {
            AddPath(new KeyPathByDrive());
        }

        protected override CreatedValueOptions GetViewModelOptions() => new() { FileType = TargetFileType.Key };
        #endregion //OverrideMethods
    }
}

