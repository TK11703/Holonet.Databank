using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class DataRecordModel : IValidatableObject
{
	public int Id { get; set; }

    [Url]
    [StringLength(500, ErrorMessage = "Shard can only be {1} characters in length.")]
    public string? Shard { get; set; }

	public string? Data { get; set; }

	public int? CharacterId { get; set; }

	public int? HistoricalEventId { get; set; }

	public int? PlanetId { get; set; }

	public int? SpeciesId { get; set; }

    public AuthorModel? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Shard) && string.IsNullOrEmpty(Data))
        {
            yield return new ValidationResult("Either Shard or Data must be provided", new[] { nameof(Shard), nameof(Data) } );
        }
    }
}
