using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Forms.Controls;
using GameHub.Forms.Views;

namespace GameHub.Forms
{
    public partial class CustomerDashboardForm : Form
    {
        private readonly Panel _sidebar;
        private readonly Panel _contentHost;
        private Label _pageTitle;
        private Label _userName;
        private Label _avatar;
        private readonly Dictionary<string, Button> _navButtons = new Dictionary<string, Button>();
        private readonly Dictionary<string, LauncherPage> _pages = new Dictionary<string, LauncherPage>();
        private string _currentPage = string.Empty;

        public CustomerDashboardForm()
        {
            InitializeComponent();
            Controls.Clear();
            // Establish one DPI baseline before any programmatic controls are created.
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);

            Text = "GameHub - Desktop Launcher";
            ClientSize = new Size(1440, 860);
            MinimumSize = new Size(1180, 700);
            WindowState = FormWindowState.Maximized;
            KeyPreview = true;
            BackColor = Theme.Background;
            Padding = new Padding(0);

            SuspendLayout();

            // IMPORTANT: use TableLayoutPanel instead of competing DockStyle.Left/Fill
            // controls.  The old docking/Z-order combination allowed the Fill panel to
            // occupy the entire form and the sidebar merely painted over it.
            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Margin = new Padding(0);
            root.Padding = new Padding(0);
            root.ColumnCount = 2;
            root.RowCount = 1;
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 238F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.BackColor = Theme.Background;
            Controls.Add(root);

            _sidebar = BuildSidebar();
            _sidebar.Dock = DockStyle.Fill;
            _sidebar.Margin = new Padding(0);
            root.Controls.Add(_sidebar, 0, 0);

            Panel main = new Panel();
            main.Dock = DockStyle.Fill;
            main.Margin = new Padding(0);
            main.Padding = new Padding(0);
            main.BackColor = Theme.Background;
            root.Controls.Add(main, 1, 0);

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Margin = new Padding(0);
            mainLayout.Padding = new Padding(0);
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.BackColor = Theme.Background;
            main.Controls.Add(mainLayout);

            Panel topbar = BuildTopbar();
            topbar.Dock = DockStyle.Fill;
            topbar.Margin = new Padding(0);
            mainLayout.Controls.Add(topbar, 0, 0);

            _contentHost = new Panel();
            _contentHost.Dock = DockStyle.Fill;
            _contentHost.Margin = new Padding(0);
            _contentHost.Padding = new Padding(24, 18, 24, 24);
            _contentHost.BackColor = Theme.Background;
            mainLayout.Controls.Add(_contentHost, 0, 1);

            ResumeLayout(true);

            AppStyle.Apply(this, true);
            MinimumSize = new Size(1180, 700);
            FormClosing += CustomerDashboardForm_FormClosing;
            KeyDown += CustomerDashboardForm_KeyDown;

            Navigate("Home");
        }

