
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Repositories;

namespace Holonet.Databank.Application.Services;
public class AuthorService(IAuthorRepository authorRepository) : IAuthorService
{
	private readonly IAuthorRepository _authorRepository = authorRepository;

	public async Task<Author?> GetAuthorById(int id)
	{
		return await _authorRepository.GetAuthor(id);
	}

	public async Task<Author?> GetAuthorById(int id, bool generateIfEmpty)
	{
		var author = await _authorRepository.GetAuthor(id);
		if(author == null && generateIfEmpty)
		{
			author = new Author { Id = 0, AzureId = Guid.Empty, DisplayName = "Unknown" };
		}
		return author;
	}

	public async Task<Author?> GetAuthorByAzureId(Guid azureId)
	{
		return await _authorRepository.GetAuthor(azureId);
	}

	public async Task<int> CreateAuthor(Author author)
	{
		return await _authorRepository.CreateAuthor(author);
	}

	public bool UpdateAuthor(Author author)
	{
		return _authorRepository.UpdateAuthor(author);
	}
}
