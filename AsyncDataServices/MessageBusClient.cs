using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;
using ILogger = Serilog.ILogger;

namespace PlatformService.AsyncDataServices
{
  public class MessageBusClient : IMessageBusClient
  {
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(ILogger logger, IConfiguration configuration)
    {
      _logger = logger;
      _configuration = configuration;

      _logger.Information("--> Initializing MessageBusClient.");
      var factory = new ConnectionFactory() 
      {
        HostName = _configuration["RabbitMQHost"],
        Port = int.Parse(_configuration["RabbitMQPort"])
      };

      try
      {
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        _logger.Information("--> Connected to MessageBus.");
      }
      catch (Exception ex)
      {
        _logger.Warning("--> Could not connect to the Message Bus: {exceptionMessage}", ex.Message);
      }
    }
    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
      var message = JsonSerializer.Serialize(platformPublishedDto);

      if (_connection.IsOpen)
      {
        _logger.Information("--> RabbitMQ Connection Open, sending message...");
        SendMessage(message);
      }
      else
      {
        _logger.Information("--> RabbitMQ Connection Closed, not sending.");
      }
    }

    private void SendMessage(string message)
    {
      var body = Encoding.UTF8.GetBytes(message);

      _channel.BasicPublish(exchange: "trigger",
                            routingKey: "",
                            basicProperties: null,
                            body: body);
      
      _logger.Information("--> We have sent {message}.", message);
    }

    public void Dispose()
    {
      _logger.Information("--> MessageBus Disposed.");
      if(_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      _logger.Information("--> RabbitMQ Connection Shutdown.");
    }
  }
}