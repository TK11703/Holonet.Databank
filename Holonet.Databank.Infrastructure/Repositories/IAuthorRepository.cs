using Holonet.Databank.Core.Entities;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface IAuthorRepository
{
	Task<int> CreateAuthor(Author itemModel);
	Task<Author?> GetAuthor(Guid azureId);
	bool UpdateAuthor(Author itemModel);
}