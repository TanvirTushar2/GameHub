using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Forms.Controls;

namespace GameHub.Forms.Views
{
    internal class WalletView : LauncherPage
    {
        private readonly WalletRepository _wallet = new WalletRepository();

        private readonly Label _balance;
        private readonly Label _points;
        private readonly TextBox _amount;
        private readonly ComboBox _method;
        private readonly DataGridView _transactions;
        private readonly Button _continueButton;

        private bool _loading;

        public WalletView()
        {
            TableLayoutPanel layout = new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.Margin = new Padding(0);
            layout.Padding = new Padding(0);

            layout.ColumnCount = 1;
            layout.RowCount = 5;

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    78F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    170F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    166F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    62F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            layout.BackColor = Theme.Background;

            Controls.Add(layout);

            // =========================================================
            // HEADER
            // =========================================================

            Panel header = new Panel();

            header.Dock = DockStyle.Fill;
            header.Margin = new Padding(0);
            header.BackColor = Theme.Background;

            layout.Controls.Add(
                header,
                0,
                0);

            Label title =
                Ui.FixedLabel(
                    "GameHub Wallet",
                    0,
                    0,
                    780,
                    38,
                    Theme.TitleFont,
                    Theme.Text);

            header.Controls.Add(title);

            Label subtitle =
                Ui.FixedLabel(
                    "Manage balance, loyalty points and your complete wallet transaction history.",
                    2,
                    41,
                    900,
                    28,
                    Theme.BodyFont,
                    Theme.Muted);

            header.Controls.Add(subtitle);

            header.Resize += delegate
            {
                title.Width =
                    Math.Max(
                        300,
                        header.ClientSize.Width - 4);

                subtitle.Width =
                    Math.Max(
                        300,
                        header.ClientSize.Width - 4);
            };

            // =========================================================
            // BALANCE CARD
            // =========================================================

            GamePanel balanceCard =
                Ui.GradientCard(
                    0,
                    0,
                    100,
                    170,
                    Theme.AccentDark,
                    Theme.Background2);

            balanceCard.Dock =
                DockStyle.Fill;

            balanceCard.Margin =
                new Padding(
                    0,
                    0,
                    0,
                    12);

            layout.Controls.Add(
                balanceCard,
                0,
                1);

            balanceCard.Controls.Add(
                Ui.Lbl(
                    "AVAILABLE BALANCE",
                    26,
                    24,
                    Theme.SmallBold,
                    Color.FromArgb(
                        220,
                        240,
                        245,
                        255)));

            _balance =
                Ui.FixedLabel(
                    "Tk 0.00",
                    26,
                    50,
                    900,
                    50,
                    new Font(
                        "Segoe UI",
                        27F,
                        FontStyle.Bold),
                    Color.White);

            balanceCard.Controls.Add(
                _balance);

            balanceCard.Controls.Add(
                Ui.Lbl(
                    "LOYALTY POINTS",
                    26,
                    116,
                    Theme.SmallBold,
                    Theme.Cyan));

            _points =
                Ui.FixedLabel(
                    "0 pts",
                    176,
                    108,
                    260,
                    34,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Cream);

            balanceCard.Controls.Add(
                _points);

            balanceCard.Resize += delegate
            {
                _balance.Width =
                    Math.Max(
                        300,
                        balanceCard.ClientSize.Width - 52);

                _points.Width =
                    Math.Max(
                        180,
                        balanceCard.ClientSize.Width
                        - _points.Left
                        - 26);
            };

            // =========================================================
            // TOP UP CARD
            //
            // This uses TableLayoutPanel instead of overlapping
            // absolute Y positions, so it also works properly at
            // Windows 125% / 150% display scaling.
            // =========================================================

            GamePanel topup =
                Ui.Card(
                    0,
                    0,
                    100,
                    166);

            topup.Dock =
                DockStyle.Fill;

            topup.Margin =
                new Padding(
                    0,
                    0,
                    0,
                    8);

            layout.Controls.Add(
                topup,
                0,
                2);

            TableLayoutPanel topupLayout =
                new TableLayoutPanel();

            topupLayout.Dock =
                DockStyle.Fill;

            topupLayout.Margin =
                new Padding(0);

            topupLayout.Padding =
                new Padding(
                    18,
                    12,
                    18,
                    12);

            topupLayout.ColumnCount =
                4;

            topupLayout.RowCount =
                4;

            topupLayout.BackColor =
                Color.Transparent;

            topupLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    220F));

            topupLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    240F));

            topupLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    155F));

            topupLayout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            topupLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    28F));

            topupLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    30F));

            topupLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    25F));

            topupLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    43F));

            topup.Controls.Add(
                topupLayout);

            // TOP UP title

            Label topupTitle =
                Ui.Lbl(
                    "TOP UP WALLET",
                    0,
                    0,
                    Theme.SmallBold,
                    Theme.AccentBlue);

            topupTitle.Dock =
                DockStyle.Fill;

            topupTitle.TextAlign =
                ContentAlignment.MiddleLeft;

            topupTitle.Margin =
                new Padding(0);

            topupLayout.Controls.Add(
                topupTitle,
                0,
                0);

            topupLayout.SetColumnSpan(
                topupTitle,
                4);

            // Description

            Label topupInfo =
                Ui.FixedLabel(
                    "Choose a source first. Payment details must be confirmed before money is added to the wallet.",
                    0,
                    0,
                    100,
                    28,
                    Theme.SmallFont,
                    Theme.Muted);

            topupInfo.Dock =
                DockStyle.Fill;

            topupInfo.TextAlign =
                ContentAlignment.MiddleLeft;

            topupInfo.Margin =
                new Padding(0);

            topupLayout.Controls.Add(
                topupInfo,
                0,
                1);

            topupLayout.SetColumnSpan(
                topupInfo,
                4);

            // =========================================================
            // LABELS
            // =========================================================

            Label amountLabel =
                Ui.Lbl(
                    "Amount (Tk)",
                    0,
                    0,
                    Theme.SmallFont,
                    Theme.Muted);

            amountLabel.Dock =
                DockStyle.Fill;

            amountLabel.TextAlign =
                ContentAlignment.BottomLeft;

            amountLabel.Margin =
                new Padding(
                    0,
                    0,
                    14,
                    2);

            topupLayout.Controls.Add(
                amountLabel,
                0,
                2);

            Label methodLabel =
                Ui.Lbl(
                    "Payment method",
                    0,
                    0,
                    Theme.SmallFont,
                    Theme.Muted);

            methodLabel.Dock =
                DockStyle.Fill;

            methodLabel.TextAlign =
                ContentAlignment.BottomLeft;

            methodLabel.Margin =
                new Padding(
                    0,
                    0,
                    14,
                    2);

            topupLayout.Controls.Add(
                methodLabel,
                1,
                2);

            Label actionLabel =
                Ui.Lbl(
                    "Action",
                    0,
                    0,
                    Theme.SmallFont,
                    Theme.Muted);

            actionLabel.Dock =
                DockStyle.Fill;

            actionLabel.TextAlign =
                ContentAlignment.BottomLeft;

            actionLabel.Margin =
                new Padding(
                    0,
                    0,
                    14,
                    2);

            topupLayout.Controls.Add(
                actionLabel,
                2,
                2);

            // =========================================================
            // AMOUNT INPUT
            // =========================================================

            _amount =
                Ui.TextInput(
                    0,
                    0,
                    100,
                    32);

            _amount.Dock =
                DockStyle.Fill;

            _amount.Margin =
                new Padding(
                    0,
                    2,
                    14,
                    4);

            _amount.TextAlign =
                HorizontalAlignment.Left;

            topupLayout.Controls.Add(
                _amount,
                0,
                3);

            // =========================================================
            // PAYMENT METHOD
            // =========================================================

            _method =
                new ComboBox();

            _method.DropDownStyle =
                ComboBoxStyle.DropDownList;

            _method.Items.AddRange(
                new object[]
                {
                    "bKash",
                    "Nagad",
                    "Credit / Debit Card",
                    "Bank Transfer"
                });

            _method.SelectedIndex =
                0;

            _method.Dock =
                DockStyle.Fill;

            _method.Margin =
                new Padding(
                    0,
                    2,
                    14,
                    4);

            _method.Font =
                Theme.BodyFont;

            topupLayout.Controls.Add(
                _method,
                1,
                3);

            // =========================================================
            // CONTINUE BUTTON
            // =========================================================

            _continueButton =
                Ui.Btn(
                    "CONTINUE",
                    Theme.Green,
                    0,
                    0,
                    120,
                    36);

            _continueButton.Dock =
                DockStyle.Fill;

            _continueButton.Margin =
                new Padding(
                    0,
                    2,
                    14,
                    4);

            _continueButton.Click +=
                TopUp_Click;

            topupLayout.Controls.Add(
                _continueButton,
                2,
                3);

            // =========================================================
            // DEMO NOTE
            // =========================================================

            Label demoNote =
                Ui.FixedLabel(
                    "Demo payment verification only - no real gateway is connected.",
                    0,
                    0,
                    100,
                    36,
                    Theme.SmallFont,
                    Theme.Muted2);

            demoNote.Dock =
                DockStyle.Fill;

            demoNote.TextAlign =
                ContentAlignment.MiddleLeft;

            demoNote.Margin =
                new Padding(
                    4,
                    2,
                    0,
                    4);

            topupLayout.Controls.Add(
                demoNote,
                3,
                3);

            // =========================================================
            // TRANSACTION HEADER
            // =========================================================

            Panel transHeader =
                new Panel();

            transHeader.Dock =
                DockStyle.Fill;

            transHeader.Margin =
                new Padding(0);

            transHeader.BackColor =
                Theme.Background;

            layout.Controls.Add(
                transHeader,
                0,
                3);

            Label recentTitle =
                Ui.FixedLabel(
                    "Recent transactions",
                    0,
                    14,
                    400,
                    38,
                    Theme.H1Font,
                    Theme.Text);

            transHeader.Controls.Add(
                recentTitle);

            Button refresh =
                Ui.GhostBtn(
                    "REFRESH",
                    0,
                    11,
                    94,
                    34);

            refresh.Click += delegate
            {
                RefreshData();
            };

            transHeader.Controls.Add(
                refresh);

            transHeader.Resize += delegate
            {
                refresh.Left =
                    Math.Max(
                        0,
                        transHeader.ClientSize.Width
                        - refresh.Width
                        - 4);
            };

            // =========================================================
            // TRANSACTION GRID
            // =========================================================

            _transactions =
                Ui.Grid(
                    0,
                    0,
                    100,
                    100);

            _transactions.Dock =
                DockStyle.Fill;

            _transactions.Margin =
                new Padding(0);

            layout.Controls.Add(
                _transactions,
                0,
                4);

            VisibleChanged += delegate
            {
                if (Visible)
                    RefreshData();
            };
        }

        // =============================================================
        // LOAD WALLET DATA
        // =============================================================

        public override async void RefreshData()
        {
            if (_loading
                ||
                Program.CurrentUser == null)
            {
                return;
            }

            _loading =
                true;

            try
            {
                int userId =
                    Program.CurrentUser.UserID;

                WalletData data =
                    await Task.Run(
                        delegate
                        {
                            WalletData d =
                                new WalletData();

                            d.Balance =
                                _wallet.GetBalance(
                                    userId);

                            d.Points =
                                _wallet.GetPoints(
                                    userId);

                            d.Transactions =
                                _wallet.GetTransactions(
                                    userId);

                            return d;
                        });

                _balance.Text =
                    "Tk "
                    +
                    data.Balance.ToString(
                        "N2");

                _points.Text =
                    data.Points.ToString(
                        "N0")
                    +
                    " pts";

                _transactions.DataSource =
                    data.Transactions;

                if (_transactions.Columns.Contains(
                    "Date"))
                {
                    _transactions
                        .Columns["Date"]
                        .DefaultCellStyle
                        .Format =
                        "dd MMM yyyy  hh:mm tt";
                }

                if (_transactions.Columns.Contains(
                    "Amount"))
                {
                    _transactions
                        .Columns["Amount"]
                        .DefaultCellStyle
                        .Format =
                        "N2";

                    _transactions
                        .Columns["Amount"]
                        .DefaultCellStyle
                        .Alignment =
                        DataGridViewContentAlignment
                            .MiddleRight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not refresh the wallet.\n"
                    +
                    ex.Message,
                    "Wallet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _loading =
                    false;
            }
        }

        // =============================================================
        // TOP UP
        // =============================================================

        private void TopUp_Click(
            object sender,
            EventArgs e)
        {
            if (Program.CurrentUser == null)
                return;

            decimal amount;

            if (!decimal.TryParse(
                    _amount.Text.Trim(),
                    out amount)
                ||
                amount < 1m
                ||
                amount > 1000000m)
            {
                MessageBox.Show(
                    "Enter a valid top-up amount between Tk 1 and Tk 1,000,000.",
                    "Wallet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                _amount.Focus();

                return;
            }

            if (_method.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Select a payment method first.",
                    "Wallet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string selectedMethod =
                _method.Text;

            // Before any wallet balance is changed,
            // show the payment details window.

            using (
                WalletTopUpPaymentForm payment =
                    new WalletTopUpPaymentForm(
                        selectedMethod,
                        amount))
            {
                if (payment.ShowDialog(
                        FindForm())
                    != DialogResult.OK)
                {
                    return;
                }
            }

            // Final confirmation before SQL update.

            DialogResult finalConfirm =
                MessageBox.Show(
                    "Confirm wallet top-up?\n\n"
                    +
                    "Amount: Tk "
                    +
                    amount.ToString("N2")
                    +
                    "\n"
                    +
                    "Payment method: "
                    +
                    selectedMethod
                    +
                    "\n\n"
                    +
                    "The wallet will be credited only after you confirm.",
                    "Confirm top-up",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (finalConfirm
                != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _continueButton.Enabled =
                    false;

                string transactionType =
                    "Topup/"
                    +
                    selectedMethod;

                _wallet.TopUp(
                    Program.CurrentUser.UserID,
                    amount,
                    transactionType);

                _amount.Clear();

                MessageBox.Show(
                    "Top-up completed.\n\nTk "
                    +
                    amount.ToString("N2")
                    +
                    " has been added to your GameHub wallet.",
                    "Wallet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RaiseGlobalDataChanged();

                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not top up the wallet.\n"
                    +
                    ex.Message,
                    "Wallet",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                _continueButton.Enabled =
                    true;
            }
        }

        // =============================================================
        // PAYMENT DETAILS WINDOW
        // =============================================================

        private sealed class WalletTopUpPaymentForm : Form
        {
            private readonly string _method;
            private readonly decimal _amount;

            private readonly Label _label1;
            private readonly Label _label2;
            private readonly Label _label3;

            private readonly TextBox _field1;
            private readonly TextBox _field2;
            private readonly TextBox _field3;

            public WalletTopUpPaymentForm(
                string method,
                decimal amount)
            {
                _method =
                    method
                    ??
                    string.Empty;

                _amount =
                    amount;

                Text =
                    "GameHub - Top Up Payment";

                StartPosition =
                    FormStartPosition.CenterParent;

                FormBorderStyle =
                    FormBorderStyle.FixedDialog;

                MaximizeBox =
                    false;

                MinimizeBox =
                    false;

                ShowInTaskbar =
                    false;

                ClientSize =
                    new Size(
                        540,
                        430);

                BackColor =
                    Theme.Background;

                ForeColor =
                    Theme.Text;

                Font =
                    Theme.BodyFont;

                // Header

                Controls.Add(
                    Ui.FixedLabel(
                        "Confirm your payment",
                        28,
                        22,
                        460,
                        38,
                        Theme.TitleFont,
                        Theme.Text));

                Controls.Add(
                    Ui.FixedLabel(
                        "Enter the required details for "
                        +
                        _method
                        +
                        ". These demo details are not stored.",
                        30,
                        62,
                        480,
                        42,
                        Theme.SmallFont,
                        Theme.Muted));

                // Amount summary card

                GamePanel summary =
                    Ui.GradientCard(
                        28,
                        112,
                        484,
                        76,
                        Theme.AccentDark,
                        Theme.Background2);

                Controls.Add(summary);

                summary.Controls.Add(
                    Ui.Lbl(
                        "TOP-UP AMOUNT",
                        18,
                        13,
                        Theme.SmallBold,
                        Theme.Cyan));

                summary.Controls.Add(
                    Ui.FixedLabel(
                        "Tk "
                        +
                        _amount.ToString("N2"),
                        18,
                        36,
                        260,
                        30,
                        new Font(
                            "Segoe UI",
                            16F,
                            FontStyle.Bold),
                        Color.White));

                summary.Controls.Add(
                    Ui.Pill(
                        _method.ToUpperInvariant(),
                        286,
                        24,
                        178,
                        Theme.SurfaceAlt,
                        Theme.Cream));

                // Field 1

                _label1 =
                    Ui.Lbl(
                        "Field 1",
                        30,
                        210,
                        Theme.SmallFont,
                        Theme.Muted);

                Controls.Add(
                    _label1);

                _field1 =
                    Ui.TextInput(
                        30,
                        233,
                        225,
                        32);

                Controls.Add(
                    _field1);

                // Field 2

                _label2 =
                    Ui.Lbl(
                        "Field 2",
                        280,
                        210,
                        Theme.SmallFont,
                        Theme.Muted);

                Controls.Add(
                    _label2);

                _field2 =
                    Ui.TextInput(
                        280,
                        233,
                        232,
                        32);

                Controls.Add(
                    _field2);

                // Field 3

                _label3 =
                    Ui.Lbl(
                        "Field 3",
                        30,
                        286,
                        Theme.SmallFont,
                        Theme.Muted);

                Controls.Add(
                    _label3);

                _field3 =
                    Ui.TextInput(
                        30,
                        309,
                        225,
                        32);

                Controls.Add(
                    _field3);

                // Cancel

                Button cancel =
                    Ui.GhostBtn(
                        "CANCEL",
                        278,
                        370,
                        106,
                        40);

                cancel.Click += delegate
                {
                    DialogResult =
                        DialogResult.Cancel;

                    Close();
                };

                Controls.Add(
                    cancel);

                // Verify

                Button confirm =
                    Ui.Btn(
                        "VERIFY & CONTINUE",
                        Theme.Green,
                        396,
                        370,
                        116,
                        40);

                confirm.Font =
                    new Font(
                        "Segoe UI",
                        7.8F,
                        FontStyle.Bold);

                confirm.Click +=
                    Confirm_Click;

                Controls.Add(
                    confirm);

                ConfigureFields();
            }

            // =========================================================
            // CHANGE INPUTS DEPENDING ON PAYMENT METHOD
            // =========================================================

            private void ConfigureFields()
            {
                _field1.PasswordChar =
                    '\0';

                _field2.PasswordChar =
                    '\0';

                _field3.PasswordChar =
                    '\0';

                _field1.MaxLength =
                    100;

                _field2.MaxLength =
                    100;

                _field3.MaxLength =
                    100;

                // bKash / Nagad

                if (_method == "bKash"
                    ||
                    _method == "Nagad")
                {
                    _label1.Text =
                        "Mobile number";

                    _label2.Text =
                        "Transaction ID";

                    _label3.Text =
                        "Account PIN (demo)";

                    _field1.MaxLength =
                        11;

                    _field2.MaxLength =
                        20;

                    _field3.MaxLength =
                        6;

                    _field3.PasswordChar =
                        '●';
                }

                // Card

                else if (
                    _method
                    ==
                    "Credit / Debit Card")
                {
                    _label1.Text =
                        "Card number";

                    _label2.Text =
                        "Expiry (MM/YY)";

                    _label3.Text =
                        "CVV";

                    _field1.MaxLength =
                        19;

                    _field2.MaxLength =
                        5;

                    _field3.MaxLength =
                        4;

                    _field3.PasswordChar =
                        '●';
                }

                // Bank

                else
                {
                    _label1.Text =
                        "Bank name";

                    _label2.Text =
                        "Account / transfer reference";

                    _label3.Text =
                        "Transaction ID";

                    _field1.MaxLength =
                        60;

                    _field2.MaxLength =
                        40;

                    _field3.MaxLength =
                        40;
                }
            }

            // =========================================================
            // VERIFY DETAILS
            // =========================================================

            private void Confirm_Click(
                object sender,
                EventArgs e)
            {
                string error =
                    ValidatePaymentFields();

                if (!string.IsNullOrEmpty(
                        error))
                {
                    MessageBox.Show(
                        error,
                        "Payment details",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                DialogResult =
                    DialogResult.OK;

                Close();
            }

            private string ValidatePaymentFields()
            {
                string f1 =
                    _field1.Text.Trim();

                string f2 =
                    _field2.Text.Trim();

                string f3 =
                    _field3.Text.Trim();

                // bKash / Nagad validation

                if (_method == "bKash"
                    ||
                    _method == "Nagad")
                {
                    string mobileDigits =
                        DigitsOnly(
                            f1);

                    if (mobileDigits.Length != 11
                        ||
                        !mobileDigits.StartsWith(
                            "01"))
                    {
                        return
                            "Enter a valid 11-digit Bangladesh mobile number beginning with 01.";
                    }

                    if (f2.Length < 6)
                    {
                        return
                            "Enter the mobile-banking transaction ID.";
                    }

                    if (f3.Length < 4)
                    {
                        return
                            "Enter the demo account PIN.";
                    }
                }

                // Card validation

                else if (
                    _method
                    ==
                    "Credit / Debit Card")
                {
                    string cardDigits =
                        DigitsOnly(
                            f1);

                    if (cardDigits.Length < 13
                        ||
                        cardDigits.Length > 19)
                    {
                        return
                            "Enter a valid card number containing 13 to 19 digits.";
                    }

                    if (!IsValidExpiry(
                            f2))
                    {
                        return
                            "Enter a valid non-expired date in MM/YY format.";
                    }

                    string cvv =
                        DigitsOnly(
                            f3);

                    if (cvv.Length < 3
                        ||
                        cvv.Length > 4)
                    {
                        return
                            "Enter a 3 or 4 digit CVV.";
                    }
                }

                // Bank validation

                else
                {
                    if (f1.Length < 2)
                    {
                        return
                            "Enter the bank name.";
                    }

                    if (f2.Length < 4)
                    {
                        return
                            "Enter the account or transfer reference.";
                    }

                    if (f3.Length < 4)
                    {
                        return
                            "Enter the transaction ID.";
                    }
                }

                return
                    string.Empty;
            }

            private static string DigitsOnly(
                string value)
            {
                if (string.IsNullOrEmpty(
                        value))
                {
                    return
                        string.Empty;
                }

                return
                    new string(
                        value
                            .Where(
                                char.IsDigit)
                            .ToArray());
            }

            private static bool IsValidExpiry(
                string value)
            {
                if (string.IsNullOrWhiteSpace(
                        value))
                {
                    return
                        false;
                }

                string[] parts =
                    value
                        .Trim()
                        .Split('/');

                if (parts.Length != 2
                    ||
                    parts[1].Length != 2)
                {
                    return
                        false;
                }

                int month;
                int year;

                if (!int.TryParse(
                        parts[0],
                        out month)
                    ||
                    month < 1
                    ||
                    month > 12)
                {
                    return
                        false;
                }

                if (!int.TryParse(
                        parts[1],
                        out year))
                {
                    return
                        false;
                }

                DateTime expiry =
                    new DateTime(
                        2000 + year,
                        month,
                        1)
                    .AddMonths(1)
                    .AddDays(-1);

                return
                    expiry.Date
                    >=
                    DateTime.Today;
            }
        }

        private sealed class WalletData
        {
            public decimal Balance;
            public int Points;
            public DataTable Transactions;
        }
    }
}