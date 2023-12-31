﻿using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Configuration;
using PassManager.Commands;
using PassManager.Model;
using PassManager.Settings;
using PassManager.ViewConverters;
using PassManager.Views;
using Scrambler.NetFeistel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;

namespace PassManager.ViewModels
{
    public class PassViewModel : INotifyPropertyChanged
    {
        #region Constructions
        public PassViewModel(ScramblerManager manager, PasswordGenerator passGenerator)
        {
            _manager = manager;
            Inizialize();
            _passGenerator = passGenerator;
        }

        /// <summary>
        /// Метод инициализации и создания зависимостей.
        /// </summary>
        private void Inizialize()
        {
            PropertyChanged += (o, e) =>
            {
                AddPasswordsItemsHandler();
                UpdateCopyPassCommand();
                UpdateAddCommand();
            };

            Passwords = new();

            _manager.PropertyChanged += (o, e) =>
            {
                UpdateCopyPassCommand();
                UpdateAddCommand();
                Open();
            };

            Open();
        }

        /// <summary>
        /// Метод для связки представлений паролей с ViewModel.
        /// </summary>
        private void AddPasswordsItemsHandler()
        {
            foreach (var item in Passwords)
            {
                if (item is PasswordVisualItem i)
                    i.Request += HandlePasswordCommandAsync;
            }
            Passwords.CollectionChanged += (o1, e1) =>
            {
                if (e1.NewItems != null)
                    foreach (var item in e1.NewItems)
                    {
                        if (item is PasswordVisualItem i)
                            i.Request += HandlePasswordCommandAsync;
                    }
                SaveCommand.Enable();
                UpdateCopyPassCommand();
            };
        }

        /// <summary>
        /// Обновление состояния комманды добавления пароля.
        /// </summary>
        private void UpdateAddCommand()
        {
            if (_manager.PassReader == null)
                AddKeyCommand.Disable();
            else
                AddKeyCommand.Enable();
        }

        /// <summary>
        /// Обновление состояния команды копирования
        /// </summary>
        private void UpdateCopyPassCommand()
        {
            if (_manager.KeyReader == null)
                foreach (var pass in Passwords)
                    pass.CopyPassToClipboardCommand.Disable();
            else
                foreach (var pass in Passwords)
                    pass.CopyPassToClipboardCommand.Enable();
        }

        public void SetView(PassView view) => _view = view;
        #endregion //Constructions

        #region PrivateFields

        private ScramblerManager _manager;
        private PasswordGenerator _passGenerator;
        private PassView _view;

        #endregion //PrivateFields

        #region VisualProperties

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

        #endregion //VisualProperties

        #region Commands

        #region CommandHead

        private CommandWithEnableMethod addKeyCommand;
        public CommandWithEnableMethod AddKeyCommand => addKeyCommand ??= new(AddKeyBody, false);

        private CommandWithEnableMethod saveCommand;
        public CommandWithEnableMethod SaveCommand => saveCommand ??= new(SaveBody);

        #endregion //CommandHead

        #region CommandBody

        private async void AddKeyBody(object parameter)
        {
            PasswordVisualItem item = new(new Password());
            item.Name = DefaultValuesGenerator.GenerateKeyName(Passwords.Select(i => i.Name));
            if (await GeneratePassword(item))
                Passwords.Add(item);
        }



        private async void SaveBody(object parameter)
        {
            try
            {
                await _manager.PassReader.WritePassAsync(Passwords.Select(i => i.Password));
            }
            catch (Exception)
            {

            }
        }

