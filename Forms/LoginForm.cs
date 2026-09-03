using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Forms.Controls;
using GameHub.Models;

namespace GameHub.Forms
{
    public partial class LoginForm : Form
    {
        private readonly UserRepository _users = new UserRepository();
        private GamePanel _shell;
        private LinkLabel _forgot;
        private CheckBox _showPassword;

        public LoginForm()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            BuildUi();
            AppStyle.Apply(this, true);
            MinimumSize = new Size(1180, 720);
            WindowState = FormWindowState.Maximized;
            Resize += delegate { CenterShell(); };
            Shown += delegate { CenterShell(); txtUserId.Focus(); };
        }

        private void BuildUi()
        {
            Controls.Clear();
            Text = "GameHub - Sign In";
            ClientSize = new Size(1360, 820);
            BackColor = Theme.Background;

            _shell = Ui.Card(0, 0, 1120, 650);
            _shell.StartColor = Theme.Background2;
            _shell.OutlineColor = Theme.Border;
            _shell.CornerRadius = 26;
            Controls.Add(_shell);

            Panel formSide = new Panel();
            formSide.SetBounds(0, 0, 500, 650);
            formSide.BackColor = Theme.Background2;
            _shell.Controls.Add(formSide);

            AuthArtworkPanel artwork = new AuthArtworkPanel();
            artwork.SetBounds(500, 0, 620, 650);
            artwork.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _shell.Controls.Add(artwork);

            GamePanel logo = Ui.GradientCard(48, 38, 46, 46, Theme.AccentBlue, Theme.AccentDark);
            formSide.Controls.Add(logo);
            Label mark = Ui.FixedLabel("G", 0, 5, 46, 34, new Font("Segoe UI", 16F, FontStyle.Bold), Color.White);
            mark.TextAlign = ContentAlignment.MiddleCenter;
            logo.Controls.Add(mark);
            formSide.Controls.Add(Ui.Lbl("GameHub", 108, 47, new Font("Segoe UI", 14F, FontStyle.Bold), Theme.Text));

            formSide.Controls.Add(Ui.Pill("WELCOME BACK", 48, 118, 112, Theme.SurfaceAlt, Theme.Cyan));
            lblAppName = Ui.FixedLabel("Sign in", 48, 156, 402, 58,
                new Font("Segoe UI", 27F, FontStyle.Bold), Theme.Text);
            formSide.Controls.Add(lblAppName);
            lblSub = Ui.FixedLabel("Enter your GameHub username/email and password.",
                50, 218, 400, 38, Theme.BodyFont, Theme.Muted);
            formSide.Controls.Add(lblSub);

            lblUserId = Ui.Lbl("Username or Email", 50, 266, Theme.SmallBold, Theme.Cream);
            formSide.Controls.Add(lblUserId);
            txtUserId = CreateInput(formSide, 50, 290, 400, false);
            txtUserId.TabIndex = 0;

            lblPassword = Ui.Lbl("Password", 50, 354, Theme.SmallBold, Theme.Cream);
            formSide.Controls.Add(lblPassword);
            txtPassword = CreateInput(formSide, 50, 378, 400, true);
            txtPassword.TabIndex = 1;

            _showPassword = new CheckBox();
            _showPassword.Text = "Show password";
            _showPassword.SetBounds(50, 433, 140, 24);
            _showPassword.CheckedChanged += delegate
            {
                txtPassword.PasswordChar = _showPassword.Checked ? '\0' : '●';
            };
            formSide.Controls.Add(_showPassword);

            _forgot = new LinkLabel();
            _forgot.Text = "Forgot password?";
            _forgot.Font = Theme.SmallBold;
            _forgot.AutoSize = true;
            _forgot.Location = new Point(334, 436);
            _forgot.LinkClicked += ForgotPassword_LinkClicked;
            formSide.Controls.Add(_forgot);

            btnLogin = Ui.Btn("SIGN IN", Theme.Accent, 50, 478, 400, 44);
            btnLogin.TabIndex = 2;
            btnLogin.Click += btnLogin_Click;
            formSide.Controls.Add(btnLogin);
            AcceptButton = btnLogin;

            lnkRegister = new LinkLabel();

            lnkRegister.Text =
                "New to GameHub? Create account";

            lnkRegister.Font =
                Theme.BodyFont;

            lnkRegister.ForeColor =
                Theme.Muted;

            lnkRegister.LinkColor =
                Theme.AccentBlue;

            lnkRegister.ActiveLinkColor =
                Theme.Cyan;

            lnkRegister.VisitedLinkColor =
                Theme.AccentBlue;

            lnkRegister.AutoSize = false;

            lnkRegister.SetBounds(
                50,
                548,
                400,
                30);

        
            lnkRegister.LinkArea =
                new LinkArea(16, 14);

            lnkRegister.LinkClicked +=
                lnkRegister_LinkClicked;

            formSide.Controls.Add(lnkRegister);

            lblBrand = Ui.Lbl(
                "GameHub secure desktop authentication",
                50,
                592,
                Theme.SmallFont,
                Theme.Muted2); formSide.Controls.Add(lblBrand);

            CenterShell();
        }

