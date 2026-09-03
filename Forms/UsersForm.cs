using System;
using System.Drawing;
using System.Net.Mail;
using System.Windows.Forms;
using GameHub.Data;

namespace GameHub.Forms
{
    public partial class UsersForm : Form
    {
        private readonly UserRepository _users =
            new UserRepository();

        private int _selectedId;
        private string _selectedRole = "";
        private bool _isSuper =
            Program.CurrentUser != null &&
            Program.CurrentUser.Role == "SuperAdmin";

        private TextBox _txtUser;
        private TextBox _txtPass;
        private TextBox _txtName;
        private TextBox _txtEmail;
        private TextBox _txtPhone;

        private ComboBox _cboRole;
        private CheckBox _chkActive;

        private DataGridView _grid;

        public UsersForm()
        {
            InitializeComponent();

            BuildUi();

            AppStyle.Apply(this, true);

            MinimumSize =
                new Size(1100, 720);

            WindowState =
                FormWindowState.Maximized;

            LoadGrid();
        }

        private void BuildUi()
        {
            Controls.Clear();

            Text =
                "GameHub - Users & Staff";

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

            // =====================================================
            // HEADER
            // =====================================================

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
                    "Users & Staff",
                    0,
                    0,
                    700,
                    45,
                    new Font(
                        "Segoe UI",
                        20F,
                        FontStyle.Bold),
                    Theme.Text);

            header.Controls.Add(title);

            Label subtitle =
                Ui.FixedLabel(
                    "Create accounts, assign roles, update contact data and reset passwords.",
                    2,
                    48,
                    950,
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

            // =====================================================
            // BODY
            // =====================================================

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
                    36F));

