using System.Security.Cryptography;
using System.Text;

namespace BncPayments.Services
{
    public interface IHashService
    {
        string CreateSHA256Hash(string input);
        string ExampleBnc();
    }
    public class HashServices : IHashService
    {
        public string CreateSHA256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string ExampleBnc()
        {
            string textToHash = "Texto a calcular Hash";
            string hash = String.Empty;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(textToHash));
                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
