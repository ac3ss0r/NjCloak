using System;
using System.Drawing;
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

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, uint flags);
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const uint SWP_NOMOVE = 0x0002, SWP_NOSIZE = 0x0001, SWP_NOACTIVATE = 0x0010;

        private void CloakForm_Load(object sender, EventArgs e) {
            SetWindowDisplayAffinity(this.Handle, 0x01);
            var style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        // Called by the guard timer: an overlay that got minimized, moved or pushed out of the
        // topmost band stops hiding the screen while the user still believes it is blocking.
        public void Reassert(Rectangle bounds) {
            if (!Visible) Show();
            if (WindowState != FormWindowState.Normal) WindowState = FormWindowState.Normal;
            if (Bounds != bounds) Bounds = bounds;
            if (!TopMost) TopMost = true;
            // Form.TopMost keeps reporting true after another process strips WS_EX_TOPMOST from the
            // window, so the style is re-applied through the window manager rather than trusted.
            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }
    }
}