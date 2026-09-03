using System;
using System.Drawing;
using System.Net.Mail;
using System.Windows.Forms;
using GameHub.Data;
using GameHub.Forms.Controls;
using GameHub.Models;

namespace GameHub.Forms.Views
{
    internal class ProfileView : LauncherPage
    {
        private readonly UserRepository _users = new UserRepository();
        private readonly Panel _content;
        private readonly GamePanel _identity;
        private readonly GamePanel _infoCard;
        private readonly GamePanel _securityCard;
        private readonly Label _avatar;
        private readonly Label _name;
        private readonly Label _role;
        private readonly TextBox _username;
        private readonly TextBox _fullName;
        private readonly TextBox _email;
        private readonly TextBox _phone;
        private readonly TextBox _currentPass;
        private readonly TextBox _newPass;
        private readonly TextBox _confirmPass;
        private readonly CheckBox _showPasswords;
        private readonly Button _saveProfile;
        private readonly Button _changePassword;

        public ProfileView()
        {
            Panel scroll = new Panel();
            scroll.Dock = DockStyle.Fill;
            scroll.AutoScroll = true;
            scroll.BackColor = Theme.Background;
            Controls.Add(scroll);

            _content = new Panel();
            _content.SetBounds(0, 0, 1040, 760);
            _content.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _content.BackColor = Theme.Background;
            scroll.Controls.Add(_content);

            Label pageTitle = Ui.FixedLabel("Player Profile", 0, 0, 760, 40, Theme.TitleFont, Theme.Text);
            _content.Controls.Add(pageTitle);
            Label subtitle = Ui.FixedLabel("Manage your personal information and account password.",
                2, 41, 850, 28, Theme.BodyFont, Theme.Muted);
            _content.Controls.Add(subtitle);

            _identity = Ui.Card(0, 82, 100, 152);
            _identity.StartColor = Theme.Surface;
            _identity.EndColor = Theme.Background2;
            _identity.Gradient = true;
            _content.Controls.Add(_identity);

            GamePanel avatarPanel = Ui.GradientCard(24, 26, 92, 92, Theme.Accent, Theme.AccentDark);
            _identity.Controls.Add(avatarPanel);
            _avatar = Ui.FixedLabel("G", 0, 14, 92, 64, new Font("Segoe UI", 28F, FontStyle.Bold), Color.White);
            _avatar.TextAlign = ContentAlignment.MiddleCenter;
            avatarPanel.Controls.Add(_avatar);

            _name = Ui.FixedLabel("Player", 142, 34, 700, 42,
                new Font("Segoe UI", 19F, FontStyle.Bold), Theme.Text);
            _identity.Controls.Add(_name);
            _role = Ui.Pill("CUSTOMER", 144, 81, 96, Theme.SurfaceAlt, Theme.Cyan);
            _identity.Controls.Add(_role);

            _infoCard = Ui.Card(0, 258, 510, 390);
            _infoCard.Controls.Add(Ui.Lbl("PROFILE INFORMATION", 22, 20, Theme.SmallBold, Theme.AccentBlue));
            _infoCard.Controls.Add(Ui.Lbl("Username", 22, 58, Theme.SmallFont, Theme.Muted));
            _username = Ui.TextInput(22, 80, 466, 31);
            _username.ReadOnly = true;
            _infoCard.Controls.Add(_username);
            _fullName = AddField(_infoCard, "Full Name", 126);
            _email = AddField(_infoCard, "Email", 193);
            _phone = AddField(_infoCard, "Phone", 260);
            _saveProfile = Ui.Btn("SAVE PROFILE", Theme.Accent, 22, 333, 466, 38);
            _saveProfile.Click += SaveProfile;
            _infoCard.Controls.Add(_saveProfile);
            _content.Controls.Add(_infoCard);

            _securityCard = Ui.Card(532, 258, 508, 390);
            _securityCard.Controls.Add(Ui.Lbl("SECURITY", 22, 20, Theme.SmallBold, Theme.Red));
            _securityCard.Controls.Add(Ui.FixedLabel(
                "Change your password only after confirming the current one.",
                22, 49, 450, 28, Theme.SmallFont, Theme.Muted));
            _currentPass = AddPasswordField(_securityCard, "Current Password", 86);
            _newPass = AddPasswordField(_securityCard, "New Password", 153);
            _confirmPass = AddPasswordField(_securityCard, "Confirm New Password", 220);

            _showPasswords = new CheckBox();
            _showPasswords.Text = "Show passwords";
            _showPasswords.SetBounds(22, 286, 160, 24);
            _showPasswords.CheckedChanged += delegate
            {
                char c = _showPasswords.Checked ? '\0' : '●';
                _currentPass.PasswordChar = c;
                _newPass.PasswordChar = c;
                _confirmPass.PasswordChar = c;
            };
            _securityCard.Controls.Add(_showPasswords);

            _changePassword = Ui.Btn("CHANGE PASSWORD", Theme.Red, 22, 333, 464, 38);
            _changePassword.Click += ChangePassword;
            _securityCard.Controls.Add(_changePassword);
            _content.Controls.Add(_securityCard);

            Action relayout = delegate
            {
                int viewportWidth = Math.Max(720, scroll.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2);
                _content.Width = viewportWidth;

                pageTitle.Width = viewportWidth;
                subtitle.Width = viewportWidth;
                _identity.Width = viewportWidth;
                _name.Width = Math.Max(260, viewportWidth - _name.Left - 24);

                if (viewportWidth >= 980)
                {
                    int gap = 22;
                    int leftWidth = (viewportWidth - gap) / 2;
                    _infoCard.SetBounds(0, 258, leftWidth, 390);
                    _securityCard.SetBounds(leftWidth + gap, 258, viewportWidth - leftWidth - gap, 390);
                    _content.Height = 700;
                }
                else
                {
                    _infoCard.SetBounds(0, 258, viewportWidth, 390);
                    _securityCard.SetBounds(0, 670, viewportWidth, 390);
                    _content.Height = 1090;
                }

                ResizeCardInputs(_infoCard, _username, _fullName, _email, _phone, _saveProfile);
                ResizeSecurityInputs();
            };

            scroll.Resize += delegate { relayout(); };
            relayout();
            VisibleChanged += delegate { if (Visible) RefreshData(); };
        }

