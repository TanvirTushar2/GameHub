using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    
    public partial class ManageGamesForm : Form
    {
        private readonly GameRepository _games =
            new GameRepository();

        private int _selectedId;

        private TextBox _txtSearch;
        private TextBox _txtTitle;
        private TextBox _txtDescription;
        private TextBox _txtPrice;
        private TextBox _txtStock;

        private ComboBox _cboGenre;
        private ComboBox _cboPublisher;

        private DateTimePicker _releaseDate;

        private DataGridView _grid;

        public ManageGamesForm()
        {
            InitializeComponent();

            BuildUi();

            // Resizable + DPI aware.
            AppStyle.Apply(this, true);

            MinimumSize =
                new Size(
                    1050,
                    700);

            // Full screen.
            WindowState =
                FormWindowState.Maximized;

            LoadGenresPublishers();
            LoadGrid();
        }

        private void BuildUi()
        {
            SuspendLayout();

            Controls.Clear();

            Text =
                "GameHub - Manage Games";

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

            root.BackColor =
                Theme.Background;

            root.ColumnCount = 1;
            root.RowCount = 3;

            root.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    82F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    76F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            Controls.Add(
                root);

        
            Panel header =
                new Panel();

            header.Dock =
                DockStyle.Fill;

            header.BackColor =
                Theme.Background;

            root.Controls.Add(
                header,
                0,
                0);

            Label title =
                Ui.FixedLabel(
                    "Game Library & Inventory",
                    0,
                    0,
                    650,
                    40,
                    new Font(
                        "Segoe UI",
                        20F,
                        FontStyle.Bold),
                    Theme.Text);

            header.Controls.Add(
                title);

            Label subtitle =
                Ui.FixedLabel(
                    "Add games, edit catalog details and maintain inventory levels.",
                    2,
                    43,
                    760,
                    27,
                    Theme.BodyFont,
                    Theme.Muted);

            header.Controls.Add(
                subtitle);

            Button back =
                Ui.GhostBtn(
                    "←  BACK",
                    0,
                    5,
                    112,
                    38);

            back.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            back.Click += delegate
            {
                Close();
            };

            header.Controls.Add(
                back);

            header.Resize += delegate
            {
                back.Left =
                    header.ClientSize.Width -
                    back.Width;

                title.Width =
                    Math.Max(
                        400,
                        back.Left - 20);
            };

         
            GamePanel searchCard =
                Ui.Card(
                    0,
                    0,
                    100,
                    60);

            searchCard.Dock =
                DockStyle.Fill;

            searchCard.Margin =
                new Padding(
                    0,
                    0,
                    0,
                    12);

            searchCard.Padding =
                new Padding(
                    16,
                    12,
                    16,
                    12);

            root.Controls.Add(
                searchCard,
                0,
                1);

            TableLayoutPanel searchLayout =
                new TableLayoutPanel();

            searchLayout.Dock =
                DockStyle.Fill;

            searchLayout.ColumnCount = 4;
            searchLayout.RowCount = 1;

            searchLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    76F));

            searchLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            searchLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    110F));

            searchLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    130F));

            searchLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            searchCard.Controls.Add(
                searchLayout);

            Label searchLabel =
                Ui.FixedLabel(
                    "SEARCH",
                    0,
                    0,
                    72,
                    34,
                    Theme.SmallBold,
                    Theme.Muted);

            searchLabel.TextAlign =
                ContentAlignment.MiddleLeft;

            searchLayout.Controls.Add(
                searchLabel,
                0,
                0);

            _txtSearch =
                Ui.TextInput(
                    0,
                    0,
                    100,
                    32);

            _txtSearch.Dock =
                DockStyle.Fill;

            _txtSearch.Margin =
                new Padding(
                    0,
                    1,
                    12,
                    1);

            _txtSearch.KeyDown += delegate (
                object sender,
                KeyEventArgs e)
            {
                if (e.KeyCode ==
                    Keys.Enter)
                {
                    LoadGrid();

                    e.Handled =
                        true;

                    e.SuppressKeyPress =
                        true;
                }
            };

            searchLayout.Controls.Add(
                _txtSearch,
                1,
                0);

            Button btnSearch =
                Ui.Btn(
                    "SEARCH",
                    Theme.Accent,
                    0,
                    0,
                    100,
                    34);

            btnSearch.Dock =
                DockStyle.Fill;

            btnSearch.Margin =
                new Padding(
                    0,
                    0,
                    10,
                    0);

            btnSearch.Click += delegate
            {
                LoadGrid();
            };

            searchLayout.Controls.Add(
                btnSearch,
                2,
                0);

            Button btnAll =
                Ui.Btn(
                    "SHOW ALL",
                    Theme.SurfaceAlt,
                    0,
                    0,
                    120,
                    34);

            btnAll.Dock =
                DockStyle.Fill;

            btnAll.Click += delegate
            {
                _txtSearch.Clear();
                LoadGrid();
            };

            searchLayout.Controls.Add(
                btnAll,
                3,
                0);

         
            TableLayoutPanel body =
                new TableLayoutPanel();

            body.Dock =
                DockStyle.Fill;

            body.ColumnCount = 2;
            body.RowCount = 1;

            body.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    36F));

            body.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    64F));

            body.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            root.Controls.Add(
                body,
                0,
                2);

        
            GamePanel editor =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            editor.Dock =
                DockStyle.Fill;

            editor.Margin =
                new Padding(
                    0,
                    0,
                    10,
                    0);

            editor.Padding =
                new Padding(
                    20);

            body.Controls.Add(
                editor,
                0,
                0);

            TableLayoutPanel editorLayout =
                new TableLayoutPanel();

            editorLayout.Dock =
                DockStyle.Fill;

            editorLayout.ColumnCount = 1;
            editorLayout.RowCount = 9;

            editorLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    70F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    68F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    120F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    70F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    70F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    70F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    70F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    62F));

            editorLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            editor.Controls.Add(
                editorLayout);

          
            Panel editorHeader =
                new Panel();

            editorHeader.Dock =
                DockStyle.Fill;

            editorHeader.BackColor =
                Color.Transparent;

            editorLayout.Controls.Add(
                editorHeader,
                0,
                0);

            Label editorPill =
                Ui.Pill(
                    "EDITOR",
                    0,
                    0,
                    74,
                    Theme.Cream,
                    Theme.Background2);

            editorHeader.Controls.Add(
                editorPill);

            Label editorTitle =
                Ui.FixedLabel(
                    "Game Details",
                    0,
                    31,
                    250,
                    34,
                    new Font(
                        "Segoe UI",
                        13F,
                        FontStyle.Bold),
                    Theme.Text);

            editorHeader.Controls.Add(
                editorTitle);

          
            editorLayout.Controls.Add(
                BuildTextField(
                    "Title",
                    out _txtTitle),
                0,
                1);


            Panel descriptionPanel =
                new Panel();

            descriptionPanel.Dock =
                DockStyle.Fill;

            descriptionPanel.BackColor =
                Color.Transparent;

            descriptionPanel.Margin =
                new Padding(
                    0,
                    0,
                    0,
                    8);

            descriptionPanel.Controls.Add(
                Ui.FixedLabel(
                    "Description",
                    0,
                    0,
                    200,
                    22,
                    Theme.BodyFont,
                    Theme.Text));

            _txtDescription =
                Ui.MultiInput(
                    0,
                    25,
                    100,
                    82);

            _txtDescription.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            _txtDescription.Width =
                Math.Max(
                    200,
                    descriptionPanel.ClientSize.Width);

            _txtDescription.Height =
                80;

            _txtDescription.MaxLength =
                1500;

            descriptionPanel.Controls.Add(
                _txtDescription);

            descriptionPanel.Resize += delegate
            {
                _txtDescription.Width =
                    Math.Max(
                        150,
                        descriptionPanel.ClientSize.Width);
            };

            editorLayout.Controls.Add(
                descriptionPanel,
                0,
                2);

          
            Panel genrePanel =
                BuildComboField(
                    "Genre",
                    out _cboGenre);

            editorLayout.Controls.Add(
                genrePanel,
                0,
                3);

          
            Panel publisherPanel =
                BuildComboField(
                    "Publisher",
                    out _cboPublisher);

            editorLayout.Controls.Add(
                publisherPanel,
                0,
                4);

         
            TableLayoutPanel priceStock =
                new TableLayoutPanel();

            priceStock.Dock =
                DockStyle.Fill;

            priceStock.ColumnCount =
                2;

            priceStock.RowCount =
                1;

            priceStock.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50F));

            priceStock.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50F));

            priceStock.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            priceStock.Margin =
                new Padding(
                    0,
                    0,
                    0,
                    6);

            Panel pricePanel =
                BuildTextField(
                    "Price (BDT)",
                    out _txtPrice);

            pricePanel.Margin =
                new Padding(
                    0,
                    0,
                    8,
                    0);

            priceStock.Controls.Add(
                pricePanel,
                0,
                0);

            Panel stockPanel =
                BuildTextField(
                    "Stock",
                    out _txtStock);

            stockPanel.Margin =
                new Padding(
                    8,
                    0,
                    0,
                    0);

            priceStock.Controls.Add(
                stockPanel,
                1,
                0);

            editorLayout.Controls.Add(
                priceStock,
                0,
                5);

           
            Panel releasePanel =
                new Panel();

            releasePanel.Dock =
                DockStyle.Fill;

            releasePanel.BackColor =
                Color.Transparent;

            releasePanel.Controls.Add(
                Ui.FixedLabel(
                    "Release Date",
                    0,
                    0,
                    220,
                    22,
                    Theme.BodyFont,
                    Theme.Text));

            _releaseDate =
                new DateTimePicker();

            _releaseDate.Format =
                DateTimePickerFormat.Short;

            _releaseDate.ShowCheckBox =
                true;

            _releaseDate.SetBounds(
                0,
                26,
                220,
                30);

            releasePanel.Controls.Add(
                _releaseDate);

            editorLayout.Controls.Add(
                releasePanel,
                0,
                6);

            // ---------------------------------------------------------
            // BUTTONS
            // ---------------------------------------------------------

            TableLayoutPanel actions =
                new TableLayoutPanel();

            actions.Dock =
                DockStyle.Fill;

            actions.ColumnCount =
                4;

            actions.RowCount =
                1;

            actions.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    20F));

            actions.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    23F));

            actions.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    35F));

            actions.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    22F));

            actions.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            editorLayout.Controls.Add(
                actions,
                0,
                7);

            Button btnAdd =
                Ui.Btn(
                    "ADD",
                    Theme.Green,
                    0,
                    0,
                    60,
                    38);

            btnAdd.Dock =
                DockStyle.Fill;

            btnAdd.Margin =
                new Padding(
                    0,
                    6,
                    5,
                    6);

            btnAdd.Click +=
                BtnAdd_Click;

            actions.Controls.Add(
                btnAdd,
                0,
                0);

            Button btnUpdate =
                Ui.Btn(
                    "UPDATE",
                    Theme.Accent,
                    0,
                    0,
                    70,
                    38);

            btnUpdate.Dock =
                DockStyle.Fill;

            btnUpdate.Margin =
                new Padding(
                    5,
                    6,
                    5,
                    6);

            btnUpdate.Click +=
                BtnUpdate_Click;

            actions.Controls.Add(
                btnUpdate,
                1,
                0);

            Button btnDeactivate =
                Ui.Btn(
                    "DEACTIVATE",
                    Theme.Red,
                    0,
                    0,
                    100,
                    38);

            btnDeactivate.Dock =
                DockStyle.Fill;

            btnDeactivate.Margin =
                new Padding(
                    5,
                    6,
                    5,
                    6);

            btnDeactivate.Click +=
                BtnDelete_Click;

            actions.Controls.Add(
                btnDeactivate,
                2,
                0);

            Button btnClear =
                Ui.Btn(
                    "CLEAR",
                    Theme.SurfaceAlt,
                    0,
                    0,
                    65,
                    38);

            btnClear.Dock =
                DockStyle.Fill;

            btnClear.Margin =
                new Padding(
                    5,
                    6,
                    0,
                    6);

            btnClear.Click += delegate
            {
                ClearForm();
            };

            actions.Controls.Add(
                btnClear,
                3,
                0);

          
            GamePanel catalog =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            catalog.Dock =
                DockStyle.Fill;

            catalog.Margin =
                new Padding(
                    10,
                    0,
                    0,
                    0);

            catalog.Padding =
                new Padding(
                    18);

            body.Controls.Add(
                catalog,
                1,
                0);

            TableLayoutPanel catalogLayout =
                new TableLayoutPanel();

            catalogLayout.Dock =
                DockStyle.Fill;

            catalogLayout.ColumnCount =
                1;

            catalogLayout.RowCount =
                2;

            catalogLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            catalogLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    65F));

            catalogLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            catalog.Controls.Add(
                catalogLayout);

            Panel catalogHeader =
                new Panel();

            catalogHeader.Dock =
                DockStyle.Fill;

            catalogHeader.BackColor =
                Color.Transparent;

            catalogLayout.Controls.Add(
                catalogHeader,
                0,
                0);

            catalogHeader.Controls.Add(
                Ui.FixedLabel(
                    "Catalog",
                    0,
                    0,
                    300,
                    32,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Text));

            catalogHeader.Controls.Add(
                Ui.FixedLabel(
                    "Active games and inventory details",
                    0,
                    32,
                    400,
                    23,
                    Theme.SmallFont,
                    Theme.Muted));

            _grid =
                Ui.Grid(
                    0,
                    0,
                    100,
                    100);

            _grid.Dock =
                DockStyle.Fill;

            _grid.SelectionChanged +=
                Grid_SelectionChanged;

            catalogLayout.Controls.Add(
                _grid,
                0,
                1);

            ResumeLayout();
        }

      
        private Panel BuildTextField(
            string caption,
            out TextBox textBox)
        {
            Panel panel =
                new Panel();

            panel.Dock =
                DockStyle.Fill;

            panel.BackColor =
                Color.Transparent;

            panel.Controls.Add(
                Ui.FixedLabel(
                    caption,
                    0,
                    0,
                    200,
                    22,
                    Theme.BodyFont,
                    Theme.Text));

            textBox =
                Ui.TextInput(
                    0,
                    26,
                    100,
                    30);

            textBox.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            panel.Controls.Add(
                textBox);

            TextBox field =
                textBox;

            panel.Resize += delegate
            {
                field.Width =
                    Math.Max(
                        120,
                        panel.ClientSize.Width);
            };

            return panel;
        }

       
        private Panel BuildComboField(
            string caption,
            out ComboBox combo)
        {
            Panel panel =
                new Panel();

            panel.Dock =
                DockStyle.Fill;

            panel.BackColor =
                Color.Transparent;

            panel.Controls.Add(
                Ui.FixedLabel(
                    caption,
                    0,
                    0,
                    200,
                    22,
                    Theme.BodyFont,
                    Theme.Text));

            combo =
                new ComboBox();

            combo.DropDownStyle =
                ComboBoxStyle.DropDownList;

            combo.SetBounds(
                0,
                26,
                220,
                30);

            combo.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            panel.Controls.Add(
                combo);

            ComboBox field =
                combo;

            panel.Resize += delegate
            {
                field.Width =
                    Math.Max(
                        120,
                        panel.ClientSize.Width);
            };

            return panel;
        }

      
        private void LoadGenresPublishers()
        {
            try
            {
                _cboGenre.DisplayMember =
                    "GenreName";

                _cboGenre.ValueMember =
                    "GenreID";

                _cboGenre.DataSource =
                    _games.GetGenres();

                _cboPublisher.DisplayMember =
                    "PublisherName";

                _cboPublisher.ValueMember =
                    "PublisherID";

                _cboPublisher.DataSource =
                    _games.GetPublishers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Game Library",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

     
        private void LoadGrid()
        {
            try
            {
                string search =
                    _txtSearch == null
                        ? string.Empty
                        : _txtSearch.Text.Trim();

                _grid.DataSource =
                    _games.GetGamesTable(
                        search);

                if (_grid.Columns.Contains(
                    "Description"))
                {
                    _grid
                        .Columns["Description"]
                        .Visible = false;
                }

                if (_grid.Columns.Contains("ID"))
                {
                    _grid.Columns["ID"].FillWeight =
                        35;
                }

                if (_grid.Columns.Contains("Title"))
                {
                    _grid.Columns["Title"].FillWeight =
                        120;
                }

                if (_grid.Columns.Contains("Genre"))
                {
                    _grid.Columns["Genre"].FillWeight =
                        90;
                }

                if (_grid.Columns.Contains("Publisher"))
                {
                    _grid.Columns["Publisher"].FillWeight =
                        110;
                }

                if (_grid.Columns.Contains("Price"))
                {
                    _grid.Columns["Price"].FillWeight =
                        70;

                    _grid.Columns["Price"]
                        .DefaultCellStyle.Format =
                        "N2";
                }

                if (_grid.Columns.Contains("Stock"))
                {
                    _grid.Columns["Stock"].FillWeight =
                        55;
                }

                if (_grid.Columns.Contains(
                    "Release Date"))
                {
                    _grid
                        .Columns["Release Date"]
                        .FillWeight = 90;

                    _grid
                        .Columns["Release Date"]
                        .DefaultCellStyle.Format =
                        "dd MMM yyyy";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Game Library",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

       
        private void Grid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (_grid.CurrentRow == null ||
                _grid.CurrentRow.IsNewRow ||
                !_grid.CurrentRow.Selected)
            {
                return;
            }

            try
            {
                _selectedId =
                    Convert.ToInt32(
                        _grid.CurrentRow
                            .Cells["ID"]
                            .Value);

                _txtTitle.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Title"]
                            .Value);

                object description =
                    _grid.CurrentRow
                        .Cells["Description"]
                        .Value;

                _txtDescription.Text =
                    description == null ||
                    description == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(
                            description);

                _cboGenre.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Genre"]
                            .Value);

                _cboPublisher.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Publisher"]
                            .Value);

                _txtPrice.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Price"]
                            .Value);

                _txtStock.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Stock"]
                            .Value);

                object releaseDate =
                    _grid.CurrentRow
                        .Cells["Release Date"]
                        .Value;

                if (releaseDate == null ||
                    releaseDate == DBNull.Value)
                {
                    _releaseDate.Checked =
                        false;
                }
                else
                {
                    _releaseDate.Value =
                        Convert.ToDateTime(
                            releaseDate);

                    _releaseDate.Checked =
                        true;
                }
            }
            catch
            {
            }
        }

     
        private bool ValidateFields(
            out decimal price,
            out int stock)
        {
            price = 0m;
            stock = 0;

            if (string.IsNullOrWhiteSpace(
                _txtTitle.Text))
            {
                MessageBox.Show(
                    "Title is required.");

                return false;
            }

            if (_cboGenre.SelectedValue == null ||
                _cboPublisher.SelectedValue == null)
            {
                MessageBox.Show(
                    "Choose a genre and publisher.");

                return false;
            }

            if (!decimal.TryParse(
                    _txtPrice.Text,
                    out price) ||
                price < 0)
            {
                MessageBox.Show(
                    "Enter a valid non-negative price.");

                return false;
            }

            if (!int.TryParse(
                    _txtStock.Text,
                    out stock) ||
                stock < 0)
            {
                MessageBox.Show(
                    "Enter a valid non-negative stock quantity.");

                return false;
            }

            return true;
        }


        private void BtnAdd_Click(
            object sender,
            EventArgs e)
        {
            decimal price;
            int stock;

            if (!ValidateFields(
                out price,
                out stock))
            {
                return;
            }

            try
            {
                _games.AddGame(
                    _txtTitle.Text.Trim(),
                    _txtDescription.Text.Trim(),
                    Convert.ToInt32(
                        _cboGenre.SelectedValue),
                    Convert.ToInt32(
                        _cboPublisher.SelectedValue),
                    price,
                    stock,
                    _releaseDate.Checked
                        ? (DateTime?)
                            _releaseDate.Value.Date
                        : null);

                LoadGrid();
                ClearForm();

                MessageBox.Show(
                    "Game added successfully.",
                    "GameHub",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Game Library",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

       
        private void BtnUpdate_Click(
            object sender,
            EventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show(
                    "Select a game first.");

                return;
            }

            decimal price;
            int stock;

            if (!ValidateFields(
                out price,
                out stock))
            {
                return;
            }

            try
            {
                _games.UpdateGame(
                    _selectedId,
                    _txtTitle.Text.Trim(),
                    _txtDescription.Text.Trim(),
                    Convert.ToInt32(
                        _cboGenre.SelectedValue),
                    Convert.ToInt32(
                        _cboPublisher.SelectedValue),
                    price,
                    stock,
                    _releaseDate.Checked
                        ? (DateTime?)
                            _releaseDate.Value.Date
                        : null);

                LoadGrid();

                MessageBox.Show(
                    "Game updated successfully.",
                    "GameHub",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Game Library",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

       
        private void BtnDelete_Click(
            object sender,
            EventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show(
                    "Select a game first.");

                return;
            }

            DialogResult answer =
                MessageBox.Show(
                    "Deactivate this game?\n\n" +
                    "It will disappear from the customer store.",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (answer !=
                DialogResult.Yes)
            {
                return;
            }

            try
            {
                _games.DeleteGame(
                    _selectedId);

                LoadGrid();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Game Library",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void ClearForm()
        {
            _selectedId = 0;

            _txtTitle.Clear();
            _txtDescription.Clear();
            _txtPrice.Clear();
            _txtStock.Clear();

            if (_cboGenre.Items.Count > 0)
            {
                _cboGenre.SelectedIndex = 0;
            }

            if (_cboPublisher.Items.Count > 0)
            {
                _cboPublisher.SelectedIndex = 0;
            }

            _releaseDate.Checked =
                false;

            _grid.ClearSelection();

            _txtTitle.Focus();
        }
    }
}