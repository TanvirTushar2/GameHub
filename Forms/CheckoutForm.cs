using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Models;

namespace GameHub.Forms
{
    /// <summary>
    /// Cart review, invoice calculation and confirmed payment.
    /// </summary>
    public partial class CheckoutForm : Form
    {
        private readonly List<CartItem> _cart;

        private readonly OrderRepository _orders =
            new OrderRepository();

        private readonly SubscriptionRepository _subs =
            new SubscriptionRepository();

        private readonly WalletRepository _wallet =
            new WalletRepository();

        private decimal _subtotal;
        private decimal _discountAmt;
        private decimal _vat;
        private decimal _total;

        private int _discountPct;

        private Label _lblMeta;
        private Label _lblTotals;
        private Label _lblPaymentHint;

        private Label _lblField1;
        private Label _lblField2;
        private Label _lblField3;

        private DataGridView _grid;
        private ComboBox _cboMethod;

        private TextBox _txtField1;
        private TextBox _txtField2;
        private TextBox _txtField3;

        private Button _payButton;

        // Required by Visual Studio Designer.
        public CheckoutForm()
        {
            _cart =
                new List<CartItem>();

            InitializeComponent();
        }

        // Runtime checkout constructor.
        public CheckoutForm(
            List<CartItem> cart)
        {
            _cart =
                cart
                ??
                new List<CartItem>();

            InitializeComponent();

            BuildUi();

            AppStyle.Apply(this);

            Compute();

            FillGrid();

            ConfigurePaymentFields();
        }

