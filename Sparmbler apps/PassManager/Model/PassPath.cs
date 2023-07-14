using Newtonsoft.Json;
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


    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PassPathByDrive : PassPathReaderBase, IPathByDriveable
    {

        public PassPathByDrive() : base()
        {
            Path = new();
        }

        private PathByDrive path;
        /// <summary>
        /// Путь до файла
        /// </summary>
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
                    var drive = drives.Where(i => i.VolumeLabel == Path.DriveLabel).FirstOrDefault();
                    if (drive != null)
                    {
                        Path.DriveName = drive.Name;
                    }
                }
            }

        }

        public override Dictionary<string, string> ReadPass()
        {
            UpdateDriveName();

            if (!File.Exists(Path.Path))
                throw new FileNotFoundException(Path.Path);


            try
            {
                var temp = File.ReadAllText(Path.Path).Split('\n');
                return File.ReadAllText(Path.Path).Split('\n').ToDictionary(i =>i.Split('\t')[0], i => i.Split('\t')[1]);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void WritePass(Dictionary<string, string> pass)
        {
            UpdateDriveName();

            if (!File.Exists(Path.Path))
                throw new FileNotFoundException(Path.Path);

            try
            {
                string str = "";
                foreach (var item in pass)
                {
                    str += string.Format("{0}\t{1}\n", item.Key.Replace('\t',' '), item.Value.Replace('\t', ' '));
                }
                File.WriteAllText(Path.Path, str.Substring(0, str.Length - 1));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
