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
    /// <summary>
    /// Прокси, который представляет оболочку над моделью KeyPathReaderBase для визуализации
    /// </summary>
    internal class PathReaderVisual:INotifyPropertyChanged
    {
        public PathReaderVisual()
        {

        }
        public PathReaderVisual (PathReaderBase keyPath) 
        {
            TargetKeyPath = keyPath;
        }

        public string Name => TargetKeyPath.Name;

        private PathReaderBase targetKeyPath;
        public PathReaderBase TargetKeyPath 
        {
            get => targetKeyPath;
            set
            {
                targetKeyPath= value;
                targetKeyPath.PropertyChanged += (sender, e) => OnPropertyChanged(nameof(Name));
                OnPropertyChanged();

            }
        }

        private OnlyEnabledCommand editCommand;
        public OnlyEnabledCommand EditCommand => editCommand ??= new((p) => Request?.Invoke(this, new(RequestedOperation.Edit)));

        private OnlyEnabledCommand deleteCommand;
        public OnlyEnabledCommand DeleteCommand => deleteCommand ??= new((p) => Request?.Invoke(this, new(RequestedOperation.Delete)));


        public event Action<object, RequestedEventArgs> Request;
        private void OnPropertyChanged([CallerMemberName]string propertyName = "")=>PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal enum RequestedOperation
    {
        Edit, Delete
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
