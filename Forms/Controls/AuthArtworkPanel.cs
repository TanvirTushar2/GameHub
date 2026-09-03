using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameHub.Services;

namespace GameHub.Forms.Controls
{
    internal class AuthArtworkPanel : UserControl
    {
        private readonly Image _image;

        public AuthArtworkPanel()
        {
            BackColor = Theme.Background2;
            _image = GameAssetService.GetLoginHero(1600, 1000);
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            Controls.Add(Ui.Pill("GAMEHUB DESKTOP", 28, 26, 138, Color.FromArgb(205, Theme.Background2), Theme.Cream));
            Label title = Ui.FixedLabel("PLAY WITHOUT\nLEAVING YOUR WORLD.", 30, 0, 430, 92,
                new Font("Segoe UI", 24F, FontStyle.Bold), Color.White);
            title.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            Controls.Add(title);

            Label copy = Ui.FixedLabel("Discover • Buy • Build your library • Earn rewards", 32, 0, 430, 24,
                Theme.BodyBold, Color.FromArgb(225, 236, 242, 255));
            copy.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            Controls.Add(copy);

            Resize += delegate
            {
                title.Top = ClientSize.Height - 142;
                copy.Top = ClientSize.Height - 54;
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Ui.DrawImageCover(g, _image, ClientRectangle, 1f);

            using (LinearGradientBrush shade = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(45, Theme.Background),
                Color.FromArgb(115, Theme.Background),
                90f))
                g.FillRectangle(shade, ClientRectangle);

            Rectangle lower = new Rectangle(0, System.Math.Max(0, Height - 210), Width, 210);
            using (LinearGradientBrush fade = new LinearGradientBrush(
                lower, Color.FromArgb(0, Theme.Background), Color.FromArgb(235, Theme.Background), 90f))
                g.FillRectangle(fade, lower);
        }
    }
}
