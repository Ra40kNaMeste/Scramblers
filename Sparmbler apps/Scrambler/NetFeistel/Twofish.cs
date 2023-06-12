using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Scrambler.NetFeistel
{
    public class Twofish : SymmetricAlgorithm
    {
        public Twofish()
        {
            BlockSizeValue = 128;
            LegalBlockSizesValue = new KeySizes[] { new KeySizes(128, 128, 0) };
            LegalKeySizesValue = new KeySizes[] { new KeySizes(128, 256, 64) };
        }

        private int[] keys = new int[40];

        private int[]? S;

        private static byte[,] MDS =
        {
            {0x01, 0xEF, 0x5B, 0x5B },
            {0x5B, 0xEF, 0xEF, 0x01 },
            {0xEF, 0x5B, 0x01, 0xEF },
            {0xEF, 0x01, 0xEF, 0x5B }
        };

        private static byte[,] RS =
        {
            {0x01, 0xA4, 0x55, 0x87, 0x5A, 0x58, 0xDB, 0x9E },
            {0xA4, 0x56, 0x82, 0xF3, 0x1E, 0xC6, 0x68, 0xE5 },
            {0x02, 0xA1, 0xFC, 0xC1, 0x47, 0xAE, 0x3D, 0x19 },
            {0xA4, 0x55, 0x87, 0x5A, 0x58, 0xDB, 0x9E, 0x03 }
        };


        private static byte[,] t =
        {
            {8, 1, 7, 13, 6, 15, 3, 2, 0, 11, 5, 9, 14, 12, 10, 4 },
            {14, 12, 11, 8, 1, 2, 3, 5, 15, 4, 10, 6, 7, 0, 9, 13 },
            {11, 10, 5, 14, 6, 13, 9, 0, 12, 8, 15, 3, 2, 4, 7, 1 },
            { 13, 7, 15, 4, 1, 2, 6, 14, 9, 11, 3, 0, 8, 5, 12, 10 },
            { 2, 8, 11, 13, 15, 7, 6, 14, 3, 1, 9, 4, 0, 10, 12, 5 },
            { 1, 14, 2, 11, 4, 12, 3, 7, 6, 13, 10, 5, 15, 9, 0, 8 },
            { 4, 12, 7, 5, 1, 6, 9, 10, 0, 14, 13, 8, 2, 11, 3, 15 },
            { 11, 9, 5, 1, 12, 3, 13, 14, 6, 4, 7, 15, 2, 0, 8, 10 }
        };

        public void SetKey(byte[] key)
        {
            int[] Me = new int[KeySize / 16];
            int[] Mo = new int[KeySize / 16];
            for (int i = 0; i < KeySize / 64; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Me[i * 4 + j] = key[2 * i + j];
                    Mo[i * 4 + j] = key[2 * i + 4 + j];
                }
            }
            int r = 0x1010101;
            for (int i = 0; i < 20; i++)
            {
                var A = hFunc(Me, 2 * i * r);
                var B = hFunc(Mo, (2 * i + 1) * r) << 8;
                keys[2 * i] = A + B;
                keys[2 * i + 1] = (A + 2 * B) << 9;
            }
            S = new int[KeySize / 64];
            for (int i = 0; i < KeySize / 64; i++)
            {
                int t = 0;
                int res = 0;
                for (int j = 0; j < 4; j++)
                {
                    res <<= 8;
                    int sum = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        sum += RS[j, k] * keys[i * 8 + k];
                    }
                    res += sum % byte.MaxValue;
                }
                S[t++] = res;
            }

        }

        public byte[] Encoding(byte[] word, int offset)
        {
            int[] copyWord = new int[4];
            for (int i = 0; i < 4; i++)
            {
                copyWord[i] = BitConverter.ToInt32(word, offset + 4 * i) ^ keys[i];
            }
            for (int i = 0; i < 16; i++)
            {
                int a = gFunc(copyWord[0]) + gFunc(copyWord[1] << 8);
                int w2 = (copyWord[2] ^ (a + keys[i * 2 + 8]));
                int w3 = (copyWord[3]) ^ (a + keys[i * 2 + 9]);
                copyWord[2] = copyWord[0];
                copyWord[3] = copyWord[1];
                copyWord[0] = w2;
                copyWord[1] = w3;
            }

            byte[] res = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                BitConverter.GetBytes(copyWord[i] ^ keys[4 + i]).CopyTo(res, i * 4);
            }
            return res;
        }
        public byte[] Decoding(byte[] word, int offset)
        {
            int[] copyWord = new int[4];
            for (int i = 0; i < 4; i++)
            {
                copyWord[i] = BitConverter.ToInt32(word, offset + i * 4) ^ keys[4 + i];
            }
            for (int i = 15; i >= 0; i--)
            {
                int w2 = copyWord[2];
                int w3 = copyWord[3];
                copyWord[2] = copyWord[0];
                copyWord[3] = copyWord[1];
                copyWord[0] = w2;
                copyWord[1] = w3;
                int a = gFunc(copyWord[0]) + gFunc(copyWord[1] << 8);
                copyWord[2] = (copyWord[2]) ^ (a + keys[i * 2 + 8]);
                copyWord[3] = (copyWord[3] ^ (a + keys[i * 2 + 9]));

            }

            byte[] res = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                BitConverter.GetBytes(copyWord[i] ^ keys[i]).CopyTo(res, i * 4);
            }
            return res;
        }

        private int gFunc(int word)
        {
            return hFunc(S, word);
        }

        private int hFunc(int[] rnd, int word)
        {
            var wordBytes = BitConverter.GetBytes(word);
            for (int i = 0; i < wordBytes.Length / 4; i++)
            {
                hFuncStep(wordBytes, rnd, i * 4);
            }
            int res = 0;
            for (int i = 0; i < 4; i++)
            {
                int sum = 0;
                for (int j = 0; j < 4; j++)
                {
                    sum += wordBytes[j] * MDS[i, j];
                }
                res += sum % byte.MaxValue;
                res <<= 8;
            }
            return res;
        }
        private static void hFuncStep(byte[] value, int[] rnd, int start)
        {
            int max = rnd.Length;
            for (int i = start; i < start + 4; i++)
            {
                for (int j = 0; j < max; j++)
                {
                    value[i] = qTranposition(value[i], Math.Abs(rnd[j] % 2));
                }
            }
        }
        private static byte qTranposition(byte b, int i)
        {
            int a0 = b / 16;
            int b0 = b % 16;
            int a1 = a0 ^ b0;
            int b1 = a0 ^ (b0 >> 4) ^ (8 * a0 % 16);
            int a2 = t[4 * i + 0, a1];
            int b2 = t[4 * i + 1, b1];
            int a3 = a2 ^ b2;
            int b3 = a2 ^ (b2 >> 4) ^ (8 * a2 % 16);
            int a4 = t[4 * i + 2, a3];
            int b4 = t[4 * i + 3, b3];
            return Convert.ToByte(16 * b4 + a4);

        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV)
        {
            return new TwofishTransform(Decoding);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV)
        {
            return new TwofishTransform(Encoding);
        }

        public override void GenerateIV()
        {
            IV = new byte[BlockSize / 8];
            for (int i = 0; i < BlockSize / 8; i++)
                IV[i] = 0;
        }

        public override void GenerateKey()
        {
            Key = new byte[KeySize / 8];
            for (int i = Key.GetLowerBound(0); i < Key.GetUpperBound(0); i++)
                Key[i] = 0;
        }
    }

    public class TwofishTransform : ICryptoTransform
    {
        public TwofishTransform(Func<byte[], int, byte[]> trans)
        {
            _trans = trans;
        }

        private Func<byte[], int, byte[]> _trans;
        public bool CanReuseTransform => true;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => 16;

        public int OutputBlockSize => 16;



        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            //if (inputCount != 16)
            //    throw new ArgumentException("inputOffset");
            int max = inputCount / 16;
            for (int i = 0; i < max; i++)
            {
                _trans(inputBuffer, inputOffset + i * 16).CopyTo(outputBuffer, outputOffset + i * 16);
            }
            return max*16;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount == 0)
                return new byte[0];
            return _trans(inputBuffer, inputOffset);
        }

        public void Dispose()
        {

        }
    }
}
