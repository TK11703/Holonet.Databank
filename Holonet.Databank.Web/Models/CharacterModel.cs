using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class CharacterModel
{
	public int Id { get; set; }

	[Required(ErrorMessage ="Given name is required.")]
	[StringLength(150, ErrorMessage = "Given name can only be {1} characters in length.")]
	public string GivenName { get; set; } = string.Empty;

	[StringLength(150, ErrorMessage = "Family name can only be {1} characters in length.")]
	public string? FamilyName { get; set; } = string.Empty;

	[Url]
	[StringLength(500, ErrorMessage = "Shard can only be {1} characters in length.")]
	public string? Shard { get; set; }

	[StringLength(200)]
	public string? BirthDate
	{
		get
		{
			return BirthYear.HasValue && !string.IsNullOrEmpty(DateSystem) ? $"{BirthYear} {DateSystem}" : null;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				BirthYear = null;
				DateSystem = null;
			}
			else
			{
				var parts = value.Split(' ', 2);
				if (parts.Length == 2)
				{
					BirthYear = int.Parse(parts[0]);
					DateSystem = parts[1];
				}

			}
		}
	}

	public int? BirthYear { get; set; }

	public string? DateSystem { get; set; }

	public int? PlanetId { get; set; }

	public PlanetModel? Planet { get; set; }

    public List<int> SpeciesIds { get; set; } = [];
    public IEnumerable<SpeciesModel> Species { get; set; } = [];

	public List<int> DataRecordIds { get; set; } = [];
	public IEnumerable<DataRecordModel> DataRecords { get; set; } = [];

	public List<AliasModel> Aliases { get; set; } = new();

	public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }
}
