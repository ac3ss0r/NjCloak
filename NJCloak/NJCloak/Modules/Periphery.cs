using System.Collections.Generic;
using System.Management;

namespace NJCloak.Modules {
    public class Periphery {

        public static void Toggle(DeviceType type, bool state) {
            var devices = new List<ManagementObject>();
            switch (type) {
                case DeviceType.MIC:
                    devices.AddRange(GetMicrophones());
                    break;
                case DeviceType.WEBCAM:
                    devices.AddRange(GetWebcams());
                    break;
            }
            foreach (var device in devices) {
                var method = state ? "Enable" : "Disable";
                ManagementBaseObject inParams = device.GetMethodParameters(method);
                device.InvokeMethod(method, inParams, null);
            }
        }

        public static ManagementObject FindDevice(string deciceName) { 
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity")) {
                foreach (ManagementObject device in searcher.Get()) {
                    string name = (string)device.Properties["Name"].Value;
                    if (name != null && name == deciceName) {
                        return device;
                    }
                }
            }
            return null;
        }

        public static List<ManagementObject> GetAllPeripherals() {
            List<ManagementObject> devices = GetMicrophones();
            devices.AddRange(GetWebcams());
            return devices;
        }

        public static List<ManagementObject> GetMicrophones() {
            List<ManagementObject> devices = new List<ManagementObject>();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity")) {
                foreach (ManagementObject device in searcher.Get()) {
                    string cls = (string)device.Properties["PNPClass"].Value,
                           name = (string)device.Properties["Name"].Value;
                    if (cls != null && name != null) {
                        if (cls == "AudioEndpoint") {
                            devices.Add(device);
                        }
                    }
                }
            }
            return devices;
        }

        public static List<ManagementObject> GetWebcams() {
            List<ManagementObject> devices = new List<ManagementObject>();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity")) {
                foreach (ManagementObject device in searcher.Get()) {
                    if ((string)device.Properties["PNPClass"].Value == "Camera") {
                        devices.Add(device);
                    }
                }
            }
            return devices;
        }

    }

    public enum DeviceType {
        MIC, WEBCAM
    }
}
