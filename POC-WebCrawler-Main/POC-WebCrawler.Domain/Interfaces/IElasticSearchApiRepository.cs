using Nest;
using POC_WebCrawler.Domain.Entities;

namespace POC_WebCrawler.Domain.Interfaces
{
    public interface IElasticSearchApiRepository
    {
        Task<IEnumerable<T>> SearchDataByProperty<T>(string index, string propertyName, string value) where T : BaseEntity;
        Task<bool> DataExists<T>(string documentId, string index) where T : BaseEntity;
        Task<bool> IndexExists(string index);
        Task<IndexResponse> IndexData<T>(string index, T obj, string documentId) where T : BaseEntity;
        Task<CreateIndexResponse> CreateIndex<T>(string index) where T : BaseEntity;
    }
}
