using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NJCloak {
    public class Cleaner {

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static void CleanWindows() {
            EnumWindows(new EnumWindowsProc(EnumWindow), IntPtr.Zero);
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam) {
            int length = GetWindowTextLength(hWnd);
            if (length > 0) {
                StringBuilder windowTitle = new StringBuilder(length + 1);
                GetWindowText(hWnd, windowTitle, windowTitle.Capacity);
                SetWindowText(hWnd, string.Empty);
            }
            return true;
        }
}
}
