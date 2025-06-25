using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record UpdateSpeciesDto(
	[Required] int Id,
	[Required][StringLength(150)] string Name,	
	IEnumerable<string> Aliases,
	[Required] Guid AzureId
);