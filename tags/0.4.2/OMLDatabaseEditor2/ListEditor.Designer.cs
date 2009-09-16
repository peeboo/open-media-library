namespace OMLDatabaseEditor
{
    partial class ListEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListEditor));
            this.lbItems = new DevExpress.XtraEditors.ListBoxControl();
            this.cbeItem = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.lbItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeItem.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lbItems
            // 
            this.lbItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbItems.HotTrackItems = true;
            this.lbItems.Location = new System.Drawing.Point(0, 0);
            this.lbItems.Name = "lbItems";
            this.lbItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbItems.Size = new System.Drawing.Size(216, 224);
            this.lbItems.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.lbItems.TabIndex = 0;
            this.lbItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbItems_KeyDown);
            // 
            // cbeItem
            // 
            this.cbeItem.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbeItem.Location = new System.Drawing.Point(0, 224);
            this.cbeItem.Name = "cbeItem";
            this.cbeItem.Padding = new System.Windows.Forms.Padding(2);
            this.cbeItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.cbeItem.Size = new System.Drawing.Size(216, 20);
            this.cbeItem.TabIndex = 1;
            this.cbeItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbeItem_ButtonClick);
            // 
            // ListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 244);
            this.Controls.Add(this.lbItems);
            this.Controls.Add(this.cbeItem);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ListEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ListEditor";
            ((System.ComponentModel.ISupportInitialize)(this.lbItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeItem.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ListBoxControl lbItems;
        private DevExpress.XtraEditors.ComboBoxEdit cbeItem;
    }
}