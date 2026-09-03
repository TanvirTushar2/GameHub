using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Forms.Controls;
using GameHub.Models;

namespace GameHub.Forms.Views
{
    internal class HomeView : LauncherPage
    {
        private readonly GameRepository _games = new GameRepository();
        private readonly WalletRepository _wallet = new WalletRepository();
        private readonly SubscriptionRepository _subscriptions = new SubscriptionRepository();
        private readonly CustomerRepository _customer = new CustomerRepository();

        private readonly Panel _content;
        private readonly FeaturedGameControl _hero;
        private readonly FlowLayoutPanel _stats;
        private readonly FlowLayoutPanel _popular;
        private readonly FlowLayoutPanel _newReleases;
        private readonly Timer _heroTimer;
        private readonly List<Game> _featured = new List<Game>();
        private int _heroIndex;
        private bool _loading;

        public HomeView()
        {
            Panel scroll = new Panel();
            scroll.Dock = DockStyle.Fill;
            scroll.AutoScroll = true;
            scroll.BackColor = Theme.Background;
            Controls.Add(scroll);

            _content = new Panel();
            _content.BackColor = Theme.Background;
            _content.SetBounds(0, 0, 1120, 1110);
            _content.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            scroll.Controls.Add(_content);

            Label title = Ui.FixedLabel("Welcome back, " + CurrentName(), 0, 0, 720, 40, Theme.TitleFont, Theme.Text);
            title.Name = "HomeTitle";
            _content.Controls.Add(title);
            Label subtitle = Ui.FixedLabel("Your live GameHub feed: featured titles, rewards and latest releases.",
                2, 41, 860, 28, Theme.BodyFont, Theme.Muted);
            _content.Controls.Add(subtitle);

            Button storeButton = Ui.Btn("OPEN STORE  →", Theme.Accent, 930, 8, 154, 38);
            storeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            storeButton.Click += delegate { RequestNavigate("Store"); };
            _content.Controls.Add(storeButton);

            _hero = new FeaturedGameControl();
            _hero.SetBounds(0, 78, 1084, 292);
            _hero.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _hero.DetailsRequested += delegate(Game game) { if (game != null) RequestOpenGame(game.GameID); };
            _hero.AddToCartRequested += delegate(Game game) { if (game != null) RequestOpenGame(game.GameID); };
            _hero.WishlistToggleRequested += ToggleWishlist;
            _content.Controls.Add(_hero);

            _stats = new FlowLayoutPanel();
            _stats.SetBounds(0, 392, 1084, 110);
            _stats.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _stats.WrapContents = false;
            _stats.AutoScroll = false;
            _stats.BackColor = Theme.Background;
            _stats.Padding = new Padding(0);
            _content.Controls.Add(_stats);

            _content.Controls.Add(Ui.Lbl("Popular now", 0, 530, Theme.H1Font, Theme.Text));
            Label popularHint = Ui.Lbl("Most purchased titles in the current catalog", 0, 558, Theme.SmallFont, Theme.Muted);
            _content.Controls.Add(popularHint);

            _popular = new FlowLayoutPanel();
            _popular.SetBounds(0, 592, 1084, 350);
            _popular.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _popular.WrapContents = true;
            _popular.AutoScroll = false;
            _popular.BackColor = Theme.Background;
            _content.Controls.Add(_popular);

            _content.Controls.Add(Ui.Lbl("New releases", 0, 970, Theme.H1Font, Theme.Text));
            _newReleases = new FlowLayoutPanel();
            _newReleases.SetBounds(0, 1006, 1084, 350);
            _newReleases.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _newReleases.WrapContents = true;
            _newReleases.AutoScroll = false;
            _newReleases.BackColor = Theme.Background;
            _content.Controls.Add(_newReleases);
            _content.Height = 1380;

            scroll.Resize += delegate
            {
                _content.Width = Math.Max(720, scroll.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2);
                title.Width = Math.Max(260, _content.Width - 210);
                subtitle.Width = Math.Max(260, _content.Width - 210);
                LayoutResponsive();
            };

            _heroTimer = new Timer();
            _heroTimer.Interval = 8000;
            _heroTimer.Tick += delegate { RotateHero(); };

            VisibleChanged += delegate
            {
                if (Visible)
                {
                    _heroTimer.Start();
                    RefreshData();
                }
                else _heroTimer.Stop();
            };

            LayoutResponsive();
        }

