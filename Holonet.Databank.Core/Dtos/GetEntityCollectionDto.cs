using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record GetEntityCollectionDto(
	[Required] bool PopulateEntities,
    [Required] bool PopulateDataRecords,    
	long? UtcTicks
);
