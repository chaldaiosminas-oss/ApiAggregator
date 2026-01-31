using ApiAggregator.Models;

namespace ApiAggregator.Services
{
    public interface IExternalApiService
    {
        string Name { get; }
        Task<IEnumerable<UnifiedResult>> FetchAsync();
    }
}
