using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace GameHub.Forms
{
    /// <summary>Rounded, anti-aliased surface used throughout GameHub.</summary>
    internal class GamePanel : Panel
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public Color OutlineColor { get; set; }
        public int CornerRadius { get; set; }
        public bool Gradient { get; set; }
        public float GradientAngle { get; set; }

        public GamePanel()
        {
            StartColor = Theme.CardBg;
            EndColor = Theme.CardBg;
            OutlineColor = Theme.BorderSoft;
            CornerRadius = Theme.Radius;
            Gradient = false;
            GradientAngle = 35f;
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateRegion();
        }

        private void UpdateRegion()
        {
            if (Width <= 1 || Height <= 1) return;
            using (GraphicsPath path = Ui.RoundedPath(new Rectangle(0, 0, Width, Height), CornerRadius))
            {
                Region old = Region;
                Region = new Region(path);
                if (old != null) old.Dispose();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle rect = new Rectangle(0, 0, Math.Max(1, Width), Math.Max(1, Height));
            using (GraphicsPath path = Ui.RoundedPath(rect, CornerRadius))
            {
                if (Gradient)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(rect, StartColor, EndColor, GradientAngle))
                        e.Graphics.FillPath(brush, path);
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(StartColor))
                        e.Graphics.FillPath(brush, path);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (OutlineColor.A <= 0 || Width < 2 || Height < 2) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = Ui.RoundedPath(rect, CornerRadius))
            using (Pen pen = new Pen(OutlineColor, 1f))
                e.Graphics.DrawPath(pen, path);
        }
    }

    internal static class Ui
    {
        public static Button Btn(string text, Color back, int x, int y, int w, int h)
        {
            Button b = new Button();
            b.Text = text;
            b.BackColor = back;
            b.ForeColor = Theme.Text;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Lighten(back, 1.12);
            b.FlatAppearance.MouseDownBackColor = Darken(back, 0.86);
            b.Font = Theme.BodyBold;
            b.SetBounds(x, y, w, h);
            b.UseVisualStyleBackColor = false;
            b.UseCompatibleTextRendering = false;
            b.Cursor = Cursors.Hand;
            b.TabStop = true;
            RoundControl(b, 11);
            b.Resize += delegate { RoundControl(b, 11); };
            return b;
        }

        public static Button GhostBtn(string text, int x, int y, int w, int h)
        {
            Button b = Btn(text, Theme.SurfaceAlt, x, y, w, h);
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = Theme.Border;
            return b;
        }

        public static Label Lbl(string text, int x, int y, Font font, Color color)
        {
            Label l = new Label();
            l.Text = text;
            l.Font = font;
            l.ForeColor = color;
            l.AutoSize = true;
            l.BackColor = Color.Transparent;
            l.UseCompatibleTextRendering = false;
            l.Location = new Point(x, y);
            return l;
        }

        public static Label FixedLabel(string text, int x, int y, int w, int h, Font font, Color color)
        {
            Label l = Lbl(text, x, y, font, color);
            l.AutoSize = false;
            l.SetBounds(x, y, w, h);
            return l;
        }

        public static Button Back(Form f)
        {
            Button b = GhostBtn("←  Back", f.ClientSize.Width - 118, 20, 96, 34);
            b.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            b.Click += (s, e) => f.Close();
            return b;
        }

        public static GamePanel Card(int x, int y, int w, int h)
        {
            GamePanel p = new GamePanel();
            p.SetBounds(x, y, w, h);
            p.StartColor = Theme.CardBg;
            p.EndColor = Theme.CardBg;
            p.OutlineColor = Theme.BorderSoft;
            p.CornerRadius = Theme.Radius;
            return p;
        }

        public static GamePanel GradientCard(int x, int y, int w, int h, Color start, Color end)
        {
            GamePanel p = Card(x, y, w, h);
            p.StartColor = start;
            p.EndColor = end;
            p.Gradient = true;
            p.GradientAngle = 22f;
            p.OutlineColor = Color.FromArgb(42, 255, 255, 255);
            p.CornerRadius = Theme.RadiusLarge;
            return p;
        }

        public static DataGridView Grid(int x, int y, int w, int h)
        {
            DataGridView g = new DataGridView();
            g.SetBounds(x, y, w, h);
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.BackgroundColor = Theme.Surface;
            g.BorderStyle = BorderStyle.None;
            g.GridColor = Theme.BorderSoft;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Theme.Sidebar;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Theme.Text;
            g.ColumnHeadersDefaultCellStyle.Font = Theme.BodyBold;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Theme.Sidebar;
            g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Theme.Text;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 40;
            g.DefaultCellStyle.BackColor = Theme.CardBg;
            g.DefaultCellStyle.ForeColor = Theme.Text;
            g.DefaultCellStyle.SelectionBackColor = Theme.AccentDark;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Padding = new Padding(5, 3, 5, 3);
            g.AlternatingRowsDefaultCellStyle.BackColor = Theme.SurfaceAlt;
            g.AlternatingRowsDefaultCellStyle.ForeColor = Theme.Text;
            g.RowTemplate.Height = 34;
            return g;
        }

        public static TextBox TextInput(int x, int y, int w, int h)
        {
            TextBox t = new TextBox();
            t.SetBounds(x, y, w, h);
            t.BackColor = Theme.SurfaceAlt;
            t.ForeColor = Theme.Text;
            t.BorderStyle = BorderStyle.FixedSingle;
            t.Font = Theme.BodyFont;
            t.Margin = new Padding(0);
            return t;
        }

        public static TextBox MultiInput(int x, int y, int w, int h)
        {
            TextBox t = TextInput(x, y, w, h);
            t.Multiline = true;
            t.ScrollBars = ScrollBars.Vertical;
            return t;
        }

        public static Label Pill(string text, int x, int y, int w, Color back, Color fore)
        {
            Label l = new Label();
            l.Text = text;
            l.Font = Theme.SmallBold;
            l.ForeColor = fore;
            l.BackColor = back;
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.SetBounds(x, y, w, 26);
            l.UseCompatibleTextRendering = false;
            RoundControl(l, 9);
            l.Resize += delegate { RoundControl(l, 9); };
            return l;
        }

        public static Label SectionTitle(string text)
        {
            Label l = new Label();
            l.Text = text;
            l.Font = Theme.H1Font;
            l.ForeColor = Theme.Text;
            l.AutoSize = true;
            l.BackColor = Color.Transparent;
            l.Margin = new Padding(0, 0, 0, 12);
            return l;
        }

        public static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            GraphicsPath p = new GraphicsPath();
            if (r.Width <= 1 || r.Height <= 1)
            {
                p.AddRectangle(r);
                return p;
            }

            int d = Math.Max(2, Math.Min(radius * 2, Math.Min(r.Width, r.Height)));
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static void RoundControl(Control c, int radius)
        {
            if (c.Width <= 1 || c.Height <= 1) return;
            using (GraphicsPath p = RoundedPath(new Rectangle(0, 0, c.Width, c.Height), radius))
            {
                Region old = c.Region;
                c.Region = new Region(p);
                if (old != null) old.Dispose();
            }
        }

        public static void DrawImageCover(Graphics g, Image image, Rectangle destination, float zoom)
        {
            if (image == null || destination.Width <= 0 || destination.Height <= 0) return;
            if (zoom < 1f) zoom = 1f;

            float imageRatio = (float)image.Width / image.Height;
            float destRatio = (float)destination.Width / destination.Height;
            RectangleF source;

            if (imageRatio > destRatio)
            {
                float sourceWidth = image.Height * destRatio;
                float x = (image.Width - sourceWidth) / 2f;
                source = new RectangleF(x, 0, sourceWidth, image.Height);
            }
            else
            {
                float sourceHeight = image.Width / destRatio;
                float y = (image.Height - sourceHeight) / 2f;
                source = new RectangleF(0, y, image.Width, sourceHeight);
            }

            if (zoom > 1f)
            {
                float newW = source.Width / zoom;
                float newH = source.Height / zoom;
                source = new RectangleF(
                    source.X + (source.Width - newW) / 2f,
                    source.Y + (source.Height - newH) / 2f,
                    newW, newH);
            }

            InterpolationMode oldInterpolation = g.InterpolationMode;
            PixelOffsetMode oldPixel = g.PixelOffsetMode;
            CompositingQuality oldComp = g.CompositingQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.DrawImage(image, destination, source.X, source.Y, source.Width, source.Height, GraphicsUnit.Pixel);
            g.InterpolationMode = oldInterpolation;
            g.PixelOffsetMode = oldPixel;
            g.CompositingQuality = oldComp;
        }

        public static Color Lighten(Color c, double factor)
        {
            return Color.FromArgb(c.A,
                Clamp((int)(c.R * factor)),
                Clamp((int)(c.G * factor)),
                Clamp((int)(c.B * factor)));
        }

        public static Color Darken(Color c, double factor)
        {
            return Color.FromArgb(c.A,
                Clamp((int)(c.R * factor)),
                Clamp((int)(c.G * factor)),
                Clamp((int)(c.B * factor)));
        }

        private static int Clamp(int v)
        {
            return Math.Max(0, Math.Min(255, v));
        }
    }

    /// <summary>Central form styling and DPI setup.</summary>
    internal static class AppStyle
    {
        public static void Apply(Form f)
        {
            Apply(f, false);
        }

        public static void Apply(Form f, bool resizable)
        {
            f.AutoScaleMode = AutoScaleMode.Dpi;
            f.FormBorderStyle = resizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
            f.MaximizeBox = resizable;
            f.MinimizeBox = true;
            f.StartPosition = FormStartPosition.CenterScreen;
            f.BackColor = Theme.Background;
            f.Font = Theme.BodyFont;
            f.MinimumSize = new Size(760, 520);
            f.BackgroundImage = null;
            EnableDoubleBuffer(f);
            StyleChildren(f);

            f.Load += delegate { StyleChildren(f); };
            f.ControlAdded += delegate(object sender, ControlEventArgs e)
            {
                StyleTree(e.Control);
            };
        }

        public static void StyleTree(Control root)
        {
            if (root == null) return;
            StyleControl(root);
            EnableDoubleBuffer(root);
            foreach (Control child in root.Controls) StyleTree(child);
        }

        private static void StyleChildren(Control parent)
        {
            foreach (Control c in parent.Controls) StyleTree(c);
        }

        private static void StyleControl(Control c)
        {
            TextBox tb = c as TextBox;
            Button btn = c as Button;
            ComboBox combo = c as ComboBox;
            LinkLabel link = c as LinkLabel;
            CheckBox chk = c as CheckBox;
            DataGridView grid = c as DataGridView;
            NumericUpDown numeric = c as NumericUpDown;

            if (tb != null)
            {
                if (!(tb.Parent is GamePanel) || tb.BorderStyle != BorderStyle.None)
                    tb.BorderStyle = BorderStyle.FixedSingle;
                tb.BackColor = Theme.SurfaceAlt;
                tb.ForeColor = Theme.Text;
            }
            else if (btn != null)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = btn.FlatAppearance.BorderSize;
                btn.Cursor = Cursors.Hand;
                btn.ForeColor = Theme.Text;
                btn.UseVisualStyleBackColor = false;
                btn.UseCompatibleTextRendering = false;
            }
            else if (combo != null)
            {
                combo.BackColor = Theme.SurfaceAlt;
                combo.ForeColor = Theme.Text;
                combo.FlatStyle = FlatStyle.Flat;
            }
            else if (link != null)
            {
                link.BackColor = Color.Transparent;
                link.LinkColor = Theme.AccentBlue;
                link.ActiveLinkColor = Theme.Cream;
                link.VisitedLinkColor = Theme.AccentBlue;
            }
            else if (chk != null)
            {
                chk.BackColor = Color.Transparent;
                chk.ForeColor = Theme.Text;
            }
            else if (grid != null)
            {
                grid.BackgroundColor = Theme.Surface;
                grid.BorderStyle = BorderStyle.None;
                grid.GridColor = Theme.BorderSoft;
            }
            else if (numeric != null)
            {
                numeric.BackColor = Theme.SurfaceAlt;
                numeric.ForeColor = Theme.Text;
                numeric.BorderStyle = BorderStyle.FixedSingle;
            }

            Label label = c as Label;
            if (label != null) label.UseCompatibleTextRendering = false;
        }

        private static void EnableDoubleBuffer(Control control)
        {
            try
            {
                PropertyInfo property = typeof(Control).GetProperty(
                    "DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null) property.SetValue(control, true, null);
            }
            catch
            {
                // Rendering optimization must never block the app from opening.
            }
        }
    }
}
