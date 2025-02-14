
namespace NJCloak {
    partial class MainForm {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cloakButton = new System.Windows.Forms.Button();
            this.BottomMenu = new System.Windows.Forms.StatusStrip();
            this.LogsLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.AboutLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.screenBlockerTab = new System.Windows.Forms.Panel();
            this.cleanTitlesCheckbox = new System.Windows.Forms.CheckBox();
            this.firewallTab = new System.Windows.Forms.Panel();
            this.firewallSelectButton = new System.Windows.Forms.Button();
            this.firewallStatusLabel = new System.Windows.Forms.Label();
            this.blockAllButton = new System.Windows.Forms.Button();
            this.resetFirewall = new System.Windows.Forms.Button();
            this.whitelistRemoveButton = new System.Windows.Forms.Button();
            this.whitelistAddButton = new System.Windows.Forms.Button();
            this.whitelistAppPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.clipperDetectorTab = new System.Windows.Forms.Panel();
            this.clipperStatusLabel = new System.Windows.Forms.Label();
            this.clipperDetectorButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.clipperDetectorTimer = new System.Windows.Forms.Timer(this.components);
            this.aboutTab = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.creditsLabel = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.windowCleanerTimer = new System.Windows.Forms.Timer(this.components);
            this.BottomMenu.SuspendLayout();
            this.screenBlockerTab.SuspendLayout();
            this.firewallTab.SuspendLayout();
            this.clipperDetectorTab.SuspendLayout();
            this.aboutTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cloakButton
            // 
            this.cloakButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cloakButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cloakButton.ForeColor = System.Drawing.Color.LightGray;
            this.cloakButton.Location = new System.Drawing.Point(228, 95);
            this.cloakButton.Name = "cloakButton";
            this.cloakButton.Size = new System.Drawing.Size(141, 44);
            this.cloakButton.TabIndex = 1;
            this.cloakButton.Text = "Start blocking";
            this.cloakButton.UseVisualStyleBackColor = true;
            this.cloakButton.Click += new System.EventHandler(this.cloakButton_Click);
            // 
            // BottomMenu
            // 
            this.BottomMenu.AutoSize = false;
            this.BottomMenu.BackColor = System.Drawing.Color.Black;
            this.BottomMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LogsLbl,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.AboutLbl});
            this.BottomMenu.Location = new System.Drawing.Point(0, 213);
            this.BottomMenu.Name = "BottomMenu";
            this.BottomMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.BottomMenu.Size = new System.Drawing.Size(601, 22);
            this.BottomMenu.TabIndex = 2;
            this.BottomMenu.Text = "statusStrip1";
            // 
            // LogsLbl
            // 
            this.LogsLbl.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LogsLbl.ForeColor = System.Drawing.Color.LightGray;
            this.LogsLbl.IsLink = true;
            this.LogsLbl.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.LogsLbl.LinkColor = System.Drawing.Color.LightGray;
            this.LogsLbl.Name = "LogsLbl";
            this.LogsLbl.Size = new System.Drawing.Size(102, 17);
            this.LogsLbl.Tag = "screenBlockerTab";
            this.LogsLbl.Text = "[Screen blocker]";
            this.LogsLbl.Click += new System.EventHandler(this.onTabSelected);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.LightGray;
            this.toolStripStatusLabel2.IsLink = true;
            this.toolStripStatusLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.toolStripStatusLabel2.LinkColor = System.Drawing.Color.LightGray;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusLabel2.Tag = "firewallTab";
            this.toolStripStatusLabel2.Text = "[Firewall]";
            this.toolStripStatusLabel2.Click += new System.EventHandler(this.onTabSelected);
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolStripStatusLabel3.ForeColor = System.Drawing.Color.LightGray;
            this.toolStripStatusLabel3.IsLink = true;
            this.toolStripStatusLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.toolStripStatusLabel3.LinkColor = System.Drawing.Color.LightGray;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(80, 17);
            this.toolStripStatusLabel3.Tag = "clipperDetectorTab";
            this.toolStripStatusLabel3.Text = "[Anti-clipper]";
            this.toolStripStatusLabel3.Click += new System.EventHandler(this.onTabSelected);
            // 
            // AboutLbl
            // 
            this.AboutLbl.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AboutLbl.ForeColor = System.Drawing.Color.LightGray;
            this.AboutLbl.IsLink = true;
            this.AboutLbl.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.AboutLbl.LinkColor = System.Drawing.Color.LightGray;
            this.AboutLbl.Name = "AboutLbl";
            this.AboutLbl.Size = new System.Drawing.Size(48, 17);
            this.AboutLbl.Tag = "aboutTab";
            this.AboutLbl.Text = "[About]";
            this.AboutLbl.Click += new System.EventHandler(this.onTabSelected);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(577, 47);
            this.label1.TabIndex = 3;
            this.label1.Text = "* When enabled this tool will block any kind of screen capture. This will hide yo" +
    "ur PC screen from capturing by malware  allowing maximum privacy.";
            // 
            // screenBlockerTab
            // 
            this.screenBlockerTab.Controls.Add(this.cleanTitlesCheckbox);
            this.screenBlockerTab.Controls.Add(this.label1);
            this.screenBlockerTab.Controls.Add(this.cloakButton);
            this.screenBlockerTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenBlockerTab.Location = new System.Drawing.Point(0, 0);
            this.screenBlockerTab.MinimumSize = new System.Drawing.Size(601, 235);
            this.screenBlockerTab.Name = "screenBlockerTab";
            this.screenBlockerTab.Size = new System.Drawing.Size(601, 235);
            this.screenBlockerTab.TabIndex = 4;
            // 
            // cleanTitlesCheckbox
            // 
            this.cleanTitlesCheckbox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cleanTitlesCheckbox.AutoSize = true;
            this.cleanTitlesCheckbox.Location = new System.Drawing.Point(233, 145);
            this.cleanTitlesCheckbox.Name = "cleanTitlesCheckbox";
            this.cleanTitlesCheckbox.Size = new System.Drawing.Size(136, 19);
            this.cleanTitlesCheckbox.TabIndex = 4;
            this.cleanTitlesCheckbox.Text = "Clean window titles";
            this.cleanTitlesCheckbox.UseVisualStyleBackColor = true;
            // 
            // firewallTab
            // 
            this.firewallTab.AllowDrop = true;
            this.firewallTab.Controls.Add(this.firewallSelectButton);
            this.firewallTab.Controls.Add(this.firewallStatusLabel);
            this.firewallTab.Controls.Add(this.blockAllButton);
            this.firewallTab.Controls.Add(this.resetFirewall);
            this.firewallTab.Controls.Add(this.whitelistRemoveButton);
            this.firewallTab.Controls.Add(this.whitelistAddButton);
            this.firewallTab.Controls.Add(this.whitelistAppPath);
            this.firewallTab.Controls.Add(this.label2);
            this.firewallTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.firewallTab.Location = new System.Drawing.Point(0, 0);
            this.firewallTab.Name = "firewallTab";
            this.firewallTab.Size = new System.Drawing.Size(601, 235);
            this.firewallTab.TabIndex = 5;
            this.firewallTab.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.firewallTab.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // firewallSelectButton
            // 
            this.firewallSelectButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.firewallSelectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.firewallSelectButton.Location = new System.Drawing.Point(531, 91);
            this.firewallSelectButton.Name = "firewallSelectButton";
            this.firewallSelectButton.Size = new System.Drawing.Size(61, 23);
            this.firewallSelectButton.TabIndex = 14;
            this.firewallSelectButton.Text = "Select";
            this.firewallSelectButton.UseVisualStyleBackColor = true;
            this.firewallSelectButton.Click += new System.EventHandler(this.firewallSelectButton_Click);
            this.firewallSelectButton.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.firewallSelectButton.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // firewallStatusLabel
            // 
            this.firewallStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.firewallStatusLabel.BackColor = System.Drawing.Color.Black;
            this.firewallStatusLabel.Location = new System.Drawing.Point(18, 150);
            this.firewallStatusLabel.Name = "firewallStatusLabel";
            this.firewallStatusLabel.Size = new System.Drawing.Size(574, 14);
            this.firewallStatusLabel.TabIndex = 13;
            this.firewallStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.firewallStatusLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.firewallStatusLabel.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // blockAllButton
            // 
            this.blockAllButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.blockAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.blockAllButton.Location = new System.Drawing.Point(300, 53);
            this.blockAllButton.Name = "blockAllButton";
            this.blockAllButton.Size = new System.Drawing.Size(131, 32);
            this.blockAllButton.TabIndex = 12;
            this.blockAllButton.Text = "Block all (whitelist)";
            this.blockAllButton.UseVisualStyleBackColor = true;
            this.blockAllButton.Click += new System.EventHandler(this.blockAllButton_Click);
            this.blockAllButton.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.blockAllButton.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // resetFirewall
            // 
            this.resetFirewall.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.resetFirewall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetFirewall.Location = new System.Drawing.Point(182, 53);
            this.resetFirewall.Name = "resetFirewall";
            this.resetFirewall.Size = new System.Drawing.Size(112, 32);
            this.resetFirewall.TabIndex = 11;
            this.resetFirewall.Text = "Reset firewall";
            this.resetFirewall.UseVisualStyleBackColor = true;
            this.resetFirewall.Click += new System.EventHandler(this.resetFirewall_Click);
            this.resetFirewall.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.resetFirewall.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // whitelistRemoveButton
            // 
            this.whitelistRemoveButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.whitelistRemoveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.whitelistRemoveButton.Location = new System.Drawing.Point(300, 120);
            this.whitelistRemoveButton.Name = "whitelistRemoveButton";
            this.whitelistRemoveButton.Size = new System.Drawing.Size(81, 23);
            this.whitelistRemoveButton.TabIndex = 10;
            this.whitelistRemoveButton.Text = "Remove";
            this.whitelistRemoveButton.UseVisualStyleBackColor = true;
            this.whitelistRemoveButton.Click += new System.EventHandler(this.whitelistRemoveButton_Click);
            this.whitelistRemoveButton.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.whitelistRemoveButton.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // whitelistAddButton
            // 
            this.whitelistAddButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.whitelistAddButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.whitelistAddButton.Location = new System.Drawing.Point(216, 120);
            this.whitelistAddButton.Name = "whitelistAddButton";
            this.whitelistAddButton.Size = new System.Drawing.Size(78, 23);
            this.whitelistAddButton.TabIndex = 9;
            this.whitelistAddButton.Text = "Add";
            this.whitelistAddButton.UseVisualStyleBackColor = true;
            this.whitelistAddButton.Click += new System.EventHandler(this.whitelistAddButton_Click);
            this.whitelistAddButton.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.whitelistAddButton.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // whitelistAppPath
            // 
            this.whitelistAppPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.whitelistAppPath.BackColor = System.Drawing.Color.Black;
            this.whitelistAppPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.whitelistAppPath.ForeColor = System.Drawing.Color.LightGray;
            this.whitelistAppPath.Location = new System.Drawing.Point(102, 93);
            this.whitelistAppPath.Name = "whitelistAppPath";
            this.whitelistAppPath.Size = new System.Drawing.Size(423, 21);
            this.whitelistAppPath.TabIndex = 8;
            this.whitelistAppPath.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.whitelistAppPath.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Whitelist app";
            this.label2.DragDrop += new System.Windows.Forms.DragEventHandler(this.firewallTab_DragDrop);
            this.label2.DragEnter += new System.Windows.Forms.DragEventHandler(this.On_DragEnter);
            // 
            // clipperDetectorTab
            // 
            this.clipperDetectorTab.Controls.Add(this.clipperStatusLabel);
            this.clipperDetectorTab.Controls.Add(this.clipperDetectorButton);
            this.clipperDetectorTab.Controls.Add(this.label3);
            this.clipperDetectorTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clipperDetectorTab.Location = new System.Drawing.Point(0, 0);
            this.clipperDetectorTab.Name = "clipperDetectorTab";
            this.clipperDetectorTab.Size = new System.Drawing.Size(601, 235);
            this.clipperDetectorTab.TabIndex = 6;
            // 
            // clipperStatusLabel
            // 
            this.clipperStatusLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.clipperStatusLabel.ForeColor = System.Drawing.Color.LightGray;
            this.clipperStatusLabel.Location = new System.Drawing.Point(12, 153);
            this.clipperStatusLabel.Name = "clipperStatusLabel";
            this.clipperStatusLabel.Size = new System.Drawing.Size(577, 19);
            this.clipperStatusLabel.TabIndex = 7;
            this.clipperStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // clipperDetectorButton
            // 
            this.clipperDetectorButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.clipperDetectorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clipperDetectorButton.ForeColor = System.Drawing.Color.LightGray;
            this.clipperDetectorButton.Location = new System.Drawing.Point(228, 98);
            this.clipperDetectorButton.Name = "clipperDetectorButton";
            this.clipperDetectorButton.Size = new System.Drawing.Size(142, 41);
            this.clipperDetectorButton.TabIndex = 6;
            this.clipperDetectorButton.Text = "Start clipper detector";
            this.clipperDetectorButton.UseVisualStyleBackColor = true;
            this.clipperDetectorButton.Click += new System.EventHandler(this.clipperDetectorButton_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.ForeColor = System.Drawing.Color.Yellow;
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(577, 44);
            this.label3.TabIndex = 4;
            this.label3.Text = "* Clippers monitor your clipboard and replace your cryptowallet address when it\'s" +
    " copied. This tool allows to detect that. Do not copy anything yourself while it" +
    "\'s working.";
            // 
            // clipperDetectorTimer
            // 
            this.clipperDetectorTimer.Interval = 1000;
            // 
            // aboutTab
            // 
            this.aboutTab.Controls.Add(this.label6);
            this.aboutTab.Controls.Add(this.creditsLabel);
            this.aboutTab.Controls.Add(this.label5);
            this.aboutTab.Controls.Add(this.label4);
            this.aboutTab.Controls.Add(this.pictureBox1);
            this.aboutTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aboutTab.Location = new System.Drawing.Point(0, 0);
            this.aboutTab.Name = "aboutTab";
            this.aboutTab.Size = new System.Drawing.Size(601, 235);
            this.aboutTab.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(131, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "for public use.";
            // 
            // creditsLabel
            // 
            this.creditsLabel.AutoSize = true;
            this.creditsLabel.BackColor = System.Drawing.Color.Transparent;
            this.creditsLabel.Location = new System.Drawing.Point(77, 46);
            this.creditsLabel.Name = "creditsLabel";
            this.creditsLabel.Size = new System.Drawing.Size(55, 15);
            this.creditsLabel.TabIndex = 3;
            this.creditsLabel.TabStop = true;
            this.creditsLabel.Text = "Acessor";
            this.creditsLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.creditsLabel_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(12, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Created by ";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(12, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(577, 35);
            this.label4.TabIndex = 1;
            this.label4.Text = "NjCloak is an opensource tool created to protect your privacy on Windows systems " +
    "using various techniques.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = global::NJCloak.Properties.Resources.NjCloak;
            this.pictureBox1.Location = new System.Drawing.Point(233, 81);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 120);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // windowCleanerTimer
            // 
            this.windowCleanerTimer.Interval = 500;
            this.windowCleanerTimer.Tick += new System.EventHandler(this.windowCleanerTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(601, 235);
            this.Controls.Add(this.BottomMenu);
            this.Controls.Add(this.screenBlockerTab);
            this.Controls.Add(this.firewallTab);
            this.Controls.Add(this.clipperDetectorTab);
            this.Controls.Add(this.aboutTab);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "NJCloak";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.BottomMenu.ResumeLayout(false);
            this.BottomMenu.PerformLayout();
            this.screenBlockerTab.ResumeLayout(false);
            this.screenBlockerTab.PerformLayout();
            this.firewallTab.ResumeLayout(false);
            this.firewallTab.PerformLayout();
            this.clipperDetectorTab.ResumeLayout(false);
            this.aboutTab.ResumeLayout(false);
            this.aboutTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cloakButton;
        private System.Windows.Forms.StatusStrip BottomMenu;
        private System.Windows.Forms.ToolStripStatusLabel LogsLbl;
        private System.Windows.Forms.ToolStripStatusLabel AboutLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.Panel screenBlockerTab;
        private System.Windows.Forms.Panel firewallTab;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox whitelistAppPath;
        private System.Windows.Forms.Button whitelistAddButton;
        private System.Windows.Forms.Button whitelistRemoveButton;
        private System.Windows.Forms.Panel clipperDetectorTab;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button clipperDetectorButton;
        private System.Windows.Forms.CheckBox cleanTitlesCheckbox;
        private System.Windows.Forms.Button resetFirewall;
        private System.Windows.Forms.Button blockAllButton;
        private System.Windows.Forms.Timer clipperDetectorTimer;
        private System.Windows.Forms.Label clipperStatusLabel;
        private System.Windows.Forms.Panel aboutTab;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel creditsLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label firewallStatusLabel;
        private System.Windows.Forms.Button firewallSelectButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Timer windowCleanerTimer;
    }
}

