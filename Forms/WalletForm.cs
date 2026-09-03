using System;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    /// <summary>Customer wallet, loyalty points, membership and transactions.</summary>
    public partial class WalletForm : Form
    {
        private readonly WalletRepository _wallet = new WalletRepository();
        private readonly SubscriptionRepository _subs = new SubscriptionRepository();
        private Label _lblBalance, _lblPoints, _lblPlan;
        private TextBox _txtAmount;
        private DataGridView _grid;

        public WalletForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();
            AppStyle.Apply(this);
            LoadData();
        }

        private void BuildUi()
        {
            ClientSize = new Size(900, 620);
            Text = "GameHub - My Wallet & Rewards";
            Controls.Add(Ui.Lbl("Wallet & Rewards", 28, 22, new Font("Segoe UI", 18F, FontStyle.Bold), Theme.Text));
            Controls.Add(Ui.Lbl("Manage balance, loyalty points and membership benefits.", 30, 54, Theme.SmallFont, Theme.Muted));
            { var back = Ui.Back(this); Controls.Add(back); back.BringToFront(); }

            Panel bal = Ui.GradientCard(28, 94, 404, 142, Theme.Accent, Theme.Background2);
            Controls.Add(bal);
            bal.Controls.Add(Ui.Pill("WALLET", 18, 18, 66, Theme.Cream, Theme.Background2));
            bal.Controls.Add(Ui.Lbl("Available balance", 18, 56, Theme.SmallFont, Color.FromArgb(244, 214, 211)));
            _lblBalance = Ui.Lbl("Tk 0.00", 18, 78, new Font("Segoe UI", 24F, FontStyle.Bold), Color.White);
            bal.Controls.Add(_lblBalance);

            Panel pts = Ui.GradientCard(450, 94, 422, 142, Theme.Purple, Theme.Background2);
            Controls.Add(pts);
            pts.Controls.Add(Ui.Pill("REWARDS", 18, 18, 78, Theme.Cream, Theme.Background2));
            pts.Controls.Add(Ui.Lbl("Loyalty points", 18, 56, Theme.SmallFont, Color.FromArgb(235, 217, 245)));
            _lblPoints = Ui.Lbl("0 pts", 18, 78, new Font("Segoe UI", 24F, FontStyle.Bold), Color.White);
            pts.Controls.Add(_lblPoints);

            Panel membership = Ui.Card(28, 256, 844, 76);
            Controls.Add(membership);
            membership.Controls.Add(Ui.Lbl("ACTIVE MEMBERSHIP", 18, 15, new Font("Segoe UI", 8F, FontStyle.Bold), Theme.Cream));
            _lblPlan = Ui.Lbl("GameHub Pass - None", 18, 38, new Font("Segoe UI", 12F, FontStyle.Bold), Theme.Text);
            membership.Controls.Add(_lblPlan);
            Button pass = Ui.Btn("View Pass", Theme.SurfaceAlt, 710, 21, 116, 34);
            pass.Click += (s, e) =>
            {
                using (SubscriptionForm sf = new SubscriptionForm())
                    sf.ShowDialog(this);
                LoadData();
            };
            membership.Controls.Add(pass);

            Panel topup = Ui.Card(28, 352, 300, 236);
            Controls.Add(topup);
            topup.Controls.Add(Ui.Lbl("Top Up Wallet", 18, 16, new Font("Segoe UI", 12F, FontStyle.Bold), Theme.Text));
            topup.Controls.Add(Ui.Lbl("Add funds to use at checkout.", 18, 42, Theme.SmallFont, Theme.Muted));
            topup.Controls.Add(Ui.Lbl("Amount (BDT)", 18, 82, Theme.BodyFont, Theme.Text));
            _txtAmount = Ui.TextInput(18, 106, 264, 30); topup.Controls.Add(_txtAmount);
            Button topupBtn = Ui.Btn("TOP UP", Theme.Green, 18, 154, 264, 38);
            topupBtn.Click += TopUp_Click; topup.Controls.Add(topupBtn);
            topup.Controls.Add(Ui.Lbl("Funds are added immediately.", 18, 203, Theme.SmallFont, Theme.Muted));

            Panel tx = Ui.Card(348, 352, 524, 236);
            Controls.Add(tx);
            tx.Controls.Add(Ui.Lbl("Recent Transactions", 18, 16, new Font("Segoe UI", 12F, FontStyle.Bold), Theme.Text));
            tx.Controls.Add(Ui.Lbl("Latest wallet activity", 18, 42, Theme.SmallFont, Theme.Muted));
            _grid = Ui.Grid(18, 70, 488, 146);
            tx.Controls.Add(_grid);
        }

        private void LoadData()
        {
            if (Program.CurrentUser == null) return;
            try
            {
                int id = Program.CurrentUser.UserID;
                _lblBalance.Text = "Tk " + _wallet.GetBalance(id).ToString("N2");
                _lblPoints.Text = _wallet.GetPoints(id) + " pts";
                _lblPlan.Text = "GameHub Pass - " + _subs.GetActivePlanName(id);
                _grid.DataSource = _wallet.GetTransactions(id);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void TopUp_Click(object sender, EventArgs e)
        {
            decimal amt;
            if (!decimal.TryParse(_txtAmount.Text, out amt) || amt <= 0)
            { MessageBox.Show("Enter a valid amount."); return; }
            try
            {
                _wallet.TopUp(Program.CurrentUser.UserID, amt, "Topup");
                _txtAmount.Clear();
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }
    }
}
