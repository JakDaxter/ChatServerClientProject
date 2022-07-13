using Microsoft.Extensions.Configuration;
using NLog;
using System.Net;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;
using Thrift.Transport.Client;

namespace Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            //Config Connection to Server
            var Configuration = new TConfiguration();
            TTransport transport = new TSocketTransport(IPAddress.Loopback, 9090, Configuration);
            transport = new TBufferedTransport(transport);
            var protocol = new TBinaryProtocol(transport);
            //Creating a RabbitMQConsumer class
            IMyRabbitMQConsumer rabbitMQ = new MyRabbitMQConsumer();
            try
            {
                //logger
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var client = new ThriftTechChat.Networking.Service.Client(protocol);
                //Creating Forms 
                var loginForm = new LoginForm(client, rabbitMQ);
                var chatForm = new ChatForm(loginForm, client, rabbitMQ);
                loginForm.setChatForm(chatForm);
                rabbitMQ.setForm(chatForm);

                transport.OpenAsync(new CancellationToken());
                Application.Run(loginForm);
            
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                rabbitMQ.CloseConnection();
                protocol.Transport.Close();
                LogManager.Shutdown();
            }
        }
    }
}