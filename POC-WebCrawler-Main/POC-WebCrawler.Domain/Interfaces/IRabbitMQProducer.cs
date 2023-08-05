namespace POC_WebCrawler.Domain.Interfaces
{
    public interface IRabbitMQProducer
    {
        Task Send(string queueName, string message);
    }
}
