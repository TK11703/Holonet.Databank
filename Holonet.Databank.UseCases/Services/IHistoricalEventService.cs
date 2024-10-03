using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Application.Services;
public interface IHistoricalEventService
{
	Task<int> CreateHistoricalEvent(HistoricalEvent historicalEvent);
	Task<bool> DeleteHistoricalEvent(int id);
	Task<HistoricalEvent?> GetHistoricalEventById(int id);
	Task<IEnumerable<HistoricalEvent>> GetHistoricalEvents();
	Task<PageResult<HistoricalEvent>> GetPagedAsync(PageRequest pageRequest);
	Task<bool> UpdateHistoricalEvent(HistoricalEvent historicalEvent);
}