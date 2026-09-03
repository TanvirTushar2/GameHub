using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Models;

namespace GameHub.Forms
{
    /// <summary>Customer store: browse, search, filter, wishlist and buy games.</summary>
    public partial class StoreForm : Form
    {
        private readonly GameRepository _games = new GameRepository();
        private readonly CustomerRepository _customer = new CustomerRepository();
        private readonly List<CartItem> _cart = new List<CartItem>();

        private TextBox _txtSearch;
        private ComboBox _cboGenre;
        private Button _btnCart;
        private FlowLayoutPanel _flow;

        public StoreForm()
        {
            InitializeComponent();
            BuildUi();
            AppStyle.Apply(this);
            LoadGenres();
            LoadGames();
        }

        private void BuildUi()
        {
            Controls.Clear();
            ClientSize = new Size(1120, 700);
            Text = "GameHub - Store";

            Controls.Add(Ui.Lbl("GameHub Store", 28, 22,
                new Font("Segoe UI", 18F, FontStyle.Bold), Theme.Text));
            Controls.Add(Ui.Lbl("Browse games, build your wishlist and complete purchases.",
                30, 55, Theme.SmallFont, Theme.Muted));
            Controls.Add(Ui.Back(this));

            Panel hero = Ui.GradientCard(28, 92, 1064, 144, Theme.AccentDark, Theme.Background2);
            Controls.Add(hero);
            hero.Controls.Add(Ui.Pill("STORE", 22, 18, 64, Theme.Cream, Theme.Background2));
            hero.Controls.Add(Ui.Lbl("Find your next favorite game.", 22, 52,
                new Font("Segoe UI", 21F, FontStyle.Bold), Color.White));
            hero.Controls.Add(Ui.Lbl("Search the catalog, filter by genre and add titles to your cart or wishlist.",
                24, 94, Theme.BodyFont, Color.FromArgb(222, 237, 248)));

            Panel searchPanel = Ui.Card(28, 254, 1064, 64);
            Controls.Add(searchPanel);
            searchPanel.Controls.Add(Ui.Lbl("SEARCH", 18, 22,
                new Font("Segoe UI", 8F, FontStyle.Bold), Theme.Muted));

            _txtSearch = Ui.TextInput(84, 17, 396, 30);
            searchPanel.Controls.Add(_txtSearch);
            _txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadGames(); };

            Button btnSearch = Ui.Btn("Search", Theme.Accent, 492, 15, 88, 34);
            btnSearch.Click += (s, e) => LoadGames();
            searchPanel.Controls.Add(btnSearch);

            _cboGenre = new ComboBox();
            _cboGenre.SetBounds(598, 18, 200, 29);
            _cboGenre.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboGenre.SelectedIndexChanged += (s, e) => LoadGames();
            searchPanel.Controls.Add(_cboGenre);

            Button wishlist = Ui.Btn("Wishlist", Theme.SurfaceAlt, 814, 15, 104, 34);
            wishlist.Click += (s, e) =>
            {
                using (WishlistForm wf = new WishlistForm()) wf.ShowDialog(this);
                LoadGames();
            };
            searchPanel.Controls.Add(wishlist);

            _btnCart = Ui.Btn("Cart (0)", Theme.Green, 930, 15, 116, 34);
            _btnCart.Click += (s, e) => OpenCart();
            searchPanel.Controls.Add(_btnCart);

            _flow = new FlowLayoutPanel();
            _flow.SetBounds(28, 338, 1064, 334);
            _flow.AutoScroll = true;
            _flow.Padding = new Padding(8);
            _flow.BackColor = Theme.Background2;
            _flow.WrapContents = true;
            Controls.Add(_flow);
        }

