
using CyberDog.Controls;
using System.Data;
using System.Security.Policy;
using UpdatePlugin.Classes;

namespace UpdatePlugin.Forms
{
    partial class UpdateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBox1 = new ListPanel<ReleaseListItem>();
            richTextBox1 = new RichTextBox();
            linkLabel1 = new LinkLabel();
            comboBox1 = new ComboBox();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            actionBtn = new Button();
            Restart = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.White;
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Location = new Point(20, 50);
            listBox1.Margin = new Padding(0);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(330, 360);
            listBox1.TabIndex = 0;
            // 
            // richTextBox1
            // 
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Location = new Point(353, 50);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(435, 359);
            richTextBox1.TabIndex = 3;
            richTextBox1.Text = "";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.LinkBehavior = LinkBehavior.AlwaysUnderline;
            linkLabel1.Location = new Point(353, 415);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(103, 15);
            linkLabel1.TabIndex = 4;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "View release notes";
            linkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // comboBox1
            // 
            comboBox1.AllowDrop = true;
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(700, 21);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(88, 24);
            comboBox1.TabIndex = 5;
            comboBox1.DrawItem += ComboBox1_DrawItem;
            // 
            // lblTitle
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(71, 16);
            label1.Name = "lblTitle";
            label1.Size = new Size(156, 25);
            label1.TabIndex = 6;
            label1.Text = "Available PlugIns";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(25, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(40, 40);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // actionBtn
            // 
            actionBtn.Location = new Point(553, 21);
            actionBtn.Name = "actionBtn";
            actionBtn.Size = new Size(141, 24);
            actionBtn.TabIndex = 8;
            actionBtn.Text = "btnSave";
            actionBtn.UseVisualStyleBackColor = true;
            actionBtn.Click += actionBtn_Click;
            // 
            // Restart
            // 
            Restart.Location = new Point(694, 415);
            Restart.Name = "Restart";
            Restart.Size = new Size(94, 23);
            Restart.TabIndex = 10;
            Restart.Text = "Restart";
            Restart.UseVisualStyleBackColor = true;
            Restart.Click += Restart_Click;
            // 
            // UpdateForm
            // 
            Controls.Add(Restart);
            Controls.Add(actionBtn);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(comboBox1);
            Controls.Add(linkLabel1);
            Controls.Add(richTextBox1);
            Controls.Add(listBox1);
            Name = "UpdateForm";
            Size = new Size(812, 458);
            Load += UpdateForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }


        #endregion

        private ListPanel<ReleaseListItem> listBox1;
        private RichTextBox richTextBox1;
        private LinkLabel linkLabel1;
        private ComboBox comboBox1;
        private Label label1;
        private PictureBox pictureBox1;
        private Button actionBtn;
        private Button Restart;
    }
}