
using Microsoft.Extensions.Configuration;
using PassManager.Commands;
using PassManager.Model;
using PassManager.Settings;
using PassManager.ViewConverters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewModels
{
    public class PassViewModel:INotifyPropertyChanged
    {
        public PassViewModel(ScramblerManager manager, IConfiguration configuration)
        {
            _manager = manager;
            _configuration = configuration;
            manager.PropertyChanged += OnPasswordSourceChanged;
            Open();
            Passwords = new();
            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(Passwords))
                    Passwords.CollectionChanged += (o1, e1) =>
                    {
                        foreach (var item in e1.NewItems)
                        {
                            if (item is PasswordVisualItem i)
                                i.Request += HandlePasswordCommandAsync;
                        }
                    };
            };


        }

        private ScramblerManager _manager;
        private IConfiguration _configuration;

        private void OnPasswordSourceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScramblerManager.PassReader))
                try
                {
                    Open();
                }
                catch (Exception)
                {

                }
        }

        private ObservableCollection<PasswordVisualItem> passwords;
        public ObservableCollection<PasswordVisualItem> Passwords 
        {
            get => passwords;
            set
            {
                passwords = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        #region CommandHead

        private OnlyEnabledCommand addKeyCommand;
        public OnlyEnabledCommand AddKeyCommand => addKeyCommand ??= new(AddKeyBody);

        private OnlyEnabledCommand saveCommand;
        public OnlyEnabledCommand SaveCommand => saveCommand ??= new(SaveBody);


        #endregion //CommandHead

        #region CommandBody

        private void AddKeyBody(object parameter)
        {
            int size = Convert.ToInt32(_configuration.GetRequiredSection("Settings").Get<Settings.Settings>().DefaultSizeKey);
            Passwords.Add(new() { Name = DefaultValuesGenerator.GenerateKeyName(Passwords.Select(i => i.Name)), Key = DefaultValuesGenerator.GeneratePass(size) });
        }



        private async void SaveBody(object parameter)
        {
            await _manager.PassReader.WritePassAsync(Passwords.ToDictionary(i => i.Name, i => i.Key));
        }

        private async void HandlePasswordCommandAsync(object sender, RequestedEventArgs e)
        {
            PasswordVisualItem password = (PasswordVisualItem)sender;
            switch (e.Operation)
            {
                case RequestedOperation.Delete:
                    RemoveKeyBody(password);
                    break;
                case RequestedOperation.Copy:
                    await CopyPassToClipboardBodyAsync(password);
                    break;
                default:
                    break;
            }
        }
        private void RemoveKeyBody(PasswordVisualItem key)
        {
            Passwords.Remove(key);
        }

        private async void Open()
        {
            if (_manager.PassReader == null)
                return;
            try
            {
                var temp = await _manager.PassReader.ReadPassAsync();
                Passwords = new(temp.Select(i => new PasswordVisualItem() { Key = i.Value, Name = i.Key }));
            }
            catch (Exception)
            {
                
            }
        }
        private async Task CopyPassToClipboardBodyAsync(PasswordVisualItem key)
        {
            var scrambler = _manager.Algorithm;
            using MemoryStream ms = new(Encoding.UTF8.GetBytes(key.Key));
            using MemoryStream res = new MemoryStream();
            using CryptoStream cs = new(ms, scrambler.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] buffer = new byte[1024];
            while (cs.Read(buffer) != 0)
            {
                res.Write(buffer);
            }
            await Clipboard.Default.SetTextAsync(BitConverter.ToString(res.GetBuffer()));
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }

        }

        #endregion //Command body

        #endregion //Commands

        private void OnPropertyChanged([CallerMemberName] string property = null) => PropertyChanged?.Invoke(this, new(property));
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PasswordVisualItem
    {

        public OnlyEnabledCommand removeCommand;
        public OnlyEnabledCommand RemoveCommand => removeCommand ??= new(RemoveKeyBody);

        private OnlyEnabledCommand copyPassToClipboardCommand;
        public OnlyEnabledCommand CopyPassToClipboardCommand => copyPassToClipboardCommand ??= new(CopyPassToClipboardBody);

        private void RemoveKeyBody(object parameter)
        {
            OnRequest(RequestedOperation.Delete);
        }

        private void CopyPassToClipboardBody(object parameter) => OnRequest(RequestedOperation.Copy);

        public string Name { get; set; }
        public string Key { get; set; }

        private void OnRequest(RequestedOperation operation) => Request?.Invoke(this, new(operation));
        public event Action<object, RequestedEventArgs> Request;
    }
}
