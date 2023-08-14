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
    public abstract class PassPathReaderBase : PathReaderBase
    {

        public async Task<IEnumerable<Password>> ReadPassAsync()
        {
            return await Task.Run(ReadPass);
        }
        public abstract IEnumerable<Password> ReadPass();

        public async Task WritePassAsync(IEnumerable<Password> pass)
        {
            await Task.Run(() => WritePass(pass));
        }
        public abstract void WritePass(IEnumerable<Password> pass);
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

        public override IEnumerable<Password> ReadPass()
        {
            UpdateDriveName();

            if (!File.Exists(Path.Path))
                throw new FileNotFoundException(Path.Path);


            try
            {
                var temp = File.ReadAllText(Path.Path);
                if (temp.Length > 0)
                    return temp.Split('\n').Select(p =>
                    {
                        try
                        {
                            var temp = p.Split('\t');
                            return new Password()
                            {
                                Id = Convert.ToInt32(temp[0]),
                                Name = temp[1],
                                Value = Convert.FromBase64String(temp[2])
                            };
                        }
                        catch (Exception)
                        {
                            return new Password();
                        }
                    });
                if (temp.Length == 0)
                    return new List<Password>();
                throw new FileLoadException(Path.Path);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override void WritePass(IEnumerable<Password> pass)
        {
            UpdateDriveName();

            if (!File.Exists(Path.Path))
                throw new FileNotFoundException(Path.Path);

            try
            {
                string str = "";
                foreach (var item in pass)
                {
                    var errsymb = new string[] { "\t", "\r", "\n" };
                    string key = item.Name, value = Convert.ToBase64String(item.Value), id = item.Id.ToString();
                    foreach (var c in errsymb)
                    {
                        key = key.Replace(c, string.Empty);
                    }
                    str += string.Format("{0}\t{1}\t{2}\n", id, key, value);
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

    [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
    public class PassPathBySqlite : PassPathReaderBase, IPathByDriveable, IDisposable
    {

        private PathByDrive path;
        public PathByDrive Path
        {
            get => path;
            set
            {
                path = value;
                if (Context != null)
                {
                    Context.SaveChanges();
                    Context.Dispose();
                }
                Context = new(path.Path);
            }
        }

        private PassDBContext Context { get; set; }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public override IEnumerable<Password> ReadPass()
        {
            return Context.Passwords;
        }

        public override void WritePass(IEnumerable<Password> pass)
        {
            var old = Context.Passwords;
            var newDates = pass.Where(i => !old.Contains(i));
            var deleteDates = old.Where(i => !pass.Contains(i));
            Context.Passwords.AddRange(newDates);
            Context.Passwords.RemoveRange(deleteDates);
            Context.UpdateRange(pass);
            Context.SaveChanges();
        }

        protected override bool CanEnable()
        {
            throw new NotImplementedException();
        }
    }
}
