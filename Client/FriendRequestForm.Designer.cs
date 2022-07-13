namespace Client
{
    partial class FriendRequestForm
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
            this.dataGridViewFriendRequest = new System.Windows.Forms.DataGridView();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.buttonDeny = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFriendRequest)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewFriendRequest
            // 
            this.dataGridViewFriendRequest.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFriendRequest.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewFriendRequest.MultiSelect = false;
            this.dataGridViewFriendRequest.Name = "dataGridViewFriendRequest";
            this.dataGridViewFriendRequest.ReadOnly = true;
            this.dataGridViewFriendRequest.RowTemplate.Height = 25;
            this.dataGridViewFriendRequest.Size = new System.Drawing.Size(240, 150);
            this.dataGridViewFriendRequest.TabIndex = 0;
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(12, 168);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(115, 31);
            this.buttonAccept.TabIndex = 1;
            this.buttonAccept.Text = "Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // buttonDeny
            // 
            this.buttonDeny.Location = new System.Drawing.Point(137, 168);
            this.buttonDeny.Name = "buttonDeny";
            this.buttonDeny.Size = new System.Drawing.Size(115, 31);
            this.buttonDeny.TabIndex = 2;
            this.buttonDeny.Text = "Deny";
            this.buttonDeny.UseVisualStyleBackColor = true;
            this.buttonDeny.Click += new System.EventHandler(this.buttonDeny_Click);
            // 
            // FriendRequestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 255);
            this.Controls.Add(this.buttonDeny);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.dataGridViewFriendRequest);
            this.Name = "FriendRequestForm";
            this.Text = "FriendRequestForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FriendRequestForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFriendRequest)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView dataGridViewFriendRequest;
        private Button buttonAccept;
        private Button buttonDeny;
    }
}