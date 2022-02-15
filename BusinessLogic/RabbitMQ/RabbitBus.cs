using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BusinessLogic.RabbitMQ
{
    public class RabbitBus : IBus
    {
        private readonly IModel _channel;
        public RabbitBus(IModel channel)
        {
            _channel = channel;
        }
        public async Task SendAsync<T>(string queueName, T message)
        {
            await Task.Run(() =>
            {
                _channel.QueueDeclare(queueName, true, false, false);
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                var output = JsonConvert.SerializeObject(message);
                _channel.BasicPublish("", queueName, properties,
                Encoding.UTF8.GetBytes(output));
            });
        }
    }
}
