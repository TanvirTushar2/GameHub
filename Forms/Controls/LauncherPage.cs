using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameHub.Forms.Controls
{
    internal  class LauncherPage : UserControl
    {
        public event Action<string> NavigateRequested;
        public event Action<int> GameOpenRequested;
        public event Action GlobalDataChanged;

        protected LauncherPage()
        {
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            Dock = DockStyle.Fill;
            Margin = new Padding(0);
            Padding = new Padding(0);
            BackColor = Theme.Background;
            Font = Theme.BodyFont;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
        }

        public virtual void RefreshData()
        {
        }

        protected void RequestNavigate(string page)
        {
            Action<string> handler = NavigateRequested;
            if (handler != null) handler(page);
        }

        protected void RequestOpenGame(int gameId)
        {
            Action<int> handler = GameOpenRequested;
            if (handler != null) handler(gameId);
        }

        protected void RaiseGlobalDataChanged()
        {
            Action handler = GlobalDataChanged;
            if (handler != null) handler();
        }

        // Retained for any future simple page, but places subtitle after title explicitly.
        protected static Label Header(string title, string subtitle, Control parent)
        {
            Panel holder = new Panel();
            holder.Dock = DockStyle.Top;
            holder.Height = 78;
            holder.BackColor = Theme.Background;
            parent.Controls.Add(holder);

            Label h = Ui.FixedLabel(title, 0, 0, 720, 38, Theme.TitleFont, Theme.Text);
            holder.Controls.Add(h);

            Label s = Ui.FixedLabel(subtitle, 2, 41, 900, 28, Theme.BodyFont, Theme.Muted);
            holder.Controls.Add(s);
            holder.Resize += delegate
            {
                h.Width = Math.Max(220, holder.ClientSize.Width);
                s.Width = Math.Max(220, holder.ClientSize.Width - 4);
            };
            return h;
        }
    }
}
