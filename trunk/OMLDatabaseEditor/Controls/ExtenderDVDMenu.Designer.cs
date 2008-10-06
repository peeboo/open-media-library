namespace OMLDatabaseEditor.Controls
{
    partial class ExtenderDVDMenu
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
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
            this.txtAudioTracks = new System.Windows.Forms.TextBox();
            this.txtSubtitles = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cbEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMenu = new System.Windows.Forms.Label();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblInfo = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 156);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Audio Track:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 183);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Subtitles:";
            // 
            // txtStartChapters
            // 
            this.txtStartChapters.Location = new System.Drawing.Point(81, 127);
            this.txtStartChapters.Name = "txtStartChapters";
            this.txtStartChapters.Size = new System.Drawing.Size(43, 20);
            this.txtStartChapters.TabIndex = 8;
            this.txtStartChapters.TextChanged += new System.EventHandler(this.txtStartChapters_TextChanged);
            // 
            // txtEndChapters
            // 
            this.txtEndChapters.Location = new System.Drawing.Point(145, 127);
            this.txtEndChapters.Name = "txtEndChapters";
            this.txtEndChapters.Size = new System.Drawing.Size(43, 20);
            this.txtEndChapters.TabIndex = 10;
            this.txtEndChapters.TextChanged += new System.EventHandler(this.txtEndChapters_TextChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(120, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "-";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Chapters:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Location = new System.Drawing.Point(10, 312);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(91, 312);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "DVD:";
            // 
            // lblDVDName
            // 
            this.lblDVDName.AutoSize = true;
            this.lblDVDName.Location = new System.Drawing.Point(78, 9);
            this.lblDVDName.Name = "lblDVDName";
            this.lblDVDName.Size = new System.Drawing.Size(61, 13);
            this.lblDVDName.TabIndex = 1;
            this.lblDVDName.Text = "DVD-Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Title:";
            // 
            // cbTitle
            // 
            this.cbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTitle.FormattingEnabled = true;
            this.cbTitle.Location = new System.Drawing.Point(81, 40);
            this.cbTitle.Name = "cbTitle";
            this.cbTitle.Size = new System.Drawing.Size(239, 21);
            this.cbTitle.TabIndex = 3;
            this.cbTitle.SelectedIndexChanged += new System.EventHandler(this.cbTitle_SelectedIndexChanged);
            // 
            // lblChapters
            // 
            this.lblChapters.AutoSize = true;
            this.lblChapters.Location = new System.Drawing.Point(194, 130);
            this.lblChapters.Name = "lblChapters";
            this.lblChapters.Size = new System.Drawing.Size(26, 13);
            this.lblChapters.TabIndex = 19;
            this.lblChapters.Text = "max";
            // 
            // txtAudioTracks
            // 
            this.txtAudioTracks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAudioTracks.Enabled = false;
            this.txtAudioTracks.Location = new System.Drawing.Point(81, 153);
            this.txtAudioTracks.Name = "txtAudioTracks";
            this.txtAudioTracks.Size = new System.Drawing.Size(239, 20);
            this.txtAudioTracks.TabIndex = 12;
            // 
            // txtSubtitles
            // 
            this.txtSubtitles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubtitles.Enabled = false;
            this.txtSubtitles.Location = new System.Drawing.Point(81, 180);
            this.txtSubtitles.Name = "txtSubtitles";
            this.txtSubtitles.Size = new System.Drawing.Size(239, 20);
            this.txtSubtitles.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 104);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(81, 101);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(239, 20);
            this.txtName.TabIndex = 6;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // cbEnabled
            // 
            this.cbEnabled.AutoSize = true;
            this.cbEnabled.Location = new System.Drawing.Point(81, 78);
            this.cbEnabled.Name = "cbEnabled";
            this.cbEnabled.Size = new System.Drawing.Size(65, 17);
            this.cbEnabled.TabIndex = 4;
            this.cbEnabled.Text = "Enabled";
            this.cbEnabled.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblMenu);
            this.groupBox1.Location = new System.Drawing.Point(10, 206);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 100);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Virtual Menu";
            // 
            // lblMenu
            // 
            this.lblMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMenu.Location = new System.Drawing.Point(6, 16);
            this.lblMenu.Name = "lblMenu";
            this.lblMenu.Size = new System.Drawing.Size(298, 81);
            this.lblMenu.TabIndex = 0;
            this.lblMenu.Text = "Main Feature";
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(253, 125);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(29, 23);
            this.btnPrev.TabIndex = 21;
            this.btnPrev.Text = "<=";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(285, 125);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(29, 23);
            this.btnNext.TabIndex = 22;
            this.btnNext.Text = "=>";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(172, 317);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(0, 13);
            this.lblInfo.TabIndex = 18;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(218, 125);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(29, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = ">";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ExtenderDVDMenu
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(332, 347);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbEnabled);
            this.Controls.Add(this.txtSubtitles);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtAudioTracks);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblChapters);
            this.Controls.Add(this.cbTitle);
            this.Controls.Add(this.lblDVDName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtEndChapters);
            this.Controls.Add(this.txtStartChapters);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ExtenderDVDMenu";
            this.Text = "Extender DVD Menu";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.TextBox txtAudioTracks;
        private System.Windows.Forms.TextBox txtSubtitles;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.CheckBox cbEnabled;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblMenu;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button button1;
    }
}