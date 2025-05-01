using Holonet.Databank.Core.Entities;

namespace Holonet.Databank.Application.Services;
public interface IDataRecordService
{
	Task<bool> CreateDataRecord(DataRecord record);
    Task<bool> UpdateDataRecord(DataRecord record);
    Task<bool> DeleteDataRecord(int id, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
	Task<IEnumerable<DataRecord>> GetDataRecordsById(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);

    Task<DataRecord?> GetDataRecordById(int id, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
}