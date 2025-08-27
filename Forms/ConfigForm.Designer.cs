namespace UpdatePlugin.Forms
{
    partial class ConfigForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            txtRepository = new TextBox();
            lblRepository = new Label();
            txtInstallLocation = new TextBox();
            lblInstallLocation = new Label();
            btnSave = new Button();
            btnRevertRepository = new Button();
            btnRevertInstall = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(18, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(261, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Plugin Updater Configuration";
            // 
            // txtRepository
            // 
            txtRepository.Location = new Point(121, 78);
            txtRepository.Name = "txtRepository";
            txtRepository.Size = new Size(476, 23);
            txtRepository.TabIndex = 1;
            txtRepository.TextChanged += txtRepository_TextChanged;
            // 
            // lblRepository
            // 
            lblRepository.AutoSize = true;
            lblRepository.Location = new Point(28, 81);
            lblRepository.Name = "lblRepository";
            lblRepository.Size = new Size(93, 15);
            lblRepository.TabIndex = 2;
            lblRepository.Text = "Repository URL :";
            // 
            // txtInstallLocation
            // 
            txtInstallLocation.Location = new Point(121, 122);
            txtInstallLocation.Name = "txtInstallLocation";
            txtInstallLocation.Size = new Size(476, 23);
            txtInstallLocation.TabIndex = 3;
            txtInstallLocation.TextChanged += txtInstallLocation_TextChanged;
            // 
            // lblInstallLocation
            // 
            lblInstallLocation.AutoSize = true;
            lblInstallLocation.Location = new Point(50, 127);
            lblInstallLocation.Name = "lblInstallLocation";
            lblInstallLocation.Size = new Size(71, 15);
            lblInstallLocation.TabIndex = 4;
            lblInstallLocation.Text = "Install Path :";
            lblInstallLocation.TextAlign = ContentAlignment.TopRight;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(566, 166);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 5;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += button1_Click;
            // 
            // btnRevertRepository
            // 
            btnRevertRepository.BackgroundImageLayout = ImageLayout.Zoom;
            btnRevertRepository.Location = new Point(603, 78);
            btnRevertRepository.Name = "btnRevertRepository";
            btnRevertRepository.Size = new Size(23, 23);
            btnRevertRepository.TabIndex = 6;
            btnRevertRepository.UseVisualStyleBackColor = true;
            btnRevertRepository.Click += btnRevertRepository_Click;
            // 
            // btnRevertInstall
            // 
            btnRevertInstall.BackgroundImageLayout = ImageLayout.Zoom;
            btnRevertInstall.Location = new Point(603, 123);
            btnRevertInstall.Name = "btnRevertInstall";
            btnRevertInstall.Size = new Size(23, 23);
            btnRevertInstall.TabIndex = 7;
            btnRevertInstall.UseVisualStyleBackColor = true;
            btnRevertInstall.Click += btnRevertInstall_Click;
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnRevertInstall);
            Controls.Add(btnRevertRepository);
            Controls.Add(btnSave);
            Controls.Add(lblInstallLocation);
            Controls.Add(txtInstallLocation);
            Controls.Add(lblRepository);
            Controls.Add(txtRepository);
            Controls.Add(lblTitle);
            Name = "ConfigForm";
            Size = new Size(646, 210);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TextBox txtRepository;
        private Label lblRepository;
        private TextBox txtInstallLocation;
        private Label lblInstallLocation;
        private Button btnSave;
        private Button btnRevertRepository;
        private Button btnRevertInstall;
    }
}
