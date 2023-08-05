namespace POC_WebCrawler.Domain.Interfaces
{
    public interface IRabbitMQConsumer
    {
        Task<string> Consume(string queueName);
    }
}
