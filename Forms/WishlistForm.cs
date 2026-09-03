using System;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    public partial class WishlistForm : Form
    {
        private readonly CustomerRepository _customer = new CustomerRepository();
        private DataGridView _grid;
        private int _selectedGameId;

        public WishlistForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();
            AppStyle.Apply(this);
            LoadWishlist();
        }

        private void BuildUi()
        {
            ClientSize = new Size(980, 600);
            Text = "GameHub - Wishlist";

            Controls.Add(Ui.Lbl("My Wishlist", 28, 22,
                new Font("Segoe UI", 18F, FontStyle.Bold), Theme.Text));
            Controls.Add(Ui.Lbl("Keep track of games you want to purchase later.",
                30, 54, Theme.SmallFont, Theme.Muted));
            Controls.Add(Ui.Back(this));

            Panel card = Ui.Card(28, 94, 924, 478);
            Controls.Add(card);
            card.Controls.Add(Ui.Pill("WISHLIST", 18, 16, 86, Theme.Cream, Theme.Background2));
            card.Controls.Add(Ui.Lbl("Saved Games", 18, 54,
                new Font("Segoe UI", 13F, FontStyle.Bold), Theme.Text));

            Button remove = Ui.Btn("Remove Selected", Theme.Red, 584, 42, 140, 34);
            remove.Click += Remove_Click;
            card.Controls.Add(remove);

            Button store = Ui.Btn("Open Store", Theme.Accent, 738, 42, 168, 34);
            store.Click += (s, e) =>
            {
                using (StoreForm sf = new StoreForm()) sf.ShowDialog(this);
                LoadWishlist();
            };
            card.Controls.Add(store);

            _grid = Ui.Grid(18, 96, 888, 360);
            _grid.SelectionChanged += (s, e) =>
            {
                if (_grid.CurrentRow == null) return;
                try { _selectedGameId = Convert.ToInt32(_grid.CurrentRow.Cells["Game ID"].Value); }
                catch { _selectedGameId = 0; }
            };
            card.Controls.Add(_grid);
        }

        private void LoadWishlist()
        {
            if (Program.CurrentUser == null) return;
            try
            {
                _grid.DataSource = _customer.GetWishlist(Program.CurrentUser.UserID);
                if (_grid.Columns.Contains("Game ID")) _grid.Columns["Game ID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Wishlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (_selectedGameId == 0)
            {
                MessageBox.Show("Select a wishlist game first.");
                return;
            }

            try
            {
                _customer.RemoveFromWishlist(Program.CurrentUser.UserID, _selectedGameId);
                _selectedGameId = 0;
                LoadWishlist();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Wishlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
