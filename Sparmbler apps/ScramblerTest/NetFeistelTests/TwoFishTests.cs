using Scrambler.NetFeistel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScramblerTest.NetFeistelTests
{
    [TestClass]
    public class TwoFishTests
    {
        [TestMethod]
        public void OnDefaultTest()
        {
            Random random = new Random();
            byte[] key = new byte[32];
            random.NextBytes(key);
            Twofish al = new();
            al.KeySize = 256;
            al.BlockSize = 128;
            al.SetKey(key);
            var values = new int[] { 4, 7, 8, 9 };
            var bytes = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                BitConverter.GetBytes(values[i]).CopyTo(bytes, i * 4);
            }

            var code = al.Encoding(bytes, 0);

            var res = al.Decoding(code, 0);
            for (int i = 0; i < res.Length; i++)
            {
                Assert.IsTrue(res[i] == bytes[i]);
            }
        }
        [TestMethod]
        public void OnKeySize128Test()
        {
            Random random = new Random();
            byte[] key = new byte[32];
            random.NextBytes(key);
            Twofish al = new();
            al.KeySize = 128;
            al.BlockSize = 128;
            al.SetKey(key);
            var values = new int[] { 4, 7, 8, 9 };
            var bytes = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                BitConverter.GetBytes(values[i]).CopyTo(bytes, i * 4);
            }

            var code = al.Encoding(bytes, 0);

            var res = al.Decoding(code, 0);
            for (int i = 0; i < res.Length; i++)
            {
                Assert.IsTrue(res[i] == bytes[i]);
            }
        }
        [TestMethod]
        public void OnKeySize192Test()
        {
            Random random = new Random();
            byte[] key = new byte[32];
            random.NextBytes(key);
            Twofish al = new();
            al.KeySize = 192;
            al.BlockSize = 128;
            al.SetKey(key);
            var values = new int[] { 4, 7, 8, 9 };
            var bytes = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                BitConverter.GetBytes(values[i]).CopyTo(bytes, i * 4);
            }

            var code = al.Encoding(bytes, 0);

            var res = al.Decoding(code, 0);
            for (int i = 0; i < res.Length; i++)
            {
                Assert.IsTrue(res[i] == bytes[i]);
            }
        }

        [TestMethod]
        public void OnTwofishAsSimmetricAlgoritm()
        {
            Random random = new Random();
            byte[] key = new byte[32];
            random.NextBytes(key);
            using Twofish twofish = new();
            twofish.BlockSize = 128;
            twofish.KeySize = 256;
            twofish.SetKey(key);

            string text = "Hello World!";
            byte[] bytes = UTF32Encoding.UTF8.GetBytes(text);

            //Формирование стрима-памяти
            int sizeMemory = ((bytes.Length-1) / 16 + 1) * 16;
            byte[] memoryBuffer = new byte[sizeMemory];
            using MemoryStream memory = new(memoryBuffer);

            //Шифруем
            using CryptoStream encoder = new(memory, twofish.CreateEncryptor(), CryptoStreamMode.Write);
            encoder.Write(bytes, 0, bytes.Length);
            encoder.FlushFinalBlock();

            //Читаем, то зашифровали
            memory.Position = 0;
            byte[] result = new byte[bytes.Length];

            using CryptoStream decoder = new(memory, twofish.CreateDecryptor(), CryptoStreamMode.Read);
            decoder.Read(result, 0, result.Length);
            decoder.FlushFinalBlock();

            string res = UTF32Encoding.UTF8.GetString(result);
            Assert.AreEqual(text, res);
        }

        [TestMethod]
        public void OnTwofishAsSimmetricAlgoritmLongWord()
        {
            Random random = new Random();
            byte[] key = new byte[32];
            random.NextBytes(key);
            using Twofish twofish = new();
            twofish.BlockSize = 128;
            twofish.KeySize = 256;
            twofish.SetKey(key);

            string text = "Hello World! My name is Billy. I don't know";
            byte[] bytes = UTF32Encoding.UTF8.GetBytes(text);

            //Формирование стрима-памяти
            int sizeMemory = ((bytes.Length - 1) / 16 + 1) * 16;
            byte[] memoryBuffer = new byte[sizeMemory];
            using MemoryStream memory = new(memoryBuffer);

            //Шифруем
            using CryptoStream encoder = new(memory, twofish.CreateEncryptor(), CryptoStreamMode.Write);
            encoder.Write(bytes, 0, bytes.Length);
            encoder.FlushFinalBlock();

            //Читаем, то зашифровали
            memory.Position = 0;
            byte[] result = new byte[1024];

            using CryptoStream decoder = new(memory, twofish.CreateDecryptor(), CryptoStreamMode.Read);
            int len = decoder.Read(result, 0, 1024);
            using MemoryStream resMemory = new();
            do
            {
                resMemory.Write(result, 0, len);
                len = decoder.Read(result, 0, 1024);
            } while (len>0);
            decoder.Flush();
            //Удаление нулей
            string res = UTF32Encoding.UTF8.GetString(resMemory.ToArray().Reverse().SkipWhile(i => i == 0).Reverse().ToArray());
            Assert.AreEqual(text, res);
        }

    }
}
