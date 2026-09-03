using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Models;
using GameHub.Services;

namespace GameHub.Forms.Controls
{
    /// <summary>Internal Store details page: hero, screenshots, wishlist, reviews and purchase action.</summary>
    internal class GameDetailsView : UserControl
    {
        private readonly CustomerRepository _customer = new CustomerRepository();
        private readonly Panel _content;
        private readonly PictureBox _cover;
        private readonly Label _lblTitle;
        private readonly Label _lblMeta;
        private readonly Label _lblPrice;
        private readonly Label _lblStock;
        private readonly Label _lblDescription;
        private readonly Label _lblRating;
        private readonly Button _btnWish;
        private readonly Button _btnCart;
        private readonly FlowLayoutPanel _screens;
        private readonly DataGridView _reviews;
        private readonly GamePanel _reviewCard;
        private readonly NumericUpDown _rating;
        private readonly TextBox _comment;
        private readonly Button _saveReview;
        private readonly Label _screensTitle;
        private readonly Label _reviewsTitle;
        private Game _game;

        public event Action BackRequested;
        public event Action<Game> AddToCartRequested;
        public event Action DataChanged;

        public GameDetailsView()
        {
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            Dock = DockStyle.Fill;
            Margin = new Padding(0);
            BackColor = Theme.Background;

            Panel scroll = new Panel();
            scroll.Dock = DockStyle.Fill;
            scroll.AutoScroll = true;
            scroll.BackColor = Theme.Background;
            Controls.Add(scroll);

            _content = new Panel();
            _content.BackColor = Theme.Background;
            _content.SetBounds(0, 0, 1120, 1040);
            _content.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            scroll.Controls.Add(_content);

            Button back = Ui.GhostBtn("←  BACK TO STORE", 0, 0, 150, 38);
            back.Click += delegate { if (BackRequested != null) BackRequested(); };
            _content.Controls.Add(back);

            _cover = new PictureBox();
            _cover.SetBounds(0, 62, 286, 384);
            _cover.SizeMode = PictureBoxSizeMode.Zoom;
            _cover.BackColor = Theme.Surface;
            _content.Controls.Add(_cover);
            Ui.RoundControl(_cover, Theme.Radius);
            _cover.Resize += delegate { Ui.RoundControl(_cover, Theme.Radius); };

            _lblTitle = Ui.FixedLabel("Game", 322, 70, 700, 58, Theme.DisplayFont, Theme.Text);
            _content.Controls.Add(_lblTitle);

            _lblMeta = Ui.FixedLabel("Genre • Publisher", 325, 132, 700, 24, Theme.BodyBold, Theme.Cyan);
            _content.Controls.Add(_lblMeta);

            _lblRating = Ui.FixedLabel("★ New / unrated", 325, 166, 270, 26, Theme.BodyBold, Theme.Gold);
            _content.Controls.Add(_lblRating);

            _lblPrice = Ui.FixedLabel("Tk 0", 325, 210, 240, 36,
                new Font("Segoe UI", 18F, FontStyle.Bold), Theme.Cream);
            _content.Controls.Add(_lblPrice);

            _lblStock = Ui.FixedLabel("", 565, 217, 240, 25, Theme.BodyBold, Theme.Green);
            _content.Controls.Add(_lblStock);

            _lblDescription = Ui.FixedLabel("", 325, 264, 690, 98, Theme.BodyFont, Theme.Muted);
            _content.Controls.Add(_lblDescription);

            _btnWish = Ui.GhostBtn("♡  WISHLIST", 325, 382, 142, 42);
            _btnWish.Click += ToggleWishlist;
            _content.Controls.Add(_btnWish);

            _btnCart = Ui.Btn("ADD TO CART", Theme.Accent, 480, 382, 160, 42);
            _btnCart.Click += delegate
            {
                if (_game != null && _game.StockQuantity > 0 && AddToCartRequested != null)
                    AddToCartRequested(_game);
            };
            _content.Controls.Add(_btnCart);

            _screensTitle = Ui.FixedLabel("SCREENSHOTS", 0, 478, 300, 28, Theme.H2Font, Theme.Text);
            _content.Controls.Add(_screensTitle);

            _screens = new FlowLayoutPanel();
            _screens.SetBounds(0, 510, 1040, 150);
            _screens.WrapContents = false;
            _screens.AutoScroll = true;
            _screens.BackColor = Theme.Background;
            _screens.Padding = new Padding(0);
            _content.Controls.Add(_screens);

            _reviewsTitle = Ui.FixedLabel("PLAYER REVIEWS", 0, 692, 300, 28, Theme.H2Font, Theme.Text);
            _content.Controls.Add(_reviewsTitle);

            _reviews = Ui.Grid(0, 725, 660, 212);
            _content.Controls.Add(_reviews);

            _reviewCard = Ui.Card(684, 725, 356, 212);
            _content.Controls.Add(_reviewCard);
            _reviewCard.Controls.Add(Ui.Lbl("WRITE / UPDATE YOUR REVIEW", 18, 16, Theme.SmallBold, Theme.AccentBlue));
            _reviewCard.Controls.Add(Ui.Lbl("Rating", 18, 48, Theme.SmallFont, Theme.Muted));

            _rating = new NumericUpDown();
            _rating.SetBounds(18, 70, 88, 30);
            _rating.Minimum = 1;
            _rating.Maximum = 5;
            _rating.Value = 5;
            _reviewCard.Controls.Add(_rating);

            _comment = Ui.MultiInput(18, 108, 320, 55);
            _comment.MaxLength = 500;
            _reviewCard.Controls.Add(_comment);

            _saveReview = Ui.Btn("SAVE REVIEW", Theme.AccentDark, 190, 171, 148, 30);
            _saveReview.Click += SaveReview;
            _reviewCard.Controls.Add(_saveReview);

            Action relayout = delegate
            {
                int viewportWidth = Math.Max(720, scroll.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2);
                _content.Width = viewportWidth;
                LayoutContent();
            };
            scroll.Resize += delegate { relayout(); };
            relayout();
        }

