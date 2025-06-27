using Holonet.Databank.Core.Entities;

namespace Holonet.Databank.Application.Services;
public interface IDataRecordService
{
	Task<int?> CreateDataRecord(DataRecord record);
    Task<bool> RecordExists(string shard, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
    Task<bool> UpdateDataRecord(DataRecord record);
    Task<bool> DeleteDataRecord(int id, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
	Task<IEnumerable<DataRecord>> GetDataRecordsById(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);

    Task<DataRecord?> GetDataRecordById(int id, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
}