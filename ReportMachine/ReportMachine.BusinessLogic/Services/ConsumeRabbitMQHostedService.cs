using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReportMachine.BusinessLogic.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace ReportMachine.BusinessLogic.Services
{
    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        private readonly IMessageHandlingService handlingService;
        private IConnection _connection;
        private IModel _channel;

        public ConsumeRabbitMQHostedService(ILoggerFactory loggerFactory, IMessageHandlingService handlingService)
        {
            InitRabbitMQ();
            this.handlingService = handlingService;
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("create.queue", true, false, false);
            _channel.QueueDeclare("approve.queue", true, false, false);
            _channel.QueueDeclare("decline.queue", true, false, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var createConsumer = new EventingBasicConsumer(_channel);
            createConsumer.Received += async (sender, e) =>
            {
                var content = Encoding.UTF8.GetString(e.Body.ToArray());
                await handlingService.HandleNewBooking(content);
                _channel.BasicAck(e.DeliveryTag, false);
            };

            var approveConsumer = new EventingBasicConsumer(_channel);
            approveConsumer.Received += async (sender, e) =>
            {
                var content = Encoding.UTF8.GetString(e.Body.ToArray());
                await handlingService.HandleApprovedBooking(content);
                _channel.BasicAck(e.DeliveryTag, false);
            };

            var declineConsumer = new EventingBasicConsumer(_channel);
            declineConsumer.Received += async (sender, e) =>
            {
                var content = Encoding.UTF8.GetString(e.Body.ToArray());
                await handlingService.HandleDeclinedBooking(content);
                _channel.BasicAck(e.DeliveryTag, false);
            };

            _channel.BasicConsume("create.queue", false, createConsumer);
            _channel.BasicConsume("approve.queue", false, approveConsumer);
            _channel.BasicConsume("decline.queue", false, declineConsumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
