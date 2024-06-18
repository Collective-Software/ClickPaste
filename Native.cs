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
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll")]
        public static extern int ActivateKeyboardLayout(int HKL, int flags);
        /*
        private static uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private static int HWND_BROADCAST = 0xffff;
        private static uint KLF_ACTIVATE = 1;
        */
    }
}
