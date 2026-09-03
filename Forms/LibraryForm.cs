using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    /// <summary>Purchased games, activation keys and customer reviews.</summary>
    public partial class LibraryForm : Form
    {
        private readonly CustomerRepository _customer = new CustomerRepository();
        private DataGridView _libraryGrid;
        private DataGridView _reviewsGrid;
        private ComboBox _rating;
        private TextBox _comment;
        private Label _selectedGame;
        private int _selectedGameId;

        public LibraryForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();
            AppStyle.Apply(this);
            LoadLibrary();
        }

        private void BuildUi()
        {
            ClientSize = new Size(1080, 650);
            Text = "GameHub - My Library";

            Controls.Add(Ui.Lbl("My Library", 28, 22,
                new Font("Segoe UI", 18F, FontStyle.Bold), Theme.Text));
            Controls.Add(Ui.Lbl("Purchased games, activation keys and your reviews.",
                30, 54, Theme.SmallFont, Theme.Muted));
            Controls.Add(Ui.Back(this));

            Panel library = Ui.Card(28, 94, 650, 528);
            Controls.Add(library);
            library.Controls.Add(Ui.Pill("LIBRARY", 18, 16, 76, Theme.Cream, Theme.Background2));
            library.Controls.Add(Ui.Lbl("Owned Games", 18, 54,
                new Font("Segoe UI", 13F, FontStyle.Bold), Theme.Text));
            library.Controls.Add(Ui.Lbl("Select a game to write or update your review.",
                18, 80, Theme.SmallFont, Theme.Muted));

            _libraryGrid = Ui.Grid(18, 112, 614, 396);
            _libraryGrid.SelectionChanged += LibraryGrid_SelectionChanged;
            library.Controls.Add(_libraryGrid);

            Panel review = Ui.Card(696, 94, 356, 528);
            Controls.Add(review);
            review.Controls.Add(Ui.Pill("REVIEWS", 18, 16, 76, Theme.Accent, Color.White));
            review.Controls.Add(Ui.Lbl("Review Selected Game", 18, 54,
                new Font("Segoe UI", 13F, FontStyle.Bold), Theme.Text));

            _selectedGame = Ui.Lbl("Select a game from your library", 18, 84,
                Theme.BodyFont, Theme.Muted);
            review.Controls.Add(_selectedGame);

            review.Controls.Add(Ui.Lbl("Rating", 18, 120, Theme.BodyFont, Theme.Text));
            _rating = new ComboBox();
            _rating.SetBounds(18, 144, 110, 28);
            _rating.DropDownStyle = ComboBoxStyle.DropDownList;
            _rating.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            _rating.SelectedIndex = 4;
            review.Controls.Add(_rating);

            review.Controls.Add(Ui.Lbl("Comment", 18, 184, Theme.BodyFont, Theme.Text));
            _comment = Ui.MultiInput(18, 208, 320, 82);
            _comment.MaxLength = 500;
            review.Controls.Add(_comment);

            Button save = Ui.Btn("SAVE REVIEW", Theme.Accent, 18, 302, 320, 36);
            save.Click += SaveReview_Click;
            review.Controls.Add(save);

            review.Controls.Add(Ui.Lbl("Community Reviews", 18, 360,
                new Font("Segoe UI", 11F, FontStyle.Bold), Theme.Text));
            _reviewsGrid = Ui.Grid(18, 390, 320, 118);
            review.Controls.Add(_reviewsGrid);
        }

        private void LoadLibrary()
        {
            if (Program.CurrentUser == null) return;
            try
            {
                _libraryGrid.DataSource = _customer.GetLibrary(Program.CurrentUser.UserID);
                if (_libraryGrid.Columns.Contains("Game ID"))
                    _libraryGrid.Columns["Game ID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Library", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LibraryGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (_libraryGrid.CurrentRow == null || _libraryGrid.CurrentRow.IsNewRow) return;

            try
            {
                _selectedGameId = Convert.ToInt32(_libraryGrid.CurrentRow.Cells["Game ID"].Value);
                string title = Convert.ToString(_libraryGrid.CurrentRow.Cells["Title"].Value);
                _selectedGame.Text = title;

                DataTable own = _customer.GetUserReview(Program.CurrentUser.UserID, _selectedGameId);
                if (own.Rows.Count > 0)
                {
                    int rating = Convert.ToInt32(own.Rows[0]["Rating"]);
                    _rating.SelectedItem = rating.ToString();
                    _comment.Text = own.Rows[0]["Comment"] == DBNull.Value
                        ? string.Empty : Convert.ToString(own.Rows[0]["Comment"]);
                }
                else
                {
                    _rating.SelectedIndex = 4;
                    _comment.Clear();
                }

                _reviewsGrid.DataSource = _customer.GetReviewsForGame(_selectedGameId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Reviews", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveReview_Click(object sender, EventArgs e)
        {
            if (_selectedGameId == 0)
            {
                MessageBox.Show("Select a game from your library first.");
                return;
            }

            try
            {
                int rating = Convert.ToInt32(_rating.Text);
                _customer.SaveReview(Program.CurrentUser.UserID, _selectedGameId, rating, _comment.Text.Trim());
                _reviewsGrid.DataSource = _customer.GetReviewsForGame(_selectedGameId);
                MessageBox.Show("Your review was saved.", "GameHub",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Review", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
