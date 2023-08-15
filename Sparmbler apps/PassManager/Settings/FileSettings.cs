using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Settings
{
    
    internal class FileSettings
    {
        public string AndroidPassFileType { get; set; }
        public string AndroidKeyFileType { get; set; }
        public string[] KeyFileTypes { get; set; }
        public string[] PassFileTypes { get; set; }
    }
    internal class SaveUserPathsListSettings
    {
        public string PassFilesXAMLPath { get; set; }
        public string KeyFilesXAMLPath { get; set; }
    }
    internal class Settings
    {
        public string DataFolder { get; set; }
    }
}
