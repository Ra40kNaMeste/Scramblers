using PassManager.Commands;
using PassManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewConverters
{
    internal class KeyPathVisual:INotifyPropertyChanged
    {

        public KeyPathVisual(KeyPathReaderBase keyPath) 
        {
            TargetKeyPath = keyPath;
            keyPath.PropertyChanged += (sender, e) => OnPropertyChanged(nameof(Name)); 
        }

        public string Name => TargetKeyPath.Name;

        public KeyPathReaderBase TargetKeyPath { get; init; }

        private OnlyEnabledCommand editCommand;
        public OnlyEnabledCommand EditCommand => editCommand ??= new((p) => Request?.Invoke(this, new(RequestedOperation.Edit)));

        private UniversalCommand selectCommand;
        public UniversalCommand SelectCommand => selectCommand ??= new((p) => Request?.Invoke(this, new(RequestedOperation.Select)), (p) =>
        {
            TargetKeyPath.Update();
            return TargetKeyPath.Enable;
        });

        public event Action<object, RequestedEventArgs> Request;
        private void OnPropertyChanged([CallerMemberName]string propertyName = "")=>PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal enum RequestedOperation
    {
        Edit, Select
    }
    internal class RequestedEventArgs
    {
        public RequestedEventArgs(RequestedOperation operation)
        {
            Operation = operation;
        }

        public RequestedOperation Operation { get; init; }
    }
}
