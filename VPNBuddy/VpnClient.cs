namespace VPNBuddy
{
    using System;
    using System.Net;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Handles connecting to the Vpn
    /// </summary>
    public class VpnSessionHost
    {
        /// <summary>
        /// Vpn configuration
        /// </summary>
        private readonly VpnData _vpnConfig;

        private Process _vpnProcess;

        /// <summary>
        /// Create a new Instance of the Vpn Client
        /// </summary>
        /// <param name="vpnConfig">Vpn configuration</param>
        public VpnSessionHost(VpnData vpnConfig)
        {
            _vpnConfig = vpnConfig;
        }

        /// <summary>
        /// Vpn Client exe name
        /// </summary>
        private const string VpnCliExe = "vpncli.exe";

        /// <summary>
        /// Connect to Vpn with the following credentials
        /// </summary>
        /// <param name="credentials">Vpn credentials</param>
        /// <param name="vpnHost">Vpn Host url</param>
        /// <returns>Output of Vpn connection</returns>
        public string Connect(ICredentials credentials, string vpnHost)
        {
            var vpnCliExePath = Path.Combine(VpnData.VpnClientInstallationPath, VpnCliExe);
            // Get rid of any existing vpn client running
            Disconnect();

            var cred = credentials.GetCredential(new Uri("http://localhost"), String.Empty);

            var vpnProcessInfo = new ProcessStartInfo
            {
                Arguments = string.Format(@"-s "),
                FileName = vpnCliExePath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            _vpnProcess = Process.Start(vpnProcessInfo);
            Uri vpnUri = new Uri(vpnHost);
            _vpnProcess.StandardInput.WriteLine(string.Format("connect {0}", vpnUri.Host));
            _vpnProcess.StandardInput.WriteLine(cred.UserName);
            _vpnProcess.StandardInput.WriteLine(cred.Password);
            _vpnProcess.StandardInput.Close();

            return _vpnProcess.StandardOutput.ReadToEnd();
        }

        /// <summary>
        /// Disconnects the vpn
        /// </summary>
        public void Disconnect()
        {
            if (_vpnProcess != null && !_vpnProcess.HasExited)
            {
                _vpnProcess.Kill();
                _vpnProcess = null;
            }

            var vpnCliExePath = Path.Combine(VpnData.VpnClientInstallationPath, VpnCliExe);

            var vpnCliProcessInfo = new ProcessStartInfo
            {
                Arguments = "disconnect",
                UseShellExecute = false,
                FileName = vpnCliExePath,
            };
            var vpnCliProcess = Process.Start(vpnCliProcessInfo);

            if (vpnCliProcess != null)
            {
                vpnCliProcess.WaitForExit(1000);
                if (vpnCliProcess != null && !vpnCliProcess.HasExited)
                {
                    vpnCliProcess.Kill();
                }
            }
        }

        /// <summary>
        /// Dispose the vpn connection
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
    }
}
