using System;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    public partial class DashboardForm : Form
    {
        private readonly ReportRepository _reports =
            new ReportRepository();

        private readonly OrderRepository _orders =
            new OrderRepository();

        private readonly bool _isAdmin;
        private readonly bool _isSuperAdmin;

        private Label[] _cards;
        private DataGridView _grid;

        public DashboardForm()
        {
            InitializeComponent();

            _isAdmin =
                Program.CurrentUser != null &&
                (Program.CurrentUser.Role == "Admin" ||
                 Program.CurrentUser.Role == "SuperAdmin");

            _isSuperAdmin =
                Program.CurrentUser != null &&
                Program.CurrentUser.Role == "SuperAdmin";

            BuildUi();

            AppStyle.Apply(this, true);

            MinimumSize =
                new Size(1100, 700);

            WindowState =
                FormWindowState.Maximized;

            LoadData();
        }

        private void BuildUi()
        {
            SuspendLayout();

            Controls.Clear();

            Text =
                "GameHub - " +
                (_isSuperAdmin ? "Super Admin" : (_isAdmin ? "Admin" : "Staff")) +
                " Dashboard";

            BackColor =
                Theme.Background;

            TableLayoutPanel shell =
                new TableLayoutPanel();

            shell.Dock =
                DockStyle.Fill;

            shell.ColumnCount =
                2;

            shell.RowCount =
                1;

            shell.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    255F));

            shell.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            shell.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            Controls.Add(shell);

            Panel side =
                new Panel();

            side.Dock =
                DockStyle.Fill;

            side.BackColor =
                Theme.Sidebar;

            shell.Controls.Add(
                side,
                0,
                0);

            GamePanel logo =
                Ui.GradientCard(
                    18,
                    18,
                    48,
                    48,
                    Theme.Accent,
                    Theme.AccentDark);

            side.Controls.Add(logo);

            Label logoText =
                Ui.FixedLabel(
                    "G",
                    0,
                    4,
                    48,
                    40,
                    new Font(
                        "Segoe UI",
                        19F,
                        FontStyle.Bold),
                    Color.White);

            logoText.TextAlign =
                ContentAlignment.MiddleCenter;

            logo.Controls.Add(logoText);

            Label brand =
                Ui.FixedLabel(
                    "GameHub",
                    82,
                    17,
                    155,
                    34,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Text);

            brand.TextAlign =
                ContentAlignment.MiddleLeft;

            side.Controls.Add(brand);

            Label brandSub =
                Ui.FixedLabel(
                    _isSuperAdmin
                        ? "OWNER CONTROL CENTER"
                        : (_isAdmin
                            ? "ADMIN CONTROL CENTER"
                            : "STAFF CONTROL CENTER"),
                    83,
                    48,
                    155,
                    20,
                    new Font(
                        "Segoe UI",
                        7F,
                        FontStyle.Bold),
                    Theme.Muted2);

            brandSub.TextAlign =
                ContentAlignment.MiddleLeft;

            side.Controls.Add(brandSub);

            side.Controls.Add(
                Ui.FixedLabel(
                    "MANAGEMENT",
                    20,
                    93,
                    180,
                    24,
                    Theme.SmallBold,
                    Theme.Muted2));

            string[] items;

            if (_isSuperAdmin)
            {
                items =
                    new string[]
                    {
                        "Dashboard",
                        "Manage Games",
                        "Orders",
                        "Payments",
                        "Users & Staff",
                        "Reports",
                        "Settings",
                        "System Log",
                        "Logout"
                    };
            }
            else if (_isAdmin)
            {
                items =
                    new string[]
                    {
                        "Dashboard",
                        "Manage Games",
                        "Orders",
                        "Payments",
                        "Users & Staff",
                        "Reports",
                        "Logout"
                    };
            }
            else
            {
                items =
                    new string[]
                    {
                        "Dashboard",
                        "Inventory",
                        "Orders",
                        "Customer Support",
                        "Logout"
                    };
            }

            int y =
                125;

            foreach (string item in items)
            {
                Button button =
                    Ui.Btn(
                        "   " + item,
                        item == "Dashboard"
                            ? Theme.AccentDark
                            : Theme.Sidebar,
                        18,
                        y,
                        219,
                        44);

                button.TextAlign =
                    ContentAlignment.MiddleLeft;

                button.Font =
                    new Font(
                        "Segoe UI",
                        9.5F,
                        item == "Dashboard"
                            ? FontStyle.Bold
                            : FontStyle.Regular);

                string captured =
                    item;

                button.Click +=
                    delegate
                    {
                        Navigate(captured);
                    };

                side.Controls.Add(button);

                y += 50;
            }

            GamePanel userCard =
                Ui.Card(
                    18,
                    0,
                    219,
                    82);

            userCard.Anchor =
                AnchorStyles.Left |
                AnchorStyles.Right |
                AnchorStyles.Bottom;

            side.Controls.Add(userCard);

            userCard.Controls.Add(
                Ui.FixedLabel(
                    "SIGNED IN",
                    15,
                    9,
                    180,
                    18,
                    Theme.SmallBold,
                    Theme.Muted2));

            string username =
                Program.CurrentUser == null
                    ? "User"
                    : Program.CurrentUser.Username;

            userCard.Controls.Add(
                Ui.FixedLabel(
                    username,
                    15,
                    29,
                    180,
                    24,
                    new Font(
                        "Segoe UI",
                        10F,
                        FontStyle.Bold),
                    Theme.Text));

            userCard.Controls.Add(
                Ui.FixedLabel(
                    _isSuperAdmin
                        ? "● Super Admin"
                        : (_isAdmin
                            ? "● Administrator"
                            : "● Staff member"),
                    15,
                    54,
                    185,
                    19,
                    Theme.SmallFont,
                    Theme.Green));

            side.Resize +=
                delegate
                {
                    userCard.Left =
                        18;

                    userCard.Width =
                        side.ClientSize.Width - 36;

                    userCard.Top =
                        side.ClientSize.Height -
                        userCard.Height -
                        18;
                };

            Panel content =
                new Panel();

            content.Dock =
                DockStyle.Fill;

            content.BackColor =
                Theme.Background;

            content.Padding =
                new Padding(
                    28,
                    24,
                    28,
                    26);

            shell.Controls.Add(
                content,
                1,
                0);

            TableLayoutPanel main =
                new TableLayoutPanel();

            main.Dock =
                DockStyle.Fill;

            main.ColumnCount =
                1;

            main.RowCount =
                4;

            main.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            main.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    105F));

            main.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    125F));

            main.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    60F));

            main.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            content.Controls.Add(main);

            Panel header =
                new Panel();

            header.Dock =
                DockStyle.Fill;

            header.BackColor =
                Theme.Background;

            main.Controls.Add(
                header,
                0,
                0);

            string fullName =
                Program.CurrentUser == null
                    ? string.Empty
                    : Program.CurrentUser.FullName;

            if (string.IsNullOrWhiteSpace(fullName))
                fullName = username;

            Label welcome =
                Ui.FixedLabel(
                    "Welcome back, " + fullName,
                    0,
                    3,
                    900,
                    48,
                    new Font(
                        "Segoe UI",
                        21F,
                        FontStyle.Bold),
                    Theme.Text);

            header.Controls.Add(welcome);

            Label subtitle =
                Ui.FixedLabel(
                    _isAdmin
                        ? "Manage users, games, orders, payments and reports from one control center."
                        : "Manage inventory, orders and customer support from one control center.",
                    2,
                    55,
                    1000,
                    30,
                    Theme.BodyFont,
                    Theme.Muted);

            header.Controls.Add(subtitle);

            Label role =
                Ui.Pill(
                    _isSuperAdmin
                        ? "SUPER ADMIN"
                        : (_isAdmin ? "ADMIN" : "STAFF"),
                    0,
                    10,
                    _isSuperAdmin ? 120 : 92,
                    Theme.Cream,
                    Theme.Background2);

            role.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            header.Controls.Add(role);

            header.Resize +=
                delegate
                {
                    role.Left =
                        header.ClientSize.Width -
                        role.Width;

                    welcome.Width =
                        Math.Max(
                            400,
                            role.Left - 20);

                    subtitle.Width =
                        Math.Max(
                            400,
                            header.ClientSize.Width - 20);
                };

            TableLayoutPanel stats =
                new TableLayoutPanel();

            stats.Dock =
                DockStyle.Fill;

            stats.ColumnCount =
                4;

            stats.RowCount =
                1;

            for (int i = 0; i < 4; i++)
            {
                stats.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent,
                        25F));
            }

            stats.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            main.Controls.Add(
                stats,
                0,
                1);

            _cards =
                new Label[4];

            string[] statNames =
            {
                "TOTAL GAMES",
                "TOTAL ORDERS",
                "TOTAL REVENUE",
                "ACTIVE USERS"
            };

            Color[] statColors =
            {
                Theme.Cyan,
                Theme.Purple,
                Theme.Green,
                Theme.Gold
            };

            for (int i = 0; i < 4; i++)
            {
                GamePanel card =
                    Ui.Card(
                        0,
                        0,
                        100,
                        100);

                card.Dock =
                    DockStyle.Fill;

                card.Margin =
                    new Padding(
                        i == 0 ? 0 : 7,
                        0,
                        i == 3 ? 0 : 7,
                        8);

                stats.Controls.Add(
                    card,
                    i,
                    0);

                card.Controls.Add(
                    Ui.FixedLabel(
                        statNames[i],
                        18,
                        15,
                        200,
                        21,
                        Theme.SmallBold,
                        statColors[i]));

                Label value =
                    Ui.FixedLabel(
                        "...",
                        18,
                        45,
                        260,
                        48,
                        new Font(
                            "Segoe UI",
                            19F,
                            FontStyle.Bold),
                        Theme.Text);

                value.Anchor =
                    AnchorStyles.Left |
                    AnchorStyles.Top |
                    AnchorStyles.Right;

                card.Controls.Add(value);

                _cards[i] =
                    value;
            }

            Panel section =
                new Panel();

            section.Dock =
                DockStyle.Fill;

            main.Controls.Add(
                section,
                0,
                2);

            section.Controls.Add(
                Ui.FixedLabel(
                    "Recent Orders",
                    0,
                    13,
                    350,
                    40,
                    Theme.H1Font,
                    Theme.Text));

            GamePanel gridCard =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            gridCard.Dock =
                DockStyle.Fill;

            gridCard.Padding =
                new Padding(16);

            main.Controls.Add(
                gridCard,
                0,
                3);

            _grid =
                Ui.Grid(
                    0,
                    0,
                    100,
                    100);

            _grid.Dock =
                DockStyle.Fill;

            gridCard.Controls.Add(_grid);

            ResumeLayout();
        }

        private void LoadData()
        {
            try
            {
                _cards[0].Text =
                    _reports.TotalGames().ToString();

                _cards[1].Text =
                    _reports.TotalOrders().ToString();

                _cards[2].Text =
                    "Tk " +
                    _reports.TotalRevenue().ToString("N0");

                _cards[3].Text =
                    _reports.ActiveUsers().ToString();

                _grid.DataSource =
                    _orders.GetRecentOrders(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load dashboard data.\n\n" +
                    ex.Message,
                    "Dashboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Navigate(string item)
        {
            switch (item)
            {
                case "Dashboard":
                    LoadData();
                    break;

                case "Manage Games":
                case "Inventory":
                    OpenChild(
                        new ManageGamesForm());
                    break;

                case "Orders":
                    OpenChild(
                        new OrdersForm());
                    break;

                case "Payments":
                    if (_isAdmin)
                        OpenChild(
                            new PaymentsForm());
                    break;

                case "Users & Staff":
                    if (_isAdmin)
                        OpenChild(
                            new UsersForm());
                    break;

                case "Reports":
                    if (_isAdmin)
                        OpenChild(
                            new ReportsForm());
                    break;

                case "Settings":
                    if (_isSuperAdmin)
                        OpenChild(
                            new SettingsForm());
                    break;

                case "System Log":
                    if (_isSuperAdmin)
                        OpenChild(
                            new AuditLogForm());
                    break;

                case "Customer Support":
                    if (!_isAdmin)
                        OpenChild(
                            new CustomerSupportForm());
                    break;

                case "Logout":
                    if (
                        MessageBox.Show(
                            "Log out of GameHub?",
                            "Confirm logout",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        Close();
                    }
                    break;
            }
        }

        private void OpenChild(Form form)
        {
            using (form)
            {
                form.WindowState =
                    FormWindowState.Maximized;

                form.ShowDialog(this);
            }

            LoadData();
        }
    }
}
