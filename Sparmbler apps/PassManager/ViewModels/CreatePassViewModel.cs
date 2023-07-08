
using CommunityToolkit.Maui.Storage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PassManager.Commands;
using PassManager.Settings;
using PassManager.ViewConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModels
{
    internal class CreatePassViewModel : INotifyPropertyChanged
    {

        public CreatePassViewModel(IConfiguration configuration, Page targetPage)
        {
            _configuration = configuration;
            TargetPage = targetPage;
            PropertyChanged += (o, e) =>
            {
                SavePasswordCommand.OnCommandChanged();
            };
            GeneratePassBody(null);
        }
        private IConfiguration _configuration;

        private Page TargetPage { get; init; }

        private string password;
        public string Password
        {
            get => password;
            private set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        private UniversalCommand savePasswordCommand;
        public UniversalCommand SavePasswordCommand => savePasswordCommand ??= new(SavePasswordBody, (p) => Password != null);

        private OnlyEnabledCommand generatePassCommand;
        public OnlyEnabledCommand GeneratePassCommand => generatePassCommand ??= new(GeneratePassBody);

        private string savePath;
        public string SavePath 
        {
            get => savePath;
            private set
            {
                savePath = value;
                OnPropertyChanged();
            }
        }

        public async void SavePasswordBody(object parameter)
        {
            var options = new PickOptions();
            options.FileTypes = new(VisualOperations.CreatePassFileTypeDictionary(_configuration));
            options.PickerTitle = Properties.Resources.SaveFileTitle;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(Password));
            try
            {
                var result = await FileSaver.Default.SaveAsync(Properties.Resources.SavePassDefaultName, stream, new CancellationToken());
                if(result != null)
                {
                    SavePath = result.FilePath;
                    await TargetPage.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void GeneratePassBody(object parameter)
        {
            int length = (int)(parameter ?? 32);
            Password = Convert.ToBase64String(RandomNumberGenerator.GetBytes(length));
        }
        private void OnPropertyChanged([CallerMemberName]string propertyName = "")=>PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
