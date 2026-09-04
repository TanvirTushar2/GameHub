using System;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    public partial class OrdersForm : Form
    {
        private readonly OrderRepository _orders =
            new OrderRepository();

        private TextBox _search;
        private DataGridView _grid;
        private DataGridView _details;
        private ComboBox _status;
        private Label _selected;

        private int _selectedOrderId;

        public OrdersForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();

            AppStyle.Apply(this, true);

            MinimumSize =
                new Size(1050, 700);

            WindowState =
                FormWindowState.Maximized;

            LoadOrders();
        }

        private void BuildUi()
        {
            Text =
                "GameHub - Order Management";

            BackColor =
                Theme.Background;

            TableLayoutPanel root =
                CreateRoot();

            Controls.Add(root);

            // =====================================================
            // HEADER
            // =====================================================

            Panel header =
                CreateHeader(
                    "Order Management",
                    "Review customer orders, inspect items and update order status.");

            root.Controls.Add(
                header,
                0,
                0);

            // =====================================================
            // SEARCH
            // =====================================================

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
                AnchorStyles.Left |
                AnchorStyles.Top |
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
                    LoadOrders();
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
                    LoadOrders();
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
                            250,
                            search.Left - 36);
                };

            // =====================================================
            // BODY
            // =====================================================

            TableLayoutPanel body =
                new TableLayoutPanel();

            body.Dock =
                DockStyle.Fill;

            body.ColumnCount =
                2;

            body.RowCount =
                1;

            body.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    65F));

            body.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    35F));

            body.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            root.Controls.Add(
                body,
                0,
                2);

            // =====================================================
            // ORDER LIST
            // =====================================================

            GamePanel list =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            list.Dock =
                DockStyle.Fill;

            list.Margin =
                new Padding(
                    0,
                    0,
                    8,
                    0);

            list.Padding =
                new Padding(
                    18,
                    16,
                    18,
                    18);

            body.Controls.Add(
                list,
                0,
                0);

            Label ordersTitle =
                Ui.FixedLabel(
                    "Orders",
                    18,
                    15,
                    260,
                    34,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Text);

            list.Controls.Add(ordersTitle);

            _grid =
                Ui.Grid(
                    18,
                    58,
                    100,
                    100);

            _grid.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            _grid.SelectionChanged +=
                Grid_SelectionChanged;

            list.Controls.Add(_grid);

            list.Resize +=
                delegate
                {
                    _grid.SetBounds(
                        18,
                        58,
                        Math.Max(
                            100,
                            list.ClientSize.Width - 36),
                        Math.Max(
                            100,
                            list.ClientSize.Height - 76));
                };

            // =====================================================
            // PROCESS PANEL
            // =====================================================

            GamePanel action =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            action.Dock =
                DockStyle.Fill;

            action.Margin =
                new Padding(
                    8,
                    0,
                    0,
                    0);

            body.Controls.Add(
                action,
                1,
                0);

            action.Controls.Add(
                Ui.Pill(
                    "PROCESS",
                    18,
                    18,
                    84,
                    Theme.Cream,
                    Theme.Background2));

            action.Controls.Add(
                Ui.FixedLabel(
                    "Selected Order",
                    18,
                    57,
                    280,
                    32,
                    new Font(
                        "Segoe UI",
                        13F,
                        FontStyle.Bold),
                    Theme.Text));

            _selected =
                Ui.FixedLabel(
                    "No order selected",
                    18,
                    91,
                    400,
                    26,
                    Theme.BodyFont,
                    Theme.Muted);

            _selected.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            action.Controls.Add(_selected);

            action.Controls.Add(
                Ui.FixedLabel(
                    "Status",
                    18,
                    131,
                    150,
                    24,
                    Theme.BodyFont,
                    Theme.Text));

            _status =
                new ComboBox();

            _status.SetBounds(
                18,
                158,
                320,
                31);

            _status.DropDownStyle =
                ComboBoxStyle.DropDownList;

            _status.Items.AddRange(
                new object[]
                {
                    "Pending",
                    "Paid",
                    "Completed",
                    "Cancelled"
                });

            _status.SelectedIndex =
                0;

            _status.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            action.Controls.Add(_status);

            Button update =
                Ui.Btn(
                    "UPDATE STATUS",
                    Theme.Accent,
                    18,
                    207,
                    320,
                    40);

            update.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            update.Click +=
                Update_Click;

            action.Controls.Add(update);

            action.Controls.Add(
                Ui.FixedLabel(
                    "Order Items",
                    18,
                    278,
                    250,
                    30,
                    new Font(
                        "Segoe UI",
                        12F,
                        FontStyle.Bold),
                    Theme.Text));

            _details =
                Ui.Grid(
                    18,
                    314,
                    320,
                    150);

            _details.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            action.Controls.Add(_details);

            action.Resize +=
                delegate
                {
                    int width =
                        Math.Max(
                            160,
                            action.ClientSize.Width - 36);

                    _selected.Width =
                        width;

                    _status.Width =
                        width;

                    update.Width =
                        width;

                    _details.SetBounds(
                        18,
                        314,
                        width,
                        Math.Max(
                            120,
                            action.ClientSize.Height - 334));
                };
        }

        private TableLayoutPanel CreateRoot()
        {
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

            return root;
        }

        private Panel CreateHeader(
            string title,
            string subtitle)
        {
            Panel header =
                new Panel();

            header.Dock =
                DockStyle.Fill;

            Label lblTitle =
                Ui.FixedLabel(
                    title,
                    0,
                    0,
                    900,
                    45,
                    new Font(
                        "Segoe UI",
                        20F,
                        FontStyle.Bold),
                    Theme.Text);

            header.Controls.Add(lblTitle);

            Label lblSubtitle =
                Ui.FixedLabel(
                    subtitle,
                    2,
                    48,
                    1000,
                    30,
                    Theme.BodyFont,
                    Theme.Muted);

            header.Controls.Add(lblSubtitle);

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

                    lblTitle.Width =
                        Math.Max(
                            300,
                            back.Left - 20);

                    lblSubtitle.Width =
                        Math.Max(
                            300,
                            header.ClientSize.Width - 10);
                };

            return header;
        }

        private void LoadOrders()
        {
            try
            {
                _grid.DataSource =
                    _orders.GetOrders(
                        _search == null
                            ? string.Empty
                            : _search.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Orders",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Grid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (_grid.CurrentRow == null ||
                _grid.CurrentRow.IsNewRow)
                return;

            try
            {
                _selectedOrderId =
                    Convert.ToInt32(
                        _grid.CurrentRow
                            .Cells["Order ID"]
                            .Value);

                _selected.Text =
                    "Order #" +
                    _selectedOrderId +
                    "  •  " +
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Customer"]
                            .Value);

                _status.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Status"]
                            .Value);

                _details.DataSource =
                    _orders.GetOrderDetails(
                        _selectedOrderId);
            }
            catch
            {
                _selectedOrderId =
                    0;
            }
        }

        private void Update_Click(
            object sender,
            EventArgs e)
        {
            if (_selectedOrderId == 0)
            {
                MessageBox.Show(
                    "Select an order first.");

                return;
            }

            try
            {
                int? staffId =
                    null;

                if (
                    Program.CurrentUser != null &&
                    Program.CurrentUser.Role == "Staff")
                {
                    staffId =
                        Program.CurrentUser.UserID;
                }

                _orders.UpdateOrderStatus(
                    _selectedOrderId,
                    _status.Text,
                    staffId);

                LoadOrders();

                MessageBox.Show(
                    "Order status updated.",
                    "GameHub",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Orders",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
