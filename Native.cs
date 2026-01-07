using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ClickPaste
{
    class Native
    {
        static string _targetLayoutId = null;
        static bool _useEnglishUSTable = false;
        static string _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clickpaste.log");

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(string msg)
        {
            try
            {
                File.AppendAllText(_logPath, $"{DateTime.Now:HH:mm:ss.fff} {msg}\n");
            }
            catch { }
        }

        public static void SetTargetKeyboardLayout(string layoutId)
        {
            Log($"SetTargetKeyboardLayout called with: '{layoutId}'");
            _targetLayoutId = layoutId;
            _useEnglishUSTable = !string.IsNullOrEmpty(layoutId) && 
                (layoutId.Equals("00000409", StringComparison.OrdinalIgnoreCase) ||
                 layoutId.Contains("409"));
            Log($"_targetLayoutId={_targetLayoutId}, _useEnglishUSTable={_useEnglishUSTable}");
        }

        public static bool IsEnglishUSTableActive => _useEnglishUSTable;

        struct KeyMapping
        {
            public ushort ScanCode;
            public bool Shift;
            public KeyMapping(ushort sc, bool s = false) { ScanCode = sc; Shift = s; }
        }

        static Dictionary<char, KeyMapping> _englishUS = BuildEnglishUSMap();

        static Dictionary<char, KeyMapping> BuildEnglishUSMap()
        {
            var m = new Dictionary<char, KeyMapping>();
            // Row 1: `1234567890-=
            m['`'] = new KeyMapping(0x29); m['~'] = new KeyMapping(0x29, true);
            m['1'] = new KeyMapping(0x02); m['!'] = new KeyMapping(0x02, true);
            m['2'] = new KeyMapping(0x03); m['@'] = new KeyMapping(0x03, true);
            m['3'] = new KeyMapping(0x04); m['#'] = new KeyMapping(0x04, true);
            m['4'] = new KeyMapping(0x05); m['$'] = new KeyMapping(0x05, true);
            m['5'] = new KeyMapping(0x06); m['%'] = new KeyMapping(0x06, true);
            m['6'] = new KeyMapping(0x07); m['^'] = new KeyMapping(0x07, true);
            m['7'] = new KeyMapping(0x08); m['&'] = new KeyMapping(0x08, true);
            m['8'] = new KeyMapping(0x09); m['*'] = new KeyMapping(0x09, true);
            m['9'] = new KeyMapping(0x0A); m['('] = new KeyMapping(0x0A, true);
            m['0'] = new KeyMapping(0x0B); m[')'] = new KeyMapping(0x0B, true);
            m['-'] = new KeyMapping(0x0C); m['_'] = new KeyMapping(0x0C, true);
            m['='] = new KeyMapping(0x0D); m['+'] = new KeyMapping(0x0D, true);
            // Row 2: qwertyuiop[]
            m['q'] = new KeyMapping(0x10); m['Q'] = new KeyMapping(0x10, true);
            m['w'] = new KeyMapping(0x11); m['W'] = new KeyMapping(0x11, true);
            m['e'] = new KeyMapping(0x12); m['E'] = new KeyMapping(0x12, true);
            m['r'] = new KeyMapping(0x13); m['R'] = new KeyMapping(0x13, true);
            m['t'] = new KeyMapping(0x14); m['T'] = new KeyMapping(0x14, true);
            m['y'] = new KeyMapping(0x15); m['Y'] = new KeyMapping(0x15, true);
            m['u'] = new KeyMapping(0x16); m['U'] = new KeyMapping(0x16, true);
            m['i'] = new KeyMapping(0x17); m['I'] = new KeyMapping(0x17, true);
            m['o'] = new KeyMapping(0x18); m['O'] = new KeyMapping(0x18, true);
            m['p'] = new KeyMapping(0x19); m['P'] = new KeyMapping(0x19, true);
            m['['] = new KeyMapping(0x1A); m['{'] = new KeyMapping(0x1A, true);
            m[']'] = new KeyMapping(0x1B); m['}'] = new KeyMapping(0x1B, true);
            m['\\'] = new KeyMapping(0x2B); m['|'] = new KeyMapping(0x2B, true);
            // Row 3: asdfghjkl;'
            m['a'] = new KeyMapping(0x1E); m['A'] = new KeyMapping(0x1E, true);
            m['s'] = new KeyMapping(0x1F); m['S'] = new KeyMapping(0x1F, true);
            m['d'] = new KeyMapping(0x20); m['D'] = new KeyMapping(0x20, true);
            m['f'] = new KeyMapping(0x21); m['F'] = new KeyMapping(0x21, true);
            m['g'] = new KeyMapping(0x22); m['G'] = new KeyMapping(0x22, true);
            m['h'] = new KeyMapping(0x23); m['H'] = new KeyMapping(0x23, true);
            m['j'] = new KeyMapping(0x24); m['J'] = new KeyMapping(0x24, true);
            m['k'] = new KeyMapping(0x25); m['K'] = new KeyMapping(0x25, true);
            m['l'] = new KeyMapping(0x26); m['L'] = new KeyMapping(0x26, true);
            m[';'] = new KeyMapping(0x27); m[':'] = new KeyMapping(0x27, true);
            m['\''] = new KeyMapping(0x28); m['"'] = new KeyMapping(0x28, true);
            // Row 4: zxcvbnm,./
            m['z'] = new KeyMapping(0x2C); m['Z'] = new KeyMapping(0x2C, true);
            m['x'] = new KeyMapping(0x2D); m['X'] = new KeyMapping(0x2D, true);
            m['c'] = new KeyMapping(0x2E); m['C'] = new KeyMapping(0x2E, true);
            m['v'] = new KeyMapping(0x2F); m['V'] = new KeyMapping(0x2F, true);
            m['b'] = new KeyMapping(0x30); m['B'] = new KeyMapping(0x30, true);
            m['n'] = new KeyMapping(0x31); m['N'] = new KeyMapping(0x31, true);
            m['m'] = new KeyMapping(0x32); m['M'] = new KeyMapping(0x32, true);
            m[','] = new KeyMapping(0x33); m['<'] = new KeyMapping(0x33, true);
            m['.'] = new KeyMapping(0x34); m['>'] = new KeyMapping(0x34, true);
            m['/'] = new KeyMapping(0x35); m['?'] = new KeyMapping(0x35, true);
            // Space, Tab, Enter
            m[' '] = new KeyMapping(0x39);
            m['\t'] = new KeyMapping(0x0F);
            m['\r'] = new KeyMapping(0x1C);
            m['\n'] = new KeyMapping(0x1C);
            return m;
        }

        static bool TrySendWithEnglishUSTable(char c)
        {
            if (!_englishUS.TryGetValue(c, out KeyMapping km))
                return false;

            if (km.Shift)
                SendScanCode(0x2A, false); // Left Shift down
            SendScanCode(km.ScanCode, false);
            SendScanCode(km.ScanCode, true);
            if (km.Shift)
                SendScanCode(0x2A, true); // Left Shift up
            return true;
        }

        static void SendScanCode(ushort scanCode, bool keyUp)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].ki.wVk = 0;
            inputs[0].ki.wScan = scanCode;
            inputs[0].ki.dwFlags = KEYEVENTF_SCANCODE | (keyUp ? KEYEVENTF_KEYUP : 0);
            inputs[0].ki.time = 0;
            inputs[0].ki.dwExtraInfo = IntPtr.Zero;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static Dictionary<string, string> GetAvailableKeyboardLayouts()
        {
            var result = new Dictionary<string, string>();
            int count = (int)GetKeyboardLayoutList(0, null);
            if (count > 0)
            {
                IntPtr[] layouts = new IntPtr[count];
                GetKeyboardLayoutList(count, layouts);
                foreach (var hkl in layouts)
                {
                    string layoutId = ((uint)hkl.ToInt32() & 0xFFFF).ToString("X4").PadLeft(8, '0');
                    string name = GetLayoutDisplayName(layoutId);
                    if (!result.ContainsKey(layoutId))
                        result[layoutId] = name;
                }
            }
            result["00000409"] = "English (US)";
            result["00000809"] = "English (UK)";
            result["0000040B"] = "Finnish";
            result["00000407"] = "German";
            result["0000040C"] = "French";
            result["00000410"] = "Italian";
            result["00000406"] = "Danish";
            result["0000041D"] = "Swedish";
            result["00000414"] = "Norwegian";
            return result.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        static string GetLayoutDisplayName(string layoutId)
        {
            try
            {
                int lcid = int.Parse(layoutId.Substring(4), NumberStyles.HexNumber);
                var culture = CultureInfo.GetCultureInfo(lcid);
                return culture.DisplayName;
            }
            catch
            {
                return layoutId;
            }
        }

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

        private static readonly Dictionary<char, (ushort scanCode, bool shift)> USKeyboardMap = new Dictionary<char, (ushort, bool)>
        {
            {'`', (0x29, false)}, {'~', (0x29, true)},
            {'1', (0x02, false)}, {'!', (0x02, true)},
            {'2', (0x03, false)}, {'@', (0x03, true)},
            {'3', (0x04, false)}, {'#', (0x04, true)},
            {'4', (0x05, false)}, {'$', (0x05, true)},
            {'5', (0x06, false)}, {'%', (0x06, true)},
            {'6', (0x07, false)}, {'^', (0x07, true)},
            {'7', (0x08, false)}, {'&', (0x08, true)},
            {'8', (0x09, false)}, {'*', (0x09, true)},
            {'9', (0x0A, false)}, {'(', (0x0A, true)},
            {'0', (0x0B, false)}, {')', (0x0B, true)},
            {'-', (0x0C, false)}, {'_', (0x0C, true)},
            {'=', (0x0D, false)}, {'+', (0x0D, true)},
            {'q', (0x10, false)}, {'Q', (0x10, true)},
            {'w', (0x11, false)}, {'W', (0x11, true)},
            {'e', (0x12, false)}, {'E', (0x12, true)},
            {'r', (0x13, false)}, {'R', (0x13, true)},
            {'t', (0x14, false)}, {'T', (0x14, true)},
            {'y', (0x15, false)}, {'Y', (0x15, true)},
            {'u', (0x16, false)}, {'U', (0x16, true)},
            {'i', (0x17, false)}, {'I', (0x17, true)},
            {'o', (0x18, false)}, {'O', (0x18, true)},
            {'p', (0x19, false)}, {'P', (0x19, true)},
            {'[', (0x1A, false)}, {'{', (0x1A, true)},
            {']', (0x1B, false)}, {'}', (0x1B, true)},
            {'\\', (0x2B, false)}, {'|', (0x2B, true)},
            {'a', (0x1E, false)}, {'A', (0x1E, true)},
            {'s', (0x1F, false)}, {'S', (0x1F, true)},
            {'d', (0x20, false)}, {'D', (0x20, true)},
            {'f', (0x21, false)}, {'F', (0x21, true)},
            {'g', (0x22, false)}, {'G', (0x22, true)},
            {'h', (0x23, false)}, {'H', (0x23, true)},
            {'j', (0x24, false)}, {'J', (0x24, true)},
            {'k', (0x25, false)}, {'K', (0x25, true)},
            {'l', (0x26, false)}, {'L', (0x26, true)},
            {';', (0x27, false)}, {':', (0x27, true)},
            {'\'', (0x28, false)}, {'"', (0x28, true)},
            {'z', (0x2C, false)}, {'Z', (0x2C, true)},
            {'x', (0x2D, false)}, {'X', (0x2D, true)},
            {'c', (0x2E, false)}, {'C', (0x2E, true)},
            {'v', (0x2F, false)}, {'V', (0x2F, true)},
            {'b', (0x30, false)}, {'B', (0x30, true)},
            {'n', (0x31, false)}, {'N', (0x31, true)},
            {'m', (0x32, false)}, {'M', (0x32, true)},
            {',', (0x33, false)}, {'<', (0x33, true)},
            {'.', (0x34, false)}, {'>', (0x34, true)},
            {'/', (0x35, false)}, {'?', (0x35, true)},
            {' ', (0x39, false)},
            {'\r', (0x1C, false)}, {'\n', (0x1C, false)},
            {'\t', (0x0F, false)},
        };

        public static void SendCharViaScanCode(char c)
        {
            try
            {
                Log($"SendCharViaScanCode: '{c}' (0x{((int)c):X4}), targetLayoutId='{_targetLayoutId}'");
                
                if (_targetLayoutId == "00000409" && USKeyboardMap.TryGetValue(c, out var mapping))
                {
                    Log($"Using US scan code: 0x{mapping.scanCode:X2}, shift={mapping.shift}");
                    if (mapping.shift) SendScanCode(0x2A, false);
                    SendScanCode(mapping.scanCode, false);
                    SendScanCode(mapping.scanCode, true);
                    if (mapping.shift) SendScanCode(0x2A, true);
                    return;
                }
                
                if (!string.IsNullOrEmpty(_targetLayoutId))
                {
                    Log("Using SendUnicodeChar");
                    SendUnicodeChar(c);
                    return;
                }

                int count = (int)GetKeyboardLayoutList(0, null);
                Log($"GetKeyboardLayoutList returned {count}");
                if (count > 0)
                {
                    IntPtr[] layouts = new IntPtr[count];
                    GetKeyboardLayoutList(count, layouts);

                    foreach (var hkl in layouts)
                    {
                        if (TrySendCharWithLayout(c, hkl))
                            return;
                    }
                }

                Log("Falling back to ALT numpad");
                SendCharViaAltNumpad(c);
            }
            catch (Exception ex)
            {
                Log($"ERROR in SendCharViaScanCode: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
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

        private static void SendKeyWithScanCodeEx(byte vk, bool keyUp, IntPtr hkl)
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
        private static uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private static int HWND_BROADCAST = 0xffff;
        private static uint KLF_ACTIVATE = 1;

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