        public override async void RefreshData()
        {
            if (_loading || Program.CurrentUser == null) return;
            _loading = true;
            try
            {
                int userId = Program.CurrentUser.UserID;
                HomeData data = await Task.Run(delegate
                {
                    HomeData d = new HomeData();
                    d.Balance = _wallet.GetBalance(userId);
                    d.Points = _wallet.GetPoints(userId);
                    d.Plan = _subscriptions.GetActivePlanName(userId);
                    d.Library = _customer.GetLibraryCount(userId);
                    d.Wishlist = _customer.GetWishlistCount(userId);
                    d.Featured = _games.GetFeaturedGames(5);
                    d.Popular = _games.GetPopularGames(3);
                    d.NewReleases = _games.GetNewReleases(3);
                    return d;
                });

                Label title = _content.Controls["HomeTitle"] as Label;
                if (title != null) title.Text = "Welcome back, " + CurrentName();

                BuildStats(data);
                _featured.Clear();
                _featured.AddRange(data.Featured);
                _heroIndex = 0;
                ShowHero();
                FillCards(_popular, data.Popular);
                FillCards(_newReleases, data.NewReleases);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not refresh GameHub Home.\n" + ex.Message,
                    "GameHub", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _loading = false;
            }
        }

        private void BuildStats(HomeData data)
        {
            _stats.SuspendLayout();
            _stats.Controls.Clear();
            int available = Math.Max(760, _stats.ClientSize.Width);
            int cardWidth = Math.Max(145, (available - 60) / 5);

            AddStat("Wallet", "Tk " + data.Balance.ToString("N0"), "Available balance", Theme.AccentBlue, cardWidth);
            AddStat("Loyalty", data.Points.ToString("N0") + " pts", "GameHub rewards", Theme.Gold, cardWidth);
            AddStat("GameHub Pass", data.Plan, "Current subscription", Theme.Purple, cardWidth);
            AddStat("Library", data.Library + " games", "Purchased titles", Theme.Green, cardWidth);
            AddStat("Wishlist", data.Wishlist + " saved", "Games you follow", Theme.Red, cardWidth);
            _stats.ResumeLayout();
        }

        private void AddStat(string caption, string value, string note, Color accent, int width)
        {
            StatCardControl card = new StatCardControl(caption, value, note);
            card.Size = new Size(width, 96);
            card.AccentColor = accent;
            _stats.Controls.Add(card);
        }

        private void FillCards(FlowLayoutPanel flow, List<Game> games)
        {
            flow.SuspendLayout();
            flow.Controls.Clear();
            foreach (Game game in games)
            {
                bool inWish = false;
                try
                {
                    if (Program.CurrentUser != null)
                        inWish = _customer.IsInWishlist(Program.CurrentUser.UserID, game.GameID);
                }
                catch { }

                GameCardControl card = new GameCardControl();
                card.SetGame(game, inWish);
                card.DetailsRequested += delegate(Game g) { RequestOpenGame(g.GameID); };
                card.AddToCartRequested += delegate(Game g) { RequestOpenGame(g.GameID); };
                card.WishlistToggleRequested += ToggleWishlistFromCard;
                flow.Controls.Add(card);
            }

            if (games.Count == 0)
                flow.Controls.Add(Ui.Lbl("No games are available yet.", 0, 0, Theme.BodyFont, Theme.Muted));
            flow.ResumeLayout();
        }

        private void ToggleWishlistFromCard(Game game, GameCardControl card)
        {
            bool state = ToggleWishlistCore(game);
            card.SetWishlistState(state);
        }

        private void ToggleWishlist(Game game)
        {
            bool state = ToggleWishlistCore(game);
            _hero.SetWishlistState(state);
        }

        private bool ToggleWishlistCore(Game game)
        {
            if (game == null || Program.CurrentUser == null) return false;
            try
            {
                int userId = Program.CurrentUser.UserID;
                bool current = _customer.IsInWishlist(userId, game.GameID);
                if (current) _customer.RemoveFromWishlist(userId, game.GameID);
                else _customer.AddToWishlist(userId, game.GameID);
                RaiseGlobalDataChanged();
                return !current;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Wishlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void RotateHero()
        {
            if (_featured.Count <= 1) return;
            _heroIndex++;
            if (_heroIndex >= _featured.Count) _heroIndex = 0;
            ShowHero();
        }

        private void ShowHero()
        {
            if (_featured.Count == 0)
            {
                _hero.SetGame(null, false);
                return;
            }

            Game game = _featured[_heroIndex];
            bool inWish = false;
            try
            {
                if (Program.CurrentUser != null)
                    inWish = _customer.IsInWishlist(Program.CurrentUser.UserID, game.GameID);
            }
            catch { }
            _hero.SetGame(game, inWish);
        }

        private void LayoutResponsive()
        {
            int width = Math.Max(720, _content.ClientSize.Width - 4);
            _hero.Width = width;
            _stats.Width = width;
            _popular.Width = width;
            _newReleases.Width = width;
        }

        private static string CurrentName()
        {
            if (Program.CurrentUser == null || string.IsNullOrWhiteSpace(Program.CurrentUser.FullName)) return "Player";
            return Program.CurrentUser.FullName;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _heroTimer != null) _heroTimer.Dispose();
            base.Dispose(disposing);
        }

        private sealed class HomeData
        {
            public decimal Balance;
            public int Points;
            public string Plan;
            public int Library;
            public int Wishlist;
            public List<Game> Featured;
            public List<Game> Popular;
            public List<Game> NewReleases;
        }
    }
}
