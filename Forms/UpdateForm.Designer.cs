
using System.Security.Policy;

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
            listBox1 = new ListBox();
            richTextBox1 = new RichTextBox();
            linkLabel1 = new LinkLabel();
            comboBox1 = new ComboBox();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.White;
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.Font = new Font("Segoe UI", 12F);
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 72;
            listBox1.Location = new Point(20, 50);
            listBox1.Margin = new Padding(0);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(330, 360);
            listBox1.TabIndex = 0;
            listBox1.DrawItem += ListBox1_DrawItem;
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
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
            // UpdateForm
            // 
            Controls.Add(comboBox1);
            Controls.Add(linkLabel1);
            Controls.Add(richTextBox1);
            Controls.Add(listBox1);
            Name = "UpdateForm";
            Size = new Size(812, 458);
            Load += UpdateForm_Load;
            ResumeLayout(false);
            PerformLayout();

        }


        #endregion

        private ListBox listBox1;
        private RichTextBox richTextBox1;
        private LinkLabel linkLabel1;
        private ComboBox comboBox1;
    }
}