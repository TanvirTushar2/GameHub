namespace GameHub.Forms
{
    partial class PaymentsForm
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
            this.designerTitle.Size = new System.Drawing.Size(750, 44);
            this.designerTitle.Text = "Payment Management";
            this.Controls.Add(this.designerTitle);
            this.designerSubtitle = new System.Windows.Forms.Label();
            this.designerSubtitle.AutoSize = false;
            this.designerSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.designerSubtitle.ForeColor = System.Drawing.Color.FromArgb(158, 166, 195);
            this.designerSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.designerSubtitle.Location = new System.Drawing.Point(30, 62);
            this.designerSubtitle.Name = "designerSubtitle";
            this.designerSubtitle.Size = new System.Drawing.Size(850, 28);
            this.designerSubtitle.Text = "Review customer payments, transaction methods and payment status.";
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
            this.designerSearchCard = new System.Windows.Forms.Panel();
            this.designerSearchCard.BackColor = System.Drawing.Color.FromArgb(22, 26, 48);
            this.designerSearchCard.Location = new System.Drawing.Point(28, 106);
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
            this.designerPaymentsCard = new System.Windows.Forms.Panel();
            this.designerPaymentsCard.BackColor = System.Drawing.Color.FromArgb(22, 26, 48);
            this.designerPaymentsCard.Location = new System.Drawing.Point(28, 184);
            this.designerPaymentsCard.Name = "designerPaymentsCard";
            this.designerPaymentsCard.Size = new System.Drawing.Size(1136, 508);
            this.Controls.Add(this.designerPaymentsCard);
            this.designerPaymentsPill = new System.Windows.Forms.Label();
            this.designerPaymentsPill.AutoSize = false;
            this.designerPaymentsPill.BackColor = System.Drawing.Color.FromArgb(13, 16, 31);
            this.designerPaymentsPill.ForeColor = System.Drawing.Color.FromArgb(235, 237, 255);
            this.designerPaymentsPill.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.designerPaymentsPill.Location = new System.Drawing.Point(18, 16);
            this.designerPaymentsPill.Name = "designerPaymentsPill";
            this.designerPaymentsPill.Size = new System.Drawing.Size(88, 26);
            this.designerPaymentsPill.Text = "PAYMENTS";
            this.designerPaymentsPill.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.designerPaymentsCard.Controls.Add(this.designerPaymentsPill);
            this.designerPaymentsTitle = new System.Windows.Forms.Label();
            this.designerPaymentsTitle.AutoSize = false;
            this.designerPaymentsTitle.BackColor = System.Drawing.Color.Transparent;
            this.designerPaymentsTitle.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerPaymentsTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.designerPaymentsTitle.Location = new System.Drawing.Point(18, 52);
            this.designerPaymentsTitle.Name = "designerPaymentsTitle";
            this.designerPaymentsTitle.Size = new System.Drawing.Size(320, 32);
            this.designerPaymentsTitle.Text = "Payment Transactions";
            this.designerPaymentsCard.Controls.Add(this.designerPaymentsTitle);
            this._grid = new System.Windows.Forms.DataGridView();
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.BackgroundColor = System.Drawing.Color.FromArgb(24, 28, 52);
            this._grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._grid.ColumnHeadersHeight = 36;
            this._grid.Location = new System.Drawing.Point(18, 96);
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(1100, 394);
            this.designerPaymentsCard.Controls.Add(this._grid);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(8, 10, 20);
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.MinimumSize = new System.Drawing.Size(1000, 650);
            this.Name = "PaymentsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameHub - Payments";
        }

        private System.Windows.Forms.Label designerTitle;
        private System.Windows.Forms.Label designerSubtitle;
        private System.Windows.Forms.Button designerBack;
        private System.Windows.Forms.Panel designerSearchCard;
        private System.Windows.Forms.Button designerSearch;
        private System.Windows.Forms.Button designerAll;
        private System.Windows.Forms.Panel designerPaymentsCard;
        private System.Windows.Forms.Label designerPaymentsPill;
        private System.Windows.Forms.Label designerPaymentsTitle;
    }
}
