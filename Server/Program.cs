

using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistance;
using Server;
using System.Reflection;
using Thrift;
using Thrift.Processor;
using Thrift.Protocol;
using Thrift.Server;
using Thrift.Transport;
using Thrift.Transport.Server;
[assembly: XmlConfigurator(Watch = true)]

//log4net configuracion
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
var loggerFactory = LoggerFactory.Create(builder => {
    builder.AddConsole();
    builder.AddDebug();
});
//Repo, DbContex
var context = new MyDbContext(loggerFactory);
_log.Info("DbContext was Creted");

//thrift Server configuracion
TConfiguration Configuration = new TConfiguration();
var transport = new TServerSocketTransport(9090, Configuration);
var buffer = new TBufferedTransport.Factory();
var protocol = new TBinaryProtocol.Factory();
var implementacion = new ThriftImplementation(context);
var procces = new ThriftTechChat.Networking.Service.AsyncProcessor(implementacion);
var server = new TThreadPoolAsyncServer(
    processorFactory: new TSingletonProcessorFactory(procces),
    serverTransport: transport,
    inputTransportFactory: buffer,
    outputTransportFactory: buffer,
    inputProtocolFactory: protocol,
    outputProtocolFactory: protocol,
    minThreadPoolThreads: 1,
    maxThreadPoolThreads: 9,
    logger: loggerFactory.CreateLogger("Services")
    );
_log.Info("Server Created and starting server");

//start server
await server.ServeAsync(new CancellationToken());
_log.Info("Server Closing");