        public override void RefreshData()
        {
            if (Program.CurrentUser == null) return;
            try
            {
                User user = _users.GetUserById(Program.CurrentUser.UserID);
                if (user == null) return;
                Program.CurrentUser = user;
                _name.Text = user.FullName;
                _role.Text = user.Role.ToUpperInvariant();
                _avatar.Text = string.IsNullOrWhiteSpace(user.FullName) ? "G" : user.FullName.Substring(0, 1).ToUpperInvariant();
                _username.Text = user.Username;
                _fullName.Text = user.FullName;
                _email.Text = user.Email;
                _phone.Text = user.Phone;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load your profile.\n" + ex.Message,
                    "Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static TextBox AddField(Control parent, string label, int y)
        {
            parent.Controls.Add(Ui.Lbl(label, 22, y, Theme.SmallFont, Theme.Muted));
            TextBox box = Ui.TextInput(22, y + 22, 466, 31);
            parent.Controls.Add(box);
            return box;
        }

        private static TextBox AddPasswordField(Control parent, string label, int y)
        {
            TextBox box = AddField(parent, label, y);
            box.PasswordChar = '●';
            return box;
        }

        private static void ResizeCardInputs(GamePanel card, TextBox username, TextBox fullName,
            TextBox email, TextBox phone, Button button)
        {
            int width = Math.Max(180, card.ClientSize.Width - 44);
            username.Width = width;
            fullName.Width = width;
            email.Width = width;
            phone.Width = width;
            button.Width = width;
        }

        private void ResizeSecurityInputs()
        {
            int width = Math.Max(180, _securityCard.ClientSize.Width - 44);
            _currentPass.Width = width;
            _newPass.Width = width;
            _confirmPass.Width = width;
            _changePassword.Width = width;
        }

        private void SaveProfile(object sender, EventArgs e)
        {
            if (Program.CurrentUser == null) return;
            string fullName = _fullName.Text.Trim();
            string email = _email.Text.Trim();
            string phone = _phone.Text.Trim();

            if (fullName.Length < 2 || fullName.Length > 100)
            {
                MessageBox.Show("Full Name must be between 2 and 100 characters.", "Profile",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidEmail(email) || email.Length > 100)
            {
                MessageBox.Show("Enter a valid email address.", "Profile",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidPhone(phone) || phone.Length > 20)
            {
                MessageBox.Show("Enter a valid phone number.", "Profile",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_users.EmailUsedByAnotherUser(email, Program.CurrentUser.UserID))
                {
                    MessageBox.Show("That email address belongs to another account.", "Profile",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _users.UpdateProfile(Program.CurrentUser.UserID, fullName, email, phone);
                Program.CurrentUser = _users.GetUserById(Program.CurrentUser.UserID);
                MessageBox.Show("Profile updated successfully.", "Profile",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RaiseGlobalDataChanged();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not update your profile.\n" + ex.Message,
                    "Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangePassword(object sender, EventArgs e)
        {
            if (Program.CurrentUser == null) return;
            if (_currentPass.Text.Length == 0 || _newPass.Text.Length == 0 || _confirmPass.Text.Length == 0)
            {
                MessageBox.Show("Complete all password fields.", "Security",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_newPass.Text.Length < 6)
            {
                MessageBox.Show("The new password must be at least 6 characters.", "Security",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_newPass.Text != _confirmPass.Text)
            {
                MessageBox.Show("New Password and Confirm New Password do not match.", "Security",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User verified = _users.Login(Program.CurrentUser.Username, _currentPass.Text);
                if (verified == null)
                {
                    MessageBox.Show("The current password is incorrect.", "Security",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _users.ResetPassword(Program.CurrentUser.UserID, _newPass.Text);
                _currentPass.Clear();
                _newPass.Clear();
                _confirmPass.Clear();
                MessageBox.Show("Your password has been changed.", "Security",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not change the password.\n" + ex.Message,
                    "Security", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (string.IsNullOrWhiteSpace(phone)) return false;
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
