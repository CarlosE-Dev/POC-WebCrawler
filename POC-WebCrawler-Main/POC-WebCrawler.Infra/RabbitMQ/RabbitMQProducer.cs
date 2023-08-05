using POC_WebCrawler.Domain.Interfaces;
using RabbitMQ.Client;

namespace POC_WebCrawler.Infra.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IModel _channel;
        public RabbitMQProducer(IModel channel)
        {
            _channel = channel;
        }

        public Task Send(string queueName, string message)
        {
            try
            {
                _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var body = System.Text.Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
