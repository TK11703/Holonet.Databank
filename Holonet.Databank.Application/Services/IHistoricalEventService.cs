using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Application.Services;
public interface IHistoricalEventService
{
    Task<int> CreateHistoricalEvent(HistoricalEvent historicalEvent);
    Task<bool> DeleteHistoricalEvent(int id);
    Task<HistoricalEvent?> GetHistoricalEventById(int id);
    Task<IEnumerable<HistoricalEvent>> GetHistoricalEvents(bool populate = false, bool populateDataRecords = false);
    Task<IEnumerable<HistoricalEvent>> GetHistoricalEvents(long utcTicks, bool populate = false, bool populateDataRecords = false);
    Task<PageResult<HistoricalEvent>> GetPagedAsync(PageRequest pageRequest);
    Task<bool> HistoricalEventExists(int id, string name);
    Task<bool> UpdateHistoricalEvent(HistoricalEvent historicalEvent);
}