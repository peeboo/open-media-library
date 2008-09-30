namespace OMLDatabaseEditor.Controls
{
    partial class ExtenderExtraOptions
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbAudioTracks = new System.Windows.Forms.ComboBox();
            this.cbSubtitles = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStartChapters = new System.Windows.Forms.TextBox();
            this.txtEndChapters = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lblDVDName = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbTitle = new System.Windows.Forms.ComboBox();
            this.lblChapters = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Audio Track:";
            // 
            // cbAudioTracks
            // 
            this.cbAudioTracks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAudioTracks.FormattingEnabled = true;
            this.cbAudioTracks.Location = new System.Drawing.Point(81, 73);
            this.cbAudioTracks.Name = "cbAudioTracks";
            this.cbAudioTracks.Size = new System.Drawing.Size(239, 21);
            this.cbAudioTracks.TabIndex = 1;
            this.cbAudioTracks.SelectedIndexChanged += new System.EventHandler(this.cbAudioTracks_SelectedIndexChanged);
            // 
            // cbSubtitles
            // 
            this.cbSubtitles.FormattingEnabled = true;
            this.cbSubtitles.Location = new System.Drawing.Point(81, 100);
            this.cbSubtitles.Name = "cbSubtitles";
            this.cbSubtitles.Size = new System.Drawing.Size(171, 21);
            this.cbSubtitles.TabIndex = 1;
            this.cbSubtitles.SelectedIndexChanged += new System.EventHandler(this.cbSubtitles_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Subtitles:";
            // 
            // txtStartChapters
            // 
            this.txtStartChapters.Location = new System.Drawing.Point(81, 127);
            this.txtStartChapters.Name = "txtStartChapters";
            this.txtStartChapters.Size = new System.Drawing.Size(70, 20);
            this.txtStartChapters.TabIndex = 2;
            // 
            // txtEndChapters
            // 
            this.txtEndChapters.Location = new System.Drawing.Point(182, 127);
            this.txtEndChapters.Name = "txtEndChapters";
            this.txtEndChapters.Size = new System.Drawing.Size(70, 20);
            this.txtEndChapters.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(157, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "-";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Chapters:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Location = new System.Drawing.Point(81, 157);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(165, 157);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Name:";
            // 
            // lblDVDName
            // 
            this.lblDVDName.AutoSize = true;
            this.lblDVDName.Location = new System.Drawing.Point(78, 20);
            this.lblDVDName.Name = "lblDVDName";
            this.lblDVDName.Size = new System.Drawing.Size(61, 13);
            this.lblDVDName.TabIndex = 9;
            this.lblDVDName.Text = "DVD-Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Title:";
            // 
            // cbTitle
            // 
            this.cbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTitle.FormattingEnabled = true;
            this.cbTitle.Location = new System.Drawing.Point(81, 47);
            this.cbTitle.Name = "cbTitle";
            this.cbTitle.Size = new System.Drawing.Size(239, 21);
            this.cbTitle.TabIndex = 10;
            this.cbTitle.SelectedIndexChanged += new System.EventHandler(this.cbTitle_SelectedIndexChanged);
            // 
            // lblChapters
            // 
            this.lblChapters.AutoSize = true;
            this.lblChapters.Location = new System.Drawing.Point(258, 131);
            this.lblChapters.Name = "lblChapters";
            this.lblChapters.Size = new System.Drawing.Size(26, 13);
            this.lblChapters.TabIndex = 11;
            this.lblChapters.Text = "max";
            // 
            // ExtenderExtraOptions
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(332, 192);
            this.Controls.Add(this.lblChapters);
            this.Controls.Add(this.cbTitle);
            this.Controls.Add(this.lblDVDName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEndChapters);
            this.Controls.Add(this.txtStartChapters);
            this.Controls.Add(this.cbSubtitles);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbAudioTracks);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ExtenderExtraOptions";
            this.Text = "Extender Extra Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAudioTracks;
        private System.Windows.Forms.ComboBox cbSubtitles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtStartChapters;
        private System.Windows.Forms.TextBox txtEndChapters;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDVDName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbTitle;
        private System.Windows.Forms.Label lblChapters;
    }
}