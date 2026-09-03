using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Forms.Controls;
using GameHub.Models;

namespace GameHub.Forms.Views
{
    /// <summary>
    /// SQL-backed store page with live polling, reusable cards,
    /// in-page details and responsive cart drawer.
    /// </summary>
    internal class StoreView : LauncherPage
    {
        private readonly GameRepository _games =
            new GameRepository();

        private readonly CustomerRepository _customer =
            new CustomerRepository();

        private readonly List<CartItem> _cart =
            new List<CartItem>();

        private readonly Timer _refreshTimer;

        private readonly Panel _browser;
        private readonly TextBox _search;
        private readonly ComboBox _genre;
        private readonly Button _cartButton;
        private readonly FeaturedGameControl _hero;
        private readonly FlowLayoutPanel _flow;

        private readonly GamePanel _cartDrawer;
        private readonly FlowLayoutPanel _cartItems;
        private readonly Label _cartTotal;
        private readonly Button _checkoutButton;

        private readonly GameDetailsView _details;

        private List<Game> _currentGames =
            new List<Game>();

        private bool _loading;
        private bool _genresLoaded;

        private string _lastSignature =
            string.Empty;

        private int _displayCount = 8;

        public StoreView()
        {
            _browser =
                new Panel();

            _browser.Dock =
                DockStyle.Fill;

            _browser.Margin =
                new Padding(0);

            _browser.Padding =
                new Padding(0);

            _browser.BackColor =
                Theme.Background;

            Controls.Add(_browser);

            // ==========================================================
            // MAIN STORE LAYOUT
            // ==========================================================

            TableLayoutPanel browserLayout =
                new TableLayoutPanel();

            browserLayout.Dock =
                DockStyle.Fill;

            browserLayout.Margin =
                new Padding(0);

            browserLayout.Padding =
                new Padding(0);

            browserLayout.ColumnCount = 1;
            browserLayout.RowCount = 5;

            browserLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            browserLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    72F));

            browserLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    62F));

            browserLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    260F));

            browserLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    58F));

            browserLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            browserLayout.BackColor =
                Theme.Background;

            _browser.Controls.Add(
                browserLayout);

            // ==========================================================
            // HEADER
            // ==========================================================

            Panel header =
                new Panel();

            header.Dock =
                DockStyle.Fill;

            header.Margin =
                new Padding(0);

            header.BackColor =
                Theme.Background;

            browserLayout.Controls.Add(
                header,
                0,
                0);

            header.Controls.Add(
                Ui.FixedLabel(
                    "GameHub Store",
                    0,
                    0,
                    640,
                    38,
                    Theme.TitleFont,
                    Theme.Text));

            Label headerSub =
                Ui.FixedLabel(
                    "Live catalog • search, compare, wishlist and purchase without leaving the launcher.",
                    2,
                    40,
                    900,
                    26,
                    Theme.BodyFont,
                    Theme.Muted);

            header.Controls.Add(
                headerSub);

            Label live =
                Ui.Pill(
                    "●  LIVE",
                    0,
                    5,
                    76,
                    Theme.SurfaceAlt,
                    Theme.Green);

            header.Controls.Add(
                live);

            header.Resize += delegate
            {
                live.Left =
                    Math.Max(
                        0,
                        header.ClientSize.Width
                        - live.Width
                        - 4);

                headerSub.Width =
                    Math.Max(
                        260,
                        live.Left - 18);
            };

            // ==========================================================
            // SEARCH TOOLBAR
            // ==========================================================

            GamePanel toolbar =
                Ui.Card(
                    0,
                    0,
                    100,
                    62);

            toolbar.Dock =
                DockStyle.Fill;

            toolbar.Margin =
                new Padding(0);

            toolbar.Padding =
                new Padding(
                    14,
                    12,
                    14,
                    10);

            browserLayout.Controls.Add(
                toolbar,
                0,
                1);

            Label searchLabel =
                Ui.Lbl(
                    "SEARCH",
                    16,
                    21,
                    Theme.SmallBold,
                    Theme.Muted);

            toolbar.Controls.Add(
                searchLabel);

            _search =
                Ui.TextInput(
                    80,
                    16,
                    350,
                    31);

            _search.KeyDown += delegate (
                object sender,
                KeyEventArgs e)
            {
                if (e.KeyCode
                    == Keys.Enter)
                {
                    _displayCount = 8;

                    LoadGamesAsync(true);

                    e.SuppressKeyPress =
                        true;
                }
            };

            toolbar.Controls.Add(
                _search);

            Button searchButton =
                Ui.Btn(
                    "SEARCH",
                    Theme.AccentDark,
                    440,
                    14,
                    98,
                    34);

            searchButton.Font =
                new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold);

            searchButton.Click += delegate
            {
                _displayCount = 8;

                LoadGamesAsync(true);
            };

            toolbar.Controls.Add(
                searchButton);

            _genre =
                new ComboBox();

            _genre.DropDownStyle =
                ComboBoxStyle.DropDownList;

            _genre.SetBounds(
                544,
                16,
                185,
                31);

            _genre.SelectedIndexChanged +=
                delegate
                {
                    if (_genresLoaded)
                    {
                        _displayCount = 8;

                        LoadGamesAsync(true);
                    }
                };

            toolbar.Controls.Add(
                _genre);

            Button refresh =
                Ui.GhostBtn(
                    "↻",
                    742,
                    14,
                    42,
                    34);

            refresh.Font =
                new Font(
                    "Segoe UI Symbol",
                    12F,
                    FontStyle.Bold);

            refresh.Click += delegate
            {
                LoadGamesAsync(true);
            };

            toolbar.Controls.Add(
                refresh);

            _cartButton =
                Ui.Btn(
                    "CART  0",
                    Theme.Green,
                    802,
                    14,
                    118,
                    34);

            _cartButton.Click += delegate
            {
                ToggleCart();
            };

            toolbar.Controls.Add(
                _cartButton);

            Action layoutToolbar =
                delegate
                {
                    int w =
                        toolbar.ClientSize.Width;

                    _cartButton.Left =
                        Math.Max(
                            500,
                            w
                            - _cartButton.Width
                            - 14);

                    refresh.Left =
                        _cartButton.Left
                        - refresh.Width
                        - 10;

                    _genre.Left =
                        refresh.Left
                        - _genre.Width
                        - 12;

                    searchButton.Left =
                        _genre.Left
                        - searchButton.Width
                        - 12;

                    searchLabel.Visible =
                        searchButton.Left >= 270;

                    if (searchLabel.Visible)
                    {
                        _search.Left = 80;
                    }
                    else
                    {
                        _search.Left = 16;
                    }

                    int inputWidth =
                        searchButton.Left
                        - _search.Left
                        - 10;

                    _search.Width =
                        Math.Max(
                            150,
                            inputWidth);
                };

            toolbar.Resize += delegate
            {
                layoutToolbar();
            };

            // ==========================================================
            // FEATURED GAME
            // ==========================================================

            _hero =
                new FeaturedGameControl();

            _hero.Dock =
                DockStyle.Fill;

            _hero.Margin =
                new Padding(0);

            _hero.DetailsRequested +=
                ShowDetails;

            _hero.AddToCartRequested +=
                AddToCart;

            _hero.WishlistToggleRequested +=
                ToggleWishlistFromHero;

            browserLayout.Controls.Add(
                _hero,
                0,
                2);

            // ==========================================================
            // BROWSE HEADER
            // ==========================================================

            Panel section =
                new Panel();

            section.Dock =
                DockStyle.Fill;

            section.Margin =
                new Padding(0);

            section.BackColor =
                Theme.Background;

            browserLayout.Controls.Add(
                section,
                0,
                3);

            section.Controls.Add(
                Ui.FixedLabel(
                    "Browse all games",
                    0,
                    13,
                    330,
                    36,
                    Theme.H1Font,
                    Theme.Text));

            Label hint =
                Ui.FixedLabel(
                    "Cards update automatically when SQL price or stock changes.",
                    0,
                    19,
                    430,
                    24,
                    Theme.SmallFont,
                    Theme.Muted);

            section.Controls.Add(
                hint);

            section.Resize += delegate
            {
                hint.Left =
                    Math.Max(
                        340,
                        section.ClientSize.Width
                        - hint.Width
                        - 4);

                hint.Visible =
                    section.ClientSize.Width
                    >= 760;
            };

            // ==========================================================
            // GAME CARDS
            // ==========================================================

            _flow =
                new FlowLayoutPanel();

            _flow.Dock =
                DockStyle.Fill;

            _flow.Margin =
                new Padding(0);

            _flow.AutoScroll =
                true;

            _flow.WrapContents =
                true;

            _flow.BackColor =
                Theme.Background;

            _flow.Padding =
                new Padding(
                    0,
                    8,
                    0,
                    20);

            browserLayout.Controls.Add(
                _flow,
                0,
                4);

            // ==========================================================
            // FIXED CART DRAWER
            // ==============================================================

            _cartDrawer =
                Ui.Card(
                    0,
                    0,
                    330,
                    300);

            _cartDrawer.Width =
                330;

            _cartDrawer.Visible =
                false;

            _cartDrawer.Padding =
                new Padding(18);

            _cartDrawer.StartColor =
                Theme.Sidebar;

            _cartDrawer.OutlineColor =
                Theme.Border;

            // IMPORTANT:
            // Do NOT anchor Bottom.
            // This drawer now sizes itself to its actual cart content.
            _cartDrawer.Anchor =
                AnchorStyles.Top
                |
                AnchorStyles.Right;

            _browser.Controls.Add(
                _cartDrawer);

            _cartDrawer.BringToFront();

            Label cartTitle =
                Ui.FixedLabel(
                    "Your cart",
                    18,
                    17,
                    220,
                    34,
                    Theme.H1Font,
                    Theme.Text);

            _cartDrawer.Controls.Add(
                cartTitle);

            Button closeCart =
                Ui.GhostBtn(
                    "×",
                    278,
                    14,
                    34,
                    32);

            closeCart.Font =
                new Font(
                    "Segoe UI",
                    12F,
                    FontStyle.Bold);

            closeCart.Click += delegate
            {
                _cartDrawer.Visible =
                    false;
            };

            _cartDrawer.Controls.Add(
                closeCart);

            _cartItems =
                new FlowLayoutPanel();

            _cartItems.SetBounds(
                18,
                62,
                294,
                100);

            _cartItems.FlowDirection =
                FlowDirection.TopDown;

            _cartItems.WrapContents =
                false;

            _cartItems.AutoScroll =
                true;

            _cartItems.BackColor =
                Theme.Sidebar;

            _cartDrawer.Controls.Add(
                _cartItems);

            _cartTotal =
                Ui.FixedLabel(
                    "Total  Tk 0",
                    18,
                    174,
                    294,
                    34,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Cream);

            _cartDrawer.Controls.Add(
                _cartTotal);

            _checkoutButton =
                Ui.Btn(
                    "PROCEED TO CHECKOUT",
                    Theme.Accent,
                    18,
                    216,
                    294,
                    42);

            _checkoutButton.Click += delegate
            {
                OpenCheckout();
            };

            _cartDrawer.Controls.Add(
                _checkoutButton);

            Action layoutDrawer =
                delegate
                {
                    LayoutCartDrawer();

                    _cartDrawer
                        .BringToFront();
                };

            _browser.Resize += delegate
            {
                layoutToolbar();

                layoutDrawer();
            };

            layoutToolbar();

            layoutDrawer();

            // ==========================================================
            // DETAILS PAGE
            // ==============================================================

            _details =
                new GameDetailsView();

            _details.Visible =
                false;

            _details.BackRequested += delegate
            {
                ShowBrowser();
            };

            _details.AddToCartRequested +=
                AddToCart;

            _details.DataChanged += delegate
            {
                LoadGamesAsync(true);

                RaiseGlobalDataChanged();
            };

            Controls.Add(
                _details);

            _details.BringToFront();

            // ==========================================================
            // AUTO REFRESH
            // ==============================================================

            _refreshTimer =
                new Timer();

            _refreshTimer.Interval =
                15000;

            _refreshTimer.Tick += delegate
            {
                if (Visible
                    &&
                    _browser.Visible
                    &&
                    !_loading)
                {
                    LoadGamesAsync(false);
                }
            };

            VisibleChanged += delegate
            {
                if (Visible)
                {
                    _refreshTimer.Start();

                    RefreshData();
                }
                else
                {
                    _refreshTimer.Stop();
                }
            };

            LoadGenresAsync();

            UpdateCartUi();
        }

        public override void RefreshData()
        {
            LoadGenresAsync();

            LoadGamesAsync(true);
        }

        public async void OpenGameById(
            int gameId)
        {
            try
            {
                Game found = null;

                foreach (
                    Game game
                    in _currentGames)
                {
                    if (game.GameID
                        == gameId)
                    {
                        found = game;

                        break;
                    }
                }

                if (found == null)
                {
                    found =
                        await Task.Run(
                            delegate
                            {
                                return
                                    _games
                                    .GetGameById(
                                        gameId);
                            });
                }

                if (found != null)
                {
                    ShowDetails(found);
                }
                else
                {
                    MessageBox.Show(
                        "That game is no longer available in the store.",
                        "GameHub",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not open the game.\n"
                    + ex.Message,
                    "Store",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void LoadGenresAsync()
        {
            if (_genresLoaded)
                return;

            try
            {
                DataTable genres =
                    await Task.Run(
                        delegate
                        {
                            return
                                _games.GetGenres();
                        });

                _genre.Items.Clear();

                _genre.Items.Add(
                    "All Genres");

                foreach (
                    DataRow row
                    in genres.Rows)
                {
                    _genre.Items.Add(
                        Convert.ToString(
                            row["GenreName"]));
                }

                _genresLoaded =
                    true;

                _genre.SelectedIndex =
                    0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load genres.\n"
                    + ex.Message,
                    "Store",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private async void LoadGamesAsync(
            bool force)
        {
            if (_loading)
                return;

            _loading = true;

            try
            {
                string search =
                    _search.Text.Trim();

                string genre =
                    _genre.SelectedIndex <= 0
                    ?
                    string.Empty
                    :
                    _genre.Text;

                List<Game> games =
                    await Task.Run(
                        delegate
                        {
                            return
                                _games.GetGames(
                                    search,
                                    genre);
                        });

                string signature =
                    BuildSignature(games)
                    + "|"
                    + search
                    + "|"
                    + genre
                    + "|"
                    + _displayCount;

                _currentGames =
                    games;

                if (force
                    ||
                    !string.Equals(
                        signature,
                        _lastSignature,
                        StringComparison.Ordinal))
                {
                    _lastSignature =
                        signature;

                    RenderGames();
                }

                if (_hero.Game == null
                    &&
                    games.Count > 0)
                {
                    SetHero(
                        games[0]);
                }
                else if (_hero.Game != null)
                {
                    Game refreshed =
                        null;

                    foreach (
                        Game g
                        in games)
                    {
                        if (g.GameID
                            ==
                            _hero.Game.GameID)
                        {
                            refreshed =
                                g;

                            break;
                        }
                    }

                    if (refreshed != null)
                    {
                        SetHero(
                            refreshed);
                    }
                    else if (games.Count > 0)
                    {
                        SetHero(
                            games[0]);
                    }
                    else
                    {
                        _hero.SetGame(
                            null,
                            false);
                    }
                }
            }
            catch (Exception ex)
            {
                if (force)
                {
                    MessageBox.Show(
                        "Could not refresh the store.\n"
                        + ex.Message,
                        "Store",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            finally
            {
                _loading = false;
            }
        }

        private void RenderGames()
        {
            _flow.SuspendLayout();

            _flow.Controls.Clear();

            int count =
                Math.Min(
                    _displayCount,
                    _currentGames.Count);

            for (
                int i = 0;
                i < count;
                i++)
            {
                Game game =
                    _currentGames[i];

                bool wish =
                    IsWishlisted(
                        game.GameID);

                GameCardControl card =
                    new GameCardControl();

                card.SetGame(
                    game,
                    wish);

                card.DetailsRequested +=
                    ShowDetails;

                card.AddToCartRequested +=
                    AddToCart;

                card.WishlistToggleRequested +=
                    ToggleWishlistFromCard;

                _flow.Controls.Add(
                    card);
            }

            if (_currentGames.Count == 0)
            {
                Label empty =
                    Ui.FixedLabel(
                        "No games matched your search. Try a different title, genre or publisher.",
                        0,
                        0,
                        620,
                        70,
                        Theme.H2Font,
                        Theme.Muted);

                empty.Margin =
                    new Padding(
                        10,
                        20,
                        0,
                        0);

                _flow.Controls.Add(
                    empty);
            }
            else if (
                _displayCount
                <
                _currentGames.Count)
            {
                GamePanel loadPanel =
                    Ui.Card(
                        0,
                        0,
                        228,
                        120);

                loadPanel.Size =
                    new Size(
                        228,
                        120);

                loadPanel.Margin =
                    new Padding(
                        0,
                        0,
                        18,
                        20);

                loadPanel.Controls.Add(
                    Ui.Lbl(
                        (_currentGames.Count
                         - _displayCount)
                        +
                        " more games",
                        20,
                        20,
                        Theme.H2Font,
                        Theme.Text));

                Button load =
                    Ui.Btn(
                        "LOAD MORE",
                        Theme.AccentDark,
                        20,
                        62,
                        188,
                        34);

                load.Click += delegate
                {
                    _displayCount += 8;

                    _lastSignature =
                        string.Empty;

                    RenderGames();
                };

                loadPanel.Controls.Add(
                    load);

                _flow.Controls.Add(
                    loadPanel);
            }

            _flow.ResumeLayout();
        }

        private void ShowDetails(
            Game game)
        {
            if (game == null)
                return;

            _cartDrawer.Visible =
                false;

            _details.LoadGame(
                game);

            _browser.Visible =
                false;

            _details.Visible =
                true;

            _details.BringToFront();
        }

        private void ShowBrowser()
        {
            _details.Visible =
                false;

            _browser.Visible =
                true;

            _browser.BringToFront();

            LoadGamesAsync(true);
        }

        private void SetHero(
            Game game)
        {
            _hero.SetGame(
                game,
                game != null
                &&
                IsWishlisted(
                    game.GameID));
        }

        private bool IsWishlisted(
            int gameId)
        {
            if (Program.CurrentUser
                == null)
            {
                return false;
            }

            try
            {
                return
                    _customer
                    .IsInWishlist(
                        Program.CurrentUser.UserID,
                        gameId);
            }
            catch
            {
                return false;
            }
        }

        private void ToggleWishlistFromCard(
            Game game,
            GameCardControl card)
        {
            bool state =
                ToggleWishlistCore(
                    game);

            card.SetWishlistState(
                state);

            if (_hero.Game != null
                &&
                _hero.Game.GameID
                ==
                game.GameID)
            {
                _hero.SetWishlistState(
                    state);
            }
        }

        private void ToggleWishlistFromHero(
            Game game)
        {
            bool state =
                ToggleWishlistCore(
                    game);

            _hero.SetWishlistState(
                state);
        }

        private bool ToggleWishlistCore(
            Game game)
        {
            if (game == null
                ||
                Program.CurrentUser == null)
            {
                return false;
            }

            try
            {
                int id =
                    Program.CurrentUser.UserID;

                bool current =
                    _customer
                    .IsInWishlist(
                        id,
                        game.GameID);

                if (current)
                {
                    _customer
                        .RemoveFromWishlist(
                            id,
                            game.GameID);
                }
                else
                {
                    _customer
                        .AddToWishlist(
                            id,
                            game.GameID);
                }

                RaiseGlobalDataChanged();

                return !current;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Wishlist",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
        }

        // ==============================================================
        // CART
        // ==============================================================

        private void AddToCart(
            Game game)
        {
            if (game == null)
                return;

            if (game.StockQuantity <= 0)
            {
                MessageBox.Show(
                    "This game is currently out of stock.",
                    "Store",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            CartItem existing =
                _cart.Find(
                    delegate (
                        CartItem item)
                    {
                        return
                            item.Game.GameID
                            ==
                            game.GameID;
                    });

            if (existing == null)
            {
                _cart.Add(
                    new CartItem
                    {
                        Game = game,
                        Quantity = 1
                    });
            }
            else if (
                existing.Quantity
                <
                game.StockQuantity)
            {
                existing.Quantity++;
            }
            else
            {
                MessageBox.Show(
                    "You already added the maximum available quantity for "
                    + game.Title
                    + ".",
                    "Cart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // Important fix:
            // If ADD TO CART is clicked from game details,
            // return to browser before showing the cart.
            if (_details != null
                &&
                _details.Visible)
            {
                _details.Visible =
                    false;

                _browser.Visible =
                    true;

                _browser.BringToFront();
            }

            UpdateCartUi();

            _cartDrawer.Visible =
                true;

            _cartDrawer.BringToFront();
        }

        private void ToggleCart()
        {
            if (_cartDrawer.Visible)
            {
                _cartDrawer.Visible =
                    false;

                return;
            }

            UpdateCartUi();

            _cartDrawer.Visible =
                true;

            _cartDrawer.BringToFront();
        }

        private void UpdateCartUi()
        {
            int quantity = 0;

            decimal total = 0m;

            foreach (
                CartItem item
                in _cart)
            {
                quantity +=
                    item.Quantity;

                total +=
                    item.Subtotal;
            }

            _cartButton.Text =
                "CART  "
                + quantity;

            _cartTotal.Text =
                "Total  Tk "
                + total.ToString("N0");

            _checkoutButton.Enabled =
                _cart.Count > 0;

            _checkoutButton.BackColor =
                _cart.Count > 0
                ?
                Theme.Accent
                :
                Theme.SurfaceAlt;

            _cartItems.SuspendLayout();

            _cartItems.Controls.Clear();

            if (_cart.Count == 0)
            {
                Label empty =
                    Ui.FixedLabel(
                        "Your cart is empty.\nAdd a game from the store.",
                        0,
                        0,
                        250,
                        64,
                        Theme.BodyFont,
                        Theme.Muted);

                empty.Margin =
                    new Padding(
                        0,
                        10,
                        0,
                        0);

                _cartItems.Controls.Add(
                    empty);
            }
            else
            {
                foreach (
                    CartItem item
                    in _cart)
                {
                    GamePanel row =
                        Ui.Card(
                            0,
                            0,
                            270,
                            100);

                    row.Size =
                        new Size(
                            270,
                            100);

                    row.Margin =
                        new Padding(
                            0,
                            0,
                            0,
                            10);

                    row.Controls.Add(
                        Ui.FixedLabel(
                            item.Game.Title,
                            12,
                            9,
                            188,
                            22,
                            Theme.BodyBold,
                            Theme.Text));

                    row.Controls.Add(
                        Ui.Lbl(
                            item.Quantity
                            +
                            " × Tk "
                            +
                            item.Game.Price.ToString(
                                "N0"),
                            12,
                            36,
                            Theme.SmallFont,
                            Theme.Muted));

                    row.Controls.Add(
                        Ui.Lbl(
                            "Tk "
                            +
                            item.Subtotal.ToString(
                                "N0"),
                            12,
                            61,
                            Theme.SmallBold,
                            Theme.Cream));

                    // MINUS
                    Button minus =
                        Ui.GhostBtn(
                            "−",
                            174,
                            56,
                            30,
                            30);

                    minus.Tag =
                        item;

                    minus.Click += delegate (
                        object sender,
                        EventArgs e)
                    {
                        Button button =
                            sender as Button;

                        CartItem ci =
                            button == null
                            ?
                            null
                            :
                            button.Tag
                                as CartItem;

                        if (ci == null)
                            return;

                        if (ci.Quantity > 1)
                        {
                            ci.Quantity--;
                        }
                        else
                        {
                            _cart.Remove(
                                ci);
                        }

                        UpdateCartUi();
                    };

                    row.Controls.Add(
                        minus);

                    // PLUS
                    Button plus =
                        Ui.GhostBtn(
                            "+",
                            208,
                            56,
                            30,
                            30);

                    plus.Tag =
                        item;

                    plus.Click += delegate (
                        object sender,
                        EventArgs e)
                    {
                        Button button =
                            sender as Button;

                        CartItem ci =
                            button == null
                            ?
                            null
                            :
                            button.Tag
                                as CartItem;

                        if (ci == null)
                            return;

                        if (ci.Quantity
                            >=
                            ci.Game.StockQuantity)
                        {
                            MessageBox.Show(
                                "No more stock is available for "
                                + ci.Game.Title
                                + ".",
                                "Cart",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            return;
                        }

                        ci.Quantity++;

                        UpdateCartUi();
                    };

                    row.Controls.Add(
                        plus);

                    // REMOVE
                    Button remove =
                        Ui.GhostBtn(
                            "×",
                            238,
                            9,
                            28,
                            28);

                    remove.Tag =
                        item;

                    remove.Click += delegate (
                        object sender,
                        EventArgs e)
                    {
                        Button button =
                            sender as Button;

                        CartItem ci =
                            button == null
                            ?
                            null
                            :
                            button.Tag
                                as CartItem;

                        if (ci != null)
                        {
                            _cart.Remove(
                                ci);
                        }

                        UpdateCartUi();
                    };

                    row.Controls.Add(
                        remove);

                    _cartItems.Controls.Add(
                        row);
                }
            }

            _cartItems.ResumeLayout();

            LayoutCartDrawer();
        }

        // ==============================================================
        // CART COLLAPSE FIX
        // ==============================================================

        private void LayoutCartDrawer()
        {
            if (_browser == null
                ||
                _cartDrawer == null
                ||
                _cartItems == null
                ||
                _cartTotal == null
                ||
                _checkoutButton == null)
            {
                return;
            }

            int browserWidth =
                Math.Max(
                    330,
                    _browser.ClientSize.Width);

            int browserHeight =
                Math.Max(
                    260,
                    _browser.ClientSize.Height);

            int desiredItemsHeight;

            if (_cart.Count == 0)
            {
                desiredItemsHeight =
                    84;
            }
            else
            {
                desiredItemsHeight =
                    Math.Min(
                        330,
                        (_cart.Count * 110)
                        + 2);
            }

            const int headerHeight = 62;
            const int gapAfterItems = 10;
            const int totalHeight = 34;
            const int gapBeforeCheckout = 8;
            const int checkoutHeight = 42;
            const int bottomPadding = 18;

            int desiredDrawerHeight =
                headerHeight
                +
                desiredItemsHeight
                +
                gapAfterItems
                +
                totalHeight
                +
                gapBeforeCheckout
                +
                checkoutHeight
                +
                bottomPadding;

            int maxDrawerHeight =
                Math.Max(
                    260,
                    browserHeight - 8);

            int drawerHeight =
                Math.Min(
                    desiredDrawerHeight,
                    maxDrawerHeight);

            drawerHeight =
                Math.Max(
                    260,
                    drawerHeight);

            int drawerX =
                Math.Max(
                    0,
                    browserWidth
                    - _cartDrawer.Width);

            _cartDrawer.SetBounds(
                drawerX,
                0,
                _cartDrawer.Width,
                drawerHeight);

            int footerHeight =
                totalHeight
                +
                gapBeforeCheckout
                +
                checkoutHeight
                +
                bottomPadding;

            int itemsHeight =
                Math.Max(
                    74,
                    drawerHeight
                    -
                    headerHeight
                    -
                    gapAfterItems
                    -
                    footerHeight);

            _cartItems.SetBounds(
                18,
                headerHeight,
                294,
                itemsHeight);

            _cartTotal.SetBounds(
                18,
                _cartItems.Bottom
                + gapAfterItems,
                294,
                totalHeight);

            _checkoutButton.SetBounds(
                18,
                _cartTotal.Bottom
                + gapBeforeCheckout,
                294,
                checkoutHeight);
        }

        // ==============================================================
        // CHECKOUT
        // ==============================================================

        private void OpenCheckout()
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show(
                    "Your cart is empty.",
                    "Cart",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            using (
                CheckoutForm checkout =
                    new CheckoutForm(
                        _cart))
            {
                if (checkout.ShowDialog(
                        FindForm())
                    ==
                    DialogResult.OK)
                {
                    _cart.Clear();

                    UpdateCartUi();

                    _cartDrawer.Visible =
                        false;

                    _lastSignature =
                        string.Empty;

                    LoadGamesAsync(true);

                    RaiseGlobalDataChanged();
                }
            }
        }

        private static string BuildSignature(
            List<Game> games)
        {
            StringBuilder sb =
                new StringBuilder();

            foreach (
                Game game
                in games)
            {
                sb.Append(
                        game.GameID)
                    .Append('|')
                    .Append(
                        game.Title)
                    .Append('|')
                    .Append(
                        game.Price)
                    .Append('|')
                    .Append(
                        game.StockQuantity)
                    .Append(';');
            }

            return sb.ToString();
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing
                &&
                _refreshTimer != null)
            {
                _refreshTimer.Dispose();
            }

            base.Dispose(
                disposing);
        }
    }
}