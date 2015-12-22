namespace VPNBuddy
{
    public class VpnData
    {
        public string Name { get; set; }
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Display
        {
            get { return string.Format("{0} [{1}]", Name, HostName); }
        }

        public static string VpnClientInstallationPath = @"C:\Program Files (x86)\Cisco\Cisco AnyConnect Secure Mobility Client\";
    }
}