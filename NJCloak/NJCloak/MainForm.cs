using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace NJCloak {
    public partial class MainForm : Form {

        private CloakForm ScreenBlocker;
        private static string[] wallets = {
            "0xE92681C4d082175d2102f196503CA0E5aA3b1A17", // eth
            "TCVCVfo1WYqEfYso9akqtyoFVBPqbXTWZE", // USDT TRC20
            "bc1qjhw46crf2mm7d06g3wp2xhmenxwthey0yqmu4q", // BTC
            "ltc1qlhrnsa5h8s58r8pvlmz0g7yve0j5g8thgt7nxv", // Litecoin
            "UQBm0OsvEzXel2OLObYRF2x-wVJmdTT5wppHuoKhxmZ7IHzT" // Ton
        };
        private bool clipperDetectorEnabled = false;
        private Thread clipperDetectorThread;
        private Panel[] tabs;

        public MainForm() {
            InitializeComponent();
            tabs = new Panel[] {
                screenBlockerTab,
                firewallTab,
                clipperDetectorTab,
                aboutTab
            };
        }

        private void Form1_Load(object sender, EventArgs e) {
            ScreenBlocker = new CloakForm();
        }

        // Custom table implementation

        private void SelectTab(string name) {
            foreach (var tab in tabs)
                if (tab.Name == name) tab.BringToFront();
        }

        private void onTabSelected(object sender, EventArgs e) {
            SelectTab(((ToolStripStatusLabel)sender).Tag as string);
        }

        // Screen blocker

        private void cloakButton_Click(object sender, EventArgs e) {
            cloakButton.Text = ScreenBlocker.Visible ? "Start blocking" : 
                                                       "Stop blocking";
            ScreenBlocker.Visible = !ScreenBlocker.Visible;
            windowCleanerTimer.Enabled = !windowCleanerTimer.Enabled;
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
                clipperDetectorButton.Text = "Stop clipper detector";
                clipperDetectorThread = new Thread(
                    new ThreadStart(delegate () {
                        while (true) {
                            try {
                                foreach (var address in wallets) {
                                    clipperStatusLabel.Invoke((MethodInvoker)delegate () {
                                        clipperStatusLabel.Text = "Copied address " + address;
                                    });
                                    Clipboard.SetText(address);
                                    Thread.Sleep(3000);
                                    string currentAddr = Clipboard.GetText();
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
                            } catch { }
                        }
                    }));
                clipperDetectorThread.SetApartmentState(ApartmentState.STA);
                clipperDetectorThread.Start();
            } else {
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

        private void creditsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            OpenBrowserSafe("https://github.com/ac3ss0r/NJCloak");
        }
    }
}