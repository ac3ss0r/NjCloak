using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NJCloak.Modules {

    public class PeripheralDevice {

        public string Name { get; private set; }
        public string DeviceId { get; private set; }
        public string Category { get; private set; }
        public bool Enabled { get; private set; }

        public PeripheralDevice(string name, string deviceId, string category, bool enabled) {
            Name = name;
            DeviceId = deviceId;
            Category = category;
            Enabled = enabled;
        }

        public PeripheralDevice WithState(bool enabled) {
            return new PeripheralDevice(Name, DeviceId, Category, enabled);
        }
    }

    public static class Periphery {

        private const string ScopePath = @"root\cimv2";
        private const int SettleAttempts = 20;
        private const int SettleDelay = 250;

        // cfgmgr32.h: CM_PROB_DISABLED, shown by Device Manager as "This device has been disabled".
        private const uint CmProbDisabled = 22;

        // setupapi.h / cfgmgr32.h
        private const int DifPropertyChange = 0x00000013;
        private const uint DicsEnable = 0x00000001;
        private const uint DicsDisable = 0x00000002;
        private const uint DicsFlagGlobal = 0x00000001;
        private const uint DigfAllClasses = 0x00000004;
        private const int ErrorCallNotImplemented = 120;
        private const int CrSuccess = 0x00000000;
        private const int CrInvalidDevNode = 0x00000005;
        private const int CrNoSuchDevNode = 0x0000000D;
        private const int CrAccessDenied = 0x0000001F;
        private static readonly IntPtr InvalidHandle = new IntPtr(-1);

        public static List<PeripheralDevice> GetAllPeripherals() {
            List<PeripheralDevice> devices = new List<PeripheralDevice>();
            devices.AddRange(Query("SELECT * FROM Win32_PnPEntity WHERE PNPClass='AudioEndpoint'", "Audio"));
            devices.AddRange(Query("SELECT * FROM Win32_PnPEntity WHERE PNPClass='Camera'", "Webcam"));
            return devices;
        }

        // Returns the state the device actually ended up in, which is not always the one that was
        // asked for: some drivers only apply a disablement after a restart.
        public static bool ApplyDeviceState(string deviceId, bool enable) {
            ChangeState(deviceId, enable);
            for (int attempt = 0; attempt < SettleAttempts; attempt++) {
                bool? state = TryGetState(deviceId);
                if (state.HasValue && state.Value == enable) return state.Value;
                Thread.Sleep(SettleDelay);
            }
            bool? settled = TryGetState(deviceId);
            return settled.HasValue ? settled.Value : enable;
        }

        // Checked state comes from the PnP manager rather than from its WMI mirror, which keeps
        // reporting the previous state for a moment after a change.
        public static bool? TryGetState(string deviceId) {
            uint devInst;
            if (CM_Locate_DevNode(out devInst, deviceId, 0) != CrSuccess) return null;
            uint status, problem;
            if (CM_Get_DevNode_Status(out status, out problem, devInst, 0) != CrSuccess) return null;
            return problem != CmProbDisabled;
        }

        private static void ChangeState(string deviceId, bool enable) {
            Exception classInstallerError;
            if (TryClassInstaller(deviceId, enable, out classInstallerError)) return;
            Exception configManagerError;
            if (TryConfigManager(deviceId, enable, out configManagerError)) return;
            // A driver that does not implement DIF_PROPERTYCHANGE has nothing useful to say about
            // it, so its fallback is the error that explains the failure. Any other class installer
            // failure (access denied for instance) is the real reason and must not be hidden.
            if (IsNotImplemented(classInstallerError)) throw configManagerError ?? classInstallerError;
            throw classInstallerError;
        }

        private static bool IsNotImplemented(Exception error) {
            Win32Exception win32 = error as Win32Exception;
            return win32 != null && win32.NativeErrorCode == ErrorCallNotImplemented;
        }

        // This is the request Device Manager sends for its "Enable device"/"Disable device" menu
        // entries: a property change handed to the class installer of the devnode.
        private static bool TryClassInstaller(string deviceId, bool enable, out Exception failure) {
            failure = null;
            IntPtr infoSet = SetupDiGetClassDevs(IntPtr.Zero, null, IntPtr.Zero, DigfAllClasses);
            if (infoSet == InvalidHandle) {
                failure = new Win32Exception(Marshal.GetLastWin32Error());
                return false;
            }
            try {
                SpDevinfoData data = new SpDevinfoData();
                data.cbSize = Marshal.SizeOf(typeof(SpDevinfoData));
                for (uint index = 0; SetupDiEnumDeviceInfo(infoSet, index, ref data); index++) {
                    if (!IsDevice(infoSet, ref data, deviceId)) continue;

                    SpPropchangeParams parameters = new SpPropchangeParams();
                    parameters.cbSize = Marshal.SizeOf(typeof(SpPropchangeParams));
                    parameters.InstallFunction = DifPropertyChange;
                    parameters.StateChange = enable ? DicsEnable : DicsDisable;
                    parameters.Scope = DicsFlagGlobal;
                    parameters.HardwareProfile = 0;

                    if (!SetupDiSetClassInstallParams(infoSet, ref data, ref parameters, parameters.cbSize)) {
                        failure = new Win32Exception(Marshal.GetLastWin32Error());
                        return false;
                    }
                    if (!SetupDiCallClassInstaller(DifPropertyChange, infoSet, ref data)) {
                        failure = new Win32Exception(Marshal.GetLastWin32Error());
                        return false;
                    }
                    return true;
                }
                failure = new InvalidOperationException("The device is no longer present on the system.");
                return false;
            } finally {
                SetupDiDestroyDeviceInfoList(infoSet);
            }
        }

        // Devices whose class installer does not implement DIF_PROPERTYCHANGE report
        // ERROR_CALL_NOT_IMPLEMENTED; the PnP manager entry points still reach them.
        private static bool TryConfigManager(string deviceId, bool enable, out Exception failure) {
            failure = null;
            uint devInst;
            int code = CM_Locate_DevNode(out devInst, deviceId, 0);
            if (code != CrSuccess) {
                failure = ConfigManagerError(code);
                return false;
            }
            code = enable ? CM_Enable_DevNode(devInst, 0) : CM_Disable_DevNode(devInst, 0);
            if (code != CrSuccess) {
                failure = ConfigManagerError(code);
                return false;
            }
            return true;
        }

        private static bool IsDevice(IntPtr infoSet, ref SpDevinfoData data, string deviceId) {
            StringBuilder id = new StringBuilder(512);
            int required;
            if (!SetupDiGetDeviceInstanceId(infoSet, ref data, id, id.Capacity, out required)) return false;
            return string.Equals(id.ToString(), deviceId, StringComparison.OrdinalIgnoreCase);
        }

        private static Exception ConfigManagerError(int code) {
            if (code == CrNoSuchDevNode || code == CrInvalidDevNode) {
                return new InvalidOperationException("The device is no longer present on the system.");
            }
            if (code == CrAccessDenied) return new Win32Exception(5);
            return new InvalidOperationException("Windows refused to change the device state (code " + code + ").");
        }

        private static List<PeripheralDevice> Query(string wql, string category) {
            List<PeripheralDevice> devices = new List<PeripheralDevice>();
            ManagementScope scope = Scope();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery(wql)))
            using (ManagementObjectCollection found = searcher.Get()) {
                foreach (ManagementBaseObject device in found) {
                    string name = device["Name"] as string;
                    string id = device["DeviceID"] as string;
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id)) continue;
                    devices.Add(new PeripheralDevice(name, id, category, ResolveState(id, device)));
                }
            }
            devices.Sort(delegate (PeripheralDevice a, PeripheralDevice b) {
                return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
            });
            return devices;
        }

        // Devices are described by plain objects: a ManagementObject read after its searcher has
        // been disposed (or from another thread) faults the whole process.
        private static ManagementScope Scope() {
            ManagementScope scope = new ManagementScope(ScopePath);
            scope.Options.Timeout = TimeSpan.FromSeconds(30);
            scope.Options.Impersonation = ImpersonationLevel.Impersonate;
            return scope;
        }

        private static bool ResolveState(string deviceId, ManagementBaseObject device) {
            bool? state = TryGetState(deviceId);
            if (state.HasValue) return state.Value;
            return WmiState(device);
        }

        private static bool WmiState(ManagementBaseObject device) {
            object code = device["ConfigManagerErrorCode"];
            if (code != null) {
                try { if (Convert.ToUInt32(code) == CmProbDisabled) return false; }
                catch (FormatException) { }
                catch (InvalidCastException) { }
                catch (OverflowException) { }
            }
            string status = device["Status"] as string;
            if (status == null) return true;
            return status != "Error" && status != "Degraded";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SpDevinfoData {
            public int cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SpPropchangeParams {
            public int cbSize;
            public int InstallFunction;
            public uint StateChange;
            public uint Scope;
            public uint HardwareProfile;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(IntPtr classGuid, string enumerator, IntPtr hwndParent, uint flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, uint memberIndex, ref SpDevinfoData deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SpDevinfoData deviceInfoData,
            StringBuilder deviceInstanceId, int deviceInstanceIdSize, out int requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiSetClassInstallParams(IntPtr deviceInfoSet, ref SpDevinfoData deviceInfoData,
            ref SpPropchangeParams params_, int paramsSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiCallClassInstaller(int installFunction, IntPtr deviceInfoSet, ref SpDevinfoData deviceInfoData);

        [DllImport("setupapi.dll")]
        private static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        private static extern int CM_Locate_DevNode(out uint devInst, string deviceInstanceId, uint flags);

        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Get_DevNode_Status(out uint status, out uint problemCode, uint devInst, uint flags);

        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Enable_DevNode(uint devInst, uint flags);

        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Disable_DevNode(uint devInst, uint flags);
    }
}
