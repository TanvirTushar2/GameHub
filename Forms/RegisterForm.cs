using System;
using System.Drawing;
using System.Net.Mail;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Forms.Controls;

namespace GameHub.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly UserRepository _users = new UserRepository();
        private TextBox _txtName;
        private TextBox _txtUser;
        private TextBox _txtEmail;
        private TextBox _txtPhone;
        private TextBox _txtPass;
        private TextBox _txtConfirm;

        public RegisterForm()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            BuildUi();
            AppStyle.Apply(this, true);
            MinimumSize = new Size(1120, 720);
        }

        private void BuildUi()
        {
            Controls.Clear();
            Text = "GameHub - Create Account";
            ClientSize = new Size(1180, 720);
            BackColor = Theme.Background;

            GamePanel shell = Ui.Card(24, 24, 1132, 672);
            shell.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            shell.StartColor = Theme.Background2;
            shell.OutlineColor = Theme.Border;
            shell.CornerRadius = 26;
            Controls.Add(shell);

            Panel formSide = new Panel();
            formSide.SetBounds(0, 0, 548, 672);
            formSide.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            formSide.BackColor = Theme.Background2;
            shell.Controls.Add(formSide);

            AuthArtworkPanel artwork = new AuthArtworkPanel();
            artwork.SetBounds(548, 0, 584, 672);
            artwork.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            shell.Controls.Add(artwork);

            GamePanel logo = Ui.GradientCard(42, 28, 44, 44, Theme.AccentBlue, Theme.AccentDark);
            formSide.Controls.Add(logo);
            Label mark = Ui.FixedLabel("G", 0, 5, 44, 32, new Font("Segoe UI", 15F, FontStyle.Bold), Color.White);
            mark.TextAlign = ContentAlignment.MiddleCenter;
            logo.Controls.Add(mark);
            formSide.Controls.Add(Ui.Lbl("GameHub", 100, 37, new Font("Segoe UI", 13F, FontStyle.Bold), Theme.Text));

            formSide.Controls.Add(Ui.Pill("START FOR FREE", 42, 88, 114, Theme.SurfaceAlt, Theme.Cyan));
            formSide.Controls.Add(Ui.FixedLabel("Create account", 42, 123, 462, 58,
                new Font("Segoe UI", 25F, FontStyle.Bold), Theme.Text));
            formSide.Controls.Add(Ui.FixedLabel("Every public registration is safely created as Customer.",
                44, 185, 462, 30, Theme.SmallFont, Theme.Muted));

            _txtName = CreateInput(formSide, "Full Name *", 42, 224, 222, false);
            _txtUser = CreateInput(formSide, "Username *", 282, 224, 222, false);
            _txtEmail = CreateInput(formSide, "Email *", 42, 298, 462, false);
            _txtPhone = CreateInput(formSide, "Phone *", 42, 372, 462, false);
            _txtPass = CreateInput(formSide, "Password *", 42, 446, 222, true);
            _txtConfirm = CreateInput(formSide, "Confirm Password *", 282, 446, 222, true);

            CheckBox show = new CheckBox();
            show.Text = "Show passwords";
            show.SetBounds(42, 518, 150, 24);
            show.CheckedChanged += delegate
            {
                char c = show.Checked ? '\0' : '●';
                _txtPass.PasswordChar = c;
                _txtConfirm.PasswordChar = c;
            };
            formSide.Controls.Add(show);

            Button create = Ui.Btn("CREATE ACCOUNT", Theme.Accent, 42, 554, 462, 44);
            create.Click += Register_Click;
            formSide.Controls.Add(create);
            AcceptButton = create;

            LinkLabel login = new LinkLabel();

            login.Text =
                "Already a member? Back to Sign In";

            login.Font =
                Theme.BodyFont;

            login.ForeColor =
                Theme.Muted;

            login.LinkColor =
                Theme.AccentBlue;

            login.ActiveLinkColor =
                Theme.Cyan;

            login.VisitedLinkColor =
                Theme.AccentBlue;

            login.AutoSize = false;

            login.SetBounds(
                42,
                614,
                462,
                32);

         
            login.LinkArea =
                new LinkArea(18, 15);

            login.LinkClicked += delegate
            {
                Close();
            };

            formSide.Controls.Add(login);
        }

        private static TextBox CreateInput(Control parent, string label, int x, int y, int width, bool password)
        {
            parent.Controls.Add(Ui.Lbl(label, x, y, Theme.SmallBold, Theme.Cream));
            GamePanel shell = Ui.Card(x, y + 22, width, 42);
            shell.StartColor = Theme.SurfaceAlt;
            shell.OutlineColor = Theme.BorderSoft;
            shell.CornerRadius = 11;
            parent.Controls.Add(shell);

            TextBox box = new TextBox();
            box.BorderStyle = BorderStyle.None;
            box.BackColor = Theme.SurfaceAlt;
            box.ForeColor = Theme.Text;
            box.Font = new Font("Segoe UI", 10F);
            box.SetBounds(13, 11, width - 26, 24);
            if (password) box.PasswordChar = '●';
            shell.Controls.Add(box);
            return box;
        }

        private void Register_Click(object sender, EventArgs e)
        {
            string fullName = _txtName.Text.Trim();
            string username = _txtUser.Text.Trim();
            string email = _txtEmail.Text.Trim();
            string phone = _txtPhone.Text.Trim();
            string password = _txtPass.Text;

            if (fullName.Length == 0 || username.Length == 0 || email.Length == 0 ||
                phone.Length == 0 || password.Length == 0 || _txtConfirm.Text.Length == 0)
            {
                MessageBox.Show("Please complete all required fields.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (fullName.Length > 100)
            {
                MessageBox.Show("Full Name cannot exceed 100 characters.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (username.Length < 3 || username.Length > 50 || username.Contains(" "))
            {
                MessageBox.Show("Username must be 3-50 characters and cannot contain spaces.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidEmail(email) || email.Length > 100)
            {
                MessageBox.Show("Enter a valid email address.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (phone.Length > 20 || !IsValidPhone(phone))
            {
                MessageBox.Show("Enter a valid phone number (maximum 20 characters).", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (password != _txtConfirm.Text)
            {
                MessageBox.Show("Password and Confirm Password do not match.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_users.UsernameExists(username))
                {
                    MessageBox.Show("That username is already taken.", "Registration",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_users.EmailExists(email))
                {
                    MessageBox.Show("That email address is already used by another account.", "Registration",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _users.Register(username, password, fullName, email, phone);
                MessageBox.Show("Account created successfully.\n\nYour role is Customer. You can now sign in.",
                    "Welcome to GameHub", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the account.\n" + ex.Message,
                    "Registration error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        private static bool IsValidPhone(string phone)
        {
            int digits = 0;
            foreach (char c in phone)
            {
                if (char.IsDigit(c)) digits++;
                else if (c != '+' && c != '-' && c != ' ' && c != '(' && c != ')') return false;
            }
            return digits >= 6;
        }
    }
}
