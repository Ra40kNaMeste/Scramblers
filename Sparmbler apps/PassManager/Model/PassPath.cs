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

        public async Task<Dictionary<string, byte[]>> ReadPassAsync()
        {
            return await Task.Run(ReadPass);
        }
        public abstract Dictionary<string, byte[]> ReadPass();

        public async Task WritePassAsync(Dictionary<string, byte[]> pass)
        {
            await Task.Run(()=>WritePass(pass));
        }
        public abstract void WritePass(Dictionary<string, byte[]> pass);
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

        public override Dictionary<string, byte[]> ReadPass()
        {
            UpdateDriveName();

            if (!File.Exists(Path.Path))
                throw new FileNotFoundException(Path.Path);


            try
            {
                var temp = File.ReadAllText(Path.Path);
                if(temp.Length > 0)
                    return temp.Split('\n').ToDictionary(i =>i.Split('\t')[1], i => Convert.FromBase64String(i.Split('\t')[0]));
                throw new FileLoadException(Path.Path);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void WritePass(Dictionary<string, byte[]> pass)
        {
            UpdateDriveName();

            if (!File.Exists(Path.Path))
                throw new FileNotFoundException(Path.Path);

            try
            {
                string str = "";
                foreach (var item in pass)
                {
                    var errsymb= new string[] {"\t","\r", "\n"};
                    string key = item.Key, value = Convert.ToBase64String(item.Value);
                    foreach (var c in errsymb)
                    {
                        key = key.Replace(c, string.Empty);
                        value = value.Replace(c, string.Empty);
                    }
                    str += string.Format("{0}\t{1}\n", value, key);
                }
                int strLen = str.Length;

                //Удаляем последний перенос строки
                if (strLen > 0)
                    strLen--;
                File.WriteAllText(Path.Path, str.Substring(0, strLen));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
