using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class DataRecordModel
{
	public int Id { get; set; }

    [Url]
    [StringLength(500, ErrorMessage = "Shard can only be {1} characters in length.")]
    public string? Shard { get; set; }

    [Required(ErrorMessage = "Data is required.")]
	public string Data { get; set; } = string.Empty;

	public int? CharacterId { get; set; }

	public int? HistoricalEventId { get; set; }

	public int? PlanetId { get; set; }

	public int? SpeciesId { get; set; }

	public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }

}