        private static TextBox CreateInput(Control parent, int x, int y, int width, bool password)
        {
            GamePanel shell = Ui.Card(x, y, width, 44);
            shell.StartColor = Theme.SurfaceAlt;
            shell.OutlineColor = Theme.BorderSoft;
            shell.CornerRadius = 12;
            parent.Controls.Add(shell);

            TextBox box = new TextBox();
            box.BorderStyle = BorderStyle.None;
            box.BackColor = Theme.SurfaceAlt;
            box.ForeColor = Theme.Text;
            box.Font = new Font("Segoe UI", 10.5F);
            box.SetBounds(14, 12, width - 28, 24);
            if (password) box.PasswordChar = '●';
            shell.Controls.Add(box);
            return box;
        }

        private void CenterShell()
        {
            if (_shell == null) return;
            _shell.Left = Math.Max(18, (ClientSize.Width - _shell.Width) / 2);
            _shell.Top = Math.Max(18, (ClientSize.Height - _shell.Height) / 2);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string identifier = txtUserId.Text.Trim();
            string password = txtPassword.Text;

            if (identifier.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Please enter your username/email and password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "SIGNING IN...";
            try
            {
                User user = _users.Login(identifier, password);
                if (user == null)
                {
                    MessageBox.Show("Invalid credentials or the account is inactive.", "Login failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Program.CurrentUser = user;

                try { new AuditRepository().Log(user.UserID, user.Username, "Login", "Signed in to GameHub"); } catch { }
                OpenHome(user);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not complete login.\n\n" + ex.Message,
                    "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "SIGN IN";
            }
        }

        private void OpenHome(User user)
        {
            Form home = user.GetHomeForm() == "CustomerDashboard"
                ? (Form)new CustomerDashboardForm()
                : new DashboardForm();

            Hide();
            using (home) home.ShowDialog(this);

            Program.CurrentUser = null;
            txtPassword.Clear();
            _showPassword.Checked = false;
            Show();
            WindowState = FormWindowState.Maximized;
            CenterShell();
            txtUserId.Focus();
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (RegisterForm register = new RegisterForm()) register.ShowDialog(this);
        }

        private void ForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string identifier = txtUserId.Text.Trim();
            if (identifier.Length == 0)
            {
                MessageBox.Show("Enter your username or email first, then select Forgot password.",
                    "Password recovery", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                DataTable data = _users.FindAccountForRecovery(identifier);
                if (data.Rows.Count == 0)
                {
                    MessageBox.Show("No GameHub account was found for that username/email.",
                        "Password recovery", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataRow row = data.Rows[0];
                if (!Convert.ToBoolean(row["IsActive"]))
                {
                    MessageBox.Show("This account is inactive. Contact a GameHub administrator.",
                        "Password recovery", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string email = row["Email"] == DBNull.Value ? string.Empty : Convert.ToString(row["Email"]);
                string phone = row["Phone"] == DBNull.Value ? string.Empty : Convert.ToString(row["Phone"]);
                string contact = string.Empty;
                if (!string.IsNullOrWhiteSpace(email)) contact += "Registered email: " + MaskEmail(email) + "\n";
                if (!string.IsNullOrWhiteSpace(phone)) contact += "Registered phone: " + MaskPhone(phone) + "\n";
                if (contact.Length == 0) contact = "No recovery contact is stored for this account.\n";

                MessageBox.Show(contact +
                    "\nSelf-service email/OTP reset is not configured. An Admin can reset the password from Users & Staff.",
                    "Password recovery", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not check the account.\n" + ex.Message,
                    "Password recovery", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string MaskEmail(string email)
        {
            int at = email.IndexOf('@');
            if (at <= 1) return "***" + (at >= 0 ? email.Substring(at) : string.Empty);
            return email.Substring(0, 1) + new string('*', Math.Min(5, at - 1)) + email.Substring(at);
        }

        private static string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length <= 4) return "****";
            return new string('*', Math.Max(4, phone.Length - 4)) + phone.Substring(phone.Length - 4);
        }
    }
}
