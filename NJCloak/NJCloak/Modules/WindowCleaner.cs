using System;
using System.Runtime.InteropServices;

namespace NJCloak {
    public class WindowCleaner {

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static void CleanWindows() {
            EnumWindows(new EnumWindowsProc(EnumWindow), IntPtr.Zero);
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam) {
            int length = GetWindowTextLength(hWnd);
            if (length > 0) {
                SetWindowText(hWnd, string.Empty);
            }
            return true;
        }
}
}
