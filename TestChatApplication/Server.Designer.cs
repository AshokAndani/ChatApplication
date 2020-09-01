namespace TestChatApplication
{
    partial class Server
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
            this.ClientListBox = new System.Windows.Forms.CheckedListBox();
            this.MessagelistBox = new System.Windows.Forms.ListBox();
            this.MessagetextBox = new System.Windows.Forms.TextBox();
            this.BtnSend = new System.Windows.Forms.Button();
            this.Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ClientListBox
            // 
            this.ClientListBox.FormattingEnabled = true;
            this.ClientListBox.Location = new System.Drawing.Point(12, 12);
            this.ClientListBox.Name = "ClientListBox";
            this.ClientListBox.Size = new System.Drawing.Size(120, 169);
            this.ClientListBox.TabIndex = 0;
            // 
            // MessagelistBox
            // 
            this.MessagelistBox.FormattingEnabled = true;
            this.MessagelistBox.Location = new System.Drawing.Point(139, 13);
            this.MessagelistBox.Name = "MessagelistBox";
            this.MessagelistBox.Size = new System.Drawing.Size(254, 173);
            this.MessagelistBox.TabIndex = 1;
            // 
            // MessagetextBox
            // 
            this.MessagetextBox.Location = new System.Drawing.Point(12, 199);
            this.MessagetextBox.Name = "MessagetextBox";
            this.MessagetextBox.Size = new System.Drawing.Size(243, 20);
            this.MessagetextBox.TabIndex = 2;
            // 
            // BtnSend
            // 
            this.BtnSend.Location = new System.Drawing.Point(262, 196);
            this.BtnSend.Name = "BtnSend";
            this.BtnSend.Size = new System.Drawing.Size(75, 23);
            this.BtnSend.TabIndex = 3;
            this.BtnSend.Text = "Send";
            this.BtnSend.UseVisualStyleBackColor = true;
            this.BtnSend.Click += new System.EventHandler(this.BtnSend_Click);
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(12, 239);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(0, 13);
            this.Label.TabIndex = 4;
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 264);
            this.Controls.Add(this.Label);
            this.Controls.Add(this.BtnSend);
            this.Controls.Add(this.MessagetextBox);
            this.Controls.Add(this.MessagelistBox);
            this.Controls.Add(this.ClientListBox);
            this.Name = "Server";
            this.Text = "Server";
            this.Load += new System.EventHandler(this.Server_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox ClientListBox;
        private System.Windows.Forms.ListBox MessagelistBox;
        private System.Windows.Forms.TextBox MessagetextBox;
        private System.Windows.Forms.Button BtnSend;
        private System.Windows.Forms.Label Label;
    }
}