        public Game CurrentGame
        {
            get { return _game; }
        }

        public void LoadGame(Game game)
        {
            _game = game;
            if (game == null) return;

            _cover.Image = GameAssetService.GetCover(game, 700, 1000);
            _lblTitle.Text = game.Title;
            _lblMeta.Text = (game.GenreName ?? "Game") + "  •  " + (game.PublisherName ?? "Unknown publisher") +
                            (game.ReleaseDate.HasValue ? "  •  " + game.ReleaseDate.Value.Year : string.Empty);
            _lblPrice.Text = "Tk " + game.Price.ToString("N0");
            _lblStock.Text = game.StockQuantity > 0 ? game.StockQuantity + " copies available" : "Out of stock";
            _lblStock.ForeColor = game.StockQuantity > 0 ? Theme.Green : Theme.Red;
            _lblDescription.Text = string.IsNullOrWhiteSpace(game.Description)
                ? "No description has been added for this game yet."
                : game.Description;
            _btnCart.Enabled = game.StockQuantity > 0;
            _btnCart.Text = game.StockQuantity > 0 ? "ADD TO CART" : "OUT OF STOCK";
            _btnCart.BackColor = game.StockQuantity > 0 ? Theme.Accent : Theme.SurfaceAlt;

            LoadScreenshots();
            RefreshWishlist();
            RefreshReviews();
        }

        private void LayoutContent()
        {
            int w = Math.Max(720, _content.ClientSize.Width);

            if (w >= 850)
            {
                // Desktop/two-column game summary.
                _cover.SetBounds(0, 62, 286, 384);
                int rightX = 322;
                int rightW = Math.Max(360, w - rightX);
                _lblTitle.SetBounds(rightX, 68, rightW, 58);
                _lblMeta.SetBounds(rightX + 3, 132, rightW - 3, 24);
                _lblRating.SetBounds(rightX + 3, 166, Math.Min(320, rightW), 26);
                _lblPrice.SetBounds(rightX + 3, 210, 230, 36);
                _lblStock.SetBounds(rightX + 242, 217, Math.Max(140, rightW - 242), 25);
                _lblDescription.SetBounds(rightX + 3, 264, Math.Max(300, rightW - 18), 98);
                _btnWish.SetBounds(rightX + 3, 382, 142, 42);
                _btnCart.SetBounds(rightX + 158, 382, 160, 42);

                _screensTitle.Top = 478;
                _screens.SetBounds(0, 510, w, 150);
                _reviewsTitle.Top = 692;

                if (w >= 1040)
                {
                    int reviewGap = 24;
                    int cardW = 356;
                    _reviewCard.SetBounds(w - cardW, 725, cardW, 212);
                    _reviews.SetBounds(0, 725, Math.Max(460, _reviewCard.Left - reviewGap), 212);
                    _content.Height = 980;
                }
                else
                {
                    // Narrow desktop: stack review editor below the grid instead of overlapping it.
                    _reviews.SetBounds(0, 725, w, 212);
                    _reviewCard.SetBounds(0, 955, w, 222);
                    _content.Height = 1210;
                }
            }
            else
            {
                // Compact window: stack cover and details vertically.
                int coverW = Math.Min(286, w);
                _cover.SetBounds(0, 62, coverW, 360);
                int y = 448;
                _lblTitle.SetBounds(0, y, w, 58); y += 62;
                _lblMeta.SetBounds(0, y, w, 24); y += 32;
                _lblRating.SetBounds(0, y, Math.Min(320, w), 26); y += 36;
                _lblPrice.SetBounds(0, y, 230, 36);
                _lblStock.SetBounds(Math.Min(240, Math.Max(0, w - 240)), y + 7, 230, 25); y += 48;
                _lblDescription.SetBounds(0, y, w, 100); y += 112;
                _btnWish.SetBounds(0, y, 142, 42);
                _btnCart.SetBounds(155, y, 160, 42); y += 76;

                _screensTitle.Top = y;
                _screens.SetBounds(0, y + 32, w, 150); y += 206;
                _reviewsTitle.Top = y;
                _reviews.SetBounds(0, y + 34, w, 212); y += 264;
                _reviewCard.SetBounds(0, y, w, 222);
                _content.Height = y + 250;
            }

            ResizeReviewCard();
        }

