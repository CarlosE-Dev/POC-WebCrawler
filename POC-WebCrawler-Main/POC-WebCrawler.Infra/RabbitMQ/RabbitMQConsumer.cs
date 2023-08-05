using POC_WebCrawler.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace POC_WebCrawler.Infra.RabbitMQ
{
    public class RabbitMQConsumer : IRabbitMQConsumer
    {
        private readonly IModel _channel;
        public RabbitMQConsumer(IModel channel)
        {
            _channel = channel;
        }

        public async Task<string> Consume(string queueName)
        {
            try
            {
                var message = "";
                _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Message received: {0}", message);
                };
                _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                for (var index = 0; index < 3 && string.IsNullOrEmpty(message); index++)
                {
                    await Task.Delay(1500);
                }

                return await Task.FromResult(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
