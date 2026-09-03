using System;
using System.Drawing;
using System.Net.Mail;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Models;

namespace GameHub.Forms
{
    public partial class ProfileForm : Form
    {
        private readonly UserRepository _users = new UserRepository();
        private TextBox _name, _email, _phone;
        private TextBox _oldPassword, _newPassword, _confirmPassword;
        private Label _username, _role;

        public ProfileForm()
        {
            InitializeComponent();
            Controls.Clear();
            BuildUi();
            AppStyle.Apply(this);
            LoadProfile();
        }

        private void BuildUi()
        {
            ClientSize = new Size(900, 620);
            Text = "GameHub - Profile";

            Controls.Add(Ui.Lbl("Profile", 28, 22,
                new Font("Segoe UI", 18F, FontStyle.Bold), Theme.Text));
            Controls.Add(Ui.Lbl("Update your contact information and account password.",
                30, 54, Theme.SmallFont, Theme.Muted));
            Controls.Add(Ui.Back(this));

            Panel info = Ui.Card(28, 94, 404, 498);
            Controls.Add(info);
            info.Controls.Add(Ui.Pill("ACCOUNT", 18, 16, 78, Theme.Cream, Theme.Background2));
            info.Controls.Add(Ui.Lbl("Profile Information", 18, 54,
                new Font("Segoe UI", 13F, FontStyle.Bold), Theme.Text));

            info.Controls.Add(Ui.Lbl("Username", 18, 102, Theme.BodyFont, Theme.Text));
            _username = Ui.Lbl("-", 18, 126, new Font("Segoe UI", 11F, FontStyle.Bold), Theme.Muted);
            info.Controls.Add(_username);

            info.Controls.Add(Ui.Lbl("Role", 220, 102, Theme.BodyFont, Theme.Text));
            _role = Ui.Lbl("-", 220, 126, new Font("Segoe UI", 11F, FontStyle.Bold), Theme.Accent);
            info.Controls.Add(_role);

            info.Controls.Add(Ui.Lbl("Full Name", 18, 176, Theme.BodyFont, Theme.Text));
            _name = Ui.TextInput(18, 200, 368, 30); info.Controls.Add(_name);
            info.Controls.Add(Ui.Lbl("Email", 18, 246, Theme.BodyFont, Theme.Text));
            _email = Ui.TextInput(18, 270, 368, 30); info.Controls.Add(_email);
            info.Controls.Add(Ui.Lbl("Phone", 18, 316, Theme.BodyFont, Theme.Text));
            _phone = Ui.TextInput(18, 340, 368, 30); info.Controls.Add(_phone);

            Button save = Ui.Btn("SAVE PROFILE", Theme.Accent, 18, 410, 368, 40);
            save.Click += SaveProfile_Click;
            info.Controls.Add(save);

            Panel password = Ui.Card(450, 94, 422, 498);
            Controls.Add(password);
            password.Controls.Add(Ui.Pill("SECURITY", 18, 16, 82, Theme.Purple, Color.White));
            password.Controls.Add(Ui.Lbl("Change Password", 18, 54,
                new Font("Segoe UI", 13F, FontStyle.Bold), Theme.Text));
            password.Controls.Add(Ui.Lbl("Your current password is required.", 18, 82,
                Theme.SmallFont, Theme.Muted));

            password.Controls.Add(Ui.Lbl("Current Password", 18, 128, Theme.BodyFont, Theme.Text));
            _oldPassword = Ui.TextInput(18, 152, 386, 30); _oldPassword.PasswordChar = '●'; password.Controls.Add(_oldPassword);
            password.Controls.Add(Ui.Lbl("New Password", 18, 204, Theme.BodyFont, Theme.Text));
            _newPassword = Ui.TextInput(18, 228, 386, 30); _newPassword.PasswordChar = '●'; password.Controls.Add(_newPassword);
            password.Controls.Add(Ui.Lbl("Confirm New Password", 18, 280, Theme.BodyFont, Theme.Text));
            _confirmPassword = Ui.TextInput(18, 304, 386, 30); _confirmPassword.PasswordChar = '●'; password.Controls.Add(_confirmPassword);

            CheckBox show = new CheckBox();
            show.Text = "Show passwords";
            show.SetBounds(18, 350, 150, 24);
            show.CheckedChanged += (s, e) =>
            {
                char c = show.Checked ? '\0' : '●';
                _oldPassword.PasswordChar = c;
                _newPassword.PasswordChar = c;
                _confirmPassword.PasswordChar = c;
            };
            password.Controls.Add(show);

            Button change = Ui.Btn("CHANGE PASSWORD", Theme.Purple, 18, 410, 386, 40);
            change.Click += ChangePassword_Click;
            password.Controls.Add(change);
        }

