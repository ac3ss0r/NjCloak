using NJCloak.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace NJCloak {
    public partial class MainForm : Form {

        private const int E_ACCESSDENIED = unchecked((int)0x80070005);
        private const int ErrorAccessDenied = 5;

        private bool screenBlockerEnabled = false, clipperDetectorEnabled = false;
        private Dictionary<string, ScreenBlocker> screenBlockers = new Dictionary<string, ScreenBlocker>();
        private Thread clipperDetectorThread;
        private Panel[] tabs;

        private List<PeripheralDevice> peripherals = new List<PeripheralDevice>();
        private bool peripheralsBusy = false;
        private List<FirewallRule> firewallRules = new List<FirewallRule>();
        private bool firewallBusy = false;

        public MainForm() {
            InitializeComponent();
            tabs = new Panel[] {
                screenBlockerTab,
                firewallTab,
                clipperDetectorTab,
                aboutTab,
                peripheryTab
            };
            trayIcon.Icon = Icon;
        }

        private void Form1_Load(object sender, EventArgs e) {
            RefreshPeripherals();
        }

        // Custom table implementation

        private void SelectTab(string name) {
            foreach (var tab in tabs) {
                tab.Visible = tab.Name == name;
            }
            if (name == firewallTab.Name) RefreshFirewallTab();
        }

        private void onTabSelected(object sender, EventArgs e) {
            SelectTab(((ToolStripStatusLabel)sender).Tag as string);
        }

        // Screen blocker

        private void cloakButton_Click(object sender, EventArgs e) {
            cloakButton.Text = screenBlockerEnabled ? "Start blocking" :
                                                       "Stop blocking";
            if (screenBlockerEnabled) {
                LowLevelInputHooks.Disable();
                windowCleanerTimer.Enabled = false;
                blockerGuardTimer.Enabled = false;
                ReleaseScreenBlockers();
                screenBlockerEnabled = false;
            } else {
                if (cleanTitles.Checked) {
                    windowCleanerTimer.Enabled = true;
                }
                if (blockInputs.Checked) {
                    LowLevelInputHooks.Enable();
                }
                foreach (var scr in Screen.AllScreens) {
                    AttachBlocker(scr);
                }
                blockerGuardTimer.Enabled = true;
                screenBlockerEnabled = true;
            }
        }

        private void AttachBlocker(Screen screen) {
            try {
                var blocker = new ScreenBlocker();
                blocker.StartPosition = FormStartPosition.Manual;
                // The overlay is designed maximized, which leaves the taskbar strip unhidden and
                // makes any geometry given to it land in the restore bounds instead of the screen.
                blocker.WindowState = FormWindowState.Normal;
                blocker.Bounds = CoverBounds(screen);
                blocker.Show();
                screenBlockers[screen.DeviceName] = blocker;
            } catch { }
        }

        private static Rectangle CoverBounds(Screen screen) {
            return screen.Bounds;
        }

        private static Screen FindScreen(Screen[] screens, string deviceName) {
            foreach (Screen screen in screens) {
                if (screen.DeviceName == deviceName) return screen;
            }
            return null;
        }

        private void ReleaseScreenBlockers() {
            foreach (string name in new List<string>(screenBlockers.Keys)) {
                RemoveBlocker(name);
            }
        }

        private void RemoveBlocker(string name) {
            ScreenBlocker blocker = screenBlockers[name];
            screenBlockers.Remove(name);
            blocker.Dispose();
        }

        // Win+D, "minimize all", a display change or another process claiming the topmost band all
        // drop coverage silently, so every overlay is re-asserted and every screen re-claimed.
        private void blockerGuardTimer_Tick(object sender, EventArgs e) {
            if (!screenBlockerEnabled) return;
            Screen[] screens = Screen.AllScreens;
            foreach (string name in new List<string>(screenBlockers.Keys)) {
                Screen screen = FindScreen(screens, name);
                ScreenBlocker blocker = screenBlockers[name];
                if (screen == null || blocker.IsDisposed) {
                    RemoveBlocker(name);
                    continue;
                }
                try {
                    blocker.Reassert(CoverBounds(screen));
                } catch (Exception) {
                    RemoveBlocker(name);
                }
            }
            foreach (Screen screen in screens) {
                if (!screenBlockers.ContainsKey(screen.DeviceName)) {
                    AttachBlocker(screen);
                }
            }
        }

        private void windowCleanerTimer_Tick(object sender, EventArgs e) {
            ThreadPool.QueueUserWorkItem(x => {
                WindowCleaner.CleanWindows();
            });
        }

        // Tray

        private void hideToTrayButton_Click(object sender, EventArgs e) {
            trayIcon.Visible = true;
            Hide();
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e) {
            trayIcon.Visible = false;
            ShowFromTray();
        }

        private void ShowFromTray() {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            trayIcon.Visible = false;
            windowCleanerTimer.Enabled = false;
            blockerGuardTimer.Enabled = false;
            LowLevelInputHooks.Disable();
            ReleaseScreenBlockers();
            screenBlockerEnabled = false;
        }

        // Firewall

        private void RefreshFirewallTab() {
            if (firewallBusy) return;
            SetFirewallStatus("Reading the firewall configuration...", Color.LightGray);
            RunFirewall(ReadFirewallState);
        }

        // Firewall calls are RPC bound, so they never run on the UI thread. The work has to
        // end by reading the state back, which releases the guard in FillFirewallRules.
        private void RunFirewall(Action work) {
            if (firewallBusy) return;
            firewallBusy = true;
            ThreadPool.QueueUserWorkItem(delegate {
                string error = null;
                try {
                    work();
                } catch (Exception ex) {
                    error = ErrorText(ex);
                }
                if (error != null) {
                    InvokeSafe(delegate {
                        firewallBusy = false;
                        SetFirewallStatus(error, Color.IndianRed);
                    });
                }
            });
        }

        private void ReadFirewallState() {
            List<FirewallRule> rules = Firewall.GetRules();
            bool whitelist = Firewall.IsWhitelistEnabled();
            string warning = Firewall.GetGroupPolicyWarning();
            InvokeSafe(delegate { FillFirewallRules(rules, whitelist, warning); });
        }

        private void FillFirewallRules(List<FirewallRule> rules, bool whitelist, string warning) {
            firewallRules = rules;
            firewallBusy = false;

            firewallRulesList.BeginUpdate();
            firewallRulesList.Items.Clear();
            foreach (FirewallRule rule in rules) {
                ListViewItem item = new ListViewItem(rule.Title);
                item.SubItems.Add(rule.Allow ? "Allow" : "Block");
                item.SubItems.Add(rule.Enabled ? "Enabled" : "Disabled");
                item.SubItems.Add(rule.Program);
                item.ForeColor = rule.Allow ? Color.LightGreen : Color.IndianRed;
                item.Tag = rule;
                firewallRulesList.Items.Add(item);
            }
            firewallRulesList.EndUpdate();

            blockAllButton.Text = whitelist ? "Disable whitelist mode" : "Enable whitelist mode";
            blockAllButton.ForeColor = whitelist ? Color.LightGreen : Color.LightGray;

            string status = whitelist
                ? "Whitelist mode is ON: only the allowed apps can reach the network."
                : "Whitelist mode is OFF: every app can reach the network.";
            status += rules.Count == 0 ? " No app is managed yet."
                                       : " Managed apps: " + rules.Count + ".";
            if (warning != null) status += " " + warning;
            SetFirewallStatus(status, whitelist ? Color.LightGreen : Color.LightGray);
        }

        private void SetFirewallStatus(string text, Color color) {
            firewallStatusLabel.Text = text;
            firewallStatusLabel.ForeColor = color;
        }

        private string SelectedAppPath() {
            string path = whitelistAppPath.Text.Trim().Trim('"');
            if (path.Length == 0 || !File.Exists(path)) {
                SetFirewallStatus("The specified app doesn't exist.", Color.IndianRed);
                return null;
            }
            return Path.GetFullPath(path);
        }

        private void whitelistAddButton_Click(object sender, EventArgs e) {
            ApplyAppRule(true);
        }

        private void firewallBlockButton_Click(object sender, EventArgs e) {
            ApplyAppRule(false);
        }

        private void ApplyAppRule(bool allow) {
            string path = SelectedAppPath();
            if (path == null || firewallBusy) return;
            SetFirewallStatus((allow ? "Allowing " : "Blocking ") + path + "...", Color.LightGray);
            RunFirewall(delegate {
                Firewall.SetAppRule(path, allow);
                ReadFirewallState();
            });
        }

        private void whitelistRemoveButton_Click(object sender, EventArgs e) {
            ListViewItem selected = firewallRulesList.SelectedItems.Count == 0 ? null
                                                                               : firewallRulesList.SelectedItems[0];
            FirewallRule rule = selected == null ? null : selected.Tag as FirewallRule;
            if (rule == null) {
                SetFirewallStatus("Select an app in the list first.", Color.IndianRed);
                return;
            }
            if (firewallBusy) return;
            SetFirewallStatus("Removing " + rule.Title + " from the firewall...", Color.LightGray);
            RunFirewall(delegate {
                Firewall.RemoveRule(rule.RuleName);
                ReadFirewallState();
            });
        }

        private void firewallRefreshButton_Click(object sender, EventArgs e) {
            RefreshFirewallTab();
        }

        private void firewallRulesList_SelectedIndexChanged(object sender, EventArgs e) {
            if (firewallRulesList.SelectedItems.Count == 0) return;
            FirewallRule rule = firewallRulesList.SelectedItems[0].Tag as FirewallRule;
            if (rule != null && !string.IsNullOrEmpty(rule.Program)) whitelistAppPath.Text = rule.Program;
        }

        private void blockAllButton_Click(object sender, EventArgs e) {
            if (firewallBusy) return;
            SetFirewallStatus("Applying the firewall policy...", Color.LightGray);
            RunFirewall(delegate {
                Firewall.SetWhitelistEnabled(!Firewall.IsWhitelistEnabled());
                ReadFirewallState();
            });
        }

        private void resetFirewall_Click(object sender, EventArgs e) {
            DialogResult result = MessageBox.Show(
                "This restores the Windows Firewall to its system defaults and deletes every " +
                "custom rule, including the ones created outside NJCloak. Continue?",
                "Reset firewall", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes || firewallBusy) return;
            SetFirewallStatus("Restoring the firewall defaults...", Color.LightGray);
            RunFirewall(delegate {
                Firewall.Reset();
                ReadFirewallState();
            });
        }

        private void firewallSelectButton_Click(object sender, EventArgs e) {
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK &&
               !String.IsNullOrEmpty(openFileDialog1.FileName)) {
                whitelistAppPath.Text = openFileDialog1.FileName;
            }
        }

        private void firewallTab_DragDrop(object sender, DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0) {
                whitelistAppPath.Text = files[0];
            }
        }

        private void On_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Copy;
        }

        // Anti-clipper

        private void clipperDetectorButton_Click(object sender, EventArgs e) {
            if (!clipperDetectorEnabled) {
                clipperDetectorEnabled = true;
                clipperDetectorButton.Text = "Stop clipper detector";
                clipperDetectorThread = new Thread(
                    new ThreadStart(delegate () {
                        while (true) {
                            try {
                                foreach (var walletType in new string[] { "ETH", "TRC20", "BTC", "LTC", "TON" }) {

                                    var address = WalletGenerator.GenerateRandomAddress(walletType);

                                    clipperStatusLabel.Invoke((MethodInvoker)delegate () {
                                        clipperStatusLabel.Text = "Copied address " + address;
                                    });

                                    ClipboardHelper.SetText(address);
                                    Thread.Sleep(3000);

                                    string currentAddr = ClipboardHelper.GetText();

                                    if (currentAddr != address) {
                                        clipperStatusLabel.Invoke((MethodInvoker)delegate () {
                                            clipperDetectorButton.Text = "Start clipper detector";
                                            clipperStatusLabel.Text = string.Empty;
                                        });
                                        clipperDetectorEnabled = false;
                                        MessageBox.Show("Copied address: " + currentAddr, "Crypto clipper detected!");
                                        return;
                                    }
                                }
                            } catch (Exception ex) { Console.WriteLine(ex.ToString());  }
                        }
                    }));
                // The detector loop never ends on its own: as a background thread it cannot keep
                // the process alive once the window is closed.
                clipperDetectorThread.IsBackground = true;
                clipperDetectorThread.SetApartmentState(ApartmentState.STA);
                clipperDetectorThread.Start();
            } else {
                clipperDetectorEnabled = false;
                clipperDetectorButton.Text = "Start clipper detector";
                if (clipperDetectorThread != null &&
                    clipperDetectorThread.IsAlive) {
                    clipperDetectorThread.Abort();
                }
            }
        }

        // Periphery

        private void RefreshPeripherals() {
            if (peripheralsBusy || IsDisposed) return;
            peripheralsBusy = true;
            SetPeripheryStatus("Scanning devices...", Color.LightGray);
            ThreadPool.QueueUserWorkItem(delegate {
                List<PeripheralDevice> found = null;
                string error = null;
                try {
                    found = Periphery.GetAllPeripherals();
                } catch (Exception ex) {
                    error = ErrorText(ex);
                }
                InvokeSafe(delegate {
                    // Released before the fill: FillPeripherals detaches the handler on its own, and
                    // leaving this guard set would swallow every later click on the device list.
                    peripheralsBusy = false;
                    if (found != null) {
                        FillPeripherals(found);
                        SetPeripheryStatus(found.Count + " device(s) found.", Color.LightGray);
                    } else {
                        SetPeripheryStatus(error, Color.IndianRed);
                    }
                });
            });
        }

        private void FillPeripherals(List<PeripheralDevice> found) {
            peripherals = found;
            // Detached while repopulating, otherwise the programmatic SetItemChecked calls below
            // would be read as user input and fire device changes.
            peripheralsList.ItemCheck -= peripheralsList_ItemCheck;
            try {
                peripheralsList.BeginUpdate();
                peripheralsList.Items.Clear();
                foreach (PeripheralDevice device in found) {
                    int index = peripheralsList.Items.Add(device.Name);
                    peripheralsList.SetItemChecked(index, device.Enabled);
                }
                peripheralsList.EndUpdate();
            } finally {
                peripheralsList.ItemCheck += peripheralsList_ItemCheck;
            }
        }

        private void SetPeripheryStatus(string text, Color color) {
            peripheryStatusLabel.Text = text;
            peripheryStatusLabel.ForeColor = color;
        }

        // Device changes are applied on a worker thread: setupapi is blocking, and the row has to
        // show the state the PnP manager reports, not the one that was requested.
        private void ToggleDevice(PeripheralDevice device, bool enable) {
            if (device == null || peripheralsBusy) return;
            peripheralsBusy = true;
            SetPeripheryStatus((enable ? "Enabling " : "Disabling ") + device.Name + "...", Color.LightGray);
            ThreadPool.QueueUserWorkItem(delegate {
                string error = null;
                bool state = device.Enabled;
                try {
                    state = Periphery.ApplyDeviceState(device.DeviceId, enable);
                } catch (Exception ex) {
                    error = ErrorText(ex);
                }
                InvokeSafe(delegate {
                    SetRowState(device, error != null ? device.Enabled : state);
                    peripheralsBusy = false;
                    if (error != null) {
                        SetPeripheryStatus(device.Name + ": " + error, Color.IndianRed);
                    } else if (state != enable) {
                        SetPeripheryStatus(device.Name + " is still " + (state ? "enabled." : "disabled.") +
                                           " Windows may need a restart.", Color.IndianRed);
                    } else {
                        SetPeripheryStatus(device.Name + (enable ? " enabled." : " disabled."), Color.LightGreen);
                    }
                });
            });
        }

        private void peripheralsList_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (peripheralsBusy || e.Index < 0 || e.Index >= peripherals.Count) {
                e.NewValue = e.CurrentValue;
                return;
            }
            ToggleDevice(peripherals[e.Index], e.NewValue == CheckState.Checked);
        }

        private int IndexOf(string deviceId) {
            for (int i = 0; i < peripherals.Count; i++) {
                if (peripherals[i].DeviceId == deviceId) return i;
            }
            return -1;
        }

        // Detached while writing, so that the programmatic SetItemChecked is not read as a click.
        private void SetRowState(PeripheralDevice device, bool enabled) {
            int index = IndexOf(device.DeviceId);
            if (index < 0) return;
            PeripheralDevice updated = device.WithState(enabled);
            peripherals[index] = updated;
            peripheralsList.ItemCheck -= peripheralsList_ItemCheck;
            try {
                peripheralsList.SetItemChecked(index, updated.Enabled);
            } finally {
                peripheralsList.ItemCheck += peripheralsList_ItemCheck;
            }
        }

        private void SetAllDevices(bool enable) {
            if (peripheralsBusy) return;
            List<PeripheralDevice> targets = new List<PeripheralDevice>();
            foreach (PeripheralDevice device in peripherals) {
                if (device.Enabled != enable) targets.Add(device);
            }
            if (targets.Count == 0) {
                SetPeripheryStatus("All devices are already " + (enable ? "enabled." : "disabled."), Color.LightGray);
                return;
            }
            peripheralsBusy = true;
            SetPeripheryStatus((enable ? "Enabling " : "Disabling ") + targets.Count + " device(s)...", Color.LightGray);
            ThreadPool.QueueUserWorkItem(delegate {
                string error = null;
                List<PeripheralDevice> applied = new List<PeripheralDevice>();
                foreach (PeripheralDevice device in targets) {
                    try {
                        applied.Add(device.WithState(Periphery.ApplyDeviceState(device.DeviceId, enable)));
                    } catch (Exception ex) {
                        if (error == null) error = ErrorText(ex);
                    }
                }
                InvokeSafe(delegate {
                    int done = 0;
                    foreach (PeripheralDevice device in applied) {
                        if (device.Enabled == enable) done++;
                        SetRowState(device, device.Enabled);
                    }
                    peripheralsBusy = false;
                    string text = (enable ? "Devices enabled: " : "Devices disabled: ") + done + " of " + targets.Count + ".";
                    if (error != null) text += " " + error;
                    SetPeripheryStatus(text, error == null && done == targets.Count ? Color.LightGreen : Color.IndianRed);
                });
            });
        }

        private void disableAll_Click(object sender, EventArgs e) {
            SetAllDevices(false);
        }

        private void enableAll_Click(object sender, EventArgs e) {
            SetAllDevices(true);
        }

        private void refreshDevices_Click(object sender, EventArgs e) {
            RefreshPeripherals();
        }

        // Shared

        private void InvokeSafe(Action action) {
            try {
                if (!IsDisposed && IsHandleCreated) BeginInvoke(action);
            } catch (Exception) {
                // The window was closed while the worker thread was running.
            }
        }

        private static string ErrorText(Exception ex) {
            if (ex is UnauthorizedAccessException) {
                return "Administrator privileges are required for this action.";
            }
            COMException com = ex as COMException;
            if (com != null && com.HResult == E_ACCESSDENIED) {
                return "Administrator privileges are required for this action.";
            }
            Win32Exception win32 = ex as Win32Exception;
            if (win32 != null && win32.NativeErrorCode == ErrorAccessDenied) {
                return "Administrator privileges are required for this action.";
            }
            ManagementException mgmt = ex as ManagementException;
            if (mgmt != null && mgmt.ErrorCode == ManagementStatus.AccessDenied) {
                return "Administrator privileges are required for this action.";
            }
            if (string.IsNullOrEmpty(ex.Message)) return ex.GetType().Name;
            return ex.Message;
        }

        // About

        private static void OpenBrowserSafe(string url) {
            try {
                Process.Start(url);
            } catch (Exception) {
                Clipboard.Clear();
                Clipboard.SetText(url);
                MessageBox.Show("Your computer doesn't have a default browser selected. " +
                                "The URL was copied to your clipboard instead.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void creditsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            OpenBrowserSafe("https://github.com/ac3ss0r/NJCloak");
        }
    }
}
