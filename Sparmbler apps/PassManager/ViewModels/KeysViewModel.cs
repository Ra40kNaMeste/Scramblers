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
    class KeysViewModel : PassPathsViewModel
    {
        public KeysViewModel(IConfiguration configuration):base(configuration)
        {
        }


        public override async void GenerateBodyAsync(object parameter)
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

        protected override CreatedValueOptions GetViewModelOptions() => new() { FileType = TargetFileType.Key };

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

