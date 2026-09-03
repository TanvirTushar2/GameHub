using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Models;

namespace GameHub.Forms
{
    /// <summary>GameHub Pass - tiered subscription plans.</summary>
    public partial class SubscriptionForm : Form
    {
        private readonly SubscriptionRepository _subs = new SubscriptionRepository();
        private readonly WalletRepository _wallet = new WalletRepository();

        public SubscriptionForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();
            AppStyle.Apply(this);
        }

        private void BuildUi()
        {
            ClientSize = new Size(1040, 640);
            Text = "GameHub - GameHub Pass";
            Controls.Add(Ui.Lbl("GameHub Pass", 28, 22, new Font("Segoe UI", 18F, FontStyle.Bold), Theme.Text));
            Controls.Add(Ui.Lbl("Choose a membership tier and unlock discounts, rewards and early access.", 30, 54,
                Theme.SmallFont, Theme.Muted));
            { var back = Ui.Back(this); Controls.Add(back); back.BringToFront(); }

            List<SubscriptionPlan> plans;
            try { plans = _subs.GetPlans(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); return; }

            string[][] feats = new string[][] {
                new[]{ "5% off every purchase", "Wishlist price alerts", "Standard support", "Sale event access" },
                new[]{ "10% off every purchase", "1 free game / month", "Early pre-order access", "2x loyalty points", "Priority support" },
                new[]{ "15% off every purchase", "2 free games / month", "Exclusive beta access", "3x loyalty points", "Gift game keys", "24/7 premium support" }
            };
            Color[] colors = { Theme.Accent, Theme.Purple, Theme.Gold };

            for (int i = 0; i < plans.Count && i < 3; i++)
            {
                SubscriptionPlan plan = plans[i];
                Color c = colors[i % colors.Length];
                int x = 28 + i * 332;

                Panel card = Ui.Card(x, 100, 312, 500);
                Controls.Add(card);
                Panel head = Ui.GradientCard(0, 0, 312, 120, c, Theme.Background2);
                card.Controls.Add(head);
                head.Controls.Add(Ui.Pill(i == 1 ? "POPULAR" : "PASS", 20, 18, i == 1 ? 78 : 56, Theme.Cream, Theme.Background2));
                head.Controls.Add(Ui.Lbl(plan.PlanName.ToUpper(), 20, 54, new Font("Segoe UI", 17F, FontStyle.Bold), Color.White));
                head.Controls.Add(Ui.Lbl(plan.DiscountPct + "% store discount", 20, 86, Theme.SmallFont, Color.FromArgb(245, 219, 216)));

                Label price = Ui.Lbl("Tk " + plan.MonthlyPrice.ToString("N0"), 20, 144,
                    new Font("Segoe UI", 22F, FontStyle.Bold), Theme.Text);
                card.Controls.Add(price);
                card.Controls.Add(Ui.Lbl("per month", 22, 181, Theme.SmallFont, Theme.Muted));
                card.Controls.Add(Ui.Lbl("WHAT YOU GET", 20, 220, new Font("Segoe UI", 8F, FontStyle.Bold), Theme.Cream));

                int fy = 252;
                foreach (string feature in feats[i])
                {
                    Label fl = Ui.Lbl("✓  " + feature, 20, fy, Theme.BodyFont, Theme.Text);
                    fl.AutoSize = false; fl.SetBounds(20, fy, 270, 22);
                    card.Controls.Add(fl);
                    fy += 29;
                }

                Button sub = Ui.Btn("Choose " + plan.PlanName, c, 20, 446, 272, 36);
                SubscriptionPlan captured = plan;
                sub.Click += (s, e) => Subscribe(captured);
                card.Controls.Add(sub);
            }
        }

        private void Subscribe(SubscriptionPlan plan)
        {
            if (Program.CurrentUser == null) return;
            try
            {
                decimal bal = _wallet.GetBalance(Program.CurrentUser.UserID);
                if (bal < plan.MonthlyPrice)
                {
                    MessageBox.Show("Insufficient wallet balance (Tk " + bal.ToString("N2") +
                        "). Please top up first.", "Subscription");
                    return;
                }
                _subs.Subscribe(Program.CurrentUser.UserID, plan);
                MessageBox.Show("You are now subscribed to GameHub Pass - " + plan.PlanName +
                    ".\nEnjoy " + plan.DiscountPct + "% off every purchase!", "Success");
                Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }
    }
}
