using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Forms.Controls;
using GameHub.Models;

namespace GameHub.Forms.Views
{
    internal class SubscriptionView : LauncherPage
    {
        private readonly SubscriptionRepository _subscriptions = new SubscriptionRepository();
        private readonly WalletRepository _wallet = new WalletRepository();
        private readonly Label _currentPlan;
        private readonly Label _currentMeta;
        private readonly FlowLayoutPanel _plans;
        private bool _loading;

        public SubscriptionView()
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.Margin = new Padding(0);
            layout.Padding = new Padding(0);
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.BackColor = Theme.Background;
            Controls.Add(layout);

            Panel header = new Panel();
            header.Dock = DockStyle.Fill;
            header.Margin = new Padding(0);
            header.BackColor = Theme.Background;
            layout.Controls.Add(header, 0, 0);
            header.Controls.Add(Ui.FixedLabel("GameHub Pass", 0, 0, 700, 38, Theme.TitleFont, Theme.Text));
            Label subtitle = Ui.FixedLabel(
                "Choose a monthly plan and receive store discounts and free-game benefits.",
                2, 41, 900, 28, Theme.BodyFont, Theme.Muted);
            header.Controls.Add(subtitle);
            header.Resize += delegate { subtitle.Width = Math.Max(300, header.ClientSize.Width - 4); };

            GamePanel current = Ui.GradientCard(0, 0, 100, 118, Theme.AccentDark, Theme.Background2);
            current.Dock = DockStyle.Fill;
            current.Margin = new Padding(0, 0, 0, 12);
            layout.Controls.Add(current, 0, 1);
            current.Controls.Add(Ui.Lbl("CURRENT MEMBERSHIP", 24, 16, Theme.SmallBold, Theme.Cyan));
            _currentPlan = Ui.FixedLabel("None", 24, 38, 600, 40,
                new Font("Segoe UI", 22F, FontStyle.Bold), Color.White);
            current.Controls.Add(_currentPlan);
            _currentMeta = Ui.FixedLabel("No active subscription", 26, 80, 900, 25, Theme.BodyFont, Theme.Cream);
            current.Controls.Add(_currentMeta);
            current.Resize += delegate
            {
                _currentPlan.Width = Math.Max(260, current.ClientSize.Width - 48);
                _currentMeta.Width = Math.Max(260, current.ClientSize.Width - 52);
            };

            Panel section = new Panel();
            section.Dock = DockStyle.Fill;
            section.Margin = new Padding(0);
            section.BackColor = Theme.Background;
            layout.Controls.Add(section, 0, 2);
            section.Controls.Add(Ui.FixedLabel("Available plans", 0, 13, 420, 38, Theme.H1Font, Theme.Text));

            _plans = new FlowLayoutPanel();
            _plans.Dock = DockStyle.Fill;
            _plans.Margin = new Padding(0);
            _plans.AutoScroll = true;
            _plans.WrapContents = true;
            _plans.Padding = new Padding(0, 6, 0, 20);
            _plans.BackColor = Theme.Background;
            layout.Controls.Add(_plans, 0, 3);

            VisibleChanged += delegate { if (Visible) RefreshData(); };
        }

