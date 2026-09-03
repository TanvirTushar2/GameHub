using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameHub.Models;
using GameHub.Services;

namespace GameHub.Forms.Controls
{
    internal class FeaturedGameControl : UserControl
    {
        private Game _game;
        private Image _banner;

        private readonly Label _badge;
        private readonly Label _title;
        private readonly Label _meta;
        private readonly Label _description;

        private readonly Button _btnDetails;
        private readonly Button _btnWish;
        private readonly Button _btnCart;

        private string _priceText = "";

        private static readonly Font HeroTitleFont =
            new Font("Segoe UI", 24F, FontStyle.Bold);

        private static readonly Font HeroPriceFont =
            new Font("Segoe UI", 15F, FontStyle.Bold);

        private static readonly Font HeroButtonFont =
            new Font("Segoe UI", 8.5F, FontStyle.Bold);

        public event Action<Game> DetailsRequested;
        public event Action<Game> AddToCartRequested;
        public event Action<Game> WishlistToggleRequested;

        public FeaturedGameControl()
        {
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);

            Height = 292;
            MinimumSize = new Size(700, 292);

            BackColor = Color.Transparent;
            Margin = new Padding(0);

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

           _badge = Ui.Pill(
                "FEATURED",
                30,
                28,
                96,
                Theme.Cream,
                Theme.Background2);

            Controls.Add(_badge);

            _title = Ui.FixedLabel(
                "Discover your next game",
                30,
                68,
                600,
                52,
                HeroTitleFont,
                Color.White);

            _title.TextAlign = ContentAlignment.MiddleLeft;

            Controls.Add(_title);

           _meta = Ui.FixedLabel(
                "GAMEHUB STORE",
                32,
                121,
                560,
                25,
                Theme.SmallBold,
                Theme.Cyan);

            _meta.TextAlign = ContentAlignment.MiddleLeft;

            Controls.Add(_meta);

            _description = Ui.FixedLabel(
                "Browse the live catalog and open a game for complete details.",
                32,
                150,
                580,
                46,
                Theme.BodyFont,
                Color.FromArgb(225, 239, 245, 255));

            _description.TextAlign =
                ContentAlignment.TopLeft;

            Controls.Add(_description);

           _btnDetails = Ui.GhostBtn(
                "VIEW DETAILS",
                190,
                210,
                150,
                40);

            _btnDetails.Font = HeroButtonFont;

            _btnDetails.Click += delegate
            {
                if (_game != null &&
                    DetailsRequested != null)
                {
                    DetailsRequested(_game);
                }
            };

            Controls.Add(_btnDetails);

           _btnWish = Ui.GhostBtn(
                "♡",
                348,
                210,
                46,
                40);

            _btnWish.Font =
                new Font(
                    "Segoe UI Symbol",
                    12F,
                    FontStyle.Bold);

            _btnWish.Click += delegate
            {
                if (_game != null &&
                    WishlistToggleRequested != null)
                {
                    WishlistToggleRequested(_game);
                }
            };

            Controls.Add(_btnWish);

           
            _btnCart = Ui.Btn(
                "ADD TO CART",
                Theme.Accent,
                402,
                210,
                170,
                40);

            _btnCart.Font = HeroButtonFont;

            _btnCart.Click += delegate
            {
                if (_game != null &&
                    _game.StockQuantity > 0 &&
                    AddToCartRequested != null)
                {
                    AddToCartRequested(_game);
                }
            };

            Controls.Add(_btnCart);

            Resize += delegate
            {
                LayoutResponsiveControls();
                Invalidate();
            };

            LayoutResponsiveControls();
        }

        public Game Game
        {
            get
            {
                return _game;
            }
        }

        public void SetGame(
            Game game,
            bool inWishlist)
        {
            _game = game;

            if (game != null)
            {
                _banner =
                    GameAssetService.GetBanner(
                        game,
                        1600,
                        650);
            }
            else
            {
                _banner = null;
            }

            if (game == null)
            {
                _title.Text =
                    "Discover your next game";

                _meta.Text =
                    "GAMEHUB STORE";

                _description.Text =
                    "Browse the live catalog and open a game for complete details.";

                _priceText = "";

                _btnCart.Enabled = false;
                _btnCart.Text = "ADD TO CART";

                _btnWish.Text = "♡";
                _btnWish.ForeColor = Theme.Text;
            }
            else
            {
                _title.Text =
                    game.Title;

                _meta.Text =
                    (game.GenreName ?? "Game") +
                    "  •  " +
                    (game.PublisherName ?? "Publisher");

                _description.Text =
                    string.IsNullOrWhiteSpace(
                        game.Description)
                    ?
                    "Open this title to see details, reviews and purchase options."
                    :
                    game.Description;

                _priceText =
                    "Tk " +
                    game.Price.ToString("N0");

                bool available =
                    game.StockQuantity > 0;

                _btnCart.Enabled =
                    available;

                _btnCart.Text =
                    available
                        ? "ADD TO CART"
                        : "OUT OF STOCK";

                _btnCart.BackColor =
                    available
                        ? Theme.Accent
                        : Theme.SurfaceAlt;

                _btnWish.Text =
                    inWishlist
                        ? "♥"
                        : "♡";

                _btnWish.ForeColor =
                    inWishlist
                        ? Theme.Red
                        : Theme.Text;
            }

            LayoutResponsiveControls();
            Invalidate();
        }

