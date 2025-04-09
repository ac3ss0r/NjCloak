using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NJCloak.Modules {
    public class LowLevelInputHooks {

        private const uint LLMHF_INJECTED = 0x00000001;
        private const int WH_KEYBOARD_LL = 13,
                          WH_MOUSE_LL = 14;
        private static LowLevelHook _mouse = MouseHook,
                                    _keyboard = KeyboardHook;
        private static IntPtr _mouseHook = IntPtr.Zero,
                            _keyboardHook = IntPtr.Zero;
        private static bool IsActive = false;

        private delegate IntPtr LowLevelHook(int nCode, IntPtr wParam, IntPtr lParam);

        public static void Enable() {
            if (!IsActive) {
                _keyboardHook = SetHook(WH_KEYBOARD_LL, _keyboard);
                _mouseHook = SetHook(WH_MOUSE_LL, _mouse);
                IsActive = true;
            }
        }

        public static void Disable() {
            if (IsActive) {
                UnhookWindowsHookEx(_mouseHook);
                UnhookWindowsHookEx(_keyboardHook);
                IsActive = false; ;
            }
        }

        private static IntPtr SetHook(int type, LowLevelHook proc) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(type, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelHook lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        // Injected flag marks that the action was simulated via software (non hardware) way.
        // Therefore it will be blocked.

        private static IntPtr MouseHook(int nCode, IntPtr wParam, IntPtr lParam) {
            MSLLHOOKSTRUCT replacementKey = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            if ((replacementKey.flags & LLMHF_INJECTED) != 0) {
                return (IntPtr)1;
            }
            return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
        }

        private static IntPtr KeyboardHook(int nCode, IntPtr wParam, IntPtr lParam) {
            KBDLLHOOKSTRUCT replacementKey = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            if ((replacementKey.flags & LLMHF_INJECTED) != 0) {
                return (IntPtr)1;
            }
            return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT {
            public int x;
            public int y;
        }

        public struct MSLLHOOKSTRUCT {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT {
            public int vkCode;
            public int scanCode;
            public uint flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT {
            public uint type;
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
    }
}