        private void BuildUi()
        {
            Controls.Clear();

            ClientSize =
                new Size(
                    900,
                    720);

            MinimumSize =
                new Size(
                    916,
                    759);

            StartPosition =
                FormStartPosition.CenterParent;

            Text =
                "GameHub - Checkout";

            Controls.Add(
                Ui.Lbl(
                    "Checkout",
                    28,
                    22,
                    new Font(
                        "Segoe UI",
                        18F,
                        FontStyle.Bold),
                    Theme.Text));

            Controls.Add(
                Ui.Lbl(
                    "Review your order, choose a payment method and confirm the purchase.",
                    30,
                    54,
                    Theme.SmallFont,
                    Theme.Muted));

            // ==========================================================
            // INVOICE
            // ==========================================================

            Panel invoice =
                Ui.Card(
                    28,
                    94,
                    844,
                    300);

            invoice.Anchor =
                AnchorStyles.Top
                |
                AnchorStyles.Left
                |
                AnchorStyles.Right;

            Controls.Add(
                invoice);

            invoice.Controls.Add(
                Ui.Pill(
                    "INVOICE",
                    18,
                    16,
                    72,
                    Theme.Cream,
                    Theme.Background2));

            _lblMeta =
                Ui.FixedLabel(
                    "",
                    112,
                    20,
                    690,
                    24,
                    Theme.SmallFont,
                    Theme.Muted);

            invoice.Controls.Add(
                _lblMeta);

            _grid =
                Ui.Grid(
                    18,
                    58,
                    808,
                    222);

            _grid.Anchor =
                AnchorStyles.Top
                |
                AnchorStyles.Bottom
                |
                AnchorStyles.Left
                |
                AnchorStyles.Right;

            invoice.Controls.Add(
                _grid);

            // ==========================================================
            // PAYMENT
            // ==========================================================

            Panel payment =
                Ui.Card(
                    28,
                    414,
                    474,
                    278);

            payment.Anchor =
                AnchorStyles.Top
                |
                AnchorStyles.Bottom
                |
                AnchorStyles.Left;

            Controls.Add(
                payment);

            payment.Controls.Add(
                Ui.Lbl(
                    "PAYMENT DETAILS",
                    18,
                    16,
                    Theme.SmallBold,
                    Theme.AccentBlue));

            payment.Controls.Add(
                Ui.Lbl(
                    "Payment method",
                    18,
                    47,
                    Theme.SmallFont,
                    Theme.Muted));

            _cboMethod =
                new ComboBox();

            _cboMethod.SetBounds(
                18,
                68,
                438,
                31);

            _cboMethod.DropDownStyle =
                ComboBoxStyle.DropDownList;

            _cboMethod.Items.AddRange(
                new object[]
                {
                    "Wallet",
                    "bKash",
                    "Nagad",
                    "Credit / Debit Card",
                    "Bank Transfer"
                });

            _cboMethod.SelectedIndex =
                0;

            _cboMethod.SelectedIndexChanged +=
                delegate
                {
                    ConfigurePaymentFields();
                };

            payment.Controls.Add(
                _cboMethod);

            _lblPaymentHint =
                Ui.FixedLabel(
                    "",
                    18,
                    106,
                    438,
                    36,
                    Theme.SmallFont,
                    Theme.Muted2);

            payment.Controls.Add(
                _lblPaymentHint);

            _lblField1 =
                Ui.Lbl(
                    "Field 1",
                    18,
                    150,
                    Theme.SmallFont,
                    Theme.Muted);

            payment.Controls.Add(
                _lblField1);

            _txtField1 =
                Ui.TextInput(
                    18,
                    171,
                    204,
                    31);

            payment.Controls.Add(
                _txtField1);

            _lblField2 =
                Ui.Lbl(
                    "Field 2",
                    240,
                    150,
                    Theme.SmallFont,
                    Theme.Muted);

            payment.Controls.Add(
                _lblField2);

            _txtField2 =
                Ui.TextInput(
                    240,
                    171,
                    216,
                    31);

            payment.Controls.Add(
                _txtField2);

            _lblField3 =
                Ui.Lbl(
                    "Field 3",
                    18,
                    211,
                    Theme.SmallFont,
                    Theme.Muted);

            payment.Controls.Add(
                _lblField3);

            _txtField3 =
                Ui.TextInput(
                    18,
                    232,
                    204,
                    31);

            payment.Controls.Add(
                _txtField3);

            // ==========================================================
            // ORDER SUMMARY
            // ==========================================================

            Panel totals =
                Ui.GradientCard(
                    522,
                    414,
                    350,
                    278,
                    Theme.AccentDark,
                    Theme.Background2);

            totals.Anchor =
                AnchorStyles.Top
                |
                AnchorStyles.Bottom
                |
                AnchorStyles.Left
                |
                AnchorStyles.Right;

            Controls.Add(
                totals);

            totals.Controls.Add(
                Ui.Lbl(
                    "ORDER SUMMARY",
                    18,
                    16,
                    Theme.SmallBold,
                    Theme.Cyan));

            _lblTotals =
                new Label();

            _lblTotals.SetBounds(
                18,
                50,
                314,
                130);

            _lblTotals.Font =
                new Font(
                    "Segoe UI",
                    10.5F);

            _lblTotals.ForeColor =
                Color.White;

            _lblTotals.BackColor =
                Color.Transparent;

            _lblTotals.AutoSize =
                false;

            totals.Controls.Add(
                _lblTotals);

            Label secureNote =
                Ui.FixedLabel(
                    "You will be asked to confirm before the order is created.",
                    18,
                    176,
                    314,
                    42,
                    Theme.SmallFont,
                    Color.FromArgb(
                        210,
                        230,
                        234,
                        255));

            totals.Controls.Add(
                secureNote);

            Button cancel =
                Ui.GhostBtn(
                    "← BACK",
                    18,
                    224,
                    104,
                    38);

            cancel.Click += delegate
            {
                DialogResult =
                    DialogResult.Cancel;

                Close();
            };

            totals.Controls.Add(
                cancel);

            _payButton =
                Ui.Btn(
                    "CONFIRM PURCHASE",
                    Theme.Cream,
                    138,
                    224,
                    194,
                    38);

            _payButton.ForeColor =
                Theme.Background2;

            _payButton.Click +=
                Pay_Click;

            totals.Controls.Add(
                _payButton);

            Resize += delegate
            {
                invoice.Width =
                    ClientSize.Width
                    - 56;

                _lblMeta.Width =
                    Math.Max(
                        240,
                        invoice.ClientSize.Width
                        - 130);

                _grid.Width =
                    Math.Max(
                        300,
                        invoice.ClientSize.Width
                        - 36);

                int rightWidth =
                    ClientSize.Width
                    - totals.Left
                    - 28;

                totals.Width =
                    Math.Max(
                        300,
                        rightWidth);

                _lblTotals.Width =
                    Math.Max(
                        250,
                        totals.ClientSize.Width
                        - 36);

                secureNote.Width =
                    Math.Max(
                        250,
                        totals.ClientSize.Width
                        - 36);

                _payButton.Left =
                    Math.Max(
                        138,
                        totals.ClientSize.Width
                        -
                        _payButton.Width
                        -
                        18);
            };
        }

