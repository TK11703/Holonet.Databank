using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record GetHistoricalEventDto(
	[Required] int Id,
	[Required][StringLength(150)] string Name
);
