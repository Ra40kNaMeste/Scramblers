using PassManager.ViewConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model
{
    public class KeyPathsManager
    {
        public KeyPathsManager()
        {
            Paths = new List<KeyPathReaderBase>();
        }
        public List<KeyPathReaderBase> Paths { get; init; }

        public void Update()
        {
            var drives = DriveInfo.GetDrives();

        }

    }

    /// <summary>
    /// Класс, который читает ключ из файла
    /// </summary>
    public abstract class KeyPathReaderBase : INotifyPropertyChanged, IGeneratorVisualProperties
    {
        public KeyPathReaderBase()
        {
            Enable = false;
            Name = "";
        }

        /// <summary>
        /// Имя класса
        /// </summary>
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

        /// <summary>
        /// Переменная, показывающая, можно ли читать
        /// </summary>
        private bool enable;
        public bool Enable
        {
            get => enable;
            private set
            {
                enable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// прочитать ключ
        /// </summary>
        /// <returns>Ключ</returns>
        public abstract string GetKey();

        /// <summary>
        /// Асинхронная версия GetKey
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetKeyAsync()
        {
            var task = Task.Run(GetKey);
            await task;
            return task.Result;
        }

        /// <summary>
        /// Можно ли прочитать ключ
        /// </summary>
        /// <returns></returns>
        protected abstract bool CanEnable();

        /// <summary>
        /// Обновить
        /// </summary>
        public void Update() => Enable = CanEnable();

        public virtual List<PropertyInfo> GetVisualProeprties()
        {
            return new()
            {
               GetType().GetProperty(nameof(Name))
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new(name));

        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Читает ключ с диска. Возможность работы с внутренними и внешними дисками
    /// </summary>
    public class KeyPathByDrive : KeyPathReaderBase
    {
        public KeyPathByDrive() : base() 
        {
            DriveLabel = "C://";
            DriveName = "";
            Path = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя класса</param>
        /// <param name="drive">Диск</param>
        /// <param name="path">Путь до файла без диска</param>
        public KeyPathByDrive(string name, DriveInfo drive, string path)
        {
            Name = name;
            DriveLabel = drive.VolumeLabel;
            DriveName = drive.Name;
            Path = path;
        }

        private string driveLabel;
        /// <summary>
        /// Имя диска. Если диск внутренний - нет
        /// </summary>
        public string DriveLabel
        {
            get => driveLabel;
            set
            {
                driveLabel = value;
                OnPropertyChanged();
            }
        }

        private string driveName;
        /// <summary>
        /// Имя раздела
        /// </summary>
        public string DriveName
        {
            get => driveName;
            set
            {
                driveName = value;
                OnPropertyChanged();
            }
        }

        private string path;
        /// <summary>
        /// Путь до файла
        /// </summary>
        public string Path 
        {
            get => path;
            set
            {
                path = value;
                OnPropertyChanged();
            }
        }

        public override string GetKey()
        {
            UpdateDriveName();

            if (!File.Exists(DriveName + Path))
                throw new FileNotFoundException(DriveName + Path);

            try
            {
                return File.ReadAllText(DriveName + Path);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override bool CanEnable()
        {
            UpdateDriveName();
            return File.Exists(DriveName + Path);
        }

        public override List<PropertyInfo> GetVisualProeprties()
        {
            var res = base.GetVisualProeprties();
            Type type = this.GetType();
            res.Add(type.GetProperty(nameof(DriveLabel)));
            res.Add(type.GetProperty(nameof(DriveName)));
            res.Add(type.GetProperty(nameof(Path)));
            return res;
        }

        /// <summary>
        /// Обновление имени раздела по имени диска
        /// </summary>
        private void UpdateDriveName()
        {
            if (DriveLabel != null)
            {
                var drives = DriveInfo.GetDrives();
                var drive = drives.Where(i => i.VolumeLabel == DriveLabel).FirstOrDefault();
                if (drive != null)
                    DriveName = drive.Name;
            }
        }
    }

}
