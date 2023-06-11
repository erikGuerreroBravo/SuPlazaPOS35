using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DsiCodeTech.Common.Security
{
    public sealed class AesCrypto
    {
        private static readonly byte[] KEY = new byte[] { 44, 73, 31, 74, 33, 63, 48, 50, 34, 72, 40, 33, 31, 48, 30, 67 };
        private static readonly byte[] IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private readonly Aes managed = Aes.Create();

        public AesCrypto()
        {
            managed.Mode = CipherMode.CBC;
            managed.Key = KEY;
            managed.IV = IV;
        }

        public string Encrypt(string PlainText)
        {
            byte[] byteTextEncrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = managed.Key;
                aes.IV = managed.IV;

                ICryptoTransform cryptoTransform = this.managed.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cryptoStream))
                        {
                            sw.Write(PlainText);
                        }
                        byteTextEncrypted = ms.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(byteTextEncrypted);
        }

        public string Decrypt(string EncryptText)
        {
            string plainText;
            using (Aes aes = Aes.Create())
            {
                aes.Key = managed.Key;
                aes.IV = managed.IV;

                ICryptoTransform cryptoTransform = this.managed.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(EncryptText)))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cryptoStream))
                        {
                            plainText = sr.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }
    }
}