        // ==============================================================
        // CALCULATIONS
        // ==============================================================

        private void Compute()
        {
            _subtotal =
                0m;

            foreach (
                CartItem c
                in _cart)
            {
                if (c != null)
                {
                    _subtotal +=
                        c.Subtotal;
                }
            }

            _discountPct =
                0;

            if (Program.CurrentUser
                != null)
            {
                try
                {
                    _discountPct =
                        _subs
                        .GetActiveDiscount(
                            Program
                                .CurrentUser
                                .UserID);
                }
                catch
                {
                    _discountPct =
                        0;
                }
            }

            _discountAmt =
                Math.Round(
                    _subtotal
                    *
                    _discountPct
                    /
                    100m,
                    2);

            decimal afterDiscount =
                _subtotal
                -
                _discountAmt;

            _vat =
                Math.Round(
                    afterDiscount
                    *
                    0.05m,
                    2);

            _total =
                afterDiscount
                +
                _vat;

            _lblMeta.Text =
                "Customer: "
                +
                (
                    Program.CurrentUser == null
                    ?
                    "-"
                    :
                    Program.CurrentUser.Username
                )
                +
                "     •     "
                +
                DateTime.Now.ToString(
                    "dd MMM yyyy");

            _lblTotals.Text =
                "Subtotal             Tk "
                +
                _subtotal.ToString(
                    "N2")
                +
                "\n"
                +
                "Membership "
                +
                _discountPct
                +
                "%     - Tk "
                +
                _discountAmt.ToString(
                    "N2")
                +
                "\n"
                +
                "VAT 5%               Tk "
                +
                _vat.ToString(
                    "N2")
                +
                "\n\n"
                +
                "TOTAL                Tk "
                +
                _total.ToString(
                    "N2");
        }

        private void FillGrid()
        {
            DataTable dt =
                new DataTable();

            dt.Columns.Add(
                "Game");

            dt.Columns.Add(
                "Qty");

            dt.Columns.Add(
                "Unit Price");

            dt.Columns.Add(
                "Subtotal");

            foreach (
                CartItem c
                in _cart)
            {
                if (c == null
                    ||
                    c.Game == null)
                {
                    continue;
                }

                dt.Rows.Add(
                    c.Game.Title,
                    c.Quantity,
                    "Tk "
                    +
                    c.Game.Price.ToString(
                        "N2"),
                    "Tk "
                    +
                    c.Subtotal.ToString(
                        "N2"));
            }

            _grid.DataSource =
                dt;
        }

        // ==============================================================
        // PAYMENT METHOD UI
        // ==============================================================

