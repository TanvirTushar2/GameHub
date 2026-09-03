using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameHub.Models;
using GameHub.Services;

namespace GameHub.Forms.Controls
{
   
    internal class GameCardControl : UserControl
    {
        private static readonly Font CardTitleFont =
            new Font(
                "Segoe UI",
                11F,
                FontStyle.Bold);

        private static readonly Font CardPriceFont =
            new Font(
                "Segoe UI",
                10.5F,
                FontStyle.Bold);

        private static readonly Font CardButtonFont =
            new Font(
                "Segoe UI",
                8F,
                FontStyle.Bold);

        private readonly Timer _hoverTimer;

        private readonly Button _btnWish;
        private readonly Button _btnDetails;
        private readonly Button _btnCart;

        private Game _game;
        private Image _cover;

        private bool _inWishlist;

        private float _hover;
        private float _hoverTarget;

        public event Action<Game> DetailsRequested;
        public event Action<Game> AddToCartRequested;
        public event Action<Game, GameCardControl>
            WishlistToggleRequested;

        public GameCardControl()
        {
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions =
                new SizeF(96F, 96F);

            // Keep roughly the same width so six games can still fit.
            Size =
                new Size(
                    228,
                    350);

            Margin =
                new Padding(
                    0,
                    0,
                    18,
                    20);

            BackColor =
                Color.Transparent;

            Cursor =
                Cursors.Hand;

            TabStop =
                true;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);

        
            _btnWish =
                Ui.Btn(
                    "♡",
                    Theme.SurfaceAlt,
                    14,
                    306,
                    38,
                    30);

            _btnWish.Font =
                new Font(
                    "Segoe UI Symbol",
                    11F,
                    FontStyle.Bold);

            _btnWish.Click += delegate
            {
                if (_game != null &&
                    WishlistToggleRequested != null)
                {
                    WishlistToggleRequested(
                        _game,
                        this);
                }
            };

            Controls.Add(
                _btnWish);

          
            _btnDetails =
                Ui.GhostBtn(
                    "DETAILS",
                    58,
                    306,
                    78,
                    30);

            _btnDetails.Font =
                CardButtonFont;

            _btnDetails.Click += delegate
            {
                RaiseDetails();
            };

            Controls.Add(
                _btnDetails);

          
            _btnCart =
                Ui.Btn(
                    "ADD",
                    Theme.Accent,
                    142,
                    306,
                    72,
                    30);

            _btnCart.Font =
                CardButtonFont;

            _btnCart.Click += delegate
            {
                if (_game != null &&
                    _game.StockQuantity > 0 &&
                    AddToCartRequested != null)
                {
                    AddToCartRequested(
                        _game);
                }
            };

            Controls.Add(
                _btnCart);

            AttachHover(
                _btnWish);

            AttachHover(
                _btnDetails);

            AttachHover(
                _btnCart);

            _hoverTimer =
                new Timer();

            _hoverTimer.Interval =
                15;

            _hoverTimer.Tick +=
                HoverTimer_Tick;

            MouseEnter += delegate
            {
                SetHover(true);
            };

            MouseLeave +=
                GameCardControl_MouseLeave;

            MouseClick +=
                GameCardControl_MouseClick;

            KeyDown += delegate (
                object sender,
                KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter ||
                    e.KeyCode == Keys.Space)
                {
                    RaiseDetails();

                    e.Handled =
                        true;
                }
            };

            Resize += delegate
            {
                LayoutButtons();
            };

            LayoutButtons();
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
            _game =
                game;

            _inWishlist =
                inWishlist;

            _cover =
                GameAssetService.GetCover(
                    game,
                    640,
                    900);

            UpdateButtons();

            AccessibleName =
                game == null
                    ? "Game"
                    : game.Title;

