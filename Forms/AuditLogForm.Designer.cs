namespace GameHub.Forms
{
    partial class AuditLogForm
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
            this.designerTitle.Text = "System Activity Log";
            this.Controls.Add(this.designerTitle);
            this.designerSubtitle = new System.Windows.Forms.Label();
            this.designerSubtitle.AutoSize = false;
            this.designerSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.designerSubtitle.ForeColor = System.Drawing.Color.FromArgb(158, 166, 195);
            this.designerSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular);
            this.designerSubtitle.Location = new System.Drawing.Point(30, 62);
            this.designerSubtitle.Name = "designerSubtitle";
            this.designerSubtitle.Size = new System.Drawing.Size(850, 28);
            this.designerSubtitle.Text = "Owner oversight — a record of key actions performed across GameHub.";
            this.Controls.Add(this.designerSubtitle);
            this.designerCard = new System.Windows.Forms.Panel();
            this.designerCard.BackColor = System.Drawing.Color.FromArgb(22, 26, 48);
            this.designerCard.Location = new System.Drawing.Point(28, 106);
            this.designerCard.Name = "designerCard";
            this.designerCard.Size = new System.Drawing.Size(1136, 586);
            this.Controls.Add(this.designerCard);
            this.designerPill = new System.Windows.Forms.Label();
            this.designerPill.AutoSize = false;
            this.designerPill.BackColor = System.Drawing.Color.FromArgb(13, 16, 31);
            this.designerPill.ForeColor = System.Drawing.Color.FromArgb(235, 237, 255);
            this.designerPill.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.designerPill.Location = new System.Drawing.Point(18, 16);
            this.designerPill.Name = "designerPill";
            this.designerPill.Size = new System.Drawing.Size(110, 26);
            this.designerPill.Text = "AUDIT TRAIL";
            this.designerPill.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.designerCard.Controls.Add(this.designerPill);
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(8, 10, 20);
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.MinimumSize = new System.Drawing.Size(1000, 650);
            this.Name = "AuditLogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameHub - System Log";
        }

        private System.Windows.Forms.Label designerTitle;
        private System.Windows.Forms.Label designerSubtitle;
        private System.Windows.Forms.Panel designerCard;
        private System.Windows.Forms.Label designerPill;
    }
}
