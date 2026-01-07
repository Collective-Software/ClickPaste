using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickPaste
{
    /// <summary>
    /// Provides Windows dark/light theme detection and colors.
    /// </summary>
    public static class ThemeHelper
    {
        // Windows 11 dark mode colors
        public static readonly Color DarkBackground = Color.FromArgb(32, 32, 32);      // #202020
        public static readonly Color DarkSurface = Color.FromArgb(43, 43, 43);         // #2B2B2B
        public static readonly Color DarkBorder = Color.FromArgb(60, 60, 60);          // #3C3C3C
        public static readonly Color DarkText = Color.FromArgb(255, 255, 255);         // #FFFFFF
        public static readonly Color DarkTextSecondary = Color.FromArgb(180, 180, 180);// #B4B4B4

        public static event EventHandler ThemeChanged;

        static ThemeHelper()
        {
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                ThemeChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns true if apps should use dark mode.
        /// </summary>
        public static bool IsDarkMode
        {
            get
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    var value = key?.GetValue("AppsUseLightTheme")?.RegToUint();
                    return value.HasValue && value.Value == 0;
                }
            }
        }

        /// <summary>
        /// Returns true if the system tray/taskbar is dark.
        /// </summary>
        public static bool IsSystemDark
        {
            get
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    var value = key?.GetValue("SystemUsesLightTheme")?.RegToUint();
                    return !value.HasValue || value.Value == 0;
                }
            }
        }

        /// <summary>
        /// Applies dark or light theme colors to a form and all its controls.
        /// </summary>
        public static void ApplyTheme(Control control, bool dark)
        {
            if (dark)
            {
                if (control is Form)
                {
                    control.BackColor = DarkBackground;
                    control.ForeColor = DarkText;
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = DarkBackground;
                    textBox.ForeColor = DarkText;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.BackColor = DarkBackground;
                    groupBox.ForeColor = DarkText;
                }
                else if (control is Button button)
                {
                    button.BackColor = DarkSurface;
                    button.ForeColor = DarkText;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = DarkBorder;
                }
                else if (control is Label label)
                {
                    // Labels inherit parent background
                    label.BackColor = Color.Transparent;
                    label.ForeColor = DarkText;
                }
                else if (control is RadioButton || control is CheckBox)
                {
                    control.BackColor = Color.Transparent;
                    control.ForeColor = DarkText;
                }
                else
                {
                    control.BackColor = DarkBackground;
                    control.ForeColor = DarkText;
                }
            }
            else
            {
                control.BackColor = SystemColors.Control;
                control.ForeColor = SystemColors.ControlText;

                if (control is TextBox textBox)
                {
                    textBox.BackColor = SystemColors.Window;
                    textBox.BorderStyle = BorderStyle.Fixed3D;
                }
                else if (control is Button button)
                {
                    button.BackColor = SystemColors.Control;
                    button.FlatStyle = FlatStyle.System;
                    button.UseVisualStyleBackColor = true;
                }
            }

            foreach (Control child in control.Controls)
            {
                ApplyTheme(child, dark);
            }
        }

        /// <summary>
        /// Gets a dark mode renderer for ContextMenuStrip if dark mode is enabled.
        /// </summary>
        public static ToolStripRenderer GetMenuRenderer(bool dark)
        {
            return dark ? new DarkMenuRenderer() : new ToolStripProfessionalRenderer();
        }
    }

    /// <summary>
    /// Custom renderer for dark mode context menus.
    /// </summary>
    public class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkMenuColors()) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = ThemeHelper.DarkText;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = ThemeHelper.DarkText;
            base.OnRenderArrow(e);
        }
    }

    public class DarkMenuColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected => ThemeHelper.DarkSurface;
        public override Color MenuItemSelectedGradientBegin => ThemeHelper.DarkSurface;
        public override Color MenuItemSelectedGradientEnd => ThemeHelper.DarkSurface;
        public override Color MenuItemBorder => ThemeHelper.DarkBorder;
        public override Color MenuBorder => ThemeHelper.DarkBorder;
        public override Color ToolStripDropDownBackground => ThemeHelper.DarkBackground;
        public override Color ImageMarginGradientBegin => ThemeHelper.DarkBackground;
        public override Color ImageMarginGradientMiddle => ThemeHelper.DarkBackground;
        public override Color ImageMarginGradientEnd => ThemeHelper.DarkBackground;
        public override Color SeparatorDark => ThemeHelper.DarkBorder;
        public override Color SeparatorLight => ThemeHelper.DarkBorder;
    }

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

            // Enable dark mode for context menus (must be before any menus are created)
            Native.SetAppDarkMode(ThemeHelper.IsDarkMode);

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
        AutoIt_Send = 1,
        SendInput_ScanCode = 3  // Scan codes with ALT code fallback - works everywhere including VM consoles
    }
    public enum HotKeyMode
    {
        Target = 0,
        JustGo
    }
    public class TrayApplicationContext : ApplicationContext
    {
        NotifyIcon _notify = null;
        IKeyboardMouseEvents _hook = null;
        MenuItem[] _typeMethods; // these are just sequential 0-based integers so don't need to map them like...
        Dictionary<int, MenuItem> _keyDelayMS;// we do here
        int? _usingHotKey;
        EventHandler<HotKeyEventArgs> _currentHotKeyHandler = null;
        CancellationTokenSource _stop = new CancellationTokenSource();

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
            StopHotKey();
            // wait until control keys are no longer pressed
            while (Native.IsModifierKeyPressed())
            {
                Thread.Sleep(300);
            }
            switch((HotKeyMode) Properties.Settings.Default.HotKeyMode)
            {
                case HotKeyMode.Target:
                    StartTrack();
                    break;
                case HotKeyMode.JustGo:
                    StartTyping();
                    break;
            }
        }
        private void HotKeyManager_EscapePressed(object sender, HotKeyEventArgs e)
        {
            StopHotKey();
            _stop.Cancel();
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

                    if (!_settingsOpen)
                    {
                        StartTrack();
                    }                    
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
                    StartTyping();
                    break;
            }
        }
        void StartTyping()
        {
            var clip = Clipboard.GetText();

            // whatever window is under focus right now should be the target

            if (Properties.Settings.Default.Confirm && clip.Length > Properties.Settings.Default.ConfirmOver)
            {
                SystemSounds.Beep.Play();
                var w = Native.GetForegroundWindow();
                if (DialogResult.Yes != MessageBox.Show($"Confirm typing {clip.Length} characters to window '{Native.GetText(w).First(50)}'?", "ClickPaste Confirm Typing", MessageBoxButtons.YesNo))
                {
                    StartHotKey();// resume normal hotkey listening
                    Native.SetForegroundWindow(w);
                    return;
                }
                Native.SetForegroundWindow(w);
            }
            _stop = new CancellationTokenSource();
            Task.Run(() =>
            {
                System.Drawing.Icon originalIcon = null;
                try
                {
                    Native.Log("Task.Run started");
                    if (string.IsNullOrEmpty(clip))
                    {
                        SystemSounds.Beep.Play();
                    }
                    else
                    {
                        var cancel = _stop.Token;
                        var traySize = SystemInformation.SmallIconSize;
                        originalIcon = _notify.Icon;
                        _notify.Icon = new System.Drawing.Icon(Properties.Resources.Typing, traySize.Width, traySize.Height);
                        int startDelayMS = Properties.Settings.Default.StartDelayMS;
                        Thread.Sleep(100 + startDelayMS);
                        StartHotKeyEscape();
                        int keyDelayMS = Properties.Settings.Default.KeyDelayMS;
                        var method = (TypeMethod)Properties.Settings.Default.TypeMethod;
                        var targetLayout = Properties.Settings.Default.TargetKeyboardLayout;
                        
                        Native.Log($"StartTyping: method={method}, targetLayout='{targetLayout}', clipLen={clip.Length}");
                        Native.SetTargetKeyboardLayout(targetLayout);
                        IList<string> list = PrepareKeystrokes(clip, method);
                        Native.Log($"PrepareKeystrokes returned {list.Count} items");

                        if (TypeMethod.AutoIt_Send == method)
                        {
                            AutoIt.AutoItX.AutoItSetOption("SendKeyDelay", 0);
                        }
                        foreach (var s in list)
                        {
                            switch (method)
                            {
                                case TypeMethod.AutoIt_Send:
                                    AutoIt.AutoItX.Send(s, 1);
                                    break;
                                case TypeMethod.Forms_SendKeys:
                                    SendKeys.SendWait(s);
                                    break;
                                case TypeMethod.SendInput_ScanCode:
                                    foreach (char c in s)
                                    {
                                        Native.SendCharViaScanCode(c);
                                    }
                                    break;
                            }
                            Thread.Sleep(keyDelayMS);
                            if (cancel.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Native.Log($"EXCEPTION in typing task: {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    if (originalIcon != null)
                        _notify.Icon = originalIcon;
                    StartHotKey();
                }
            });

        }
        IList<string> PrepareKeystrokes(string raw, TypeMethod method)
        {
            var list = new List<string>();
            var specials = @"{}[]+^%~()";
            raw = raw.Replace("\r\n", "\r");// both typing methods treat each of these as "hit enter" in notepads, and Word treats \n as "next page" somehow.
            foreach(char c in raw)
            {
                if(method == TypeMethod.Forms_SendKeys && (-1 != specials.IndexOf(c)))
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
            StopHotKey();
            var hotkeyLetter = Properties.Settings.Default.HotKey;
            if (!string.IsNullOrEmpty(hotkeyLetter))
            {
                try
                {
                    Keys HotKey = (Keys)Enum.Parse(typeof(Keys), hotkeyLetter);
                    _usingHotKey = HotKeyManager.RegisterHotKey(HotKey, (KeyModifiers)Properties.Settings.Default.HotKeyModifier);
                    _currentHotKeyHandler = new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
                    HotKeyManager.HotKeyPressed += _currentHotKeyHandler;
                }
                catch(Exception e)
                {
                    MessageBox.Show("Could not register hot key: " + e.Message);
                }
            }
        }
        void StartHotKeyEscape()
        {
            StopHotKey();
            try
            {
                _usingHotKey = HotKeyManager.RegisterHotKey(Keys.Escape, KeyModifiers.None);
                _currentHotKeyHandler = new EventHandler<HotKeyEventArgs>(HotKeyManager_EscapePressed);
                HotKeyManager.HotKeyPressed += _currentHotKeyHandler;
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not register hot key: " + e.Message);
            }
        }
        void StopHotKey()
        {
            if(_usingHotKey.HasValue)
            {
                HotKeyManager.HotKeyPressed -= _currentHotKeyHandler;
                HotKeyManager.UnregisterHotKey(_usingHotKey.Value);
            }
            _usingHotKey = null;
            _currentHotKeyHandler = null;
        }

        void Settings(object sender, EventArgs e)
        {
            if (_settingsOpen)
            {
                return;
            }
            _settingsOpen = true;
            StopHotKey();
            var settings = new SettingsForm();
            settings.ShowDialog();
            _settingsOpen = false;
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
