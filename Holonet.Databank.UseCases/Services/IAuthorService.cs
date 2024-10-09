using Holonet.Databank.Core.Entities;

namespace Holonet.Databank.Application.Services;
public interface IAuthorService
{
	Task<int> CreateAuthor(Author author);
	Task<Author?> GetAuthorById(int id);
	Task<Author?> GetAuthorById(int id, bool generateIfEmpty);
	Task<Author?> GetAuthorByAzureId(Guid azureId);
	bool UpdateAuthor(Author author);
}