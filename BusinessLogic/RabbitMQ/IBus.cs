using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.RabbitMQ
{
    public interface IBus
    {
        public Task SendAsync<T>(string queueName, T message);
    }
}
