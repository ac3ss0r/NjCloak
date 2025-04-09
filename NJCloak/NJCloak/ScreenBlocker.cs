using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NJCloak {
    public partial class ScreenBlocker : Form {
        public ScreenBlocker() {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        static extern bool SetWindowDisplayAffinity(IntPtr hwnd, int affinity);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int WS_EX_TRANSPARENT = 0x20;

        private void CloakForm_Load(object sender, EventArgs e) {
            SetWindowDisplayAffinity(this.Handle, 0x01);
            var style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
    }
}