        private void LoadProfile()
        {
            if (Program.CurrentUser == null) return;
            _username.Text = Program.CurrentUser.Username;
            _role.Text = Program.CurrentUser.Role;
            _name.Text = Program.CurrentUser.FullName;
            _email.Text = Program.CurrentUser.Email;
            _phone.Text = Program.CurrentUser.Phone;
        }

        private void SaveProfile_Click(object sender, EventArgs e)
        {
            string name = _name.Text.Trim();
            string email = _email.Text.Trim();
            string phone = _phone.Text.Trim();

            if (name.Length == 0)
            {
                MessageBox.Show("Full Name is required.");
                return;
            }
            if (name.Length > 100 || email.Length > 100 || phone.Length > 20)
            {
                MessageBox.Show("One or more fields exceed the database length limit.");
                return;
            }
            if (email.Length > 0 && !ValidEmail(email))
            {
                MessageBox.Show("Enter a valid email address.");
                return;
            }
            if (phone.Length > 0 && !ValidPhone(phone))
            {
                MessageBox.Show("Enter a valid phone number.");
                return;
            }

            try
            {
                if (_users.EmailUsedByAnotherUser(email, Program.CurrentUser.UserID))
                {
                    MessageBox.Show("That email address is already used by another account.");
                    return;
                }

                _users.UpdateProfile(Program.CurrentUser.UserID, name, email, phone);
                User refreshed = _users.GetUserById(Program.CurrentUser.UserID);
                if (refreshed != null) Program.CurrentUser = refreshed;
                LoadProfile();
                MessageBox.Show("Profile updated successfully.", "GameHub",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangePassword_Click(object sender, EventArgs e)
        {
            if (_oldPassword.Text.Length == 0 || _newPassword.Text.Length == 0 || _confirmPassword.Text.Length == 0)
            {
                MessageBox.Show("Complete all password fields.");
                return;
            }
            if (_newPassword.Text.Length < 6)
            {
                MessageBox.Show("New password must be at least 6 characters long.");
                return;
            }
            if (_newPassword.Text != _confirmPassword.Text)
            {
                MessageBox.Show("New Password and Confirm New Password do not match.");
                return;
            }

            try
            {
                User check = _users.Login(Program.CurrentUser.Username, _oldPassword.Text);
                if (check == null)
                {
                    MessageBox.Show("Current password is incorrect.", "Security",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _users.ResetPassword(Program.CurrentUser.UserID, _newPassword.Text);
                _oldPassword.Clear(); _newPassword.Clear(); _confirmPassword.Clear();
                MessageBox.Show("Password changed successfully.", "Security",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Security", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool ValidPhone(string phone)
        {
            int digits = 0;
            foreach (char c in phone)
            {
                if (char.IsDigit(c)) digits++;
                else if (c != '+' && c != '-' && c != ' ' && c != '(' && c != ')') return false;
            }
            return digits >= 6;
        }

        private static bool ValidEmail(string email)
        {
            try
            {
                MailAddress a = new MailAddress(email);
                return string.Equals(a.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }
    }
}
