using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClickPaste
{
    class Native
    {
        #region Windows 10/11 Dark Mode API

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        /// <summary>
        /// Enables dark mode for a window's title bar on Windows 10/11.
        /// </summary>
        public static bool SetDarkModeForWindow(IntPtr handle, bool enabled)
        {
            if (Environment.OSVersion.Version.Major < 10) return false;

            int attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
            // Try the newer attribute first, fall back to pre-20H1 if it fails
            int value = enabled ? 1 : 0;
            int result = DwmSetWindowAttribute(handle, attribute, ref value, sizeof(int));
            if (result != 0)
            {
                attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                result = DwmSetWindowAttribute(handle, attribute, ref value, sizeof(int));
            }
            return result == 0;
        }

        // uxtheme.dll ordinal 135 - SetPreferredAppMode
        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SetPreferredAppMode(int preferredAppMode);

        // uxtheme.dll ordinal 136 - FlushMenuThemes
        [DllImport("uxtheme.dll", EntryPoint = "#136", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void FlushMenuThemes();

        private const int AllowDark = 1;
        private const int ForceDark = 2;
        private const int ForceLight = 3;
        private const int Max = 4;

        /// <summary>
        /// Enables system-wide dark mode for context menus on Windows 10/11.
        /// Must be called before any menus are created.
        /// </summary>
        public static void SetAppDarkMode(bool dark)
        {
            if (Environment.OSVersion.Version.Major < 10) return;
            try
            {
                SetPreferredAppMode(dark ? AllowDark : ForceLight);
                FlushMenuThemes();
            }
            catch
            {
                // Ignore errors on older Windows versions
            }
        }

        #endregion

        #region SendInput Unicode Support (for international keyboards and Unicode characters)

        public const int INPUT_KEYBOARD = 1;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_UNICODE = 0x0004;
        public const uint KEYEVENTF_SCANCODE = 0x0008;

        // MapVirtualKey translation types
        public const uint MAPVK_VK_TO_VSC = 0;

        // Virtual key codes for modifiers and numpad
        public const byte VK_LSHIFT = 0xA0;
        public const byte VK_LCONTROL = 0xA2;
        public const byte VK_NUMPAD0 = 0x60;
        public const byte VK_NUMPAD1 = 0x61;
        public const byte VK_NUMPAD2 = 0x62;
        public const byte VK_NUMPAD3 = 0x63;
        public const byte VK_NUMPAD4 = 0x64;
        public const byte VK_NUMPAD5 = 0x65;
        public const byte VK_NUMPAD6 = 0x66;
        public const byte VK_NUMPAD7 = 0x67;
        public const byte VK_NUMPAD8 = 0x68;
        public const byte VK_NUMPAD9 = 0x69;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // INPUT structure with explicit layout to handle union properly
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public KEYBDINPUT ki;
            // Padding to match the size of the largest union member (MOUSEINPUT)
            public int padding1;
            public int padding2;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        /// <summary>
        /// Sends a Unicode character directly using SendInput with KEYEVENTF_UNICODE.
        /// This bypasses keyboard layout translation and works with any Unicode character.
        /// </summary>
        public static void SendUnicodeChar(char c)
        {
            INPUT[] inputs = new INPUT[2];
            int inputSize = Marshal.SizeOf(typeof(INPUT));

            // Key down event
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].ki.wVk = 0;
            inputs[0].ki.wScan = c;
            inputs[0].ki.dwFlags = KEYEVENTF_UNICODE;
            inputs[0].ki.time = 0;
            inputs[0].ki.dwExtraInfo = IntPtr.Zero;

            // Key up event
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].ki.wVk = 0;
            inputs[1].ki.wScan = c;
            inputs[1].ki.dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP;
            inputs[1].ki.time = 0;
            inputs[1].ki.dwExtraInfo = IntPtr.Zero;

            SendInput(2, inputs, inputSize);
        }

        /// <summary>
        /// Sends a key press/release using virtual key code AND scan code.
        /// This works with browser-based VM consoles that need scan codes.
        /// </summary>
        private static void SendKeyWithScanCode(byte vk, bool keyUp)
        {
            ushort scanCode = (ushort)MapVirtualKey(vk, MAPVK_VK_TO_VSC);

            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].ki.wVk = vk;
            inputs[0].ki.wScan = scanCode;
            inputs[0].ki.dwFlags = keyUp ? KEYEVENTF_KEYUP : 0;
            inputs[0].ki.time = 0;
            inputs[0].ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void SendCharViaScanCode(char c)
        {
            if (KeyboardTranslator.TrySendChar(c))
                return;

            KeyboardTranslator.Log($"Fallback path for '{c}' (0x{((int)c):X4})");
            int count = (int)GetKeyboardLayoutList(0, null);
            KeyboardTranslator.Log($"GetKeyboardLayoutList returned {count} layouts");
            if (count > 0)
            {
                IntPtr[] layouts = new IntPtr[count];
                GetKeyboardLayoutList(count, layouts);
                foreach (var hkl in layouts)
                {
                    if (TrySendCharWithLayout(c, hkl))
                    {
                        KeyboardTranslator.Log($"Sent via layout 0x{hkl.ToInt64():X8}");
                        return;
                    }
                }
            }

            KeyboardTranslator.Log($"Falling back to ALT numpad for '{c}'");
            SendCharViaAltNumpad(c);
        }

        static bool TrySendCharWithLayout(char c, IntPtr hkl)
        {
            short vkResult = VkKeyScanEx(c, hkl);
            if ((vkResult & 0xFF) == 0xFF && ((vkResult >> 8) & 0xFF) == 0xFF)
                return false;

            byte vk = (byte)(vkResult & 0xFF);
            byte shiftState = (byte)((vkResult >> 8) & 0xFF);

            bool needShift = (shiftState & 1) != 0;
            bool needCtrl = (shiftState & 2) != 0;
            bool needAlt = (shiftState & 4) != 0;

            if (needShift) SendKeyWithScanCodeEx(VK_LSHIFT, false, hkl);
            if (needCtrl) SendKeyWithScanCodeEx(VK_LCONTROL, false, hkl);
            if (needAlt) SendKeyWithScanCodeEx((byte)VK_MENU, false, hkl);

            SendKeyWithScanCodeEx(vk, false, hkl);
            SendKeyWithScanCodeEx(vk, true, hkl);

            if (needAlt) SendKeyWithScanCodeEx((byte)VK_MENU, true, hkl);
            if (needCtrl) SendKeyWithScanCodeEx(VK_LCONTROL, true, hkl);
            if (needShift) SendKeyWithScanCodeEx(VK_LSHIFT, true, hkl);

            return true;
        }

        static void SendKeyWithScanCodeEx(byte vk, bool keyUp, IntPtr hkl)
        {
            ushort scanCode = (ushort)MapVirtualKeyEx(vk, MAPVK_VK_TO_VSC, hkl);

            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].ki.wVk = 0;
            inputs[0].ki.wScan = scanCode;
            inputs[0].ki.dwFlags = KEYEVENTF_SCANCODE | (keyUp ? KEYEVENTF_KEYUP : 0);
            inputs[0].ki.time = 0;
            inputs[0].ki.dwExtraInfo = IntPtr.Zero;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        /// <summary>
        /// Sends a character using ALT + numpad decimal code when possible (uses Alt+0nnn for ANSI),
        /// otherwise falls back to KEYEVENTF_UNICODE via SendUnicodeChar.
        /// </summary>
        public static void SendCharViaAltNumpad(char c)
        {
            int code = c;
            // If the code point is outside BMP (or invalid), fallback to Unicode send.
            if (code > 0xFFFF || code < 0)
            {
                SendUnicodeChar(c);
                return;
            }

            byte[] numpadKeys = { VK_NUMPAD0, VK_NUMPAD1, VK_NUMPAD2, VK_NUMPAD3, VK_NUMPAD4,
                                  VK_NUMPAD5, VK_NUMPAD6, VK_NUMPAD7, VK_NUMPAD8, VK_NUMPAD9 };

            // Try to map the character to the system ANSI code page (Encoding.Default).
            // If it maps to a single ANSI byte and round-trips back to the same char,
            // we can use Alt+0nnn (the leading 0 forces the Windows/ANSI code page).
            var ansi = System.Text.Encoding.Default;
            byte[] encoded = ansi.GetBytes(new char[] { c });

            bool canUseAltAnsi = false;
            int ansiValue = 0;
            if (encoded.Length == 1)
            {
                // Verify it round-trips (some encodings use fallback '?')
                char[] roundTrip = ansi.GetChars(encoded);
                if (roundTrip.Length == 1 && roundTrip[0] == c)
                {
                    canUseAltAnsi = true;
                    ansiValue = encoded[0]; // 0..255
                }
            }

            if (canUseAltAnsi)
            {
                // Press ALT
                SendKeyWithScanCode((byte)VK_MENU, false);

                // Send leading '0' to force Windows/ANSI code page (Alt+0nnn).
                SendKeyWithScanCode(numpadKeys[0], false);
                SendKeyWithScanCode(numpadKeys[0], true);

                // Send the decimal digits of the ANSI byte value (e.g. for 169 -> '1','6','9').
                string digits = ansiValue.ToString();
                foreach (char d in digits)
                {
                    int digit = d - '0';
                    SendKeyWithScanCode(numpadKeys[digit], false);
                    SendKeyWithScanCode(numpadKeys[digit], true);
                }

                // Release ALT (this should produce the character).
                SendKeyWithScanCode((byte)VK_MENU, true);
                return;
            }

            // If we couldn't map to ANSI (or it's >255), use the Unicode send fallback.
            SendUnicodeChar(c);
        }

        #endregion

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        public static extern bool SetSystemCursor(IntPtr hcur, uint id);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SystemParametersInfo(UInt32 uiAction, UInt32
        uiParam, String pvParam, UInt32 fWinIni);

        [DllImport("user32.dll")]
        public static extern IntPtr CopyIcon(IntPtr pcur);

        public static uint CROSS = 32515;
        public static uint NORMAL = 32512;
        public static uint IBEAM = 32513;
        public static uint HAND = 32649;

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport("user32.dll")]
        public static extern uint GetKeyboardLayoutList(int nBuff, [Out] IntPtr[] lpList);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardLayoutName([Out] StringBuilder pwszKLID);
        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(uint idThread);
        private const int KEY_PRESSED = 0x8000;
        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int VK_LWIN = 0x5B;
        private const int VK_RWIN = 0x5C;
        [DllImport("USER32.dll")]
        static extern short GetKeyState(int nVirtKey);
        public static bool IsModifierKeyPressed()
        {
            return
                Convert.ToBoolean(GetKeyState(VK_SHIFT) & KEY_PRESSED) ||
                Convert.ToBoolean(GetKeyState(VK_CONTROL) & KEY_PRESSED) ||
                Convert.ToBoolean(GetKeyState(VK_MENU) & KEY_PRESSED) ||
                Convert.ToBoolean(GetKeyState(VK_LWIN) & KEY_PRESSED) ||
                Convert.ToBoolean(GetKeyState(VK_RWIN) & KEY_PRESSED);
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll")]
        public static extern int ActivateKeyboardLayout(int HKL, int flags);

        [DllImport("user32.dll")]
        public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        public static string GetText(IntPtr hwnd)
        {
            int length = GetWindowTextLength(hwnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
