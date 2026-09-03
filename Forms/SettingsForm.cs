using System;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    /// <summary>Super-Admin-only owner control panel: shop-wide settings.</summary>
    public partial class SettingsForm : Form
    {
        private readonly SettingsRepository _settings = new SettingsRepository();

        private TextBox _shopName;
        private TextBox _currency;
        private TextBox _vat;
        private TextBox _defaultDiscount;
        private CheckBox _maintenance;

        public SettingsForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();

            AppStyle.Apply(this, true);

            MinimumSize = new Size(1000, 650);
            WindowState = FormWindowState.Maximized;

            LoadSettings();
        }

        private void BuildUi()
        {
            Text = "GameHub - Settings";
            BackColor = Theme.Background;

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(28, 22, 28, 26);
            root.ColumnCount = 1;
            root.RowCount = 2;
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(root);

            // HEADER
            Panel header = new Panel();
            header.Dock = DockStyle.Fill;
            root.Controls.Add(header, 0, 0);

            Label title = Ui.FixedLabel(
                "Owner Settings", 0, 0, 750, 45,
                new Font("Segoe UI", 20F, FontStyle.Bold), Theme.Text);
            header.Controls.Add(title);

            Label subtitle = Ui.FixedLabel(
                "System-wide settings only the GameHub owner (Super Admin) can change.",
                2, 48, 900, 30, Theme.BodyFont, Theme.Muted);
            header.Controls.Add(subtitle);

            Button back = Ui.GhostBtn("←  BACK", 0, 4, 105, 36);
            back.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            back.Click += delegate { Close(); };
            header.Controls.Add(back);

            header.Resize += delegate
            {
                back.Left = header.ClientSize.Width - back.Width;
                title.Width = Math.Max(300, back.Left - 20);
                subtitle.Width = Math.Max(300, header.ClientSize.Width - 10);
            };

            // CARD
            GamePanel card = Ui.Card(0, 0, 100, 100);
            card.Dock = DockStyle.Fill;
            card.Padding = new Padding(24, 58, 24, 24);
            root.Controls.Add(card, 0, 1);

            card.Controls.Add(Ui.Pill(
                "SETTINGS", 18, 16, 100, Theme.Cream, Theme.Background2));

            int y = 70;
            _shopName = AddField(card, "Shop name", ref y);
            _currency = AddField(card, "Currency symbol", ref y);
            _vat = AddField(card, "VAT percent (%)", ref y);
            _defaultDiscount = AddField(card, "Default member discount (%)", ref y);

            _maintenance = new CheckBox();
            _maintenance.Text = "  Maintenance mode (store closed to customers)";
            _maintenance.ForeColor = Theme.Text;
            _maintenance.BackColor = Color.Transparent;
            _maintenance.Font = Theme.BodyFont;
            _maintenance.AutoSize = true;
            _maintenance.SetBounds(28, y + 6, 420, 24);
            card.Controls.Add(_maintenance);
            y += 52;

            Button save = Ui.Btn("SAVE SETTINGS", Theme.Green, 28, y + 6, 180, 40);
            save.Click += delegate { SaveSettings(); };
            card.Controls.Add(save);
        }

        private TextBox AddField(GamePanel card, string label, ref int y)
        {
            card.Controls.Add(Ui.FixedLabel(
                label, 28, y, 320, 22, Theme.BodyBold, Theme.Muted));

            TextBox input = Ui.TextInput(28, y + 24, 420, 32);
            card.Controls.Add(input);

            y += 74;
            return input;
        }

        private void LoadSettings()
        {
            try
            {
                _shopName.Text = _settings.Get("ShopName", "GameHub");
                _currency.Text = _settings.Get("Currency", "Tk");
                _vat.Text = _settings.Get("VatPercent", "5");
                _defaultDiscount.Text = _settings.Get("DefaultDiscount", "0");
                _maintenance.Checked = _settings.Get("MaintenanceMode", "false") == "true";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Settings",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings()
        {
            try
            {
                _settings.Set("ShopName", _shopName.Text.Trim());
                _settings.Set("Currency", _currency.Text.Trim());
                _settings.Set("VatPercent", _vat.Text.Trim());
                _settings.Set("DefaultDiscount", _defaultDiscount.Text.Trim());
                _settings.Set("MaintenanceMode", _maintenance.Checked ? "true" : "false");

                AuditRepository.Write("Settings changed", "Owner updated system settings");

                MessageBox.Show("Settings saved.", "Settings",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Settings",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
