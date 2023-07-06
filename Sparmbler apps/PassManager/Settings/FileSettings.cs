using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Settings
{
    
    internal class FileSettings
    {
        public string[] PassFileTypes { get; set; }
        public string[] PassTypes { get; set; }
    }
    internal class SaveUserPathsListSettings
    {
        public string PassFilesXAMLPath { get; set; }
    }
    internal class Settings
    {
        public string DataFolder { get; set; }
    }
}
