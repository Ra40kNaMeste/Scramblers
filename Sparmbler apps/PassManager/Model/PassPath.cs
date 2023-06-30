using PassManager.ViewConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model
{
    public abstract class PassPathReaderBase: PathReaderBase
    {

        public async Task<Dictionary<string, string>> ReadPassAsync()
        {
            return await Task.Run(ReadPass);
        }
        public abstract Dictionary<string, string> ReadPass();

        public async Task WritePassAsync(Dictionary<string, string> pass)
        {
            await Task.Run(()=>WritePass(pass));
        }
        public abstract void WritePass(Dictionary<string, string> pass);
    }
    public class PassPathByDrive : PassPathReaderBase
    {

        public PassPathByDrive() : base()
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
        public PassPathByDrive(string name, DriveInfo drive, string path)
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

        public override Dictionary<string, string> ReadPass()
        {
            UpdateDriveName();

            if (!File.Exists(Path))
                throw new FileNotFoundException(Path);

            try
            {
                return File.ReadAllText(Path).Split('\n').ToDictionary(i =>i.Split('\t')[0], i => i.Split('\t')[1]);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void WritePass(Dictionary<string, string> pass)
        {
            UpdateDriveName();
            if (!File.Exists(Path))
                throw new FileNotFoundException(DriveName + Path);

            try
            {
                string str = "";
                foreach (var item in pass)
                {
                    str += $"{0}\t{1}\n";
                }
                File.WriteAllText(Path, str.Substring(0, str.Length - 1));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
