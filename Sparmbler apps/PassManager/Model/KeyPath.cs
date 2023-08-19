using Newtonsoft.Json;
using PassManager.ViewConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PassManager.Model
{

    public interface IKeyPathReader
    {
        public string ReadKey();
    }

    /// <summary>
    /// Класс, который читает ключ из файла
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class PathReaderBase : INotifyPropertyChanged, IGeneratorVisualProperties
    {
        public PathReaderBase()
        {
            Enable = false;
            Name = "";
        }

        /// <summary>
        /// Имя класса
        /// </summary>
        private string name;
        [JsonProperty]
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

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class KeyPathReaderBase : PathReaderBase, IKeyPathReader
    {
        public async Task WriteKeyAsync(string value)
        {
            await Task.Run(() => WriteKey(value));
        }
        public abstract void WriteKey(string value);

        public async Task<string> ReadKeyAsync()
        {
            return await Task.Run(ReadKey);
        }
        public abstract string ReadKey();
    }


    public interface IPathByDriveable
    {
        public PathByDrive Path { get; set; }
    }

    /// <summary>
    /// Читает ключ с диска. Возможность работы с внутренними и внешними дисками
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class KeyPathByDrive : KeyPathReaderBase, IPathByDriveable
    {
        public KeyPathByDrive()
        {
            Path = new();
        }

        private PathByDrive path;
        [JsonProperty]
        public PathByDrive Path
        {
            get => path;
            set
            {
                path = value;
                OnPropertyChanged();
            }
        }

        protected override bool CanEnable()
        {
            UpdateDriveName();
            return File.Exists(Path.Path);
        }

        public override List<PropertyInfo> GetVisualProeprties()
        {
            var res = base.GetVisualProeprties();
            Type type = GetType();
            res.Add(type.GetProperty(nameof(Path)));
            return res;
        }

        /// <summary>
        /// Обновление имени раздела по имени диска
        /// </summary>
        private void UpdateDriveName()
        {
            if (Path.DriveName != null)
            {
                var drives = DriveInfo.GetDrives();
                if (Path.DriveLabel != "")
                {
                    var drive = drives.Where(i => i.IsReady && i.VolumeLabel == Path.DriveLabel).FirstOrDefault();
                    if (drive != null)
                    {
                        Path.DriveName = drive.Name;
                    }
                }
            }
        }
        public override string ReadKey()
        {
            UpdateDriveName();

            if (!File.Exists(Path.Path))
                throw new FileNotFoundException(Path.Path);

            try
            {
                return File.ReadAllText(Path.Path);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override void WriteKey(string value)
        {
            UpdateDriveName();

            if (!File.Exists(Path.Path))
                throw new FileNotFoundException(Path.Path);

            try
            {
                File.WriteAllText(Path.Path, value);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PathByDrive : INotifyPropertyChanged
    {

        public PathByDrive() : base()
        {
            DriveName = "";
            DriveLabel = "";
            Path = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drive">Диск</param>
        /// <param name="path">Путь до файла без диска</param>
        public PathByDrive(DriveInfo drive, string path)
        {
            DriveName = drive.VolumeLabel;
            DriveLabel = drive.Name;
            Path = path;
        }


        /// <summary>
        /// Подстраивается под полный путь
        /// </summary>
        /// <param name="fullPath">Путь</param>
        public void SetPath(string fullPath)
        {
            DriveName = System.IO.Path.GetPathRoot(fullPath);
            var drives = DriveInfo.GetDrives();
            DriveLabel = drives.Where(i => DriveName.Contains(i.Name)).FirstOrDefault()?.VolumeLabel ?? string.Empty;
            Path = fullPath;
        }


        private string driveLabel;
        /// <summary>
        /// Имя диска. Если диск внутренний - нет
        /// </summary>
        [JsonProperty]
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
        ///  раздела
        /// </summary>
        [JsonProperty]
        public string DriveName
        {
            get => driveName;
            set
            {
                driveName = value;
                if (Path != null && Path.Contains('\\'))
                {
                    Path = driveName + Path.Remove(0, Path.IndexOf('\\') + 1);
                }
                OnPropertyChanged();
            }
        }

        private string path;
        /// <summary>
        /// Путь до файла
        /// </summary>
        [JsonProperty]
        public string Path
        {
            get => path;
            set
            {
                path = value;
                OnPropertyChanged();
            }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
