using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace VPNBuddy
{
    public class SecurityManager
    {
        private X509Certificate2 _cert;
        private readonly RSACryptoServiceProvider _rsaProvider;

        public static string signingKeyName = "key.pfx";

        public SecurityManager(string password)
        {
            try
            {
                _cert = new X509Certificate2(signingKeyName, password, X509KeyStorageFlags.MachineKeySet);
                _rsaProvider = (RSACryptoServiceProvider) _cert.PrivateKey;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading certificate, either incorrect password, missing file or key settings. Follow the instructions in README.md. \r\n" +
                    ex.Message);
            }
        }

        public string Decrypt(string value)
        {
            byte[] valueBytes = Encoding.Unicode.GetBytes(value);
            return Encoding.Unicode.GetString(_rsaProvider.Decrypt(valueBytes, true));
        }

        public string Encrypt(string password)
        {
            byte[] valueBytes = Encoding.Unicode.GetBytes(password);
            return Encoding.Unicode.GetString(_rsaProvider.Encrypt(valueBytes, true));
        }
    }
}
