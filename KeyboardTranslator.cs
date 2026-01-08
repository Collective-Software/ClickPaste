using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ClickPaste
{
    static class KeyboardTranslator
    {
        static string _targetLayoutId;
        static readonly string _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clickpaste.log");

        public static string TargetLayoutId => _targetLayoutId;

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(string msg)
        {
            try
            {
                File.AppendAllText(_logPath, $"{DateTime.Now:HH:mm:ss.fff} {msg}\n");
            }
            catch { }
        }

        public static void SetTargetLayout(string layoutId)
        {
            Log($"SetTargetLayout: '{layoutId}'");
            _targetLayoutId = layoutId;
        }

        public static bool TrySendChar(char c)
        {
            if (string.IsNullOrEmpty(_targetLayoutId))
                return false;

            Log($"TrySendChar: '{c}' (0x{((int)c):X4}), layout='{_targetLayoutId}'");

            if (_targetLayoutId == "00000409" && USKeyboardMap.TryGetValue(c, out var mapping))
            {
                Log($"Using US scan code: 0x{mapping.scanCode:X2}, shift={mapping.shift}");
                if (mapping.shift) SendScanCode(0x2A, false);
                SendScanCode(mapping.scanCode, false);
                SendScanCode(mapping.scanCode, true);
                if (mapping.shift) SendScanCode(0x2A, true);
                return true;
            }

            Log("Using SendUnicodeChar fallback");
            Native.SendUnicodeChar(c);
            return true;
        }

        static void SendScanCode(ushort scanCode, bool keyUp)
        {
            Native.INPUT[] inputs = new Native.INPUT[1];
            inputs[0].type = Native.INPUT_KEYBOARD;
            inputs[0].ki.wVk = 0;
            inputs[0].ki.wScan = scanCode;
            inputs[0].ki.dwFlags = Native.KEYEVENTF_SCANCODE | (keyUp ? Native.KEYEVENTF_KEYUP : 0);
            inputs[0].ki.time = 0;
            inputs[0].ki.dwExtraInfo = IntPtr.Zero;
            Native.SendInput(1, inputs, Marshal.SizeOf(typeof(Native.INPUT)));
        }

        static readonly Dictionary<char, (ushort scanCode, bool shift)> USKeyboardMap = new Dictionary<char, (ushort, bool)>
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
    }
}

