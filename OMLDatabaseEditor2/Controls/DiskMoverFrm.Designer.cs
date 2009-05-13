namespace OMLDatabaseEditor.Controls
{
    partial class DiskMoverFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbcFolders = new DevExpress.XtraEditors.ListBoxControl();
            this.beDestination = new DevExpress.XtraEditors.ButtonEdit();
            this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbOK = new DevExpress.XtraEditors.SimpleButton();
            this.ceWithImages = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.lbcFolders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beDestination.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceWithImages.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lbcFolders
            // 
            this.lbcFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbcFolders.Location = new System.Drawing.Point(0, 0);
            this.lbcFolders.Name = "lbcFolders";
            this.lbcFolders.Size = new System.Drawing.Size(284, 196);
            this.lbcFolders.TabIndex = 0;
            // 
            // beDestination
            // 
            this.beDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.beDestination.Location = new System.Drawing.Point(13, 203);
            this.beDestination.Name = "beDestination";
            this.beDestination.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.beDestination.Size = new System.Drawing.Size(259, 20);
            this.beDestination.TabIndex = 1;
            this.beDestination.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beDestination_ButtonClick);
            // 
            // sbCancel
            // 
            this.sbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.sbCancel.Location = new System.Drawing.Point(197, 229);
            this.sbCancel.Name = "sbCancel";
            this.sbCancel.Size = new System.Drawing.Size(75, 23);
            this.sbCancel.TabIndex = 3;
            this.sbCancel.Text = "&Cancel";
            // 
            // sbOK
            // 
            this.sbOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sbOK.Location = new System.Drawing.Point(116, 228);
            this.sbOK.Name = "sbOK";
            this.sbOK.Size = new System.Drawing.Size(75, 23);
            this.sbOK.TabIndex = 2;
            this.sbOK.Text = "&OK";
            this.sbOK.Click += new System.EventHandler(this.sbOK_Click);
            // 
            // ceWithImages
            // 
            this.ceWithImages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ceWithImages.EditValue = true;
            this.ceWithImages.Location = new System.Drawing.Point(13, 230);
            this.ceWithImages.Name = "ceWithImages";
            this.ceWithImages.Properties.Caption = "w/Images";
            this.ceWithImages.Size = new System.Drawing.Size(75, 18);
            this.ceWithImages.TabIndex = 4;
            // 
            // DiskMoverFrm
            // 
            this.AcceptButton = this.sbOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.sbCancel;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.ceWithImages);
            this.Controls.Add(this.sbOK);
            this.Controls.Add(this.sbCancel);
            this.Controls.Add(this.beDestination);
            this.Controls.Add(this.lbcFolders);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DiskMoverFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Disk Mover";
            this.Load += new System.EventHandler(this.DiskMoverFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lbcFolders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beDestination.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceWithImages.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ListBoxControl lbcFolders;
        private DevExpress.XtraEditors.ButtonEdit beDestination;
        private DevExpress.XtraEditors.SimpleButton sbCancel;
        private DevExpress.XtraEditors.SimpleButton sbOK;
        private DevExpress.XtraEditors.CheckEdit ceWithImages;
    }
}