        public void SetWishlistState(
            bool inWishlist)
        {
            _btnWish.Text =
                inWishlist
                    ? "♥"
                    : "♡";

            _btnWish.ForeColor =
                inWishlist
                    ? Theme.Red
                    : Theme.Text;
        }

        
        private void LayoutResponsiveControls()
        {
            int availableWidth =
                Math.Max(
                    500,
                    Width - 70);

            int textWidth =
                (int)(Width * 0.52);

            if (textWidth < 520)
                textWidth = 520;

            if (textWidth > availableWidth)
                textWidth = availableWidth;

            _title.SetBounds(
                30,
                68,
                textWidth,
                52);

            _meta.SetBounds(
                32,
                121,
                textWidth,
                25);

            _description.SetBounds(
                32,
                150,
                textWidth,
                48);

            int detailsWidth =
                TextRenderer.MeasureText(
                    _btnDetails.Text,
                    _btnDetails.Font).Width + 42;

            if (detailsWidth < 145)
                detailsWidth = 145;

            int cartWidth =
                TextRenderer.MeasureText(
                    _btnCart.Text,
                    _btnCart.Font).Width + 42;

            if (cartWidth < 160)
                cartWidth = 160;

            int x = 190;

            _btnDetails.SetBounds(
                x,
                210,
                detailsWidth,
                40);

            x += detailsWidth + 8;

            _btnWish.SetBounds(
                x,
                210,
                46,
                40);

            x += 54;

            _btnCart.SetBounds(
                x,
                210,
                cartWidth,
                40);
        }

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            g.PixelOffsetMode =
                PixelOffsetMode.HighQuality;

            g.InterpolationMode =
                InterpolationMode.HighQualityBicubic;

            Rectangle body =
                new Rectangle(
                    0,
                    0,
                    Math.Max(1, Width - 1),
                    Math.Max(1, Height - 1));

            using (
                GraphicsPath path =
                    Ui.RoundedPath(
                        body,
                        Theme.RadiusLarge))
            {
                GraphicsState state =
                    g.Save();

                g.SetClip(path);

                using (
                    SolidBrush fallback =
                        new SolidBrush(
                            Theme.Surface))
                {
                    g.FillRectangle(
                        fallback,
                        body);
                }

                Ui.DrawImageCover(
                    g,
                    _banner,
                    body,
                    1f);


                int overlayWidth =
                    Math.Max(
                        610,
                        (int)(Width * 0.65));

                overlayWidth =
                    Math.Min(
                        overlayWidth,
                        Width);

                Rectangle overlayRect =
                    new Rectangle(
                        0,
                        0,
                        overlayWidth,
                        Height);

                using (
                    LinearGradientBrush overlay =
                        new LinearGradientBrush(
                            overlayRect,
                            Color.FromArgb(
                                248,
                                Theme.Background),
                            Color.FromArgb(
                                25,
                                Theme.Background),
                            0F))
                {
                    g.FillRectangle(
                        overlay,
                        overlayRect);
                }

                Rectangle fadeRect =
                    new Rectangle(
                        0,
                        Height - 92,
                        Width,
                        92);

                using (
                    LinearGradientBrush bottom =
                        new LinearGradientBrush(
                            fadeRect,
                            Color.FromArgb(
                                0,
                                Theme.Background),
                            Color.FromArgb(
                                170,
                                Theme.Background),
                            90F))
                {
                    g.FillRectangle(
                        bottom,
                        fadeRect);
                }

                g.Restore(state);

  

                using (
                    Pen border =
                        new Pen(
                            Color.FromArgb(
                                80,
                                Theme.Accent),
                            1F))
                {
                    g.DrawPath(
                        border,
                        path);
                }
            }


            Rectangle priceRect =
                new Rectangle(
                    32,
                    207,
                    150,
                    45);

            TextRenderer.DrawText(
                g,
                _priceText,
                HeroPriceFont,
                priceRect,
                Theme.Cream,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding);
        }
    }
}