        private void ResizeReviewCard()
        {
            int inner = Math.Max(180, _reviewCard.ClientSize.Width - 36);
            _comment.Width = inner;
            _saveReview.Left = Math.Max(18, _reviewCard.ClientSize.Width - _saveReview.Width - 18);
        }

        private void LoadScreenshots()
        {
            _screens.SuspendLayout();
            _screens.Controls.Clear();
            for (int i = 1; i <= 3; i++)
            {
                PictureBox shot = new PictureBox();
                shot.Size = new Size(300, 132);
                shot.Margin = new Padding(0, 0, 14, 0);
                shot.SizeMode = PictureBoxSizeMode.Zoom;
                shot.BackColor = Theme.Surface;
                shot.Image = GameAssetService.GetScreenshot(_game, i, 1280, 720);
                Ui.RoundControl(shot, 12);
                shot.Resize += delegate(object sender, EventArgs e)
                {
                    PictureBox p = sender as PictureBox;
                    if (p != null) Ui.RoundControl(p, 12);
                };
                _screens.Controls.Add(shot);
            }
            _screens.ResumeLayout();
        }

        private void RefreshWishlist()
        {
            if (_game == null || Program.CurrentUser == null) return;
            try
            {
                bool inWish = _customer.IsInWishlist(Program.CurrentUser.UserID, _game.GameID);
                _btnWish.Text = inWish ? "♥  WISHLIST" : "♡  WISHLIST";
                _btnWish.ForeColor = inWish ? Theme.Red : Theme.Text;
            }
            catch
            {
                _btnWish.Text = "♡  WISHLIST";
            }
        }

        private void ToggleWishlist(object sender, EventArgs e)
        {
            if (_game == null || Program.CurrentUser == null) return;
            try
            {
                int userId = Program.CurrentUser.UserID;
                if (_customer.IsInWishlist(userId, _game.GameID))
                    _customer.RemoveFromWishlist(userId, _game.GameID);
                else
                    _customer.AddToWishlist(userId, _game.GameID);

                RefreshWishlist();
                if (DataChanged != null) DataChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Wishlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshReviews()
        {
            if (_game == null) return;
            try
            {
                DataTable dt = _customer.GetReviewsForGame(_game.GameID);
                _reviews.DataSource = dt;
                if (_reviews.Columns.Contains("Date")) _reviews.Columns["Date"].DefaultCellStyle.Format = "dd MMM yyyy";

                if (dt.Rows.Count == 0)
                {
                    _lblRating.Text = "★ New / unrated";
                }
                else
                {
                    decimal sum = 0;
                    foreach (DataRow row in dt.Rows) sum += Convert.ToDecimal(row["Rating"]);
                    decimal average = sum / dt.Rows.Count;
                    _lblRating.Text = "★ " + average.ToString("0.0") + " / 5  •  " + dt.Rows.Count + " review" + (dt.Rows.Count == 1 ? "" : "s");
                }

                if (Program.CurrentUser != null)
                {
                    DataTable mine = _customer.GetUserReview(Program.CurrentUser.UserID, _game.GameID);
                    if (mine.Rows.Count > 0)
                    {
                        _rating.Value = Math.Max(_rating.Minimum, Math.Min(_rating.Maximum, Convert.ToDecimal(mine.Rows[0]["Rating"])));
                        _comment.Text = mine.Rows[0]["Comment"] == DBNull.Value ? string.Empty : Convert.ToString(mine.Rows[0]["Comment"]);
                    }
                    else
                    {
                        _rating.Value = 5;
                        _comment.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load reviews.\n" + ex.Message, "Reviews",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveReview(object sender, EventArgs e)
        {
            if (_game == null || Program.CurrentUser == null) return;
            try
            {
                _customer.SaveReview(Program.CurrentUser.UserID, _game.GameID,
                    Convert.ToInt32(_rating.Value), _comment.Text.Trim());
                MessageBox.Show("Your review has been saved.", "GameHub",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshReviews();
                if (DataChanged != null) DataChanged();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message + "\n\nPurchase the game first, then review it from your Library or this page.",
                    "Review", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save the review.\n" + ex.Message,
                    "Review", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
