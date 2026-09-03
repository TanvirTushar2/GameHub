using System.Drawing;

namespace GameHub.Forms
{
    /// <summary>
    /// One design system for the whole GameHub desktop launcher.
    /// Keep colours/fonts here so Login, Customer, Admin and Staff stay consistent.
    /// </summary>
    internal static class Theme
    {
        public static readonly Color Background   = Color.FromArgb(8, 10, 20);
        public static readonly Color Background2  = Color.FromArgb(13, 16, 31);
        public static readonly Color Sidebar      = Color.FromArgb(16, 19, 38);
        public static readonly Color Surface      = Color.FromArgb(24, 28, 52);
        public static readonly Color SurfaceAlt   = Color.FromArgb(31, 36, 66);
        public static readonly Color CardBg       = Color.FromArgb(22, 26, 48);
        public static readonly Color CardHover    = Color.FromArgb(30, 35, 67);
        public static readonly Color Border       = Color.FromArgb(54, 62, 105);
        public static readonly Color BorderSoft   = Color.FromArgb(35, 42, 76);

        public static readonly Color Text         = Color.FromArgb(248, 249, 255);
        public static readonly Color Muted        = Color.FromArgb(158, 166, 195);
        public static readonly Color Muted2       = Color.FromArgb(116, 126, 158);

        public static readonly Color Accent       = Color.FromArgb(104, 92, 255);
        public static readonly Color AccentDark   = Color.FromArgb(74, 65, 220);
        public static readonly Color AccentBlue   = Color.FromArgb(58, 159, 255);
        public static readonly Color Cyan         = Color.FromArgb(64, 214, 216);
        public static readonly Color Cream        = Color.FromArgb(235, 237, 255);
        public static readonly Color Gold         = Color.FromArgb(247, 190, 89);
        public static readonly Color Green        = Color.FromArgb(72, 201, 154);
        public static readonly Color Red          = Color.FromArgb(238, 93, 111);
        public static readonly Color Purple       = Color.FromArgb(166, 106, 255);
        public static readonly Color Gray         = Color.FromArgb(113, 120, 148);
        public static readonly Color Orange       = Color.FromArgb(255, 144, 72);
        public static readonly Color WinBg        = Background;

        public static readonly Color OverlayStrong = Color.FromArgb(210, 8, 10, 20);
        public static readonly Color OverlaySoft   = Color.FromArgb(120, 8, 10, 20);

        public const int RadiusSmall = 10;
        public const int Radius = 16;
        public const int RadiusLarge = 24;

        public static readonly Font DisplayFont = new Font("Segoe UI", 27F, FontStyle.Bold);
        public static readonly Font TitleFont   = new Font("Segoe UI", 20F, FontStyle.Bold);
        public static readonly Font H1Font      = new Font("Segoe UI", 17F, FontStyle.Bold);
        public static readonly Font H2Font      = new Font("Segoe UI", 12F, FontStyle.Bold);
        public static readonly Font BodyFont    = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font BodyBold    = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        public static readonly Font SmallFont   = new Font("Segoe UI", 8.5F, FontStyle.Regular);
        public static readonly Font SmallBold   = new Font("Segoe UI", 8.5F, FontStyle.Bold);
    }
}
