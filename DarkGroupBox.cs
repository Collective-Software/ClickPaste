using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClickPaste
{
    /// <summary>
    /// A GroupBox that properly renders in dark mode without transparency glitches.
    /// </summary>
    public class DarkGroupBox : GroupBox
    {
        private bool _darkMode = false;

        public bool DarkMode
        {
            get => _darkMode;
            set
            {
                _darkMode = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!_darkMode)
            {
                // Use default rendering for light mode
                base.OnPaint(e);
                return;
            }

            // Custom dark mode rendering
            Graphics g = e.Graphics;
            g.Clear(BackColor);

            // Measure the text
            Size textSize = TextRenderer.MeasureText(Text, Font);
            int textOffset = 8;
            int textPadding = 2;

            // Draw the border (rounded rectangle around the group, with gap for text)
            using (Pen borderPen = new Pen(ThemeHelper.DarkBorder, 1))
            {
                int borderY = textSize.Height / 2;
                int borderHeight = Height - borderY - 1;
                int borderWidth = Width - 1;

                // Draw the border in segments to leave gap for text
                // Left segment of top border
                g.DrawLine(borderPen, 0, borderY, textOffset - textPadding, borderY);
                // Right segment of top border (after text)
                g.DrawLine(borderPen, textOffset + textSize.Width + textPadding, borderY, borderWidth, borderY);
                // Left border
                g.DrawLine(borderPen, 0, borderY, 0, borderY + borderHeight);
                // Bottom border
                g.DrawLine(borderPen, 0, borderY + borderHeight, borderWidth, borderY + borderHeight);
                // Right border
                g.DrawLine(borderPen, borderWidth, borderY, borderWidth, borderY + borderHeight);
            }

            // Draw the text
            if (!string.IsNullOrEmpty(Text))
            {
                TextRenderer.DrawText(g, Text, Font, new Point(textOffset, 0), ForeColor);
            }
        }
    }
}
