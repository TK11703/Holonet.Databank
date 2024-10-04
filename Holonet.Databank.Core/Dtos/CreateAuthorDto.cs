using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;


public record CreateAuthorDto(
	[Required] Guid AzureId,
	[Required][StringLength(255)] string DisplayName,
	[EmailAddress][StringLength(255)] string? Email
);