        private async void HandlePasswordCommandAsync(object sender, RequestedEventArgs e)
        {
            PasswordVisualItem password = (PasswordVisualItem)sender;
            switch (e.Operation)
            {
                case RequestedOperation.Delete:
                    RemoveKeyBody(password);
                    SaveCommand.Enable();
                    break;
                case RequestedOperation.Copy:
                    string text;
                    try
                    {
                        await CopyPassToClipboardBodyAsync(password);
                        text = Properties.Resources.CopyPasswordSuccessfully;
                    }
                    catch (FileNotFoundException)
                    {
                        text = Properties.Resources.CopyPasswordKeyFailed;
                    }
                    catch(Exception)
                    {
                        text = Properties.Resources.CopyPasswordFailed;
                    }
                    CancellationTokenSource source = new();

                    var toast = CommunityToolkit.Maui.Alerts.Toast.Make(text, ToastDuration.Short, 14);

                    await toast.Show(source.Token);
                    source.Dispose();
                    break;
                case RequestedOperation.Edit:
                    await GeneratePassword(password);
                    SaveCommand.Enable();
                    break;
                default:
                    return;
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
                Passwords = new(temp.Select(i => new PasswordVisualItem(i)));
                SaveCommand.Disable();
            }
            catch (Exception)
            {

            }
        }
        private async Task CopyPassToClipboardBodyAsync(PasswordVisualItem key)
        {
            try
            {
                using var scrambler = new Twofish();
                scrambler.BlockSize = 128;
                scrambler.KeySize = 256;

                scrambler.SetKey(Encoding.UTF8.GetBytes(_manager.KeyReader.ReadKey()));

                using MemoryStream ms = new(key.Key);
                using MemoryStream res = new MemoryStream();
                using CryptoStream cs = new(ms, scrambler.CreateDecryptor(), CryptoStreamMode.Read);
                byte[] buffer = new byte[1024];
                int size;
                while ((size = cs.Read(buffer, 0, 1024)) != 0)
                {
                    res.Write(buffer, 0, size);
                }
                cs.Flush();
                var tt = res.GetBuffer();
                string t = Encoding.UTF8.GetString(res.GetBuffer().ToArray().Reverse().SkipWhile(i => i == 0).Reverse().ToArray());

                await Clipboard.Default.SetTextAsync(t);

                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = 0;
                }

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private async Task<bool> GeneratePassword(PasswordVisualItem item)
        {
            using var scrambler = new Twofish();
            scrambler.BlockSize = 128;
            scrambler.KeySize = 256;

            scrambler.SetKey(Encoding.UTF8.GetBytes(_manager.KeyReader.ReadKey()));
            try
            {
                var passBuffer = Encoding.UTF8.GetBytes(_passGenerator.GenerateString());
                using MemoryStream ms = new(passBuffer);

                int sizeRes = ((passBuffer.Length - 1) / 16 + 1) * 16;
                byte[] resBuffer = new byte[sizeRes];
                using MemoryStream res = new MemoryStream(resBuffer);
                using CryptoStream cs = new(res, scrambler.CreateEncryptor(), CryptoStreamMode.Write);

                cs.Write(passBuffer, 0, passBuffer.Length);
                cs.FlushFinalBlock();

                item.Key = resBuffer;
                return true;
            }
            catch (NotActivityGeneratorException)
            {
                await _view?.DisplayAlert(Properties.Resources.ErrorTitle, Properties.Resources.NotGeneratorsErrorMessage, Properties.Resources.CancelTitle);
                return false;
            }

        }

        #endregion //Command body

        #endregion //Commands

        private void OnPropertyChanged([CallerMemberName] string property = null) => PropertyChanged?.Invoke(this, new(property));
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PasswordVisualItem : INotifyPropertyChanged
    {
        public PasswordVisualItem(Password password)
        {
            Password = password;
            Name = password.Name;
            Key = password.Value;
            PropertyChanged += (o, e) =>
            {
                Password.Name = Name;
                Password.Value = Key;
            };
        }

        public OnlyEnabledCommand removeCommand;
        public OnlyEnabledCommand RemoveCommand => removeCommand ??= new(RemoveKeyBody);

        private CommandWithEnableMethod copyPassToClipboardCommand;
        public CommandWithEnableMethod CopyPassToClipboardCommand => copyPassToClipboardCommand ??= new(CopyPassToClipboardBody, false);

        private OnlyEnabledCommand generatePasswordCommand;
        public OnlyEnabledCommand GeneratePasswordCommand => generatePasswordCommand ??= new(GeneratePasswordBody);

        private void RemoveKeyBody(object parameter)
        {
            OnRequest(RequestedOperation.Delete);
        }

        private void CopyPassToClipboardBody(object parameter) => OnRequest(RequestedOperation.Copy);

        public Password Password { get; init; }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }



        private byte[] key;
        public byte[] Key
        {
            get => key;
            set
            {
                key = value;
                OnPropertyChanged();
            }
        }

        public void GeneratePasswordBody(object parameter)
        {
            OnRequest(RequestedOperation.Edit);
        }

        private void OnRequest(RequestedOperation operation) => Request?.Invoke(this, new(operation));
        public event Action<object, RequestedEventArgs> Request;

        private void OnPropertyChanged([CallerMemberName] string property = null) => PropertyChanged?.Invoke(this, new(property));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
