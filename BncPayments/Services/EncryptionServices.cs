using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography;
using System.Text;

namespace BncPayments.Services
{
    public interface IEncryptionServices
    {
        string DecryptText(string cypheredText, string masterKey);
        string Encrypt(string plainText, string key);
        string EncryptBnc(string deCypheredText, string encryptionKey);
        void ExampleBnc(string text, string masterKey);
    }
    public class EncryptionServices : IEncryptionServices
    {

        public string Encrypt(string plainText, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();
                var iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                    var result = new byte[iv.Length + encryptedBytes.Length];
                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }

        public string EncryptBnc(string deCypheredText, string encryptionKey)
        {
            string cypheredText = String.Empty;
            //string deCypheredText = "Texto en claro para pruebas de encriptación";
            string testText = String.Empty;
            //string encryptionKey = "LlaveDePrueba";

            Aes encryptor = Aes.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            byte[] clearBytes = Encoding.Unicode.GetBytes(deCypheredText);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length); cs.Close();
                }
                cypheredText = Convert.ToBase64String(ms.ToArray());
            }

            return cypheredText;
        }

        public string DecryptText(string cypheredText, string masterKey)
        {
            Aes encryptor = Aes.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(masterKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            byte[] cipherBytes = Convert.FromBase64String(cypheredText);

            var deCypheredText = string.Empty;


            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                deCypheredText = Encoding.Unicode.GetString(ms.ToArray());
            }

            return deCypheredText;
        }
        public void ExampleBnc(string text, string masterKey)
        {
            string cypheredText = String.Empty;
            string deCypheredText = "Texto en claro para pruebas de encriptación";
            string testText = String.Empty;
            string encryptionKey = "LlaveDePrueba";

            Aes encryptor = Aes.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);

            encryptor.IV = pdb.GetBytes(16);
            byte[] clearBytes = Encoding.Unicode.GetBytes(deCypheredText);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length); cs.Close();
                }
                cypheredText = Convert.ToBase64String(ms.ToArray());
            }

            byte[] cipherBytes = Convert.FromBase64String(cypheredText);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                deCypheredText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }

    }
}