        private Panel BuildSidebar()
        {
            Panel side = new Panel();
            side.BackColor = Theme.Sidebar;
            side.Padding = new Padding(18);

            GamePanel logo = Ui.GradientCard(20, 20, 52, 52, Theme.Accent, Theme.AccentDark);
            side.Controls.Add(logo);
            Label mark = Ui.FixedLabel("G", 0, 7, 52, 38, new Font("Segoe UI", 19F, FontStyle.Bold), Color.White);
            mark.TextAlign = ContentAlignment.MiddleCenter;
            logo.Controls.Add(mark);
            Label brandName = Ui.FixedLabel(
    "GameHub",
    82,
    21,
    145,
    31,
    new Font(
        "Segoe UI",
        13.5F,
        FontStyle.Bold),
    Theme.Text);

            brandName.TextAlign =
                ContentAlignment.MiddleLeft;

            side.Controls.Add(
                brandName);


            Label brandSub = Ui.FixedLabel(
                "DESKTOP LAUNCHER",
                83,
                50,
                145,
                18,
                new Font(
                    "Segoe UI",
                    7F,
                    FontStyle.Bold),
                Theme.Muted2);

            brandSub.TextAlign =
                ContentAlignment.MiddleLeft;

            side.Controls.Add(
                brandSub);

            side.Controls.Add(Ui.Lbl("BROWSE", 22, 106, Theme.SmallBold, Theme.Muted2));

            string[] main = { "Home", "Store", "Library", "Wishlist" };
            int y = 134;
            foreach (string item in main)
            {
                AddNavButton(side, item, NavGlyph(item), y);
                y += 50;
            }

            side.Controls.Add(Ui.Lbl("ACCOUNT", 22, y + 12, Theme.SmallBold, Theme.Muted2));
            y += 42;
            string[] account = { "Wallet", "Subscription", "Profile" };
            foreach (string item in account)
            {
                AddNavButton(side, item, NavGlyph(item), y);
                y += 50;
            }

            GamePanel user = Ui.Card(18, 0, 202, 82);
            user.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            user.StartColor = Theme.Surface;
            user.OutlineColor = Theme.BorderSoft;
            side.Controls.Add(user);

            GamePanel avatarBox = Ui.GradientCard(12, 14, 48, 48, Theme.AccentBlue, Theme.AccentDark);
            user.Controls.Add(avatarBox);
            _avatar = Ui.FixedLabel("G", 0, 5, 48, 36, new Font("Segoe UI", 16F, FontStyle.Bold), Color.White);
            _avatar.TextAlign = ContentAlignment.MiddleCenter;
            avatarBox.Controls.Add(_avatar);

            _userName = Ui.FixedLabel(CurrentDisplayName(), 72, 14, 116, 24, Theme.BodyBold, Theme.Text);
            user.Controls.Add(_userName);
            user.Controls.Add(Ui.FixedLabel("●  Online", 72, 41, 110, 20, Theme.SmallBold, Theme.Green));

            Button logout = Ui.GhostBtn("↪   Logout", 20, 0, 198, 42);
            logout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            logout.ForeColor = Theme.Red;
            logout.Click += delegate { Logout(); };
            side.Controls.Add(logout);

            Action placeBottomControls = delegate
            {
                int h = side.ClientSize.Height;
                logout.Top = Math.Max(540, h - 66);
                user.Top = Math.Max(450, h - 162);
            };
            side.Resize += delegate { placeBottomControls(); };
            placeBottomControls();

            return side;
        }

        private Panel BuildTopbar()
        {
            Panel top = new Panel();
            top.BackColor = Theme.Background2;
            top.Padding = new Padding(24, 0, 24, 0);

            // Pages already have their own headings:
            // Home -> Welcome back...
            // Store -> GameHub Store
            // Subscription -> GameHub Pass
            // etc.
            // Therefore we intentionally do not display another large title here.
            _pageTitle = null;

            Label live = Ui.Pill(
                "SQL LIVE",
                0,
                19,
                82,
                Theme.SurfaceAlt,
                Theme.Green);

            top.Controls.Add(live);

            Button quickStore = Ui.Btn(
                "STORE",
                Theme.AccentDark,
                0,
                14,
                90,
                36);

            quickStore.Click += delegate
            {
                Navigate("Store");
            };

            top.Controls.Add(quickStore);

            Label account = Ui.Pill(
                "CUSTOMER",
                0,
                19,
                98,
                Theme.SurfaceAlt,
                Theme.Cyan);

            top.Controls.Add(account);

            Action layout = delegate
            {
                int right = top.ClientSize.Width - 24;

                account.Left =
                    right - account.Width;

                quickStore.Left =
                    account.Left -
                    quickStore.Width -
                    12;

                live.Left =
                    quickStore.Left -
                    live.Width -
                    12;
            };

            top.Resize += delegate
            {
                layout();
            };

            layout();

            return top;
        }

