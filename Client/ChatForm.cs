using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThriftTechChat.Networking;

namespace Client
{
    public partial class ChatForm : Form
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private User _user;
        private readonly LoginForm _form;
        private readonly Service.Client _client;
        private Guid _actualChatId;
        private SortableBindingList<ItemInTable> _list;
        private FriendRequestForm _formRequest;
        private IMyRabbitMQConsumer _rabbitMQ;
        private static Mutex _mut = new Mutex();

        public ChatForm(LoginForm form, Service.Client client, IMyRabbitMQConsumer rabbitMQ)
        {
            InitializeComponent();
            _form = form;
            _client = client;
            _formRequest = null;
            _rabbitMQ = rabbitMQ;
            _log.Info("ChatFrom Created\n");
        }
        #region ButtonClickEvents
        /*
         * Logout User and close the ChatForm and open the LoginForm 
         */
        private async void buttonLogout_Click(object sender, EventArgs e)
        {
            _log.Info("Logout button was clicked");
            try
            {
                _mut.WaitOne();
                await _client.Logout(this._user.IdUser.ToString());
            }
            catch (ChatException ex)
            {
                _log.Error($"ChatException: {ex.Message}\n");
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex.InnerException}: {ex.Message}\n");
                MessageBox.Show($"{ex.InnerException}: {ex.Message}");
            }
            finally
            {
                //all time, if a error ocured or not, the form is closed.
                this._user = null;
                textBoxNewFriendRequest.Text = "";
                textBoxNewMessage.Text = "";
                buttonFriendRequestForm.Text = "FriendRequest";
                _rabbitMQ.CloseConnection();
                if (_formRequest != null)
                {
                    _formRequest.Close();
                }
                _list.Clear();
                this.Hide();
                _form.Show();
                _mut.ReleaseMutex();
                _log.Info("Logged is Completed\n");
            }

        }
        /*
         * Add new FriendRequest
         */
        private async void buttonAddNewFriendRequest_Click(object sender, EventArgs e)
        {
            _log.Info("AddNewFriendRequest button was clicked");
            _mut.WaitOne();
            if (textBoxNewFriendRequest.Text == "")
            {
                _mut.ReleaseMutex();
                _log.Warn("TextBoxNewFriendRequest are empty\n");
                MessageBox.Show("Put the Name of your friend in textbox");
                return;
            }
            if (textBoxNewFriendRequest.Text == _user.Name)
            {
                _mut.ReleaseMutex();
                _log.Warn("User tried to add himself to a friends\n");
                MessageBox.Show("You can not add yourself to friends");
                return;
            }
            try
            {
                labelAdding.Text = "Adding";
                buttonAddNewFiend.Enabled = false;
                var ok = await _client.AddNewFriendRequest(_user.IdUser.ToString(), textBoxNewFriendRequest.Text);
                textBoxNewFriendRequest.Text = "";
                MessageBox.Show("FriendRequest Added");
            }
            catch (ChatException ex)
            {
                _log.Error($"ChatException: {ex.Message}\n");
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex.InnerException}: {ex.Message}\n");
                MessageBox.Show($"{ex.InnerException}: {ex.Message}");
            }
            finally
            {
                _mut.ReleaseMutex();
                labelAdding.Text = "";
                buttonAddNewFiend.Enabled = true;
                _log.Info("AddNewFriendRequest are Completed\n");
            }
        }
        /*
         * Send a Message from the selected Chat
         */
        private async void buttonSend_Click(object sender, EventArgs e)
        {
            _log.Info("Send button was clicked");
            _mut.WaitOne();
            buttonSend.Enabled = false;
            try
            {
                if (dataGridViewChat.CurrentCell == null)
                {
                    _log.Warn("User didn't selected a Chat");
                    MessageBox.Show("Select a Chat first");
                    return;
                }
                if (textBoxNewMessage.Text == "")
                {
                    _log.Warn("Empty Message");
                    MessageBox.Show("Empty Message");
                    return;
                }
                var ok = await _client.AddNewMessage(textBoxNewMessage.Text, _actualChatId.ToString(), _user.Name);
                dataGridViewChat.CurrentRow.Cells[3].Value = 0;
                listBoxMessage.TopIndex = listBoxMessage.Items.Count - 1;
                textBoxNewMessage.Text = "";
            }
            catch (ChatException ex)
            {
                _log.Error($"ChatException: {ex.Message}\n");
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex.InnerException}: {ex.Message}\n");
                MessageBox.Show($"{ex.InnerException}: {ex.Message}");
            }
            finally
            {
                _mut.ReleaseMutex();
                buttonSend.Enabled = true;
                _log.Info("Send are Completed\n");
            }
        }
        /*
         * Open the FriendRequestForm and setup it
         */
        private void buttonFriendRequestForm_Click(object sender, EventArgs e)
        {
            _log.Info("FriendRequestFrom button was clicked");
            if (_formRequest == null)
            {
                _formRequest = new FriendRequestForm(_user, _client, _mut, this);
                _formRequest.SetDataInFrom();
                _formRequest.Show();
            }
            _log.Info("FriendRequestForm are Open");
        }
        #endregion
        #region FormEvents
        /*
         * Event when Form is closed, logout User and close RabbitMQ Connection
         */
        private async void ChatForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _log.Info("ChatFrom was closed, starting logout, closing connection and closing aplication");
            _mut.WaitOne();
            try
            {
                await _client.Logout(this._user.IdUser.ToString());
            }
            catch (ChatException ex)
            {
                _log.Error($"ChatException: {ex.Message}\n");
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex.InnerException}: {ex.Message}\n");
                MessageBox.Show($"{ex.InnerException}: {ex.Message}");
            }
            finally
            {
                _rabbitMQ.CloseConnection();
                _mut.ReleaseMutex();
                _log.Info("Closed are completed\n");
                Application.Exit();
            }
        }
        /*
         * When the form is open, deselect de default selected cell
         */
        private void ChatForm_VisibleChanged(object sender, EventArgs e)
        {
            _log.Info("VisibleChange event are invoke\n");
            dataGridViewChat.CurrentCell = null;
        }
        #endregion
        #region PopuletAndConfigTheFromWhenStart
        /*
         * Set up a User for this form 
         * Parameters:
         *  user: The user
         */
        public void SetUser(User user)
        {
            _log.Info("ChatFrom setUser was invoke\n");
            this._user = user;
            textBoxId.Text = $"{user.IdUser}";
        }
        /*
         * Configure the Form
         * _actualChatId put to empty
         * Clear listBoxMessage
         * Put data in dataGridViewChat and configure the columns
         * put corect value for buttonFriendRequestForm
         */
        public async Task populateAsync()
        {
            _log.Info("Starting populateAsync");
            _mut.WaitOne();
            //prepating
            _actualChatId = Guid.Empty;
            listBoxMessage.Items.Clear();
            //formating name
            var chats = await _client.GetChatsFromUser(_user.IdUser.ToString());
            chats.ForEach(x =>
            {
                if (x.IsCreatedDefaul == true)
                {
                    foreach(var y in x.Name.Split(" "))
                    {
                        if (!y.Equals(_user.Name))
                        {
                            x.Name = y;
                        }
                    }
                }
            });
            //
            //get properties from each chat from this user
            var userInChat = await _client.GetUserInChatFromUser(_user.IdUser.ToString());
            //combine the data
            var listS = new List<ItemInTable>();
            chats.ForEach(x => listS.Add(new ItemInTable()
            {
                IdChat = x.IdChat,
                Name = x.Name,
                IsCreatedDefaul = x.IsCreatedDefaul,
                NewMessagesInChat = userInChat.First(y => y.IdChat == x.IdChat).NewMessagesInChat,
                IsMuted = userInChat.First(y => y.IdChat == x.IdChat).IsMuted,
                LastMessageDate = DateTime.Parse(x.LastMessageDate)
            }));
            _list = new SortableBindingList<ItemInTable>(listS);
            //add colums config
            dataGridViewChat.DataSource = _list;
            _log.Info("Data are put in dataGridViewChat, starting colums configuration");
            foreach (var r in typeof(ItemInTable).GetProperties())
            {
                if (!r.Name.Equals("Name") && !r.Name.Equals("NewMessagesInChat") && !r.Name.Equals("IsMuted"))
                {
                    if (r.Name.Equals("LastMessageDate"))
                    {
                        dataGridViewChat.Sort(dataGridViewChat.Columns[r.Name],ListSortDirection.Descending);
                    }
                    dataGridViewChat.Columns[r.Name].Visible = false;
                }
                else
                {
                    dataGridViewChat.Columns[r.Name].ReadOnly = true;
                }
            }
            //config button FriedRequests
            buttonFriendRequestForm.Text += $": {_user.NrOfFriendRequests}";
            _mut.ReleaseMutex();
            _log.Info("populateAsync was completed");
        }
        #endregion
        #region DataGridViewEvents
        /*
         * Event for a Cell for dataFridViewChat is Clicked
         * If the column is IsMuted, change the IsMuted value
         * If is other column change Chat:
         *  Set up all Form components in relation with Chats
         *  Put new Messages in data listBoxMessages
         *  Set up the listBoxMessages to the last messages
         *  Update the parameter NewMessageInChat to 0 if it has diferent value
         */
        private async void dataGridViewChat_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _log.Info("One Cell in dataGridViewChat was clicked");
            _mut.WaitOne();
            if(e.RowIndex == -1) // When IsMuted Column is clicked  
            {
                _log.Info("The cell was clicked by ColumnsHeaderCliked event\n");
                _mut.ReleaseMutex();
                return;
            }
            if (e.ColumnIndex != -1 && dataGridViewChat.Columns[e.ColumnIndex].Name.ToString() == "IsMuted") // change mute option to a chat
            {
                var r = await _client.IsMutedChangeFromAnUserInAChat(_user.IdUser, dataGridViewChat.CurrentRow.Cells[0].Value.ToString());
                dataGridViewChat.CurrentCell.Value = r;
                _mut.ReleaseMutex();
                _log.Info("The cell was in IsMuted column, the calue was change corectly\n");
                return;
            }
            if (dataGridViewChat.CurrentRow.Index != dataGridViewChat.Rows.Count - 1 && _actualChatId != Guid.Parse((string)dataGridViewChat.CurrentRow.Cells["IdChat"].Value))
            { //bindingList, adding null last row and the chat are not the same
                _log.Info("The Cell is from other Chat, start changing data");
                _actualChatId = Guid.Parse((string)dataGridViewChat.CurrentRow.Cells["IdChat"].Value);
                labelChatName.Text = $"Chat Name: {(string)dataGridViewChat.CurrentRow.Cells["Name"].Value}";
                listBoxMessage.Items.Clear();
                var messages = await _client.GetMessagesFromChat(_actualChatId.ToString());
                if (messages.Count != 0)
                {
                    messages.Select(x => $"{x.NameOfShipper}: {x.Content}").ToList().ForEach(x => listBoxMessage.Items.Add(x));
                }
                else
                {
                    listBoxMessage.Text = "No messages";
                }
                listBoxMessage.TopIndex = listBoxMessage.Items.Count - 1;
                if((int)dataGridViewChat.CurrentRow.Cells["NewMessagesInChat"].Value != 0)
                {
                    _log.Info("New Chat had not readed Messages, start notify server that the User saw the messages now");
                    var ok = await _client.SetNewMessagesInChatTo0(_user.IdUser.ToString(), _actualChatId.ToString());
                    dataGridViewChat.CurrentRow.Cells[3].Value = "0";
                }
            }
            else
            {
                _log.Info("The selected cell are in last null row or in the same row with the last selected row");
            }
            _mut.ReleaseMutex();
            _log.Info("Cell clicked event is completed\n");
        }
        /*
         * Set Current cell to last selected Chat by User
         * if User didn't select a Chat then deselect Cell
         */
        private void dataGridViewChat_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            _log.Info("ColumnHeader in dataGridViewChat was clicked");
            if (_actualChatId != Guid.Empty)
            {
                SetCurrentCellToCurrentChat();
                _log.Info("Current Cell was change to last selected row by user");
            }
            else
            {
                dataGridViewChat.CurrentCell = null;
                _log.Info("The user has not selected any Chat before this event, therefore nothing is done");
            }
            _log.Info("ColumnHeader in dataGridViewChat clicked event was completed\n");
        }
        #endregion
        #region ListBoxMessageUpgrate
        /*
         * If Mouse is in listBoxMessage, Lock in chat for new messages and lock the last message
         *  change the value of parameter NEwMessagesInChat to 0  
         */
        private async void listBoxMessage_MouseHover(object sender, EventArgs e)
        {
            _log.Info("Hover in listBoxMessage event starting");
            _mut.WaitOne();
            if (_actualChatId != Guid.Empty) 
                if ((int)dataGridViewChat.CurrentRow.Cells["NewMessagesInChat"].Value != 0)
                    if (listBoxMessage.Items.Count > 0 && listBoxMessage.Items.Count - listBoxMessage.TopIndex <= 11)
                    {
                        var ok = await _client.SetNewMessagesInChatTo0(_user.IdUser.ToString(), _actualChatId.ToString());
                        dataGridViewChat.CurrentRow.Cells[3].Value = 0;
                    }
            _mut.ReleaseMutex();
            _log.Info("Hover in listBoxMessage event was completed");
        }
        /*
         * If Mouse is in listBoxMessage, Lock in chat for new messages and lock the last message
         *  change the value of parameter NEwMessagesInChat to 0  
         */
        private async void listBoxMessage_MouseLeave(object sender, EventArgs e)
        {
            _log.Info("Leave to listBoxMessage event Starting");
            _mut.WaitOne();
            if (_actualChatId != Guid.Empty)
                if ((int)dataGridViewChat.CurrentRow.Cells["NewMessagesInChat"].Value != 0)
                    if (listBoxMessage.Items.Count > 0 && listBoxMessage.Items.Count - listBoxMessage.TopIndex <= 11)
                    {
                        var ok = await _client.SetNewMessagesInChatTo0(_user.IdUser.ToString(), _actualChatId.ToString());
                        dataGridViewChat.CurrentRow.Cells[3].Value = 0;
                    }
            _mut.ReleaseMutex();
            _log.Info("Mouse leave to listBoxMessage event was completed");
        }
        #endregion
        #region Utily
        /*
         * Called from FormRequest when it was cloased
         */
        public void FormRequestClose()
        {
            _log.Info("ChatFrom was notify by FriendRequestForm that it has been closed");
            _formRequest = null;
        }
        /*
         * Use when the dataGridViewChat is change, and has new items
         * Change the new selected Cell to last cell User selected
         */
        private void SetCurrentCellToCurrentChat()
        {
            _log.Info("Starting SetCurrentCellToCurrentChat");
            if (_actualChatId != Guid.Empty)
            {
                foreach (DataGridViewRow row in dataGridViewChat.Rows)
                {
                    if (row.Index != dataGridViewChat.Rows.Count - 1) //SortableBinfingList add a last row with all null cells by default, is need to skip it
                    {
                        if (Guid.Parse((string)row.Cells["IdChat"].Value) == _actualChatId)
                        {
                            dataGridViewChat.CurrentCell = dataGridViewChat.Rows[row.Index].Cells["Name"];
                            _log.Info("SetCurrentCellToCurrentChat was completed and value was changed\n");
                            return;
                        }
                    }
                }
            }
            _log.Info("Value was not changed and SetCurrentCellToCurrentChat was completed");
           
        }
        #endregion
        #region Notify
        /*
         * Main notify function
         * Invoke the correct function for the message who was received
         */
        public async void NewEventFromOtherUserAsync(string text)
        {
            _log.Info("NewEventFromOtherUserAsync are starting");
            try
            {
                _mut.WaitOne();
                if (InvokeRequired)
                {
                    if (text.Split("]")[0] == "[NewChat")
                    {
                        this.Invoke(new Action(() => ReloadChatsAsync()));
                        return;
                    }
                    if (text.Split("]")[0] == "[FriendRequest")
                    {
                        this.Invoke(new Action(() => NewFriendRequest(text.Remove(0,15))));
                        if (_formRequest != null)
                        { // if fromRequest is open notify data was change 
                            _formRequest.NewEventFromOtherUser();
                        }
                        return;
                    }
                    if (text.Split("]")[0] == "[NewMessageInConversation")
                    {
                        text = text.Remove(0,26);
                        Guid chatID = Guid.Parse(text.Split(".")[0]);
                        if (chatID == _actualChatId)
                        {
                            this.Invoke(new Action(() => AddNewMessageInListBoxMessage(text.Remove(0,chatID.ToString().Length+1))));
                            return;
                        }
                        this.Invoke(new Action(() => OneMoreMessageForAChat(chatID)));
                        return;
                    }
                    throw new ChatException() {
                        Message = "Client recive an unknown notify type from the server"
                    };
                }
            }
            catch (ChatException ex)
            {
                _log.Error($"ChatException: {ex.Message}\n");
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                _log.Error($"{ex.InnerException}: {ex.Message}\n");
                MessageBox.Show($"{ex.InnerException}: {ex.Message}");
            }
            finally
            {
                _mut.ReleaseMutex();
                _log.Info("NewEventFromOtherUserAsync was completed");
            }
        }
        /*Notify
         * When a new Chat was added for this User:
         *  Get all Chats and put they in dataGridViewChat
         *  Sort the Chats
         *  deselect curent cell
         *  Change the selected Chat to last selected chat from user if User selected a Chat before this function 
         */
        private async void ReloadChatsAsync()
        {
            _log.Info("ReloadChatsAsync are starting");
            _list.Clear();
            var chats = await _client.GetChatsFromUser(_user.IdUser.ToString());
            //formating name
            chats.ForEach(x =>
            {
                if (x.IsCreatedDefaul == true)
                {
                    foreach (var y in x.Name.Split(" "))
                    {
                        if (!y.Equals(_user.Name))
                        {
                            x.Name = y;
                        }
                    }
                }
            });
            //
            //get properties from each chat from this user
            var userInChat = await _client.GetUserInChatFromUser(_user.IdUser.ToString());
            if (chats == null)
            {
                dataGridViewChat.DataSource = "";
                return;
            }
            //
            //combine
            chats.ForEach(x => _list.Add(new ItemInTable()
            {
                IdChat = x.IdChat,
                Name = x.Name,
                IsCreatedDefaul = x.IsCreatedDefaul,
                NewMessagesInChat = userInChat.First(y => y.IdChat == x.IdChat).NewMessagesInChat,
                IsMuted = userInChat.First(y => y.IdChat == x.IdChat).IsMuted,
                LastMessageDate = DateTime.Parse(x.LastMessageDate)
            }));
            dataGridViewChat.DataSource = _list;
            dataGridViewChat.Sort(dataGridViewChat.Columns["LastMessageDate"], ListSortDirection.Descending);
            dataGridViewChat.CurrentCell = null;
            if (_actualChatId != Guid.Empty)
            {
                _log.Info("Set Chat to Chat before the data are change");
                SetCurrentCellToCurrentChat();
            }
            _log.Info("ReloadChatsAsync are completed\n");
        }
        /*Notify
         * A User send to this User a New FriendRequest
         * Change the value of button to correspond the new value
         */
        public void NewFriendRequest(string text)
        {
            _log.Info("NewFriendRequest change\n");
            buttonFriendRequestForm.Text = text;
        }
        /*Notify
         * Add new Message in Current Chat
         */
        private async void AddNewMessageInListBoxMessage(string text)
        {
            _log.Info("AddNewMessageInListBoxMessage are starting");
            listBoxMessage.Items.Add(text); //add the new mesage in list box
            if (listBoxMessage.Items.Count - listBoxMessage.TopIndex <= 12) //change view to new mesage if you are in the botton
            {
                var ok = await _client.SetNewMessagesInChatTo0(_user.IdUser.ToString(), _actualChatId.ToString());
                dataGridViewChat.CurrentRow.Cells[3].Value = 0;
                listBoxMessage.TopIndex = listBoxMessage.Items.Count - 1;
                _log.Info("User was in botton of Chat");
            }
            else
            {
                dataGridViewChat.CurrentRow.Cells[3].Value = int.Parse(dataGridViewChat.CurrentRow.Cells[3].Value.ToString()) + 1;
                _log.Info("User was not in botton of Chat");
            }
            if (dataGridViewChat.CurrentRow.Index != 0) // not need change the order, because is the first Chat in dataGridViewChat
            {
                dataGridViewChat.CurrentRow.Cells["LastMessageDate"].Value = DateTime.Now; // LastMessageData was Change and 
                dataGridViewChat.Sort(dataGridViewChat.Columns["LastMessageDate"], ListSortDirection.Descending);
                SetCurrentCellToCurrentChat();
                _log.Info("dataGridViewChat was sorted");
            }
            _log.Info("AddNewMessageInListBoxMessag are completed\n");
        }
        /*Notify
         * was added new message in other Chat
         *  Change NewMessagesInChat Column value if the CHat was not Muted
         */
        private void OneMoreMessageForAChat(Guid chatID)
        {
            _log.Info("OneMoreMessageFromAChat are starting");
            foreach (DataGridViewRow row in dataGridViewChat.Rows)
            {
                if (row.Index != dataGridViewChat.Rows.Count - 1)
                {
                    var ok = (bool)row.Cells["IsMuted"].Value;
                    if (Guid.Parse((string)row.Cells["IdChat"].Value) == chatID && ok == false)
                    {
                        var a = int.Parse(row.Cells["NewMessagesInChat"].Value.ToString()) + 1;
                        row.Cells["NewMessagesInChat"].Value = a;
                        row.Cells["LastMessageDate"].Value = DateTime.Now;
                        dataGridViewChat.Sort(dataGridViewChat.Columns["LastMessageDate"], ListSortDirection.Descending);
                        SetCurrentCellToCurrentChat();
                        _log.Info("OneMoreMessageFromAChat was completed\n");
                        return;
                    }
                }
            }
        }
        #endregion
    }
}
