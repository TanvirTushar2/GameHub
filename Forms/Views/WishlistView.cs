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
    internal class WishlistView : LauncherPage
    {
        private readonly CustomerRepository _customer = new CustomerRepository();
        private readonly FlowLayoutPanel _flow;
        private readonly Label _count;
        private bool _loading;

        public WishlistView()
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Margin = new Padding(0);
            layout.Padding = new Padding(0);
            layout.ColumnCount = 1;
            layout.RowCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.BackColor = Theme.Background;
            Controls.Add(layout);

            Panel header = new Panel();
            header.Dock = DockStyle.Fill;
            header.Margin = new Padding(0);
            header.BackColor = Theme.Background;
            layout.Controls.Add(header, 0, 0);
            header.Controls.Add(Ui.FixedLabel("Wishlist", 0, 0, 620, 38, Theme.TitleFont, Theme.Text));
            Label subtitle = Ui.FixedLabel(
                "Save games you want to revisit and jump straight back to their store page.",
                2, 40, 820, 26, Theme.BodyFont, Theme.Muted);
            header.Controls.Add(subtitle);
            _count = Ui.Pill("0 SAVED", 0, 7, 90, Theme.SurfaceAlt, Theme.Red);
            header.Controls.Add(_count);
            header.Resize += delegate
            {
                _count.Left = Math.Max(0, header.ClientSize.Width - _count.Width - 4);
                subtitle.Width = Math.Max(260, _count.Left - 16);
            };

            _flow = new FlowLayoutPanel();
            _flow.Dock = DockStyle.Fill;
            _flow.Margin = new Padding(0);
            _flow.AutoScroll = true;
            _flow.WrapContents = true;
            _flow.BackColor = Theme.Background;
            _flow.Padding = new Padding(0, 18, 0, 20);
            layout.Controls.Add(_flow, 0, 1);

            VisibleChanged += delegate { if (Visible) RefreshData(); };
        }

        public override async void RefreshData()
        {
            if (_loading || Program.CurrentUser == null) return;
            _loading = true;
            try
            {
                int userId = Program.CurrentUser.UserID;
                DataTable data = await Task.Run(delegate { return _customer.GetWishlist(userId); });
                _count.Text = data.Rows.Count + " SAVED";
                Render(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load your wishlist.\n" + ex.Message, "Wishlist",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { _loading = false; }
        }

        private void Render(DataTable data)
        {
            _flow.SuspendLayout();
            _flow.Controls.Clear();
            foreach (DataRow row in data.Rows) _flow.Controls.Add(BuildCard(row));

            if (data.Rows.Count == 0)
            {
                GamePanel empty = Ui.Card(0, 0, 620, 150);
                empty.Size = new Size(620, 150);
                empty.Margin = new Padding(8, 18, 0, 0);
                empty.Controls.Add(Ui.Lbl("Your wishlist is empty", 24, 24, Theme.H1Font, Theme.Text));
                empty.Controls.Add(Ui.FixedLabel(
                    "Open the Store and press the heart on any game you want to follow.",
                    25, 62, 550, 24, Theme.BodyFont, Theme.Muted));
                Button browse = Ui.Btn("BROWSE STORE", Theme.Accent, 24, 96, 146, 36);
                browse.Click += delegate { RequestNavigate("Store"); };
                empty.Controls.Add(browse);
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
            decimal price = Convert.ToDecimal(row["Price"]);
            int stock = Convert.ToInt32(row["Stock"]);

            GamePanel card = Ui.Card(0, 0, 350, 214);
            card.Size = new Size(350, 214);
            card.Margin = new Padding(0, 0, 18, 18);

            PictureBox art = new PictureBox();
            art.SetBounds(12, 12, 122, 166);
            art.SizeMode = PictureBoxSizeMode.Zoom;
            art.BackColor = Theme.Surface;
            art.Image = GameAssetService.GetCover(new Game { GameID = gameId, Title = title, GenreName = genre }, 600, 900);
            Ui.RoundControl(art, 12);
            art.Resize += delegate { Ui.RoundControl(art, 12); };
            card.Controls.Add(art);

            card.Controls.Add(Ui.FixedLabel(title, 152, 18, 180, 43,
                new Font("Segoe UI", 12F, FontStyle.Bold), Theme.Text));
            card.Controls.Add(Ui.FixedLabel(genre + " • " + publisher, 152, 66, 180, 36,
                Theme.SmallFont, Theme.Muted));
            card.Controls.Add(Ui.Lbl("Tk " + price.ToString("N0"), 152, 112,
                new Font("Segoe UI", 12F, FontStyle.Bold), Theme.Cream));
            card.Controls.Add(Ui.Lbl(stock > 0 ? stock + " in stock" : "Out of stock", 152, 143,
                Theme.SmallBold, stock > 0 ? Theme.Green : Theme.Red));

            Button open = Ui.Btn("VIEW GAME", Theme.Accent, 152, 169, 102, 32);
            open.Click += delegate { RequestOpenGame(gameId); };
            card.Controls.Add(open);

            Button remove = Ui.GhostBtn("REMOVE", 262, 169, 76, 32);
            remove.ForeColor = Theme.Red;
            remove.Click += delegate
            {
                try
                {
                    if (Program.CurrentUser == null) return;
                    _customer.RemoveFromWishlist(Program.CurrentUser.UserID, gameId);
                    RaiseGlobalDataChanged();
                    RefreshData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Wishlist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            card.Controls.Add(remove);
            return card;
        }
    }
}
