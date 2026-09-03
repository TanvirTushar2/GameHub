using System.Drawing;
using System.Windows.Forms;

namespace GameHub.Forms.Controls
{
    internal class StatCardControl : GamePanel
    {
        private readonly Label _caption;
        private readonly Label _value;
        private readonly Label _note;

        public StatCardControl(string caption, string value, string note)
        {
            StartColor = Theme.CardBg;
            EndColor = Theme.SurfaceAlt;
            Gradient = true;
            GradientAngle = 25f;
            OutlineColor = Theme.BorderSoft;
            CornerRadius = Theme.Radius;
            Margin = new Padding(0, 0, 12, 0);
            MinimumSize = new Size(150, 92);

            _caption = Ui.Lbl(caption.ToUpperInvariant(), 16, 14, Theme.SmallBold, Theme.AccentBlue);
            Controls.Add(_caption);

            _value = Ui.Lbl(
    value,
    16,
    37,
    new Font("Segoe UI", 14.5F, FontStyle.Bold),
    Theme.Text);
            _value.AutoSize = false;
            _value.SetBounds(16, 35, 240, 34);
            Controls.Add(_value);

            _note = Ui.Lbl(note, 17, 70, Theme.SmallFont, Theme.Muted2);
            _note.AutoSize = false;
            _note.SetBounds(17, 72, 230, 18);
            Controls.Add(_note);

            Resize += delegate
            {
                int inner = System.Math.Max(50, ClientSize.Width - 32);
                _caption.MaximumSize = new Size(inner, 0);
                _value.Width = inner;
                _note.Width = inner;
            };
        }

        public string ValueText
        {
            get { return _value.Text; }
            set { _value.Text = value ?? string.Empty; }
        }

        public string NoteText
        {
            get { return _note.Text; }
            set { _note.Text = value ?? string.Empty; }
        }

        public Color AccentColor
        {
            set { _caption.ForeColor = value; }
        }
    }
}
