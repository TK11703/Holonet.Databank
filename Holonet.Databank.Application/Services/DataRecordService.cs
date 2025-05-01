
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Repositories;
using System.Collections;

namespace Holonet.Databank.Application.Services;
public class DataRecordService(IDataRecordRepository dataRecordRepository, IAuthorService authorService) : IDataRecordService
{
	private readonly IDataRecordRepository _dataRecordRepository = dataRecordRepository;
	private readonly IAuthorService _authorService = authorService;

	public async Task<IEnumerable<DataRecord>> GetDataRecordsById(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
	{
		Hashtable authors = new Hashtable();
		var records = await _dataRecordRepository.GetDataRecords(characterId, historicalEventId, planetId, speciesId);
		foreach (var record in records)
		{
			if (record != null && record.AuthorId > 0)
			{
				if (!authors.ContainsKey(record.AuthorId))
				{
					var newAuthor = await _authorService.GetAuthorById(record.AuthorId, true);
					if (newAuthor != null)
					{
						authors.Add(record.AuthorId, newAuthor);
					}
				}
				if (authors[record.AuthorId] is Author author)
				{
					record.UpdatedBy = author;
				}
			}
		}
		return records;
	}

    public async Task<DataRecord?> GetDataRecordById(int id, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
    {
        Hashtable authors = new Hashtable();
        var record = await _dataRecordRepository.GetDataRecord(id, characterId, historicalEventId, planetId, speciesId);
        if (record != null && record.AuthorId > 0)
        {
            if (!authors.ContainsKey(record.AuthorId))
            {
                var newAuthor = await _authorService.GetAuthorById(record.AuthorId, true);
                if (newAuthor != null)
                {
                    authors.Add(record.AuthorId, newAuthor);
                }
            }
            if (authors[record.AuthorId] is Author author)
            {
                record.UpdatedBy = author;
            }
        }
        return record;
    }

    public async Task<bool> CreateDataRecord(DataRecord record)
	{
		return await _dataRecordRepository.AddDataRecord(record);
	}

    public async Task<bool> UpdateDataRecord(DataRecord record)
    {
        return await _dataRecordRepository.UpdateDataRecord(record);
    }

    public async Task<bool> DeleteDataRecord(int id, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
	{
		return await _dataRecordRepository.DeleteDataRecord(id, characterId, historicalEventId, planetId, speciesId);
	}
}