            Invalidate();
        }

        public void SetWishlistState(
            bool inWishlist)
        {
            _inWishlist =
                inWishlist;

            UpdateButtons();

            Invalidate();
        }

        private void UpdateButtons()
        {
            _btnWish.Text =
                _inWishlist
                    ? "♥"
                    : "♡";

            _btnWish.ForeColor =
                _inWishlist
                    ? Theme.Red
                    : Theme.Text;

            bool available =
                _game != null &&
                _game.StockQuantity > 0;

            _btnCart.Enabled =
                available;

            _btnCart.Text =
                available
                    ? "ADD"
                    : "SOLD";

            _btnCart.BackColor =
                available
                    ? Theme.Accent
                    : Theme.SurfaceAlt;

            LayoutButtons();
        }

        private void LayoutButtons()
        {
            int y =
                Height - 44;

            int left =
                14;

            int right =
                Width - 14;

            int wishWidth =
                38;

            int gap =
                6;

            int addWidth =
                72;

            int detailsWidth =
                right -
                left -
                wishWidth -
                addWidth -
                (gap * 2);

            if (detailsWidth < 70)
                detailsWidth = 70;

            _btnWish.SetBounds(
                left,
                y,
                wishWidth,
                30);

            _btnDetails.SetBounds(
                left +
                wishWidth +
                gap,
                y,
                detailsWidth,
                30);

            _btnCart.SetBounds(
                right -
                addWidth,
                y,
                addWidth,
                30);
        }

        private void AttachHover(
            Control control)
        {
            control.MouseEnter += delegate
            {
                SetHover(true);
            };

            control.MouseLeave +=
                GameCardControl_MouseLeave;
        }

        private void GameCardControl_MouseClick(
            object sender,
            MouseEventArgs e)
        {
            if (e.Button ==
                    MouseButtons.Left &&
                e.Y <
                    Height - 55)
            {
                RaiseDetails();
            }
        }

        private void GameCardControl_MouseLeave(
            object sender,
            EventArgs e)
        {
            Point point =
                PointToClient(
                    Cursor.Position);

            if (!ClientRectangle.Contains(
                    point))
            {
                SetHover(false);
            }
        }

        private void RaiseDetails()
        {
            if (_game != null &&
                DetailsRequested != null)
            {
                DetailsRequested(
                    _game);
            }
        }

        private void SetHover(
            bool value)
        {
            _hoverTarget =
                value
                    ? 1F
                    : 0F;

            if (!_hoverTimer.Enabled)
            {
                _hoverTimer.Start();
            }
        }

        private void HoverTimer_Tick(
            object sender,
            EventArgs e)
        {
            float speed =
                0.11F;

            if (_hover <
                _hoverTarget)
            {
                _hover =
                    Math.Min(
                        _hoverTarget,
                        _hover + speed);
            }
            else if (_hover >
                     _hoverTarget)
            {
                _hover =
                    Math.Max(
                        _hoverTarget,
                        _hover - speed);
            }
            else
            {
                _hoverTimer.Stop();
            }

            Invalidate();
        }

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g =
                e.Graphics;

            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            g.PixelOffsetMode =
                PixelOffsetMode.HighQuality;

            g.InterpolationMode =
                InterpolationMode.HighQualityBicubic;

            g.TextRenderingHint =
                System.Drawing.Text
                    .TextRenderingHint
                    .ClearTypeGridFit;

            Rectangle body =
                new Rectangle(
                    4,
                    3 -
                    (int)(_hover * 2F),
                    Width - 9,
                    Height - 11);

            Rectangle shadow =
                new Rectangle(
                    body.X + 3,
                    body.Y + 5,
                    body.Width,
                    body.Height);

            using (
                GraphicsPath shadowPath =
                    Ui.RoundedPath(
                        shadow,
                        Theme.Radius))
            using (
                SolidBrush shadowBrush =
                    new SolidBrush(
                        Color.FromArgb(
                            60 +
                            (int)(_hover * 25F),
                            0,
                            0,
                            0)))
            {
                g.FillPath(
                    shadowBrush,
                    shadowPath);
            }

            Color top =
                Blend(
                    Theme.CardBg,
                    Theme.CardHover,
                    _hover);

            Color bottom =
                Blend(
                    Theme.Surface,
                    Theme.SurfaceAlt,
                    _hover);

            using (
                GraphicsPath bodyPath =
                    Ui.RoundedPath(
                        body,
                        Theme.Radius))
            using (
                LinearGradientBrush background =
                    new LinearGradientBrush(
                        body,
                        top,
                        bottom,
                        90F))
            {
                g.FillPath(
                    background,
                    bodyPath);
            }
 Rectangle coverRect =
                new Rectangle(
                    body.X + 7,
                    body.Y + 7,
                    body.Width - 14,
                    184);

            using (
                GraphicsPath coverPath =
                    Ui.RoundedPath(
                        coverRect,
                        13))
            {
                GraphicsState state =
                    g.Save();

                g.SetClip(
                    coverPath);

                Ui.DrawImageCover(
                    g,
                    _cover,
                    coverRect,
                    1F +
                    _hover * 0.04F);

                Rectangle fade =
                    new Rectangle(
                        coverRect.X,
                        coverRect.Bottom - 72,
                        coverRect.Width,
                        72);

                using (
                    LinearGradientBrush overlay =
                        new LinearGradientBrush(
                            fade,
                            Color.FromArgb(
                                0,
                                8,
                                10,
                                20),
                            Color.FromArgb(
                                225,
                                8,
                                10,
                                20),
                            90F))
                {
                    g.FillRectangle(
                        overlay,
                        fade);
                }

                g.Restore(
                    state);
            }

            if (_game == null)
                return;

           
            string genre =
                string.IsNullOrWhiteSpace(
                    _game.GenreName)
                ?
                "GAME"
                :
                _game.GenreName
                    .ToUpperInvariant();

            int genreWidth =
                TextRenderer.MeasureText(
                    genre,
                    Theme.SmallBold).Width + 20;

            if (genreWidth < 62)
                genreWidth = 62;

            if (genreWidth >
                body.Width - 28)
            {
                genreWidth =
                    body.Width - 28;
            }

            Rectangle genreRect =
                new Rectangle(
                    body.X + 14,
                    body.Y + 15,
                    genreWidth,
                    23);

            using (
                GraphicsPath genrePath =
                    Ui.RoundedPath(
                        genreRect,
                        8))
            using (
                SolidBrush pill =
                    new SolidBrush(
                        Color.FromArgb(
                            215,
                            Theme.Background2)))
            {
                g.FillPath(
                    pill,
                    genrePath);
            }

            TextRenderer.DrawText(
                g,
                genre,
                Theme.SmallBold,
                genreRect,
                Theme.Cream,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis);

        
            Rectangle titleRect =
                new Rectangle(
                    body.X + 14,
                    body.Y + 202,
                    body.Width - 28,
                    39);

            TextRenderer.DrawText(
                g,
                _game.Title,
                CardTitleFont,
                titleRect,
                Theme.Text,
                TextFormatFlags.WordBreak |
                TextFormatFlags.EndEllipsis);

         
            Rectangle publisherRect =
                new Rectangle(
                    body.X + 14,
                    body.Y + 244,
                    body.Width - 28,
                    20);

            TextRenderer.DrawText(
                g,
                _game.PublisherName ??
                string.Empty,
                Theme.SmallFont,
                publisherRect,
                Theme.Muted,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis);

         
            Rectangle priceRect =
                new Rectangle(
                    body.X + 14,
                    body.Y + 267,
                    100,
                    25);

            TextRenderer.DrawText(
                g,
                "Tk " +
                _game.Price.ToString("N0"),
                CardPriceFont,
                priceRect,
                Theme.Cream,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding);

            string stock =
                _game.StockQuantity > 0
                ?
                _game.StockQuantity +
                " in stock"
                :
                "Out of stock";

            Rectangle stockRect =
                new Rectangle(
                    body.Right - 105,
                    body.Y + 269,
                    92,
                    22);

            TextRenderer.DrawText(
                g,
                stock,
                Theme.SmallFont,
                stockRect,
                _game.StockQuantity > 0
                    ? Theme.Green
                    : Theme.Red,
                TextFormatFlags.Right |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis);

         
            Color border =
                _hover > 0.01F
                ?
                Color.FromArgb(
                    100 +
                    (int)(_hover * 90F),
                    Theme.Accent)
                :
                Theme.BorderSoft;

            using (
                GraphicsPath outline =
                    Ui.RoundedPath(
                        new Rectangle(
                            body.X,
                            body.Y,
                            body.Width - 1,
                            body.Height - 1),
                        Theme.Radius))
            using (
                Pen pen =
                    new Pen(
                        border,
                        1F + _hover))
            {
                g.DrawPath(
                    pen,
                    outline);
            }
        }

        private static Color Blend(
            Color first,
            Color second,
            float amount)
        {
            amount =
                Math.Max(
                    0F,
                    Math.Min(
                        1F,
                        amount));

            return Color.FromArgb(
                (int)(
                    first.A +
                    (second.A -
                     first.A) *
                    amount),

                (int)(
                    first.R +
                    (second.R -
                     first.R) *
                    amount),

                (int)(
                    first.G +
                    (second.G -
                     first.G) *
                    amount),

                (int)(
                    first.B +
                    (second.B -
                     first.B) *
                    amount));
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing &&
                _hoverTimer != null)
            {
                _hoverTimer.Dispose();
            }

            base.Dispose(
                disposing);
        }
    }
}