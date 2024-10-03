using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface IHistoricalEventRepository
{
	Task<int> CreateHistoricalEvent(HistoricalEvent itemModel);
	Task<bool> DeleteHistoricalEvent(int id);
	Task<HistoricalEvent?> GetHistoricalEvent(int id);
	Task<IEnumerable<HistoricalEvent>> GetHistoricalEvents();
	Task<PageResult<HistoricalEvent>> GetPagedAsync(PageRequest pageRequest);
	Task<bool> HistoricalEventExists(int id, string name);
	bool UpdateHistoricalEvent(HistoricalEvent itemModel);
}