        private void LoadGenres()
        {
            try
            {
                _cboGenre.Items.Clear();
                _cboGenre.Items.Add("All Genres");
                DataTable genres = _games.GetGenres();
                foreach (DataRow r in genres.Rows)
                    _cboGenre.Items.Add(Convert.ToString(r["GenreName"]));
                _cboGenre.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Store", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGames()
        {
            if (_flow == null || _cboGenre == null) return;

            try
            {
                _flow.Controls.Clear();
                string genre = _cboGenre.SelectedIndex <= 0 ? string.Empty : _cboGenre.Text;
                List<Game> games = _games.GetGames(_txtSearch == null ? string.Empty : _txtSearch.Text.Trim(), genre);

                foreach (Game game in games)
                    _flow.Controls.Add(BuildCard(game));

                if (games.Count == 0)
                    _flow.Controls.Add(Ui.Lbl("No games matched your search.", 10, 10, Theme.H2Font, Theme.Muted));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load the store.\n" + ex.Message,
                    "Store", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel BuildCard(Game game)
        {
            Panel card = Ui.Card(0, 0, 242, 300);
            card.Size = new Size(242, 300);
            card.Margin = new Padding(8);

            PictureBox art = new PictureBox();
            art.SetBounds(0, 0, 242, 126);
            art.SizeMode = PictureBoxSizeMode.StretchImage;
            art.Image = ArtworkFactory.CreateGameCover(game, 242, 126);
            card.Controls.Add(art);

            Label title = Ui.Lbl(game.Title, 14, 142, new Font("Segoe UI", 10F, FontStyle.Bold), Theme.Text);
            title.AutoSize = false;
            title.SetBounds(14, 142, 212, 30);
            card.Controls.Add(title);
            card.Controls.Add(Ui.Lbl(game.PublisherName, 14, 174, Theme.SmallFont, Theme.Muted));
            card.Controls.Add(Ui.Lbl("Stock: " + game.StockQuantity, 14, 195, Theme.SmallFont,
                game.StockQuantity > 0 ? Theme.Green : Theme.Red));
            card.Controls.Add(Ui.Lbl("Tk " + game.Price.ToString("N0"), 14, 226,
                new Font("Segoe UI", 12F, FontStyle.Bold), Theme.Cream));

            Button wish = Ui.Btn("♡", Theme.SurfaceAlt, 14, 258, 44, 30);
            bool inWishlist = false;
            if (Program.CurrentUser != null)
            {
                try { inWishlist = _customer.IsInWishlist(Program.CurrentUser.UserID, game.GameID); }
                catch { inWishlist = false; }
            }
            wish.Text = inWishlist ? "♥" : "♡";
            wish.ForeColor = inWishlist ? Theme.Red : Theme.Text;
            wish.Click += (s, e) => ToggleWishlist(game, wish);
            card.Controls.Add(wish);

            Button add = Ui.Btn(game.StockQuantity > 0 ? "+ Add to Cart" : "Out of Stock",
                game.StockQuantity > 0 ? Theme.Accent : Theme.SurfaceAlt, 70, 258, 156, 30);
            add.Enabled = game.StockQuantity > 0;
            add.Click += (s, e) => AddToCart(game);
            card.Controls.Add(add);
            return card;
        }

        private void ToggleWishlist(Game game, Button button)
        {
            if (Program.CurrentUser == null) return;

            try
            {
                int userId = Program.CurrentUser.UserID;
                if (_customer.IsInWishlist(userId, game.GameID))
                {
                    _customer.RemoveFromWishlist(userId, game.GameID);
                    button.Text = "♡";
                    button.ForeColor = Theme.Text;
                }
                else
                {
                    _customer.AddToWishlist(userId, game.GameID);
                    button.Text = "♥";
                    button.ForeColor = Theme.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Wishlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddToCart(Game game)
        {
            if (game.StockQuantity <= 0)
            {
                MessageBox.Show("This game is currently out of stock.");
                return;
            }

            CartItem existing = _cart.Find(c => c.Game.GameID == game.GameID);
            if (existing != null)
            {
                if (existing.Quantity >= game.StockQuantity)
                {
                    MessageBox.Show("You already added the maximum available stock for " + game.Title + ".");
                    return;
                }
                existing.Quantity++;
            }
            else
            {
                _cart.Add(new CartItem { Game = game, Quantity = 1 });
            }
            UpdateCartButton();
        }

        private void UpdateCartButton()
        {
            int count = 0;
            foreach (CartItem c in _cart) count += c.Quantity;
            _btnCart.Text = "Cart (" + count + ")";
        }

        private void OpenCart()
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Your cart is empty.", "Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (CheckoutForm cf = new CheckoutForm(_cart))
            {
                if (cf.ShowDialog(this) == DialogResult.OK)
                {
                    _cart.Clear();
                    UpdateCartButton();
                    LoadGames();
                }
            }
        }

    }
}
