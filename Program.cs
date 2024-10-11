using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickPaste
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if(Properties.Settings.Default.RunElevated)
            {

            }
            // only run one of these at a time!
            //https://stackoverflow.com/questions/502303/how-do-i-programmatically-get-the-guid-of-an-application-in-net2-0/502323#502323
            string assyGuid = Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>().Value.ToUpper();
            bool createdNew;
            new System.Threading.Mutex(false, assyGuid, out createdNew);
            if (!createdNew) return;

            Native.SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // try to make sure we don't die leaving the cursor in "+" state
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            Application.Run(new TrayApplicationContext());

        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // reset cursors
            Native.SystemParametersInfo(0x0057, 0, null, 0);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // reset cursors
            Native.SystemParametersInfo(0x0057, 0, null, 0);
        }
    }
    public static class Extensions
    {
        public static uint? RegToUint(this object value) => value == null ? (uint?)null : Convert.ToUInt32(value);
        public static string[] Names(this Type enumType) => Enum.GetNames(enumType);
        public static int Count(this Type enumType) => enumType.Names().Length;
        public static object Value(this Type enumType, string name) => Enum.Parse(enumType, name);
        public static string First(this string s, int count)
        {
            if (count > s.Length) return s;
            return s.Substring(0, count);
        }
    }
    public enum TypeMethod
    {
        Forms_SendKeys = 0,
        AutoIt_Send
    }

    public class TrayApplicationContext : ApplicationContext
    {
        NotifyIcon _notify = null;
        IKeyboardMouseEvents _hook = null;
        MenuItem[] _typeMethods; // these are just sequential 0-based integers so don't need to map them like...
        Dictionary<int, MenuItem> _keyDelayMS;// we do here
        int? _usingHotKey;
        bool _settingsOpen = false;
        public TrayApplicationContext()
        {
            StartHotKey();
            bool darkTray = true;
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
            {
                var light = key?.GetValue("SystemUsesLightTheme")?.RegToUint();
                if (light.HasValue)
                {
                    darkTray = light.Value != 1;
                }
            }
            var traySize = SystemInformation.SmallIconSize;

            _notify = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(darkTray ? Properties.Resources.Target : Properties.Resources.TargetDark, traySize.Width, traySize.Height),
                Visible = true,
                ContextMenu = 
                new ContextMenu(
                    new MenuItem[] 
                    {
                        new MenuItem("Settings", Settings),
                        new MenuItem("-"),
                        new MenuItem("Exit", Exit),
                    }
                ),
                Text = "ClickPaste: Click to choose a target"
            };
            _notify.MouseDown += _notify_MouseDown;
        }
        private void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            StartTrack();
        }
        void StartTrack()
        {
            if (_hook == null)
            {
                uint[] Cursors = { Native.NORMAL, Native.IBEAM, Native.HAND };

                for (int i = 0; i < Cursors.Length; i++)
                    Native.SetSystemCursor(Native.CopyIcon(Native.LoadCursor(IntPtr.Zero, (int)Native.CROSS)), Cursors[i]);
                _hook = Hook.GlobalEvents();
                _hook.MouseUp += _hook_MouseUp;
                _hook.KeyDown += _hook_KeyDown;
            }
        }


        private void _hook_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                EndTrack();
            }
        }

        void EndTrack()
        {
            if (_hook != null)
            {
                _hook.MouseUp -= _hook_MouseUp;
                _hook.KeyDown -= _hook_KeyDown;
                _hook.Dispose();
                _hook = null;
                Native.SystemParametersInfo(0x0057, 0, null, 0);
            }
        }

        private void _notify_MouseDown(object sender, MouseEventArgs e)
        {
            switch(e.Button)

            {
                //case MouseButtons.Middle:
                case MouseButtons.Left: // this is a lie, we only get left after mouse released
            
                    StartTrack();
                    
                    break;
            }
        }
        private void _hook_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                //case MouseButtons.Middle:
                    EndTrack();
                    var clip = Clipboard.GetText();

                    if (Properties.Settings.Default.Confirm && clip.Length > Properties.Settings.Default.ConfirmOver)
                    {
                        SystemSounds.Beep.Play();
                        var w = Native.GetForegroundWindow();
                        if (DialogResult.Yes != MessageBox.Show($"Confirm typing {clip.Length} characters to window '{Native.GetText(w).First(50)}'?", "ClickPaste Confirm Typing", MessageBoxButtons.YesNo))
                        {
                            Native.SetForegroundWindow(w);
                            return;
                        }
                        Native.SetForegroundWindow(w);
                    }
                    Task.Run(() =>
                    {
                        // check if it's my window
                        //IntPtr hwnd = Native.WindowFromPoint(e.X, e.Y);
                        // ... we don't have a window yet
                        if (string.IsNullOrEmpty(clip))
                        {
                            // nothing to paste
                            SystemSounds.Beep.Play();
                        }
                        else
                        {
                            var traySize = SystemInformation.SmallIconSize;
                            var icon = _notify.Icon;
                            _notify.Icon = new System.Drawing.Icon(Properties.Resources.Typing, traySize.Width, traySize.Height);
                            int startDelayMS = Properties.Settings.Default.StartDelayMS;
                            Thread.Sleep(100 + startDelayMS);
                            // don't listen to our own typing
                            StopHotKey();
                            // left click has selected the thing we want to paste, and placed the cursor
                            // so all we have to do is type
                            int keyDelayMS = Properties.Settings.Default.KeyDelayMS;
                            switch((TypeMethod) Properties.Settings.Default.TypeMethod)
                            {
                                case TypeMethod.AutoIt_Send:
                                    AutoIt.AutoItX.AutoItSetOption("SendKeyDelay", keyDelayMS);
                                    AutoIt.AutoItX.Send(clip, 1);
                                    break;
                                case TypeMethod.Forms_SendKeys:
                                    var list = ProcessSendKeys(clip);
                                    foreach(var s in list)
                                    {
                                        SendKeys.SendWait(s);
                                        Thread.Sleep(keyDelayMS);
                                    }
                                    break;
                            }
                            _notify.Icon = icon;
                            StartHotKey();
                        }
                    });
                    break;
            }
        }
        IList<string> ProcessSendKeys(string raw)
        {
            var list = new List<string>();
            var specials = @"{}[]+^%~()";
            foreach(char c in raw)
            {
                if(-1 != specials.IndexOf(c))
                {
                    list.Add("{" + c.ToString() + "}");
                }
                else
                {
                    list.Add(c.ToString());
                }
            }
            return list;
        }
        void StartHotKey()
        {
            var hotkeyLetter = Properties.Settings.Default.HotKey;
            if (!string.IsNullOrEmpty(hotkeyLetter))
            {
                try
                {
                    Keys HotKey = (Keys)Enum.Parse(typeof(Keys), hotkeyLetter);
                    _usingHotKey = HotKeyManager.RegisterHotKey(HotKey, (KeyModifiers)Properties.Settings.Default.HotKeyModifier);
                    HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
                }
                catch(Exception e)
                {
                    MessageBox.Show("Could not register hot key: " + e.Message);
                }
            }
        }
        void StopHotKey()
        {
            if(_usingHotKey.HasValue)
            {
                HotKeyManager.HotKeyPressed -= HotKeyManager_HotKeyPressed;
                HotKeyManager.UnregisterHotKey(_usingHotKey.Value);
            }
            _usingHotKey = null;
        }
        void Settings(object sender, EventArgs e)
        {
            if (!_settingsOpen)
            {
                _settingsOpen = true;
            }
            StopHotKey();
            var settings = new SettingsForm();
            settings.ShowDialog();
            StartHotKey();            
        }

        void Exit(object sender, EventArgs e)
        {
            EndTrack();
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _notify.Visible = false;
            _notify.Dispose();
            StopHotKey();
            Application.Exit();
        }
    }
}
