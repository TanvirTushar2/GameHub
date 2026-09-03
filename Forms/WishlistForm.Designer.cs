namespace GameHub.Forms
{
    partial class WishlistForm
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
            this.designerTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.designerTitle.Location = new System.Drawing.Point(28, 22);
            this.designerTitle.Name = "designerTitle";
            this.designerTitle.Size = new System.Drawing.Size(480, 42);
            this.designerTitle.Text = "My Wishlist";
            this.Controls.Add(this.designerTitle);
            this.designerSubtitle = new System.Windows.Forms.Label();
            this.designerSubtitle.AutoSize = false;
            this.designerSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.designerSubtitle.ForeColor = System.Drawing.Color.FromArgb(158, 166, 195);
            this.designerSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.designerSubtitle.Location = new System.Drawing.Point(30, 54);
            this.designerSubtitle.Name = "designerSubtitle";
            this.designerSubtitle.Size = new System.Drawing.Size(620, 28);
            this.designerSubtitle.Text = "Keep track of games you want to purchase later.";
            this.Controls.Add(this.designerSubtitle);
            this.designerBack = new System.Windows.Forms.Button();
            this.designerBack.BackColor = System.Drawing.Color.FromArgb(31, 36, 66);
            this.designerBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.designerBack.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerBack.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.designerBack.Location = new System.Drawing.Point(856, 20);
            this.designerBack.Name = "designerBack";
            this.designerBack.Size = new System.Drawing.Size(96, 34);
            this.designerBack.Text = "←  Back";
            this.designerBack.UseVisualStyleBackColor = false;
            this.Controls.Add(this.designerBack);
            this.designerCard = new System.Windows.Forms.Panel();
            this.designerCard.BackColor = System.Drawing.Color.FromArgb(22, 26, 48);
            this.designerCard.Location = new System.Drawing.Point(28, 94);
            this.designerCard.Name = "designerCard";
            this.designerCard.Size = new System.Drawing.Size(924, 478);
            this.Controls.Add(this.designerCard);
            this.designerPill = new System.Windows.Forms.Label();
            this.designerPill.AutoSize = false;
            this.designerPill.BackColor = System.Drawing.Color.FromArgb(13, 16, 31);
            this.designerPill.ForeColor = System.Drawing.Color.FromArgb(235, 237, 255);
            this.designerPill.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.designerPill.Location = new System.Drawing.Point(18, 16);
            this.designerPill.Name = "designerPill";
            this.designerPill.Size = new System.Drawing.Size(86, 26);
            this.designerPill.Text = "WISHLIST";
            this.designerPill.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.designerCard.Controls.Add(this.designerPill);
            this.designerSaved = new System.Windows.Forms.Label();
            this.designerSaved.AutoSize = false;
            this.designerSaved.BackColor = System.Drawing.Color.Transparent;
            this.designerSaved.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerSaved.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.designerSaved.Location = new System.Drawing.Point(18, 54);
            this.designerSaved.Name = "designerSaved";
            this.designerSaved.Size = new System.Drawing.Size(260, 32);
            this.designerSaved.Text = "Saved Games";
            this.designerCard.Controls.Add(this.designerSaved);
            this.designerRemove = new System.Windows.Forms.Button();
            this.designerRemove.BackColor = System.Drawing.Color.FromArgb(238, 93, 111);
            this.designerRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.designerRemove.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerRemove.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.designerRemove.Location = new System.Drawing.Point(584, 42);
            this.designerRemove.Name = "designerRemove";
            this.designerRemove.Size = new System.Drawing.Size(140, 34);
            this.designerRemove.Text = "Remove Selected";
            this.designerRemove.UseVisualStyleBackColor = false;
            this.designerRemove.Click += new System.EventHandler(this.Remove_Click);
            this.designerCard.Controls.Add(this.designerRemove);
            this.designerOpenStore = new System.Windows.Forms.Button();
            this.designerOpenStore.BackColor = System.Drawing.Color.FromArgb(104, 92, 255);
            this.designerOpenStore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.designerOpenStore.ForeColor = System.Drawing.Color.FromArgb(248, 249, 255);
            this.designerOpenStore.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.designerOpenStore.Location = new System.Drawing.Point(738, 42);
            this.designerOpenStore.Name = "designerOpenStore";
            this.designerOpenStore.Size = new System.Drawing.Size(168, 34);
            this.designerOpenStore.Text = "Open Store";
            this.designerOpenStore.UseVisualStyleBackColor = false;
            this.designerCard.Controls.Add(this.designerOpenStore);
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
            this._grid.Size = new System.Drawing.Size(888, 360);
            this.designerCard.Controls.Add(this._grid);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(8, 10, 20);
            this.ClientSize = new System.Drawing.Size(980, 600);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.MinimumSize = new System.Drawing.Size(900, 560);
            this.Name = "WishlistForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameHub - Wishlist";
        }

        private System.Windows.Forms.Label designerTitle;
        private System.Windows.Forms.Label designerSubtitle;
        private System.Windows.Forms.Button designerBack;
        private System.Windows.Forms.Panel designerCard;
        private System.Windows.Forms.Label designerPill;
        private System.Windows.Forms.Label designerSaved;
        private System.Windows.Forms.Button designerRemove;
        private System.Windows.Forms.Button designerOpenStore;
    }
}