        private void ConfigurePaymentFields()
        {
            if (_cboMethod == null
                ||
                _txtField1 == null)
            {
                return;
            }

            string method =
                _cboMethod.Text;

            _txtField1.Clear();
            _txtField2.Clear();
            _txtField3.Clear();

            _txtField1.PasswordChar =
                '\0';

            _txtField2.PasswordChar =
                '\0';

            _txtField3.PasswordChar =
                '\0';

            _txtField1.MaxLength =
                100;

            _txtField2.MaxLength =
                100;

            _txtField3.MaxLength =
                100;

            bool showFields =
                method != "Wallet";

            _lblField1.Visible =
                showFields;

            _lblField2.Visible =
                showFields;

            _lblField3.Visible =
                showFields;

            _txtField1.Visible =
                showFields;

            _txtField2.Visible =
                showFields;

            _txtField3.Visible =
                showFields;

            // WALLET
            if (method == "Wallet")
            {
                decimal balance =
                    0m;

                if (Program.CurrentUser
                    != null)
                {
                    try
                    {
                        balance =
                            _wallet.GetBalance(
                                Program
                                    .CurrentUser
                                    .UserID);
                    }
                    catch
                    {
                        balance =
                            0m;
                    }
                }

                _lblPaymentHint.Text =
                    "Available wallet balance: Tk "
                    +
                    balance.ToString("N2")
                    +
                    ". The wallet is charged in the same SQL transaction as the order.";
            }

            // BKASH / NAGAD
            else if (
                method == "bKash"
                ||
                method == "Nagad")
            {
                _lblPaymentHint.Text =
                    "Simulated mobile-banking payment. Enter the number, transaction ID and demo PIN.";

                _lblField1.Text =
                    "Mobile number";

                _lblField2.Text =
                    "Transaction ID";

                _lblField3.Text =
                    "Account PIN (demo)";

                _txtField1.MaxLength =
                    11;

                _txtField2.MaxLength =
                    20;

                _txtField3.MaxLength =
                    6;

                _txtField3.PasswordChar =
                    '●';
            }

            // CARD
            else if (
                method
                ==
                "Credit / Debit Card")
            {
                _lblPaymentHint.Text =
                    "Simulated card payment. Card details are validated locally and are not stored.";

                _lblField1.Text =
                    "Card number";

                _lblField2.Text =
                    "Expiry (MM/YY)";

                _lblField3.Text =
                    "CVV";

                _txtField1.MaxLength =
                    19;

                _txtField2.MaxLength =
                    5;

                _txtField3.MaxLength =
                    4;

                _txtField3.PasswordChar =
                    '●';
            }

            // BANK
            else
            {
                _lblPaymentHint.Text =
                    "Simulated bank transfer. Enter the bank name and transfer references.";

                _lblField1.Text =
                    "Bank name";

                _lblField2.Text =
                    "Account / transfer reference";

                _lblField3.Text =
                    "Transaction ID";

                _txtField1.MaxLength =
                    60;

                _txtField2.MaxLength =
                    40;

                _txtField3.MaxLength =
                    40;
            }
        }

        // ==============================================================
        // CONFIRM PURCHASE
        // ==============================================================

