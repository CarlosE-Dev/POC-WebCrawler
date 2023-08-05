using Nest;
using POC_WebCrawler.Domain.Entities;
using POC_WebCrawler.Domain.Interfaces;

namespace POC_WebCrawler.Infra.ElasticSearch
{
    public class ElasticSearchApiRepository : IElasticSearchApiRepository
    {
        private readonly IElasticClient _elasticClient;
        public ElasticSearchApiRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<IEnumerable<T>> SearchDataByProperty<T>(string index, string propertyName, string value) where T : BaseEntity
        {
            Type type = typeof(T);
            var property = type.GetProperty(propertyName);

            if(property == null)
                return new List<T>();

            var search = await _elasticClient.SearchAsync<T>(s => s
                .Index(index)
                .Query(q => q.Match(x => x.Field(propertyName.ToLower()).Query(value)))
            );

            return await Task.FromResult(search.Documents);
        }

        public async Task<bool> DataExists<T>(string documentId, string index) where T : BaseEntity
        {
            var result = await _elasticClient.DocumentExistsAsync<T>(documentId, d => d.Index(index));
            return result.Exists;
        }

        public async Task<bool> IndexExists(string index)
        {
            var result = await _elasticClient.Indices.ExistsAsync(index);
            return result.Exists;
        }

        public async Task<IndexResponse> IndexData<T>(string index, T obj, string documentId) where T : BaseEntity
        {
            return await _elasticClient.IndexAsync(obj, i => i.Index(index).Id(documentId));
        }

        public async Task<CreateIndexResponse> CreateIndex<T>(string index) where T : BaseEntity
        {
            return await _elasticClient.Indices.CreateAsync(index, c => c.Map<T>(m => m.AutoMap()));
        }
    }
}
