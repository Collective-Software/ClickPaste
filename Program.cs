using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
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
    }
    public enum TypeMethod
    {
        Forms_SendKeys = 0,
        AutoIt_Send,
        Length
    }
    public class TrayApplicationContext : ApplicationContext
    {
        NotifyIcon _notify = null;
        IKeyboardMouseEvents _hook = null;
        MenuItem[] _typeMethods;
        public TrayApplicationContext()
        {
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

            _typeMethods = new MenuItem[(int)TypeMethod.Length];
            for(int i = 0; i < (int)TypeMethod.Length; i++)
            {
                _typeMethods[i] = new MenuItem(((TypeMethod)i).ToString(), ChangeTypeMethod);
                _typeMethods[i].RadioCheck = true;
                _typeMethods[i].Tag = i;
            };
            _typeMethods[Properties.Settings.Default.TypeMethod].Checked = true;
            _notify = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(darkTray ? Properties.Resources.Target : Properties.Resources.TargetDark, traySize.Width, traySize.Height),
                Visible = true,
                ContextMenu = 
                new ContextMenu(
                    new MenuItem[] 
                    {
                        new MenuItem("Typing method", _typeMethods),
                        new MenuItem("-"),
                        new MenuItem("Exit", Exit),
                    }
                ),
                Text = "ClickPaste: Click to choose a target"
            };
            _notify.MouseDown += _notify_MouseDown;
        }

        private void ChangeTypeMethod(object sender, EventArgs e)
        {
            Properties.Settings.Default.TypeMethod = (int)((sender as MenuItem).Tag);
            Properties.Settings.Default.Save();
            foreach(var item in _typeMethods)
            {
                item.Checked = false;
            }
            _typeMethods[Properties.Settings.Default.TypeMethod].Checked = true;
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
            }
        }
        void EndTrack()
        {
            if (_hook != null)
            {
                _hook.MouseUp -= _hook_MouseUp;
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
                            Task.Delay(100);
                            // left click has selected the thing we want to paste, and placed the cursor
                            // so all we have to do is type

                            switch((TypeMethod) Properties.Settings.Default.TypeMethod)
                            {
                                case TypeMethod.AutoIt_Send:
                                    AutoIt.AutoItX.Send(clip, 1);
                                    break;
                                case TypeMethod.Forms_SendKeys:
                                    SendKeys.SendWait(EscapeSendKeys(clip));
                                    break;
                            }
                        }
                    });
                    break;
            }
        }
        string EscapeSendKeys(string raw)
        {
            string escaped = "";
            string remaining = raw;
            var specials = new char[]{ '{', '}', '[', ']', '+', '^', '%', '~', '(', ')' };
            while (remaining.Length > 0)
            {
                var idx = remaining.IndexOfAny(specials);
                if (-1 != idx)
                {
                    escaped += remaining.Substring(0, idx);
                    var special = remaining.Substring(idx, 1);
                    escaped += "{" + special + "}";
                    remaining = remaining.Substring(idx + 1);
                }
                else
                {
                    escaped += remaining;
                    remaining = "";
                }
            }
            return escaped;
        }

        void Exit(object sender, EventArgs e)
        {
            EndTrack();
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _notify.Visible = false;
            _notify.Dispose();

            Application.Exit();
        }
    }
}
