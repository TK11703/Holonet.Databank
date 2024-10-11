using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class HistoricalEventModel
{	
	public int Id { get; set; }

	[Required]
	[StringLength(150)]
	public string Name { get; set; } = string.Empty;

	public string? Description { get; set; }

	[Url]
	[StringLength(500)]
	public string? Shard { get; set; }

	[StringLength(200)]
	public string? DatePeriod 
	{
		get
		{
			if(YearStarted.HasValue && !string.IsNullOrEmpty(YearStartedDateSystem))
			{
				if (YearEnded.HasValue && !string.IsNullOrEmpty(YearEndedDateSystem))
				{
					return $"{YearStarted} {YearStartedDateSystem} - {YearEnded} {YearEndedDateSystem}";
				}
				else
				{
					return $"{YearStarted} {YearStartedDateSystem} - Present";
				}
			}
			else if(!YearStarted.HasValue && string.IsNullOrEmpty(YearStartedDateSystem) && YearEnded.HasValue && !string.IsNullOrEmpty(YearEndedDateSystem))
			{
				return $"Unknown - {YearEnded} {YearEndedDateSystem}";
			}
			return null;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				YearStarted = null;
				YearStartedDateSystem = null;
				YearEnded = null;
				YearEndedDateSystem = null;
			}
			else
			{
				//Code up a date parser to parse the values of YearStarted, YearStartedDateSystem, YearEnded, and YearEndedDateSystem. The expected value is either "YearStarted YearStartedDateSystem - YearEnded YearEndedDateSystem" or "YearStarted YearStartedDateSystem - Present" or "Unknown - YearEnded YearEndedDateSystem" or "Unknown".
				var parts = value.Split(" - ");
				if (parts.Length == 2)
				{
					var startParts = parts[0].Split(' ');
					if (startParts.Length == 2 && int.TryParse(startParts[0], out int yearStarted))
					{
						YearStarted = yearStarted;
						YearStartedDateSystem = startParts[1];
					}
					else if (parts[0] == "Unknown")
					{
						YearStarted = null;
						YearStartedDateSystem = null;
					}

					var endParts = parts[1].Split(' ');
					if (endParts.Length == 2 && int.TryParse(endParts[0], out int yearEnded))
					{
						YearEnded = yearEnded;
						YearEndedDateSystem = endParts[1];
					}
					else if (parts[1] == "Present" || parts[1] == "Unknown")
					{
						YearEnded = null;
						YearEndedDateSystem = null;
					}
				}
			}
		}
	}

	public int? YearStarted { get; set; }

	public string? YearStartedDateSystem { get; set; }

	public int? YearEnded { get; set; }

	public string? YearEndedDateSystem { get; set; }

	public List<int> CharacterIds { get; set; } = [];
	public IEnumerable<CharacterModel> Characters { get; set; } = [];
	public List<int> PlanetIds { get; set; } = [];
	public IEnumerable<PlanetModel> Planets { get; set; } = [];

	public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }
}
