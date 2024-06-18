using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickPaste
{
    public partial class SettingsForm : Form
    {
        RadioButton[] _methods;
        CheckBox[] _modifiers;
        public SettingsForm()
        {
            InitializeComponent();
            _methods = new RadioButton[2];
            _methods[0] = Method_Forms;
            _methods[1] = Method_AutoIt;
            _modifiers = new CheckBox[4];
            _modifiers[0] = HotKey_Alt;
            _modifiers[1] = HotKey_Control;
            _modifiers[2] = HotKey_Shift;
            _modifiers[3] = HotKey_Windows;

            foreach(var method in _methods)
            {
                method.Checked = (Properties.Settings.Default.TypeMethod == int.Parse(method.Tag.ToString()));
            }
            DelayBetweenMS.Text = Properties.Settings.Default.KeyDelayBetweenMS.ToString();
            DelayBeforeMS.Text = Properties.Settings.Default.KeyDelayBeforeMS.ToString();
            HotKey_Letter.Text = Properties.Settings.Default.HotKey;
            foreach(var mod in _modifiers)
            {
                mod.Checked = (0 != (Properties.Settings.Default.HotKeyModifier & int.Parse(mod.Tag.ToString())));
            }
        }
        private void HotKey_Letter_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Alt:
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Shift:
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.Control:
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.LWin:
                case Keys.RWin:
                case Keys.Return:
                    break;
                case Keys.Delete:
                case Keys.Back:
                    HotKey_Letter.Text = string.Empty;
                    break;
                default:
                    HotKey_Letter.Text = e.KeyCode.ToString();
                    break;
            }
            e.SuppressKeyPress = true;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            foreach(var method in _methods)
            {
                if (_methods[0].Checked)
                {
                    Properties.Settings.Default.TypeMethod = (int)TypeMethod.Forms_SendKeys;
                }
                else
                { 
                    Properties.Settings.Default.TypeMethod = (int)TypeMethod.AutoIt_Send;
                }
            }
            int delay;
            if (int.TryParse(DelayBetweenMS.Text, out delay))
            {
                Properties.Settings.Default.KeyDelayBetweenMS = delay;
            }
            if (int.TryParse(DelayBeforeMS.Text, out delay))
            {
                Properties.Settings.Default.KeyDelayBeforeMS = delay;
            }
            var letter = HotKey_Letter.Text;
            if (letter.Length == 1) letter = letter.ToUpperInvariant(); // can't find 'v' but knows 'V'
            Properties.Settings.Default.HotKey = letter;
            int mods = 0;
            foreach (var mod in _modifiers)
            {
                if (mod.Checked)
                {
                    mods |= int.Parse(mod.Tag.ToString());
                }
                Properties.Settings.Default.HotKeyModifier = mods;
            }
            Properties.Settings.Default.Save();
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void Method_Forms_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
