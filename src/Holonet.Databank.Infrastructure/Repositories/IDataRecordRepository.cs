using Holonet.Databank.Core.Entities;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface IDataRecordRepository
{
	Task<int?> AddDataRecord(DataRecord record);
    Task<bool> RecordExists(string shard, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
    Task<bool> UpdateDataRecord(DataRecord record);
    Task<bool> DeleteDataRecord(int recordId, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
	Task<bool> DeleteDataRecords(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
	Task<IEnumerable<DataRecord>> GetDataRecords(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
    Task<DataRecord?> GetDataRecord(int id, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
}