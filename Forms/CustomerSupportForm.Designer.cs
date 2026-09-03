namespace GameHub.Forms
{
    partial class CustomerSupportForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.designerTitle = new System.Windows.Forms.Label();
            this.designerTitle.AutoSize = false;
            this.designerTitle.BackColor = System.Drawing.Color.Transparent;
            this.designerTitle.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.designerTitle.Location = new System.Drawing.Point(28, 20);
            this.designerTitle.Name = "designerTitle";
            this.designerTitle.Size = new System.Drawing.Size(700, 44);
            this.designerTitle.Text = "Customer Support";
            this.Controls.Add(this.designerTitle);
            this.designerSubtitle = new System.Windows.Forms.Label();
            this.designerSubtitle.AutoSize = false;
            this.designerSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.designerSubtitle.ForeColor = System.Drawing.Color.FromArgb(158, 166, 195);
            this.designerSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.designerSubtitle.Location = new System.Drawing.Point(30, 62);
            this.designerSubtitle.Name = "designerSubtitle";
            this.designerSubtitle.Size = new System.Drawing.Size(850, 28);
            this.designerSubtitle.Text = "Find customer accounts and contact information for support assistance.";
            this.Controls.Add(this.designerSubtitle);
            this.designerBack = new System.Windows.Forms.Button();
            this.designerBack.BackColor = System.Drawing.Color.FromArgb(31, 36, 66);
            this.designerBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.designerBack.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerBack.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.designerBack.Location = new System.Drawing.Point(1060, 22);
            this.designerBack.Name = "designerBack";
            this.designerBack.Size = new System.Drawing.Size(105, 36);
            this.designerBack.Text = "←  BACK";
            this.designerBack.UseVisualStyleBackColor = false;
            this.Controls.Add(this.designerBack);
            this.designerNotice = new System.Windows.Forms.Panel();
            this.designerNotice.BackColor = System.Drawing.Color.FromArgb(74, 65, 220);
            this.designerNotice.Location = new System.Drawing.Point(28, 106);
            this.designerNotice.Name = "designerNotice";
            this.designerNotice.Size = new System.Drawing.Size(1136, 104);
            this.Controls.Add(this.designerNotice);
            this.designerSupportPill = new System.Windows.Forms.Label();
            this.designerSupportPill.AutoSize = false;
            this.designerSupportPill.BackColor = System.Drawing.Color.FromArgb(13, 16, 31);
            this.designerSupportPill.ForeColor = System.Drawing.Color.FromArgb(235, 237, 255);
            this.designerSupportPill.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.designerSupportPill.Location = new System.Drawing.Point(18, 14);
            this.designerSupportPill.Name = "designerSupportPill";
            this.designerSupportPill.Size = new System.Drawing.Size(80, 26);
            this.designerSupportPill.Text = "SUPPORT";
            this.designerSupportPill.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.designerNotice.Controls.Add(this.designerSupportPill);
            this.designerNoticeText = new System.Windows.Forms.Label();
            this.designerNoticeText.AutoSize = false;
            this.designerNoticeText.BackColor = System.Drawing.Color.Transparent;
            this.designerNoticeText.ForeColor = System.Drawing.Color.FromArgb(235, 237, 255);
            this.designerNoticeText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.designerNoticeText.Location = new System.Drawing.Point(18, 49);
            this.designerNoticeText.Name = "designerNoticeText";
            this.designerNoticeText.Size = new System.Drawing.Size(1080, 42);
            this.designerNoticeText.Text = "Customer account lookup and contact information are available here without changing the existing database schema.";
            this.designerNotice.Controls.Add(this.designerNoticeText);
            this.designerSearchCard = new System.Windows.Forms.Panel();
            this.designerSearchCard.BackColor = System.Drawing.Color.FromArgb(22, 26, 48);
            this.designerSearchCard.Location = new System.Drawing.Point(28, 224);
            this.designerSearchCard.Name = "designerSearchCard";
            this.designerSearchCard.Size = new System.Drawing.Size(1136, 64);
            this.Controls.Add(this.designerSearchCard);
            this._search = new System.Windows.Forms.TextBox();
            this._search.BackColor = System.Drawing.Color.FromArgb(31, 36, 66);
            this._search.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._search.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this._search.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this._search.Location = new System.Drawing.Point(18, 17);
            this._search.Name = "_search";
            this._search.Size = new System.Drawing.Size(820, 30);
            this.designerSearchCard.Controls.Add(this._search);
            this.designerSearch = new System.Windows.Forms.Button();
            this.designerSearch.BackColor = System.Drawing.Color.FromArgb(104, 92, 255);
            this.designerSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.designerSearch.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.designerSearch.Location = new System.Drawing.Point(850, 15);
            this.designerSearch.Name = "designerSearch";
            this.designerSearch.Size = new System.Drawing.Size(105, 34);
            this.designerSearch.Text = "SEARCH";
            this.designerSearch.UseVisualStyleBackColor = false;
            this.designerSearchCard.Controls.Add(this.designerSearch);
            this.designerAll = new System.Windows.Forms.Button();
            this.designerAll.BackColor = System.Drawing.Color.FromArgb(31, 36, 66);
            this.designerAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.designerAll.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.designerAll.Location = new System.Drawing.Point(965, 15);
            this.designerAll.Name = "designerAll";
            this.designerAll.Size = new System.Drawing.Size(115, 34);
            this.designerAll.Text = "SHOW ALL";
            this.designerAll.UseVisualStyleBackColor = false;
            this.designerSearchCard.Controls.Add(this.designerAll);
            this.designerList = new System.Windows.Forms.Panel();
            this.designerList.BackColor = System.Drawing.Color.FromArgb(22, 26, 48);
            this.designerList.Location = new System.Drawing.Point(28, 302);
            this.designerList.Name = "designerList";
            this.designerList.Size = new System.Drawing.Size(1136, 390);
            this.Controls.Add(this.designerList);
            this._grid = new System.Windows.Forms.DataGridView();
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.BackgroundColor = System.Drawing.Color.FromArgb(24, 28, 52);
            this._grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._grid.ColumnHeadersHeight = 36;
            this._grid.Location = new System.Drawing.Point(18, 18);
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(1100, 354);
            this.designerList.Controls.Add(this._grid);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(8, 10, 20);
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.MinimumSize = new System.Drawing.Size(1000, 650);
            this.Name = "CustomerSupportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameHub - Customer Support";
        }

        private System.Windows.Forms.Label designerTitle;
        private System.Windows.Forms.Label designerSubtitle;
        private System.Windows.Forms.Button designerBack;
        private System.Windows.Forms.Panel designerNotice;
        private System.Windows.Forms.Label designerSupportPill;
        private System.Windows.Forms.Label designerNoticeText;
        private System.Windows.Forms.Panel designerSearchCard;
        private System.Windows.Forms.Button designerSearch;
        private System.Windows.Forms.Button designerAll;
        private System.Windows.Forms.Panel designerList;
    }
}
