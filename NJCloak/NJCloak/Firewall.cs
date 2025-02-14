using System.Diagnostics;

namespace NJCloak {
    public class Firewall {

        public static void Enable() {
            ShellExecute("netsh", "advfirewall firewall delete rule all");
            ShellExecute("netsh", "advfirewall set allprofiles firewallpolicy blockinbound,blockoutbound");
        }

        public static void Reset() {
            ShellExecute("netsh", "advfirewall reset");
        }

        public static void WhitelistApp(string path) { // Identify rule name by hashsum cause we don't wanna parse all rules later
            ShellExecute("netsh", "advfirewall firewall add rule name=\"NJCloak-" + path.GetHashCode().ToString("X0") + "\" dir=out action=allow program=\"" + path + "\"");
        }

        public static void RemoveApp(string path) {
            ShellExecute("netsh", "advfirewall firewall delete rule name=\"NJCloak-" + path.GetHashCode().ToString("X0") + "\"");
        }

        public static void ShellExecute(string cmd, string args) {
            try {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = cmd;
                startInfo.Arguments = args;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                Process.Start(startInfo);
            } catch { }
        }
    }
}