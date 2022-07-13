namespace Client
{
    partial class ChatForm
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
            this.buttonLogout = new System.Windows.Forms.Button();
            this.listBoxMessage = new System.Windows.Forms.ListBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.textBoxNewMessage = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.textBoxNewFriendRequest = new System.Windows.Forms.TextBox();
            this.labelNewFriendName = new System.Windows.Forms.Label();
            this.buttonAddNewFiend = new System.Windows.Forms.Button();
            this.textBoxId = new System.Windows.Forms.TextBox();
            this.dataGridViewChat = new System.Windows.Forms.DataGridView();
            this.labelId = new System.Windows.Forms.Label();
            this.buttonFriendRequestForm = new System.Windows.Forms.Button();
            this.labelAdding = new System.Windows.Forms.Label();
            this.labelChatName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChat)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonLogout
            // 
            this.buttonLogout.Location = new System.Drawing.Point(606, 305);
            this.buttonLogout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(82, 22);
            this.buttonLogout.TabIndex = 0;
            this.buttonLogout.Text = "Logout";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // listBoxMessage
            // 
            this.listBoxMessage.FormattingEnabled = true;
            this.listBoxMessage.ItemHeight = 15;
            this.listBoxMessage.Location = new System.Drawing.Point(12, 46);
            this.listBoxMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listBoxMessage.Name = "listBoxMessage";
            this.listBoxMessage.Size = new System.Drawing.Size(336, 169);
            this.listBoxMessage.TabIndex = 2;
            this.listBoxMessage.MouseLeave += new System.EventHandler(this.listBoxMessage_MouseLeave);
            this.listBoxMessage.MouseHover += new System.EventHandler(this.listBoxMessage_MouseHover);
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(5, 221);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(56, 15);
            this.labelMessage.TabIndex = 4;
            this.labelMessage.Text = "Message:";
            // 
            // textBoxNewMessage
            // 
            this.textBoxNewMessage.Location = new System.Drawing.Point(67, 221);
            this.textBoxNewMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxNewMessage.Name = "textBoxNewMessage";
            this.textBoxNewMessage.Size = new System.Drawing.Size(189, 23);
            this.textBoxNewMessage.TabIndex = 5;
            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(262, 219);
            this.buttonSend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(82, 25);
            this.buttonSend.TabIndex = 6;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // textBoxNewFriendRequest
            // 
            this.textBoxNewFriendRequest.Location = new System.Drawing.Point(455, 221);
            this.textBoxNewFriendRequest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxNewFriendRequest.Name = "textBoxNewFriendRequest";
            this.textBoxNewFriendRequest.Size = new System.Drawing.Size(147, 23);
            this.textBoxNewFriendRequest.TabIndex = 8;
            // 
            // labelNewFriendName
            // 
            this.labelNewFriendName.AutoSize = true;
            this.labelNewFriendName.Location = new System.Drawing.Point(350, 221);
            this.labelNewFriendName.Name = "labelNewFriendName";
            this.labelNewFriendName.Size = new System.Drawing.Size(99, 15);
            this.labelNewFriendName.TabIndex = 9;
            this.labelNewFriendName.Text = "NewFriendName:";
            // 
            // buttonAddNewFiend
            // 
            this.buttonAddNewFiend.Location = new System.Drawing.Point(608, 221);
            this.buttonAddNewFiend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonAddNewFiend.Name = "buttonAddNewFiend";
            this.buttonAddNewFiend.Size = new System.Drawing.Size(82, 25);
            this.buttonAddNewFiend.TabIndex = 10;
            this.buttonAddNewFiend.Text = "Add";
            this.buttonAddNewFiend.UseVisualStyleBackColor = true;
            this.buttonAddNewFiend.Click += new System.EventHandler(this.buttonAddNewFriendRequest_Click);
            // 
            // textBoxId
            // 
            this.textBoxId.Location = new System.Drawing.Point(31, 279);
            this.textBoxId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxId.Name = "textBoxId";
            this.textBoxId.ReadOnly = true;
            this.textBoxId.Size = new System.Drawing.Size(657, 23);
            this.textBoxId.TabIndex = 11;
            // 
            // dataGridViewChat
            // 
            this.dataGridViewChat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewChat.Location = new System.Drawing.Point(351, 46);
            this.dataGridViewChat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridViewChat.MultiSelect = false;
            this.dataGridViewChat.Name = "dataGridViewChat";
            this.dataGridViewChat.RowHeadersWidth = 51;
            this.dataGridViewChat.RowTemplate.Height = 29;
            this.dataGridViewChat.Size = new System.Drawing.Size(340, 169);
            this.dataGridViewChat.TabIndex = 12;
            this.dataGridViewChat.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewChat_CellClick);
            this.dataGridViewChat.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewChat_ColumnHeaderMouseClick);
            // 
            // labelId
            // 
            this.labelId.AutoSize = true;
            this.labelId.Location = new System.Drawing.Point(5, 279);
            this.labelId.Name = "labelId";
            this.labelId.Size = new System.Drawing.Size(20, 15);
            this.labelId.TabIndex = 13;
            this.labelId.Text = "Id:";
            // 
            // buttonFriendRequestForm
            // 
            this.buttonFriendRequestForm.Location = new System.Drawing.Point(455, 251);
            this.buttonFriendRequestForm.Name = "buttonFriendRequestForm";
            this.buttonFriendRequestForm.Size = new System.Drawing.Size(235, 23);
            this.buttonFriendRequestForm.TabIndex = 14;
            this.buttonFriendRequestForm.Text = "FriendRequest";
            this.buttonFriendRequestForm.UseVisualStyleBackColor = true;
            this.buttonFriendRequestForm.Click += new System.EventHandler(this.buttonFriendRequestForm_Click);
            // 
            // labelAdding
            // 
            this.labelAdding.AutoSize = true;
            this.labelAdding.Location = new System.Drawing.Point(262, 255);
            this.labelAdding.Name = "labelAdding";
            this.labelAdding.Size = new System.Drawing.Size(0, 15);
            this.labelAdding.TabIndex = 15;
            // 
            // labelChatName
            // 
            this.labelChatName.AutoSize = true;
            this.labelChatName.Location = new System.Drawing.Point(12, 9);
            this.labelChatName.Name = "labelChatName";
            this.labelChatName.Size = new System.Drawing.Size(70, 15);
            this.labelChatName.TabIndex = 16;
            this.labelChatName.Text = "Chat Name:";
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 338);
            this.Controls.Add(this.labelChatName);
            this.Controls.Add(this.labelAdding);
            this.Controls.Add(this.buttonFriendRequestForm);
            this.Controls.Add(this.labelId);
            this.Controls.Add(this.dataGridViewChat);
            this.Controls.Add(this.textBoxId);
            this.Controls.Add(this.buttonAddNewFiend);
            this.Controls.Add(this.labelNewFriendName);
            this.Controls.Add(this.textBoxNewFriendRequest);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.textBoxNewMessage);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.listBoxMessage);
            this.Controls.Add(this.buttonLogout);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ChatForm";
            this.Text = "ChatForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChatForm_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.ChatForm_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button buttonLogout;
        private ListBox listBoxMessage;
        private Label labelMessage;
        private TextBox textBoxNewMessage;
        private Button buttonSend;
        private TextBox textBoxNewFriendRequest;
        private Label labelNewFriendName;
        private Button buttonAddNewFiend;
        private TextBox textBoxId;
        private DataGridView dataGridViewChat;
        private Label labelId;
        private Button buttonFriendRequestForm;
        private Label labelAdding;
        private Label labelChatName;
    }
}