        public override async void RefreshData()
        {
            if (_loading || Program.CurrentUser == null) return;
            _loading = true;
            try
            {
                int userId = Program.CurrentUser.UserID;
                SubscriptionData data = await Task.Run(delegate
                {
                    SubscriptionData d = new SubscriptionData();
                    d.Plans = _subscriptions.GetPlans();
                    d.ActivePlan = _subscriptions.GetActivePlanName(userId);
                    d.EndDate = _subscriptions.GetActivePlanEndDate(userId);
                    d.Balance = _wallet.GetBalance(userId);
                    return d;
                });

                _currentPlan.Text = data.ActivePlan;
                _currentMeta.Text = data.EndDate.HasValue
                    ? "Active until " + data.EndDate.Value.ToString("dd MMM yyyy") + "  •  Wallet Tk " + data.Balance.ToString("N2")
                    : "No active subscription  •  Wallet Tk " + data.Balance.ToString("N2");
                RenderPlans(data.Plans, data.ActivePlan, data.Balance);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load GameHub Pass.\n" + ex.Message,
                    "Subscription", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { _loading = false; }
        }

        private void RenderPlans(List<SubscriptionPlan> plans, string activePlan, decimal balance)
        {
            _plans.SuspendLayout();
            _plans.Controls.Clear();
            foreach (SubscriptionPlan plan in plans)
                _plans.Controls.Add(BuildPlanCard(plan, activePlan, balance));
            _plans.ResumeLayout();
        }

        private Control BuildPlanCard(SubscriptionPlan plan, string activePlan, decimal balance)
        {
            bool active = string.Equals(plan.PlanName, activePlan, StringComparison.OrdinalIgnoreCase);
            GamePanel card = Ui.Card(0, 0, 310, 330);
            card.Size = new Size(310, 330);
            card.Margin = new Padding(0, 0, 18, 18);
            card.StartColor = active ? Theme.SurfaceAlt : Theme.CardBg;
            card.EndColor = active ? Theme.AccentDark : Theme.Surface;
            card.Gradient = true;
            card.OutlineColor = active ? Theme.Accent : Theme.BorderSoft;

            card.Controls.Add(Ui.Pill(active ? "CURRENT PLAN" : "GAMEHUB PASS", 20, 20, 110,
                active ? Theme.Green : Theme.SurfaceAlt, active ? Theme.Background2 : Theme.Cream));
            card.Controls.Add(Ui.FixedLabel(plan.PlanName, 20, 60, 270, 42,
                new Font("Segoe UI", 20F, FontStyle.Bold), Theme.Text));
            card.Controls.Add(Ui.FixedLabel("Tk " + plan.MonthlyPrice.ToString("N0") + " / month", 20, 104, 270, 30,
                new Font("Segoe UI", 12F, FontStyle.Bold), Theme.Cream));

            string benefits =
                "✓ " + plan.DiscountPct + "% store discount\n" +
                "✓ " + plan.FreeGamesPerMonth + " free game" + (plan.FreeGamesPerMonth == 1 ? "" : "s") + " / month\n" +
                "✓ GameHub member badge\n" +
                "✓ Loyalty purchase benefits";
            Label benefitsLabel = Ui.FixedLabel(benefits, 22, 151, 266, 94, Theme.BodyFont, Theme.Text);
            card.Controls.Add(benefitsLabel);

            Label affordability = Ui.FixedLabel(balance >= plan.MonthlyPrice ? "Wallet balance is sufficient" : "Top up your Wallet first",
                22, 255, 266, 20, Theme.SmallBold,
                balance >= plan.MonthlyPrice ? Theme.Green : Theme.Red);
            card.Controls.Add(affordability);

            Button subscribe = Ui.Btn(active ? "ACTIVE" : "SUBSCRIBE", active ? Theme.SurfaceAlt : Theme.Accent,
                20, 286, 270, 34);
            subscribe.Enabled = !active;
            subscribe.Click += delegate { Subscribe(plan); };
            card.Controls.Add(subscribe);
            return card;
        }

        private void Subscribe(SubscriptionPlan plan)
        {
            if (Program.CurrentUser == null || plan == null) return;
            DialogResult answer = MessageBox.Show(
                "Subscribe to " + plan.PlanName + " for Tk " + plan.MonthlyPrice.ToString("N2") + "?\n\n" +
                "The amount will be charged from your GameHub Wallet and any existing active plan will be replaced.",
                "Confirm subscription", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            try
            {
                _subscriptions.Subscribe(Program.CurrentUser.UserID, plan);
                MessageBox.Show("GameHub Pass " + plan.PlanName + " is now active.",
                    "Subscription", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RaiseGlobalDataChanged();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not activate the plan.\n" + ex.Message,
                    "Subscription", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private sealed class SubscriptionData
        {
            public List<SubscriptionPlan> Plans;
            public string ActivePlan;
            public DateTime? EndDate;
            public decimal Balance;
        }
    }
}
