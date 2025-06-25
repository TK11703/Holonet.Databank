using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;
public record AuthorDto(
	int Id,
	Guid AzureId,
	string DisplayName,
	string Email
);