using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Forms.Controls;
using GameHub.Models;
using GameHub.Services;

namespace GameHub.Forms.Views
{
    internal class LibraryView : LauncherPage
    {
        private readonly CustomerRepository _customer = new CustomerRepository();
        private readonly TextBox _search;
        private readonly FlowLayoutPanel _flow;
        private readonly Label _count;
        private DataTable _data;
        private bool _loading;

        public LibraryView()
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Margin = new Padding(0);
            layout.Padding = new Padding(0);
            layout.ColumnCount = 1;
            layout.RowCount = 3;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.BackColor = Theme.Background;
            Controls.Add(layout);

            Panel header = new Panel();
            header.Dock = DockStyle.Fill;
            header.Margin = new Padding(0);
            header.BackColor = Theme.Background;
            layout.Controls.Add(header, 0, 0);
            header.Controls.Add(Ui.FixedLabel("My Library", 0, 0, 640, 38, Theme.TitleFont, Theme.Text));
            Label subtitle = Ui.FixedLabel("Purchased games, activation keys and review access.",
                2, 40, 760, 26, Theme.BodyFont, Theme.Muted);
            header.Controls.Add(subtitle);
            _count = Ui.Pill("0 GAMES", 0, 7, 90, Theme.SurfaceAlt, Theme.Cream);
            header.Controls.Add(_count);
            header.Resize += delegate
            {
                _count.Left = Math.Max(0, header.ClientSize.Width - _count.Width - 4);
                subtitle.Width = Math.Max(240, _count.Left - 16);
            };

            GamePanel toolbar = Ui.Card(0, 0, 100, 60);
            toolbar.Dock = DockStyle.Fill;
            toolbar.Margin = new Padding(0);
            layout.Controls.Add(toolbar, 0, 1);
            toolbar.Controls.Add(Ui.Lbl("SEARCH", 16, 20, Theme.SmallBold, Theme.Muted));
            _search = Ui.TextInput(78, 15, 360, 31);
            toolbar.Controls.Add(_search);
            _search.TextChanged += delegate { Render(); };
            Button refresh = Ui.GhostBtn("REFRESH", 452, 13, 94, 34);
            refresh.Click += delegate { RefreshData(); };
            toolbar.Controls.Add(refresh);
            toolbar.Resize += delegate
            {
                int usable = toolbar.ClientSize.Width - 78 - 16 - refresh.Width - 14;
                _search.Width = Math.Max(180, Math.Min(420, usable));
                refresh.Left = Math.Min(toolbar.ClientSize.Width - refresh.Width - 14, _search.Right + 14);
            };

            _flow = new FlowLayoutPanel();
            _flow.Dock = DockStyle.Fill;
            _flow.Margin = new Padding(0);
            _flow.AutoScroll = true;
            _flow.WrapContents = true;
            _flow.BackColor = Theme.Background;
            _flow.Padding = new Padding(0, 18, 0, 18);
            layout.Controls.Add(_flow, 0, 2);

            VisibleChanged += delegate { if (Visible) RefreshData(); };
        }

        public override async void RefreshData()
        {
            if (_loading || Program.CurrentUser == null) return;
            _loading = true;
            try
            {
                int id = Program.CurrentUser.UserID;
                _data = await Task.Run(delegate { return _customer.GetLibrary(id); });
                _count.Text = _data.Rows.Count + " GAMES";
                Render();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load your library.\n" + ex.Message, "Library",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { _loading = false; }
        }

        private void Render()
        {
            if (_data == null) return;
            string term = _search.Text.Trim();
            _flow.SuspendLayout();
            _flow.Controls.Clear();
            int shown = 0;

            foreach (DataRow row in _data.Rows)
            {
                string title = Convert.ToString(row["Title"]);
                string genre = Convert.ToString(row["Genre"]);
                string publisher = Convert.ToString(row["Publisher"]);
                if (term.Length > 0 &&
                    title.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0 &&
                    genre.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0 &&
                    publisher.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                shown++;
                _flow.Controls.Add(BuildCard(row));
            }

            if (shown == 0)
            {
                Label empty = Ui.FixedLabel("No purchased games match your search.", 0, 0, 520, 60,
                    Theme.H2Font, Theme.Muted);
                empty.Margin = new Padding(10, 20, 0, 0);
                _flow.Controls.Add(empty);
            }
            _flow.ResumeLayout();
        }

        private Control BuildCard(DataRow row)
        {
            int gameId = Convert.ToInt32(row["Game ID"]);
            string title = Convert.ToString(row["Title"]);
            string genre = Convert.ToString(row["Genre"]);
            string publisher = Convert.ToString(row["Publisher"]);
            string key = Convert.ToString(row["Activation Key"]);
            DateTime purchased = Convert.ToDateTime(row["Purchased"]);

            GamePanel card = Ui.Card(0, 0, 330, 245);
            card.Size = new Size(330, 245);
            card.Margin = new Padding(0, 0, 18, 18);
            card.StartColor = Theme.CardBg;
            card.EndColor = Theme.SurfaceAlt;
            card.Gradient = true;

            PictureBox cover = new PictureBox();
            cover.SetBounds(12, 12, 112, 154);
            cover.SizeMode = PictureBoxSizeMode.Zoom;
            cover.BackColor = Theme.Surface;
            cover.Image = GameAssetService.GetCover(new Game { GameID = gameId, Title = title, GenreName = genre }, 600, 900);
            Ui.RoundControl(cover, 12);
            cover.Resize += delegate { Ui.RoundControl(cover, 12); };
            card.Controls.Add(cover);

            card.Controls.Add(Ui.FixedLabel(title, 142, 18, 170, 46,
                new Font("Segoe UI", 12F, FontStyle.Bold), Theme.Text));
            card.Controls.Add(Ui.FixedLabel(genre, 142, 70, 170, 20, Theme.SmallBold, Theme.Cyan));
            card.Controls.Add(Ui.FixedLabel(publisher, 142, 95, 170, 20, Theme.SmallFont, Theme.Muted));
            card.Controls.Add(Ui.FixedLabel("Purchased " + purchased.ToString("dd MMM yyyy"),
                142, 123, 170, 20, Theme.SmallFont, Theme.Muted2));
            card.Controls.Add(Ui.Pill("OWNED", 142, 151, 70, Theme.SurfaceAlt, Theme.Green));

            Button keyButton = Ui.GhostBtn("VIEW KEY", 12, 191, 96, 36);
            keyButton.Click += delegate
            {
                MessageBox.Show("Activation key for " + title + ":\n\n" + key,
                    "GameHub Activation Key", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            card.Controls.Add(keyButton);

            Button details = Ui.Btn("DETAILS / REVIEW", Theme.Accent, 118, 191, 194, 36);
            details.Click += delegate { RequestOpenGame(gameId); };
            card.Controls.Add(details);
            return card;
        }
    }
}
