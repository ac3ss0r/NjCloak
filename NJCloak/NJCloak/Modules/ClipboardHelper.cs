using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NJCloak.Modules {
    public class ClipboardHelper {

        // Had to implement this manually cause mscorlib Clipboard is BROKEN

        [DllImport("user32.dll")]
        private static extern IntPtr OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        private static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern UIntPtr GlobalSize(IntPtr hMem);

        private const uint CF_TEXT = 1;
        private const uint GMEM_MOVEABLE = 0x0002;

        public static string GetText() {
            string result = string.Empty;
            if (OpenClipboard(IntPtr.Zero) != IntPtr.Zero) {
                if (IsClipboardFormatAvailable(CF_TEXT)) {
                    IntPtr handle = GetClipboardData(CF_TEXT);
                    if (handle != IntPtr.Zero) {
                        IntPtr pointer = GlobalLock(handle);
                        if (pointer != IntPtr.Zero) {
                            int size = (int)GlobalSize(handle) - 1;
                            byte[] buffer = new byte[size];
                            Marshal.Copy(pointer, buffer, 0, size);
                            GlobalUnlock(handle);
                            result = Encoding.ASCII.GetString(buffer);
                        }
                    }
                }
                CloseClipboard();
            }
            return result;
        }

        public static void SetText(string text) {
            if (string.IsNullOrEmpty(text)) return;
            if (OpenClipboard(IntPtr.Zero) != IntPtr.Zero) {
                EmptyClipboard();
                byte[] buffer = Encoding.ASCII.GetBytes(text);
                IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)(buffer.Length+1));
                IntPtr pointer = GlobalLock(hGlobal);
                Marshal.Copy(buffer, 0, pointer, buffer.Length);
                GlobalUnlock(hGlobal);
                SetClipboardData(CF_TEXT, hGlobal);
                CloseClipboard();
            }
        }
    }
}