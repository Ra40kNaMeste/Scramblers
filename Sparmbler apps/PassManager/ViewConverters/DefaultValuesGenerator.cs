using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.ViewConverters
{
    public static class DefaultValuesGenerator
    {
        public static string GenerateKeyName(IEnumerable<string> keys)
        {
            string def = Properties.Resources.KeyDefaultName;
            string res;
            int i = 1;
            while (keys.Equals(res = def + i.ToString()))
            {
                i++;
            }
            return res;
        }

        public static string GeneratePass(int size)
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(size));
        }
    }
}
