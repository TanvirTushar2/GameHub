using System;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    public partial class PaymentsForm : Form
    {
        private readonly OrderRepository _orders =
            new OrderRepository();

        private TextBox _search;
        private DataGridView _grid;

        public PaymentsForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();

            AppStyle.Apply(this, true);

            MinimumSize =
                new Size(1000, 650);

            WindowState =
                FormWindowState.Maximized;

            LoadPayments();
        }

        private void BuildUi()
        {
            Text =
                "GameHub - Payments";

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
                3;

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
                    "Payment Management",
                    0,
                    0,
                    750,
                    45,
                    new Font(
                        "Segoe UI",
                        20F,
                        FontStyle.Bold),
                    Theme.Text);

            header.Controls.Add(title);

            Label subtitle =
                Ui.FixedLabel(
                    "Review customer payments, transaction methods and payment status.",
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
                1);

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
                    LoadPayments();
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
                    LoadPayments();
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
            GamePanel card =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            card.Dock =
                DockStyle.Fill;

            card.Padding =
                new Padding(
                    18,
                    58,
                    18,
                    18);

            root.Controls.Add(
                card,
                0,
                2);

            card.Controls.Add(
                Ui.Pill(
                    "PAYMENTS",
                    18,
                    16,
                    90,
                    Theme.Cream,
                    Theme.Background2));

            _grid =
                Ui.Grid(
                    0,
                    0,
                    100,
                    100);

            _grid.Dock =
                DockStyle.Fill;

            card.Controls.Add(_grid);
        }

        private void LoadPayments()
        {
            try
            {
                _grid.DataSource =
                    _orders.GetPayments(
                        _search == null
                            ? string.Empty
                            : _search.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Payments",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}