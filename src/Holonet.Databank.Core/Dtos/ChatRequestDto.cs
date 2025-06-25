using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record ChatRequestDto(
	[Required] string Prompt,
	[Required] Guid AzureId
);