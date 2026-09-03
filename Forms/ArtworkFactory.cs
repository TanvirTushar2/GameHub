using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using GameHub.Models;

namespace GameHub.Forms
{
    /// <summary>
    /// Generates clean local fallback artwork when real assets have not been added yet.
    /// Covers contain a restrained title; wide banners/login backgrounds avoid giant text
    /// so they never fight with the real WinForms labels drawn above them.
    /// </summary>
    internal static class ArtworkFactory
    {
        public static Image CreateGameCover(Game game, int width, int height)
        {
            width = Math.Max(1, width);
            height = Math.Max(1, height);
            Bitmap bmp = CreateBaseArtwork(game == null ? 0 : game.GameID, width, height, false);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                string title = game == null ? "GAMEHUB" : game.Title;
                string genre = game == null ? "GAME STORE" : (game.GenreName ?? "GAME");

                RectangleF titleBox = new RectangleF(28, height * 0.58f, width - 56, height * 0.25f);
                using (Font titleFont = new Font("Segoe UI", Math.Max(18f, width / 22f), FontStyle.Bold))
                using (SolidBrush white = new SolidBrush(Color.White))
                using (StringFormat fmt = new StringFormat())
                {
                    fmt.Trimming = StringTrimming.EllipsisWord;
                    fmt.FormatFlags = StringFormatFlags.LineLimit;
                    g.DrawString(title, titleFont, white, titleBox, fmt);
                }

                using (Font small = new Font("Segoe UI", Math.Max(9f, width / 42f), FontStyle.Bold))
                using (SolidBrush soft = new SolidBrush(Color.FromArgb(225, 238, 248)))
                    g.DrawString(genre.ToUpperInvariant(), small, soft, 28, 26);
            }
            return bmp;
        }

        public static Image CreateGameBanner(Game game, int width, int height)
        {
            width = Math.Max(1, width);
            height = Math.Max(1, height);
            Bitmap bmp = CreateBaseArtwork(game == null ? 0 : game.GameID, width, height, true);

            // Only a small watermark on the far right. The FeaturedGameControl provides
            // the readable title/description as real sharp UI labels on the left.
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                string title = game == null ? "GAMEHUB" : game.Title;
                RectangleF box = new RectangleF(width * 0.64f, height * 0.69f, width * 0.31f, height * 0.16f);
                using (Font font = new Font("Segoe UI", Math.Max(16f, height / 18f), FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(80, 255, 255, 255)))
                using (StringFormat fmt = new StringFormat())
                {
                    fmt.Alignment = StringAlignment.Far;
                    fmt.Trimming = StringTrimming.EllipsisWord;
                    g.DrawString(title, font, brush, box, fmt);
                }
            }
            return bmp;
        }

        public static Image CreateLoginBackdrop(int width, int height)
        {
            width = Math.Max(1, width);
            height = Math.Max(1, height);
            Bitmap bmp = CreateBaseArtwork(11, width, height, true);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush halo = new SolidBrush(Color.FromArgb(34, 116, 108, 255)))
                    g.FillEllipse(halo, (int)(width * 0.48), (int)(height * 0.08),
                        (int)(width * 0.52), (int)(height * 0.72));
                using (Pen line = new Pen(Color.FromArgb(50, 255, 255, 255), Math.Max(1f, width / 900f)))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int y = (int)(height * (0.20 + i * 0.13));
                        g.DrawLine(line, (int)(width * 0.46), y, width, (int)(height * (0.05 + i * 0.12)));
                    }
                }
            }
            return bmp;
        }

        private static Bitmap CreateBaseArtwork(int id, int width, int height, bool wide)
        {
            Bitmap bmp = new Bitmap(width, height);
            Color a = BaseColor(id);
            Color b = Color.FromArgb(
                Math.Max(0, a.R - 62),
                Math.Max(0, a.G - 52),
                Math.Max(0, a.B - 34));

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (LinearGradientBrush bg = new LinearGradientBrush(
                    new Rectangle(0, 0, width, height), a, b, wide ? 12f : 30f))
                    g.FillRectangle(bg, 0, 0, width, height);

                using (SolidBrush glow = new SolidBrush(Color.FromArgb(38, 255, 255, 255)))
                    g.FillEllipse(glow, (int)(width * 0.58), (int)(-height * 0.20),
                        (int)(width * 0.62), (int)(height * 0.90));

                Point[] slash =
                {
                    new Point((int)(width * 0.55), 0),
                    new Point(width, 0),
                    new Point(width, (int)(height * 0.46)),
                    new Point((int)(width * 0.30), height)
                };
                using (SolidBrush shape = new SolidBrush(Color.FromArgb(34, 4, 10, 20)))
                    g.FillPolygon(shape, slash);

                using (Pen line = new Pen(Color.FromArgb(52, 255, 255, 255), Math.Max(1f, width / 1000f)))
                {
                    g.DrawLine(line, 0, (int)(height * 0.72), width, (int)(height * 0.32));
                    g.DrawLine(line, (int)(width * 0.15), height, (int)(width * 0.85), 0);
                }
            }
            return bmp;
        }

        private static Color BaseColor(int id)
        {
            Color[] palette =
            {
                Color.FromArgb(36, 128, 190),
                Color.FromArgb(96, 67, 157),
                Color.FromArgb(44, 129, 101),
                Color.FromArgb(73, 88, 167),
                Color.FromArgb(139, 67, 111),
                Color.FromArgb(172, 102, 53),
                Color.FromArgb(46, 145, 154)
            };
            return palette[Math.Abs(id) % palette.Length];
        }
    }
}
