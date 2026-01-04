using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
