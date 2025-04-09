using NJCloak.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace NJCloak {
    public partial class MainForm : Form {

        private bool screenBlockerEnabled = false, clipperDetectorEnabled = false;
        private List<ScreenBlocker> screenBlockers = new List<ScreenBlocker>();
        private Thread clipperDetectorThread;
        private Panel[] tabs;

        public MainForm() {
            InitializeComponent();
            tabs = new Panel[] {
                screenBlockerTab,
                firewallTab,
                clipperDetectorTab,
                aboutTab,
                peripheryTab
            };
        }

        private void Form1_Load(object sender, EventArgs e) {
            ThreadPool.QueueUserWorkItem(x => {
                List<ManagementObject> devices = Periphery.GetAllPeripherals();
                peripheralsList.Invoke((MethodInvoker)delegate () {
                    foreach (var device in devices) {
                        peripheralsList.Items.Add((string)device.Properties["Name"].Value);
                        peripheralsList.SetItemChecked(peripheralsList.Items.Count-1, 
                                                       (string)device.Properties["Status"].Value == "OK");
                    }
                    this.peripheralsList.ItemCheck += new ItemCheckEventHandler(this.peripheralsList_ItemCheck);
                });
            });
        }

        // Custom table implementation

        private void SelectTab(string name) {
            foreach (var tab in tabs) {
                tab.Visible = tab.Name == name;
            }
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
                foreach (var blocker in screenBlockers.ToArray()) {
                    screenBlockers.Remove(blocker);
                    blocker.Dispose();
                }
                screenBlockerEnabled = false;
            } else {
                if (cleanTitles.Checked) {
                    windowCleanerTimer.Enabled = true;
                }
                if (blockInputs.Checked) {
                    LowLevelInputHooks.Enable();
                }
                foreach (var scr in Screen.AllScreens) {
                    var blocker = new ScreenBlocker();
                    blocker.Location = scr.WorkingArea.Location;
                    blocker.StartPosition = FormStartPosition.Manual;
                    blocker.Size = new Size(scr.Bounds.Width - 1, scr.Bounds.Height);
                    blocker.Show();
                    screenBlockers.Add(blocker);
                }
                screenBlockerEnabled = true;
            }
        }

        private void windowCleanerTimer_Tick(object sender, EventArgs e) {
            ThreadPool.QueueUserWorkItem(x => {
                WindowCleaner.CleanWindows();
            });
        }

        // Firewall

        private void resetFirewall_Click(object sender, EventArgs e) {
            Firewall.Reset();
            firewallStatusLabel.Text = "Firewall has been reset. ";
            firewallStatusLabel.ForeColor = Color.Green;
        }

        private void blockAllButton_Click(object sender, EventArgs e) {
            Firewall.Enable();
            firewallStatusLabel.Text = "Whitelist mode has been enabled. ";
            firewallStatusLabel.ForeColor = Color.Green;
        }

        private void whitelistAddButton_Click(object sender, EventArgs e) {
            if (File.Exists(whitelistAppPath.Text)) {
                Firewall.WhitelistApp(whitelistAppPath.Text);
                firewallStatusLabel.Text = "Whitelisted " + whitelistAppPath.Text;
                firewallStatusLabel.ForeColor = Color.Green;
            } else {
                firewallStatusLabel.Text = "The specified app doesn't exist.";
                firewallStatusLabel.ForeColor = Color.Red;
            }
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

        private void whitelistRemoveButton_Click(object sender, EventArgs e) {
            Firewall.RemoveApp(whitelistAppPath.Text);
            firewallStatusLabel.Text = "Removed " + whitelistAppPath.Text;
            firewallStatusLabel.ForeColor = Color.Red;
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

        // About

        private static void OpenBrowserSafe(string url) {
            try {
                Process.Start(url);
            } catch (Exception e) {
                Clipboard.Clear();
                Clipboard.SetText(url);
                MessageBox.Show("Your computer doesn't have a default browser selected. " +
                                "The URL was copied to your clipboard instead.", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void peripheralsList_ItemCheck(object sender, ItemCheckEventArgs e) {
            ThreadPool.QueueUserWorkItem(x => {
                var device = Periphery.FindDevice((string)peripheralsList.Items[e.Index]);
                if (device != null) {
                    var method = e.NewValue == CheckState.Checked ? "Enable" : "Disable";
                    ManagementBaseObject inParams = device.GetMethodParameters(method);
                    device.InvokeMethod(method, inParams, null);
                }
            });
        }

        private void disableAll_Click(object sender, EventArgs e) {
            ThreadPool.QueueUserWorkItem(x => {
                for (int i =0; i < peripheralsList.Items.Count; i++) {
                    var item = peripheralsList.Items[i];
                    var device = Periphery.FindDevice((string)item);
                    if (device != null) {
                        ManagementBaseObject inParams = device.GetMethodParameters("Disable");
                        device.InvokeMethod("Disable", inParams, null);
                        peripheralsList.Invoke((MethodInvoker)delegate () { peripheralsList.SetItemChecked(i, false); });
                    }
                }
            });
        }

        private void enableAll_Click(object sender, EventArgs e) {
            ThreadPool.QueueUserWorkItem(x => {
                for (int i = 0; i < peripheralsList.Items.Count; i++) {
                    var item = peripheralsList.Items[i];
                    var device = Periphery.FindDevice((string)item);
                    if (device != null) {
                        ManagementBaseObject inParams = device.GetMethodParameters("Enable");
                        device.InvokeMethod("Enable", inParams, null);
                        peripheralsList.Invoke((MethodInvoker)delegate () { peripheralsList.SetItemChecked(i, true); });
                    }
                }
            });
        }

        private void creditsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            OpenBrowserSafe("https://github.com/ac3ss0r/NJCloak");
        }
    }
}