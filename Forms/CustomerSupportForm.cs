using System;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    public partial class CustomerSupportForm : Form
    {
        private readonly UserRepository _users =
            new UserRepository();

        private TextBox _search;
        private DataGridView _grid;

        public CustomerSupportForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();

            AppStyle.Apply(this, true);

            MinimumSize =
                new Size(1000, 650);

            WindowState =
                FormWindowState.Maximized;

            LoadCustomers();
        }

        private void BuildUi()
        {
            Text =
                "GameHub - Customer Support";

            BackColor =
                Theme.Background;

            TableLayoutPanel root =
                new TableLayoutPanel();

            root.Dock =
                DockStyle.Fill;

            root.Padding =
                new Padding(
                    28,
                    22,
                    28,
                    26);

            root.ColumnCount =
                1;

            root.RowCount =
                4;

            root.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    90F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    115F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    76F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            Controls.Add(root);

            // HEADER
            Panel header =
                new Panel();

            header.Dock =
                DockStyle.Fill;

            root.Controls.Add(
                header,
                0,
                0);

            Label title =
                Ui.FixedLabel(
                    "Customer Support",
                    0,
                    0,
                    700,
                    45,
                    new Font(
                        "Segoe UI",
                        20F,
                        FontStyle.Bold),
                    Theme.Text);

            header.Controls.Add(title);

            Label subtitle =
                Ui.FixedLabel(
                    "Find customer accounts and contact information for support assistance.",
                    2,
                    48,
                    900,
                    30,
                    Theme.BodyFont,
                    Theme.Muted);

            header.Controls.Add(subtitle);

            Button back =
                Ui.GhostBtn(
                    "←  BACK",
                    0,
                    4,
                    105,
                    36);

            back.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            back.Click +=
                delegate
                {
                    Close();
                };

            header.Controls.Add(back);

            header.Resize +=
                delegate
                {
                    back.Left =
                        header.ClientSize.Width -
                        back.Width;

                    title.Width =
                        Math.Max(
                            300,
                            back.Left - 20);

                    subtitle.Width =
                        Math.Max(
                            300,
                            header.ClientSize.Width - 10);
                };

            // NOTICE
            GamePanel notice =
                Ui.GradientCard(
                    0,
                    0,
                    100,
                    100,
                    Theme.AccentDark,
                    Theme.Background2);

            notice.Dock =
                DockStyle.Fill;

            notice.Margin =
                new Padding(
                    0,
                    0,
                    0,
                    12);

            root.Controls.Add(
                notice,
                0,
                1);

            notice.Controls.Add(
                Ui.Pill(
                    "SUPPORT",
                    18,
                    14,
                    80,
                    Theme.Cream,
                    Theme.Background2));

            Label info =
                Ui.FixedLabel(
                    "The current GameHub database has no SupportTickets table. " +
                    "This panel therefore provides customer account and contact information " +
                    "without changing your existing database schema.",
                    18,
                    49,
                    1000,
                    48,
                    Theme.BodyFont,
                    Theme.Cream);

            info.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            notice.Controls.Add(info);

            notice.Resize +=
                delegate
                {
                    info.Width =
                        Math.Max(
                            300,
                            notice.ClientSize.Width - 36);
                };

            // SEARCH
            GamePanel searchCard =
                Ui.Card(
                    0,
                    0,
                    100,
                    64);

            searchCard.Dock =
                DockStyle.Fill;

            searchCard.Margin =
                new Padding(
                    0,
                    0,
                    0,
                    12);

            root.Controls.Add(
                searchCard,
                0,
                2);

            _search =
                Ui.TextInput(
                    18,
                    17,
                    600,
                    30);

            _search.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            searchCard.Controls.Add(_search);

            Button search =
                Ui.Btn(
                    "SEARCH",
                    Theme.Accent,
                    0,
                    15,
                    105,
                    34);

            search.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            search.Click +=
                delegate
                {
                    LoadCustomers();
                };

            searchCard.Controls.Add(search);

            Button all =
                Ui.Btn(
                    "SHOW ALL",
                    Theme.SurfaceAlt,
                    0,
                    15,
                    115,
                    34);

            all.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            all.Click +=
                delegate
                {
                    _search.Clear();
                    LoadCustomers();
                };

            searchCard.Controls.Add(all);

            searchCard.Resize +=
                delegate
                {
                    all.Left =
                        searchCard.ClientSize.Width -
                        all.Width -
                        18;

                    search.Left =
                        all.Left -
                        search.Width -
                        10;

                    _search.Width =
                        Math.Max(
                            220,
                            search.Left - 36);
                };

            // GRID
            GamePanel list =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            list.Dock =
                DockStyle.Fill;

            list.Padding =
                new Padding(18);

            root.Controls.Add(
                list,
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

            list.Controls.Add(_grid);
        }

        private void LoadCustomers()
        {
            try
            {
                _grid.DataSource =
                    _users.GetCustomersForSupport(
                        _search == null
                            ? string.Empty
                            : _search.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Customer Support",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}