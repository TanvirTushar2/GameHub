using System;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    /// <summary>Super-Admin-only activity trail: who did what, and when.</summary>
    public partial class AuditLogForm : Form
    {
        private readonly AuditRepository _audit = new AuditRepository();
        private DataGridView _grid;

        public AuditLogForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();

            AppStyle.Apply(this, true);

            MinimumSize = new Size(1000, 650);
            WindowState = FormWindowState.Maximized;

            LoadLog();
        }

        private void BuildUi()
        {
            Text = "GameHub - System Log";
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
                "System Activity Log", 0, 0, 750, 45,
                new Font("Segoe UI", 20F, FontStyle.Bold), Theme.Text);
            header.Controls.Add(title);

            Label subtitle = Ui.FixedLabel(
                "Owner oversight — a record of key actions performed across GameHub.",
                2, 48, 900, 30, Theme.BodyFont, Theme.Muted);
            header.Controls.Add(subtitle);

            Button refresh = Ui.Btn("REFRESH", Theme.Accent, 0, 4, 110, 36);
            refresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            refresh.Click += delegate { LoadLog(); };
            header.Controls.Add(refresh);

            Button back = Ui.GhostBtn("←  BACK", 0, 4, 105, 36);
            back.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            back.Click += delegate { Close(); };
            header.Controls.Add(back);

            header.Resize += delegate
            {
                back.Left = header.ClientSize.Width - back.Width;
                refresh.Left = back.Left - refresh.Width - 10;
                title.Width = Math.Max(300, refresh.Left - 20);
                subtitle.Width = Math.Max(300, header.ClientSize.Width - 10);
            };

            // GRID CARD
            GamePanel card = Ui.Card(0, 0, 100, 100);
            card.Dock = DockStyle.Fill;
            card.Padding = new Padding(18, 58, 18, 18);
            root.Controls.Add(card, 0, 1);

            card.Controls.Add(Ui.Pill(
                "AUDIT TRAIL", 18, 16, 110, Theme.Cream, Theme.Background2));

            _grid = Ui.Grid(0, 0, 100, 100);
            _grid.Dock = DockStyle.Fill;
            card.Controls.Add(_grid);
        }

        private void LoadLog()
        {
            try
            {
                _grid.DataSource = _audit.GetRecent(300);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System Log",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
