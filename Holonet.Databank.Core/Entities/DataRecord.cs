
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;

public class DataRecord
{
    public int Id { get; set; }

    [Url]
    [StringLength(500)]
    public string? Shard { get; set; }

    [Required]
	public required string Data { get; set; }

	public int? CharacterId { get; set; }

	public int? PlanetId { get; set; }

	public int? SpeciesId { get; set; }

	public int? HistoricalEventId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedAuthorId { get; set; }

    public Author? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public int? UpdatedAuthorId { get; set; }

    public Author? UpdatedBy { get; set; }

}
