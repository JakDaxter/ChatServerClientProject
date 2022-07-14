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
    public partial class FriendRequestForm : Form
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private User _user;
        private readonly Service.Client _client;
        private IBindingList _users;
        private readonly ChatForm _chatForm;
        private bool _IAccept = false;
        private static Mutex _mut;
        public FriendRequestForm(User user, Service.Client client, Mutex mut, ChatForm chatForm)
        {
            InitializeComponent();
            _user = user;
            _client = client;
            _users = new BindingList<User>();
            _chatForm = chatForm;
            _mut = mut;
            _log.Info("FriendRequestForm Created Corectly\n");
        }

        /*
         * Configure and put data to DataGridView
         */
        public async void SetDataInFrom()
        {
            _log.Info("Start SetDataInFrom");
            try
            {
                _mut.WaitOne();
                var list = await _client.GetFromUsersInFriendRequestFromUser(_user.IdUser.ToString());
                list.ForEach(x => _users.Add(x));
                dataGridViewFriendRequest.DataSource = _users;

                foreach (var r in typeof(User).GetProperties())
                {
                    if (!r.Name.Equals("Name"))
                    {
                        dataGridViewFriendRequest.Columns[r.Name].Visible = false;
                    }
                    else
                    {
                        dataGridViewFriendRequest.Columns[r.Name].ReadOnly = true;
                    }
                }
                dataGridViewFriendRequest.CurrentCell = null;
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
                _log.Info("SetDataInFrom are completed\n");
            }
        }
        /*
         * Deny the FriendRequest for selected User
         */
        private async void buttonDeny_Click(object sender, EventArgs e)
        {
            _log.Info("Deny button are clicked");
            if (dataGridViewFriendRequest.CurrentCell == null)
            {
                _log.Warn("Thre is no request selected\n");
                MessageBox.Show("Select a request");
                return;
            }
            try
            {
                _mut.WaitOne();
                var idUser = (string)dataGridViewFriendRequest.CurrentRow.Cells["IdUser"].Value;
                await _client.DenyFriendRequest(_user.IdUser, idUser);
                _users.RemoveAt(dataGridViewFriendRequest.CurrentRow.Index); // Remove Row
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
                _log.Info("Deny event completed\n");
            }
        }
        /*
         * Accept the FriendRequest for selected User
         */
        private async void buttonAccept_Click(object sender, EventArgs e)
        {
            _log.Info("Accept button are clicked");
            if (dataGridViewFriendRequest.CurrentCell == null) //Error if not user is selected
            {
                _log.Warn("Thre is no request selected\n");
                MessageBox.Show("Select a request");
                return;
            }
            try
            {
                _mut.WaitOne();
                var idUser = (string)dataGridViewFriendRequest.CurrentRow.Cells["IdUser"].Value;
                _IAccept = true;
                await _client.AcceptFriendRequest(_user.IdUser, idUser);
                _users.RemoveAt(dataGridViewFriendRequest.CurrentRow.Index); //remove Row
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
                _log.Info("Deny event completed\n");
            }
        }
        /*
         * When the form is closed notify parent ChatFrom that it has been closed 
         */
        private void FriendRequestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _log.Info("Form is closed, notify ChatForm of this event");
            _chatForm.FormRequestClose();
            _log.Info("Notify Completed\n");
        }
        #region notify
        /*
         * Event from others user
         */
        public void NewEventFromOtherUser()
        {
            _log.Info("Start a event from other user notify");
            try
            {
                _mut.WaitOne();
                if (InvokeRequired)
                {
                    if (!_IAccept) // if it is not the From which started the notify, the data does not have to be changed
                    {
                        _log.Info("This form is not the started of the events");
                        this.Invoke(new Action(() => ReloadRequest()));
                    }
                    else
                    {
                        _log.Info("The event are started from this form");
                        _IAccept = false;
                    }

                }
            }
            finally
            {
                _mut.ReleaseMutex();
                _log.Info("the notify are completed\n");
            }
        }
        /*
         * If a request is set from another User when this from is open, reload all list, and keep selected row 
         */
        private async void ReloadRequest()
        {
            _log.Info("ReloadRequest are starting");
            string idUser = null;
            if (dataGridViewFriendRequest.CurrentCell != null)
            {
                idUser = (string)dataGridViewFriendRequest.CurrentRow.Cells["IdUser"].Value;
            }
            _users.Clear();
            var list = await _client.GetFromUsersInFriendRequestFromUser(_user.IdUser.ToString());
            list.ForEach(x => _users.Add(x));
            _log.Info("Dara is added to list");
            dataGridViewFriendRequest.DataSource = _users;
            if (idUser != null)
            {
                _log.Info("Starting to select the current cell where it was before");
                foreach (DataGridViewRow row in dataGridViewFriendRequest.Rows)
                {
                    if ((string)row.Cells["IdUser"].Value == idUser)
                    {
                        dataGridViewFriendRequest.CurrentCell = row.Cells["Name"];
                        _log.Info("The cell are corectly selected and ReloadRequest is completed\n");
                        return;
                    }

                }
                _log.Error("The selected row does not exist in new data");
                MessageBox.Show("Error: The selected row not exist in new data");
            }
            _log.Info("ReloadRequest is completed\n");
        }
        #endregion
    }
}
