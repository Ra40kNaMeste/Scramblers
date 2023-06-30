﻿using PassManager.ViewConverters;
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

    public interface IKeyPathReader 
    {
        public string ReadKey();
    }

    /// <summary>
    /// Класс, который читает ключ из файла
    /// </summary>
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


    /// <summary>
    /// Читает ключ с диска. Возможность работы с внутренними и внешними дисками
    /// </summary>
    public class KeyPathByDrive : KeyPathReaderBase
    {
        public KeyPathByDrive() : base() 
        {
            DriveLabel = "";
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



        protected override bool CanEnable()
        {
            UpdateDriveName();
            return File.Exists(Path);
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
        public override string ReadKey()
        {
            UpdateDriveName();

            if (!File.Exists(Path))
                throw new FileNotFoundException(Path);

            try
            {
                return File.ReadAllText(Path);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override void WriteKey(string value)
        {
            UpdateDriveName();

            if (!File.Exists(Path))
                throw new FileNotFoundException(Path);

            try
            {
                File.WriteAllText(Path, value);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
