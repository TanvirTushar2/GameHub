using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    public partial class ReportsForm : Form
    {
        private readonly ReportRepository _reports =
            new ReportRepository();

        private DataTable _data;
        private Panel _chart;
        private Label _lblSummary;

        public ReportsForm()
        {
            InitializeComponent();

            BuildUi();

            AppStyle.Apply(this, true);

            MinimumSize =
                new Size(1050, 700);

            WindowState =
                FormWindowState.Maximized;

            LoadData();
        }

        private void BuildUi()
        {
            Controls.Clear();

            Text =
                "GameHub - Reports & Analytics";

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

            root.ColumnCount =
                1;

            root.RowCount =
                2;

            root.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    90F));

            root.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            Controls.Add(root);

            // HEADER
            Panel header =
                new Panel();

            header.Dock =
                DockStyle.Fill;

            root.Controls.Add(
                header,
                0,
                0);

            Label title =
                Ui.FixedLabel(
                    "Reports & Analytics",
                    0,
                    0,
                    750,
                    45,
                    new Font(
                        "Segoe UI",
                        20F,
                        FontStyle.Bold),
                    Theme.Text);

            header.Controls.Add(title);

            Label subtitle =
                Ui.FixedLabel(
                    "Review GameHub revenue, orders, users and store performance.",
                    2,
                    48,
                    900,
                    30,
                    Theme.BodyFont,
                    Theme.Muted);

            header.Controls.Add(subtitle);

            Button back =
                Ui.GhostBtn(
                    "←  BACK",
                    0,
                    4,
                    105,
                    36);

            back.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            back.Click +=
                delegate
                {
                    Close();
                };

            header.Controls.Add(back);

            header.Resize +=
                delegate
                {
                    back.Left =
                        header.ClientSize.Width -
                        back.Width;

                    title.Width =
                        Math.Max(
                            300,
                            back.Left - 20);

                    subtitle.Width =
                        Math.Max(
                            300,
                            header.ClientSize.Width - 10);
                };

            // BODY
            TableLayoutPanel body =
                new TableLayoutPanel();

            body.Dock =
                DockStyle.Fill;

            body.ColumnCount =
                2;

            body.RowCount =
                1;

            body.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    72F));

            body.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    28F));

            body.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            root.Controls.Add(
                body,
                0,
                1);

            // CHART CARD
            GamePanel chartCard =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            chartCard.Dock =
                DockStyle.Fill;

            chartCard.Margin =
                new Padding(
                    0,
                    0,
                    9,
                    0);

            chartCard.Padding =
                new Padding(
                    18,
                    105,
                    18,
                    18);

            body.Controls.Add(
                chartCard,
                0,
                0);

            chartCard.Controls.Add(
                Ui.Pill(
                    "REPORT",
                    18,
                    16,
                    76,
                    Theme.Cream,
                    Theme.Background2));

            chartCard.Controls.Add(
                Ui.FixedLabel(
                    "Revenue by Genre",
                    18,
                    53,
                    400,
                    32,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Text));

            chartCard.Controls.Add(
                Ui.FixedLabel(
                    "Paid/completed order revenue grouped by game category",
                    18,
                    82,
                    600,
                    23,
                    Theme.SmallFont,
                    Theme.Muted));

            _chart =
                new Panel();

            _chart.Dock =
                DockStyle.Fill;

            _chart.BackColor =
                Theme.Surface;

            _chart.Paint +=
                Chart_Paint;

            chartCard.Controls.Add(_chart);

            // SUMMARY
            GamePanel summary =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            summary.Dock =
                DockStyle.Fill;

            summary.Margin =
                new Padding(
                    9,
                    0,
                    0,
                    0);

            body.Controls.Add(
                summary,
                1,
                0);

            summary.Controls.Add(
                Ui.Pill(
                    "OVERVIEW",
                    18,
                    16,
                    90,
                    Theme.Accent,
                    Color.White));

            summary.Controls.Add(
                Ui.FixedLabel(
                    "Shop Summary",
                    18,
                    55,
                    260,
                    32,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Text));

            summary.Controls.Add(
                Ui.FixedLabel(
                    "Current database totals",
                    18,
                    85,
                    260,
                    24,
                    Theme.SmallFont,
                    Theme.Muted));

            _lblSummary =
                Ui.FixedLabel(
                    "",
                    18,
                    135,
                    300,
                    380,
                    new Font(
                        "Segoe UI",
                        11F),
                    Theme.Text);

            _lblSummary.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            summary.Controls.Add(
                _lblSummary);

            summary.Resize +=
                delegate
                {
                    _lblSummary.Width =
                        Math.Max(
                            150,
                            summary.ClientSize.Width - 36);

                    _lblSummary.Height =
                        Math.Max(
                            150,
                            summary.ClientSize.Height - 155);
                };
        }

        private void LoadData()
        {
            try
            {
                _data =
                    _reports.RevenueByGenre();

                _lblSummary.Text =
                    "TOTAL GAMES\n" +
                    _reports.TotalGames() +
                    "\n\n" +

                    "TOTAL ORDERS\n" +
                    _reports.TotalOrders() +
                    "\n\n" +

                    "GROSS REVENUE\n" +
                    "Tk " +
                    _reports.TotalRevenue()
                        .ToString("N0") +
                    "\n\n" +

                    "ACTIVE USERS\n" +
                    _reports.ActiveUsers();

                _chart.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Reports",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Chart_Paint(
            object sender,
            PaintEventArgs e)
        {
            Graphics g =
                e.Graphics;

            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            Rectangle area =
                _chart.ClientRectangle;

            if (
                area.Width < 100 ||
                area.Height < 100)
                return;

            if (
                _data == null ||
                _data.Rows.Count == 0)
            {
                using (
                    Brush brush =
                        new SolidBrush(
                            Theme.Muted))
                {
                    g.DrawString(
                        "No paid/completed orders yet.",
                        Theme.BodyFont,
                        brush,
                        20,
                        25);
                }

                return;
            }

            decimal max =
                0m;

            foreach (DataRow row in _data.Rows)
            {
                decimal revenue =
                    Convert.ToDecimal(
                        row["Revenue"]);

                if (revenue > max)
                    max = revenue;
            }

            if (max <= 0)
                max = 1;

            int left =
                50;

            int right =
                25;

            int top =
                45;

            int bottom =
                70;

            int baseY =
                area.Height -
                bottom;

            int usableHeight =
                Math.Max(
                    80,
                    baseY - top);

            int count =
                Math.Min(
                    _data.Rows.Count,
                    8);

            int usableWidth =
                Math.Max(
                    200,
                    area.Width -
                    left -
                    right);

            int gap =
                20;

            int barWidth =
                Math.Max(
                    35,
                    (
                        usableWidth -
                        gap * (count + 1)
                    ) / count);

            Color[] colors =
            {
                Theme.Accent,
                Theme.Green,
                Theme.Purple,
                Theme.Gold,
                Theme.Cyan,
                Theme.Orange
            };

            using (
                Pen axis =
                    new Pen(
                        Theme.Border))
            {
                g.DrawLine(
                    axis,
                    left,
                    baseY,
                    area.Width - right,
                    baseY);
            }

            for (
                int i = 0;
                i < count;
                i++)
            {
                DataRow row =
                    _data.Rows[i];

                decimal revenue =
                    Convert.ToDecimal(
                        row["Revenue"]);

                int barHeight =
                    (int)(
                        revenue /
                        max *
                        usableHeight);

                int x =
                    left +
                    gap +
                    i *
                    (barWidth + gap);

                Rectangle bar =
                    new Rectangle(
                        x,
                        baseY - barHeight,
                        barWidth,
                        barHeight);

                using (
                    Brush brush =
                        new SolidBrush(
                            colors[
                                i %
                                colors.Length]))
                {
                    g.FillRectangle(
                        brush,
                        bar);
                }

                string genre =
                    Convert.ToString(
                        row["GenreName"]);

                TextRenderer.DrawText(
                    g,
                    genre,
                    Theme.SmallFont,
                    new Rectangle(
                        x - 10,
                        baseY + 8,
                        barWidth + 25,
                        40),
                    Theme.Text,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.EndEllipsis);

                TextRenderer.DrawText(
                    g,
                    "Tk " +
                    revenue.ToString("N0"),
                    Theme.SmallBold,
                    new Rectangle(
                        x - 20,
                        bar.Y - 26,
                        barWidth + 40,
                        22),
                    Theme.Cream,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.EndEllipsis);
            }
        }
    }
}