            body.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    64F));

            body.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            root.Controls.Add(
                body,
                0,
                1);

            // =====================================================
            // ACCOUNT EDITOR
            // =====================================================

            GamePanel editor =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            editor.Dock =
                DockStyle.Fill;

            editor.Margin =
                new Padding(
                    0,
                    0,
                    9,
                    0);

            body.Controls.Add(
                editor,
                0,
                0);

            editor.AutoScroll =
                true;

            editor.Controls.Add(
                Ui.Pill(
                    "ACCOUNT",
                    20,
                    18,
                    84,
                    Theme.Cream,
                    Theme.Background2));

            editor.Controls.Add(
                Ui.FixedLabel(
                    "Account Details",
                    20,
                    57,
                    300,
                    32,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Text));

            int y =
                105;

            // Username
            AddLabel(
                editor,
                "Username",
                y);

            _txtUser =
                AddInput(
                    editor,
                    y + 26);

            y += 72;

            // Password
            AddLabel(
                editor,
                "Password / Reset Password",
                y);

            _txtPass =
                AddInput(
                    editor,
                    y + 26);

            _txtPass.PasswordChar =
                '●';

            y += 72;

            // Full Name
            AddLabel(
                editor,
                "Full Name",
                y);

            _txtName =
                AddInput(
                    editor,
                    y + 26);

            y += 72;

            // Email
            AddLabel(
                editor,
                "Email",
                y);

            _txtEmail =
                AddInput(
                    editor,
                    y + 26);

            y += 72;

            // Phone
            AddLabel(
                editor,
                "Phone",
                y);

            _txtPhone =
                AddInput(
                    editor,
                    y + 26);

            y += 72;

            // Role
            AddLabel(
                editor,
                "Role",
                y);

            _cboRole =
                new ComboBox();

            _cboRole.DropDownStyle =
                ComboBoxStyle.DropDownList;

            // A Super Admin can assign any role (including Admin / SuperAdmin);
            // a normal Admin may only create Staff and Customer accounts.
            if (_isSuper)
                _cboRole.Items.AddRange(
                    new object[]
                    {
                        "SuperAdmin",
                        "Admin",
                        "Staff",
                        "Customer"
                    });
            else
                _cboRole.Items.AddRange(
                    new object[]
                    {
                        "Staff",
                        "Customer"
                    });

            _cboRole.SelectedIndex =
                _cboRole.Items.Count - 1;

            _cboRole.SetBounds(
                20,
                y + 26,
                300,
                31);

            _cboRole.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            editor.Controls.Add(
                _cboRole);

            y += 70;

            _chkActive =
                new CheckBox();

            _chkActive.Text =
                "Account is active";

            _chkActive.Checked =
                true;

            _chkActive.SetBounds(
                20,
                y,
                200,
                28);

            editor.Controls.Add(
                _chkActive);

            y += 45;

            // BUTTONS
            FlowLayoutPanel actions =
                new FlowLayoutPanel();

            actions.SetBounds(
                20,
                y,
                390,
                52);

            actions.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            actions.FlowDirection =
                FlowDirection.LeftToRight;

            actions.WrapContents =
                true;

            actions.BackColor =
                Color.Transparent;

            editor.Controls.Add(actions);

            Button add =
                Ui.Btn(
                    "ADD",
                    Theme.Green,
                    0,
                    0,
                    75,
                    36);

            add.Click +=
                Add_Click;

            actions.Controls.Add(add);

            Button update =
                Ui.Btn(
                    "UPDATE",
                    Theme.Accent,
                    0,
                    0,
                    82,
                    36);

            update.Click +=
                Update_Click;

            actions.Controls.Add(update);

            Button reset =
                Ui.Btn(
                    "RESET PW",
                    Theme.Purple,
                    0,
                    0,
                    95,
                    36);

            reset.Click +=
                ResetPassword_Click;

            actions.Controls.Add(reset);

            Button deactivate =
                Ui.Btn(
                    "DEACTIVATE",
                    Theme.Red,
                    0,
                    0,
                    108,
                    36);

            deactivate.Click +=
                Deactivate_Click;

            actions.Controls.Add(deactivate);

            editor.Resize +=
                delegate
                {
                    int width =
                        Math.Max(
                            180,
                            editor.ClientSize.Width - 40);

                    _txtUser.Width =
                        width;

                    _txtPass.Width =
                        width;

                    _txtName.Width =
                        width;

                    _txtEmail.Width =
                        width;

                    _txtPhone.Width =
                        width;

                    _cboRole.Width =
                        width;

                    actions.Width =
                        width;
                };

            // =====================================================
            // ACCOUNT DIRECTORY
            // =====================================================

            GamePanel directory =
                Ui.Card(
                    0,
                    0,
                    100,
                    100);

            directory.Dock =
                DockStyle.Fill;

            directory.Margin =
                new Padding(
                    9,
                    0,
                    0,
                    0);

            directory.Padding =
                new Padding(
                    18,
                    78,
                    18,
                    18);

            body.Controls.Add(
                directory,
                1,
                0);

            directory.Controls.Add(
                Ui.FixedLabel(
                    "Account Directory",
                    18,
                    16,
                    400,
                    34,
                    new Font(
                        "Segoe UI",
                        14F,
                        FontStyle.Bold),
                    Theme.Text));

            directory.Controls.Add(
                Ui.FixedLabel(
                    "Customers, staff and administrators",
                    18,
                    49,
                    400,
                    23,
                    Theme.SmallFont,
                    Theme.Muted));

            _grid =
                Ui.Grid(
                    0,
                    0,
                    100,
                    100);

            _grid.Dock =
                DockStyle.Fill;

            _grid.SelectionChanged +=
                Grid_SelectionChanged;

            directory.Controls.Add(_grid);
        }

        private void AddLabel(
            Control parent,
            string text,
            int y)
        {
            parent.Controls.Add(
                Ui.FixedLabel(
                    text,
                    20,
                    y,
                    300,
                    23,
                    Theme.BodyFont,
                    Theme.Text));
        }

        private TextBox AddInput(
            Control parent,
            int y)
        {
            TextBox box =
                Ui.TextInput(
                    20,
                    y,
                    300,
                    30);

            box.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            parent.Controls.Add(box);

            return box;
        }

        private void LoadGrid()
        {
            try
            {
                _grid.DataSource =
                    _users.GetAllUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Users",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Grid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (
                _grid.CurrentRow == null ||
                _grid.CurrentRow.IsNewRow ||
                !_grid.CurrentRow.Selected)
                return;

            try
            {
                _selectedId =
                    Convert.ToInt32(
                        _grid.CurrentRow
                            .Cells["UserID"]
                            .Value);

                _txtUser.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Username"]
                            .Value);

                _txtName.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["FullName"]
                            .Value);

                _txtEmail.Text =
                    _grid.CurrentRow
                        .Cells["Email"]
                        .Value == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Email"]
                            .Value);

                _txtPhone.Text =
                    _grid.CurrentRow
                        .Cells["Phone"]
                        .Value == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Phone"]
                            .Value);

                _cboRole.Text =
                    Convert.ToString(
                        _grid.CurrentRow
                            .Cells["Role"]
                            .Value);

                _chkActive.Checked =
                    Convert.ToBoolean(
                        _grid.CurrentRow
                            .Cells["IsActive"]
                            .Value);

                _txtPass.Clear();
            }
            catch
            {
            }
        }

        private bool ValidateAccount(
            bool creating)
        {
            string username =
                _txtUser.Text.Trim();

            string fullName =
                _txtName.Text.Trim();

            string email =
                _txtEmail.Text.Trim();

            string phone =
                _txtPhone.Text.Trim();

            if (
                username.Length == 0 ||
                fullName.Length == 0)
            {
                MessageBox.Show(
                    "Username and full name are required.");

                return false;
            }

            if (
                username.Length < 3 ||
                username.Length > 50 ||
                username.Contains(" "))
            {
                MessageBox.Show(
                    "Username must be 3-50 characters and cannot contain spaces.");

                return false;
            }

            if (fullName.Length > 100)
            {
                MessageBox.Show(
                    "Full name cannot exceed 100 characters.");

                return false;
            }

            if (
                creating &&
                _txtPass.Text.Length < 6)
            {
                MessageBox.Show(
                    "New accounts require a password of at least 6 characters.");

                return false;
            }

            if (
                email.Length > 100 ||
                (
                    email.Length > 0 &&
                    !ValidEmail(email)
                ))
            {
                MessageBox.Show(
                    "Enter a valid email address of at most 100 characters.");

                return false;
            }

            if (
                phone.Length > 20 ||
                (
                    phone.Length > 0 &&
                    !ValidPhone(phone)
                ))
            {
                MessageBox.Show(
                    "Enter a valid phone number of at most 20 characters.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Only a Super Admin may create, edit or deactivate an Admin or
        /// Super Admin account. Returns false (and shows a message) otherwise.
        /// </summary>
        private bool CanManageRole(string role)
        {
            if (_isSuper)
                return true;

            if (role == "Admin" || role == "SuperAdmin")
            {
                MessageBox.Show(
                    "Only a Super Admin can create or manage Admin accounts.",
                    "Permission denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        private void Add_Click(
            object sender,
            EventArgs e)
        {
            if (!ValidateAccount(true))
                return;

            if (!CanManageRole(_cboRole.Text))
                return;

            try
            {
                if (
                    _users.UsernameExists(
                        _txtUser.Text.Trim()))
                {
                    MessageBox.Show(
                        "Username already exists.");

                    return;
                }

                if (
                    _users.EmailExists(
                        _txtEmail.Text.Trim()))
                {
                    MessageBox.Show(
                        "Email already exists.");

                    return;
                }

                _users.AddUser(
                    _txtUser.Text.Trim(),
                    _txtPass.Text,
                    _cboRole.Text,
                    _txtName.Text.Trim(),
                    _txtEmail.Text.Trim(),
                    _txtPhone.Text.Trim());

                LoadGrid();

                ClearForm();

                AuditRepository.Write("Create account", _txtUser.Text.Trim() + " (" + _cboRole.Text + ")");

                MessageBox.Show(
                    "Account created.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Users",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Update_Click(
            object sender,
            EventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show(
                    "Select a user first.");

                return;
            }

            if (!ValidateAccount(false))
                return;

            if (!CanManageRole(_selectedRole) || !CanManageRole(_cboRole.Text))
                return;

            if (
                Program.CurrentUser != null &&
                _selectedId ==
                    Program.CurrentUser.UserID &&
                (
                    (_cboRole.Text != "Admin" &&
                     _cboRole.Text != "SuperAdmin") ||
                    !_chkActive.Checked
                ))
            {
                MessageBox.Show(
                    "You cannot remove your own Admin role or deactivate your own account while signed in.");

                return;
            }

            try
            {
                if (
                    _users.UsernameUsedByAnotherUser(
                        _txtUser.Text.Trim(),
                        _selectedId))
                {
                    MessageBox.Show(
                        "That username is already used by another account.");

                    return;
                }

                if (
                    _users.EmailUsedByAnotherUser(
                        _txtEmail.Text.Trim(),
                        _selectedId))
                {
                    MessageBox.Show(
                        "That email is already used by another account.");

                    return;
                }

                _users.UpdateUser(
                    _selectedId,
                    _txtUser.Text.Trim(),
                    _cboRole.Text,
                    _txtName.Text.Trim(),
                    _txtEmail.Text.Trim(),
                    _txtPhone.Text.Trim(),
                    _chkActive.Checked);

                if (
                    Program.CurrentUser != null &&
                    _selectedId ==
                        Program.CurrentUser.UserID)
                {
                    var refreshed =
                        _users.GetUserById(
                            _selectedId);

                    if (refreshed != null)
                        Program.CurrentUser =
                            refreshed;
                }

                LoadGrid();

                MessageBox.Show(
                    "Account updated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Users",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ResetPassword_Click(
            object sender,
            EventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show(
                    "Select a user first.");

                return;
            }

            if (_txtPass.Text.Length < 6)
            {
                MessageBox.Show(
                    "Enter a new password of at least 6 characters in the Password field.");

                return;
            }

            if (
                MessageBox.Show(
                    "Reset the selected user's password?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
                return;

            try
            {
                _users.ResetPassword(
                    _selectedId,
                    _txtPass.Text);

                _txtPass.Clear();

                MessageBox.Show(
                    "Password reset successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Users",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Deactivate_Click(
            object sender,
            EventArgs e)
        {
            if (_selectedId == 0)
            {
                MessageBox.Show(
                    "Select a user first.");

                return;
            }

            if (
                Program.CurrentUser != null &&
                _selectedId ==
                    Program.CurrentUser.UserID)
            {
                MessageBox.Show(
                    "You cannot deactivate the account you are currently using.");

                return;
            }

            if (!CanManageRole(_selectedRole))
                return;

            if (
                MessageBox.Show(
                    "Deactivate this account?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)
                != DialogResult.Yes)
                return;

            try
            {
                _users.DeactivateUser(
                    _selectedId);

                AuditRepository.Write("Deactivate account", "UserID " + _selectedId);

                LoadGrid();

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Users",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            _selectedRole =
                "";

            _selectedId =
                0;

            _txtUser.Clear();
            _txtPass.Clear();
            _txtName.Clear();
            _txtEmail.Clear();
            _txtPhone.Clear();

            if (_cboRole.Items.Count > 0)
                _cboRole.SelectedIndex =
                    _cboRole.Items.Count - 1;

            _chkActive.Checked =
                true;

            _grid.ClearSelection();
        }

        private static bool ValidPhone(
            string phone)
        {
            int digits =
                0;

            foreach (char c in phone)
            {
                if (char.IsDigit(c))
                {
                    digits++;
                }
                else if (
                    c != '+' &&
                    c != '-' &&
                    c != ' ' &&
                    c != '(' &&
                    c != ')')
                {
                    return false;
                }
            }

            return digits >= 6;
        }

        private static bool ValidEmail(
            string email)
        {
            try
            {
                MailAddress address =
                    new MailAddress(email);

                return string.Equals(
                    address.Address,
                    email,
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}