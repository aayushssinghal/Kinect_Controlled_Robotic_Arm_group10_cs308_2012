namespace DexterTest
{
    partial class Form1
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
            this.comportbox = new System.Windows.Forms.ComboBox();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.RunBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comportbox
            // 
            this.comportbox.FormattingEnabled = true;
            this.comportbox.Location = new System.Drawing.Point(35, 46);
            this.comportbox.Name = "comportbox";
            this.comportbox.Size = new System.Drawing.Size(121, 21);
            this.comportbox.TabIndex = 0;
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Location = new System.Drawing.Point(197, 44);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(75, 23);
            this.ConnectBtn.TabIndex = 1;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // RunBtn
            // 
            this.RunBtn.Location = new System.Drawing.Point(80, 109);
            this.RunBtn.Name = "RunBtn";
            this.RunBtn.Size = new System.Drawing.Size(75, 23);
            this.RunBtn.TabIndex = 2;
            this.RunBtn.Text = "Run";
            this.RunBtn.UseVisualStyleBackColor = true;
            this.RunBtn.Click += new System.EventHandler(this.RunBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.RunBtn);
            this.Controls.Add(this.ConnectBtn);
            this.Controls.Add(this.comportbox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comportbox;
        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.Button RunBtn;
    }
}

