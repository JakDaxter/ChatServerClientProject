using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Thrift.Transport;
using ThriftTechChat.Networking;
namespace Client
{
    public partial class LoginForm : Form
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private ChatForm _form;
        private readonly Service.Client _client;
        private IMyRabbitMQConsumer _rabbitMQ;
        public LoginForm(Service.Client client, IMyRabbitMQConsumer rabbitMQ)
        {
            InitializeComponent();
            this._client = client;
            this._rabbitMQ = rabbitMQ;
            _log.Info("LoginForm Created\n");
        }
        /*
         * Using to set a ChatFrom
         */
        public void setChatForm(ChatForm form)
        {
            this._form = form;
            _log.Info("ChatForm are set for ChatForm\n");
        }
        /*
         * Loggin
         * Create a rabbitMQ Connection and a ChatFrom, and configure it
         */
        private async void buttonLogin_Click(object sender, EventArgs e)
        {
            _log.Info("Login button has clicked");
            if (textBoxName.Text == "")
            {
                _log.Warn("Name is empty\n");
                MessageBox.Show("Name is empty");
                return;
            }
            if (textBoxPassword.Text == "")
            {
                _log.Warn("Password is empty\n");
                MessageBox.Show("Password is empty");
                return;
            }
            try
            {
                buttonLogin.Enabled = false;
                User user = await _client.Login(textBoxName.Text, textBoxPassword.Text);
                if (user == null)
                {
                    _log.Warn("Invalid User");
                    throw new ChatException()
{
                        Message = "Invalid User"
                    };
                }
                _log.Info("User was corect, starting change to ChatFrom");
                try
                {
                    _form.SetUser(user);
                    await _form.populateAsync();
                    _rabbitMQ.CreateConnection(user.IdUser);
                    _form.Text = user.Name;
                    _form.Show();
                    this.Hide();
                }
                catch(Exception ex)
                {
                    await _client.Logout(user.IdUser);
                    throw new Exception(ex.Message,ex.InnerException);
                }
            }
            catch(ChatException ex)
            {
                _log.Error($"ChatException: {ex.Message}\n");
                MessageBox.Show(ex.Message);
            }
            catch(TTransportException)
            {
                _log.Error($"TTransportException: Server is Close\n");
                MessageBox.Show("Error: Server is not online");
            }
            catch(Exception ex)
            {
                _log.Error($"{ex.InnerException} {ex.Message}\n");
                MessageBox.Show($"{ex.InnerException}: {ex.Message}");
            }
            finally
            {
                buttonLogin.Enabled = true;
                _log.Info("Change to ChatFrom is corectly executed");
            }
        }
        /*
         * Add a new User
         */
        private async void buttonAddNewUser_Click(object sender, EventArgs e)
        {
            _log.Info("AddNewUser was clicked");
            buttonAddNewUser.Enabled = false;
            buttonLogin.Enabled = false;
            labelAdding.Text = "Adding the new User, plase wait";
            if (textBoxName.Text == "")
            {
                _log.Warn("Name is empty\n");
                MessageBox.Show("Name is empty");
                return;
            }
            if (textBoxPassword.Text == "")
            {
                _log.Warn("Password is empty\n");
                MessageBox.Show("Password is empty");
                return;
            }
            try
            {
                await _client.AddNewUser(textBoxName.Text, textBoxPassword.Text);
                MessageBox.Show("New User added");
            }catch(ChatException ex)
            {
                _log.Error($"ChatException: {ex.Message}\n");
                MessageBox.Show(ex.Message);
            }
            catch (TTransportException)
            {
                _log.Error("TTransportException: Server is not online\n");
                MessageBox.Show("Error: Server is not online");
            }
            catch (Exception ex)
            {
                _log.Error($"{ex.InnerException}: {ex.Message}\n");
                MessageBox.Show($"{ex.InnerException}: {ex.Message}");
            }
            finally
            {
                buttonLogin.Enabled = true;
                buttonAddNewUser.Enabled = true;
                labelAdding.Text = "";
                _log.Info("New User added corectly\n");
            }
        }
    }
}