        private void Pay_Click(
            object sender,
            EventArgs e)
        {
            if (Program.CurrentUser
                == null)
            {
                MessageBox.Show(
                    "Your login session is no longer available. Please sign in again.",
                    "Checkout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                Close();

                return;
            }

            if (_cart.Count == 0)
            {
                MessageBox.Show(
                    "Your cart is empty.",
                    "Checkout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string method =
                _cboMethod.Text;

            if (string.IsNullOrWhiteSpace(
                    method))
            {
                MessageBox.Show(
                    "Choose a payment method.",
                    "Checkout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                // ======================================================
                // WALLET PAYMENT CHECK
                // ======================================================

                if (method == "Wallet")
                {
                    decimal balance =
                        _wallet.GetBalance(
                            Program
                                .CurrentUser
                                .UserID);

                    if (balance < _total)
                    {
                        MessageBox.Show(
                            "Insufficient wallet balance.\n\n"
                            +
                            "Available: Tk "
                            +
                            balance.ToString("N2")
                            +
                            "\n"
                            +
                            "Required:  Tk "
                            +
                            _total.ToString("N2")
                            +
                            "\n\n"
                            +
                            "Open Wallet from the launcher if you want to add funds.",
                            "Payment",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }
                }
                else
                {
                    string validationError =
                        ValidateExternalPaymentDetails(
                            method);

                    if (!string.IsNullOrEmpty(
                            validationError))
                    {
                        MessageBox.Show(
                            validationError,
                            "Payment details",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }
                }

                // ======================================================
                // FINAL CONFIRMATION
                // ======================================================

                DialogResult confirm =
                    MessageBox.Show(
                        "Please confirm this purchase.\n\n"
                        +
                        "Items: "
                        +
                        GetTotalQuantity()
                        +
                        "\n"
                        +
                        "Payment method: "
                        +
                        method
                        +
                        "\n"
                        +
                        "Final total: Tk "
                        +
                        _total.ToString("N2")
                        +
                        "\n\n"
                        +
                        (
                            method == "Wallet"
                            ?
                            "Your wallet will be charged after you click Yes."
                            :
                            "This project uses a simulated external payment confirmation."
                        ),
                        "Confirm purchase",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (confirm
                    != DialogResult.Yes)
                {
                    return;
                }

                _payButton.Enabled =
                    false;

                _cboMethod.Enabled =
                    false;

                List<string> keys;

                int orderId =
                    _orders.PlaceOrder(
                        Program
                            .CurrentUser
                            .UserID,
                        _cart,
                        _total,
                        method,
                        out keys);

                string keyList =
                    keys == null
                    ||
                    keys.Count == 0
                    ?
                    "No activation key was returned."
                    :
                    string.Join(
                        "\n",
                        keys.ToArray());

                // ======================================================
                // SUCCESS
                // ======================================================

                MessageBox.Show(
                    "PURCHASE SUCCESSFUL\n\n"
                    +
                    "Order #: "
                    +
                    orderId
                    +
                    "\n"
                    +
                    "Status: Paid"
                    +
                    "\n"
                    +
                    "Payment: "
                    +
                    method
                    +
                    "\n"
                    +
                    "Total: Tk "
                    +
                    _total.ToString("N2")
                    +
                    "\n\n"
                    +
                    "Activation keys:\n"
                    +
                    keyList
                    +
                    "\n\n"
                    +
                    "The purchased game is now available in your Library.",
                    "Purchase complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult =
                    DialogResult.OK;

                Close();
            }
            catch (Exception ex)
            {
                _payButton.Enabled =
                    true;

                _cboMethod.Enabled =
                    true;

                MessageBox.Show(
                    "Checkout failed. The order was not completed.\n\n"
                    +
                    ex.Message,
                    "Checkout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==============================================================
        // PAYMENT VALIDATION
        // ==============================================================

        private string ValidateExternalPaymentDetails(
            string method)
        {
            string f1 =
                _txtField1.Text.Trim();

            string f2 =
                _txtField2.Text.Trim();

            string f3 =
                _txtField3.Text.Trim();

            // BKASH / NAGAD
            if (method == "bKash"
                ||
                method == "Nagad")
            {
                string mobile =
                    DigitsOnly(f1);

                if (mobile.Length != 11
                    ||
                    !mobile.StartsWith(
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

            // CARD
            else if (
                method
                ==
                "Credit / Debit Card")
            {
                string card =
                    DigitsOnly(f1);

                if (card.Length < 13
                    ||
                    card.Length > 19)
                {
                    return
                        "Enter a valid card number containing 13 to 19 digits.";
                }

                if (!IsValidExpiry(f2))
                {
                    return
                        "Enter a valid, non-expired card date in MM/YY format.";
                }

                string cvv =
                    DigitsOnly(f3);

                if (cvv.Length < 3
                    ||
                    cvv.Length > 4)
                {
                    return
                        "Enter a 3 or 4 digit CVV.";
                }
            }

            // BANK
            else if (
                method
                ==
                "Bank Transfer")
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
                        "Enter the bank transaction ID.";
                }
            }

            return string.Empty;
        }

        private int GetTotalQuantity()
        {
            int totalQuantity =
                0;

            foreach (
                CartItem item
                in _cart)
            {
                if (item != null)
                {
                    totalQuantity +=
                        item.Quantity;
                }
            }

            return totalQuantity;
        }

        private static string DigitsOnly(
            string value)
        {
            if (string.IsNullOrEmpty(
                    value))
            {
                return string.Empty;
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
                return false;
            }

            string[] parts =
                value
                    .Trim()
                    .Split('/');

            if (parts.Length != 2
                ||
                parts[1].Length != 2)
            {
                return false;
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
                return false;
            }

            if (!int.TryParse(
                    parts[1],
                    out year))
            {
                return false;
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
}