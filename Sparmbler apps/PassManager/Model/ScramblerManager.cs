using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model
{
    public class ScramblerManager : INotifyPropertyChanged, IDisposable
    {
        private IKeyPathReader keyReader;
        public IKeyPathReader KeyReader
        {
            get => keyReader;
            set
            {
                keyReader = value;
                OnPropertyChanged();
            }
        }

        private PassPathReaderBase passReader;
        public PassPathReaderBase PassReader
        {
            get => passReader;
            set
            {
                passReader = value;
                OnPropertyChanged();
            }
        }

        private SymmetricAlgorithm algorithm;
        public SymmetricAlgorithm Algorithm
        {
            get => algorithm;
            set
            {
                algorithm = value;
                OnPropertyChanged();
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Dispose()
        {
            algorithm?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