        private void AddNavButton(Control parent, string key, string glyph, int y)
        {
            Button button = Ui.Btn(glyph + "   " + key, Theme.Sidebar, 20, y, 198, 42);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(10, 0, 0, 0);
            button.Font = Theme.BodyBold;
            button.Click += delegate { Navigate(key); };
            parent.Controls.Add(button);
            _navButtons[key] = button;
        }

        private void Navigate(string pageName)
        {
            if (string.Equals(_currentPage, pageName, StringComparison.OrdinalIgnoreCase))
            {
                LauncherPage existing;
                if (_pages.TryGetValue(pageName, out existing)) existing.RefreshData();
                return;
            }

            LauncherPage page = GetPage(pageName);
            if (page == null) return;

            _currentPage = pageName;
            if (_pageTitle != null)
                _pageTitle.Text = pageName;
            foreach (KeyValuePair<string, Button> pair in _navButtons)
            {
                bool active = string.Equals(pair.Key, pageName, StringComparison.OrdinalIgnoreCase);
                pair.Value.BackColor = active ? Theme.AccentDark : Theme.Sidebar;
                pair.Value.ForeColor = active ? Color.White : Theme.Text;
                pair.Value.FlatAppearance.MouseOverBackColor = active ? Theme.Accent : Theme.SurfaceAlt;
            }

            _contentHost.SuspendLayout();
            _contentHost.Controls.Clear();
            page.Dock = DockStyle.Fill;
            page.Margin = new Padding(0);
            _contentHost.Controls.Add(page);
            _contentHost.ResumeLayout(true);
            page.RefreshData();
        }

        private LauncherPage GetPage(string pageName)
        {
            LauncherPage page;
            if (_pages.TryGetValue(pageName, out page)) return page;

            switch (pageName)
            {
                case "Home": page = new HomeView(); break;
                case "Store": page = new StoreView(); break;
                case "Library": page = new LibraryView(); break;
                case "Wishlist": page = new WishlistView(); break;
                case "Wallet": page = new WalletView(); break;
                case "Subscription": page = new SubscriptionView(); break;
                case "Profile": page = new ProfileView(); break;
                default: return null;
            }

            page.NavigateRequested += Navigate;
            page.GameOpenRequested += OpenGame;
            page.GlobalDataChanged += GlobalDataChanged;
            _pages[pageName] = page;
            return page;
        }

        private void OpenGame(int gameId)
        {
            StoreView store = GetPage("Store") as StoreView;
            Navigate("Store");
            if (store != null) store.OpenGameById(gameId);
        }

        private void GlobalDataChanged()
        {
            UpdateUserCard();
            LauncherPage home;
            if (_pages.TryGetValue("Home", out home) && home != null && home.Visible) home.RefreshData();
        }

        private void UpdateUserCard()
        {
            _userName.Text = CurrentDisplayName();
            string name = CurrentDisplayName();
            _avatar.Text = string.IsNullOrWhiteSpace(name) ? "G" : name.Substring(0, 1).ToUpperInvariant();
        }

        private void Logout()
        {
            DialogResult answer = MessageBox.Show("Log out of GameHub?", "Confirm logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer == DialogResult.Yes) Close();
        }

        private void CustomerDashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // LoginForm owns this launcher with ShowDialog. Closing simply returns to Login.
        }

        private void CustomerDashboardForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D1) { Navigate("Home"); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.D2) { Navigate("Store"); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.D3) { Navigate("Library"); e.Handled = true; }
        }

        private static string CurrentDisplayName()
        {
            if (Program.CurrentUser == null) return "Player";
            if (!string.IsNullOrWhiteSpace(Program.CurrentUser.FullName)) return Program.CurrentUser.FullName;
            return Program.CurrentUser.Username;
        }

        private static string NavGlyph(string item)
        {
            switch (item)
            {
                case "Home": return "⌂";
                case "Store": return "▣";
                case "Library": return "▤";
                case "Wishlist": return "♥";
                case "Wallet": return "◈";
                case "Subscription": return "★";
                case "Profile": return "●";
                default: return "•";
            }
        }
    }
}
