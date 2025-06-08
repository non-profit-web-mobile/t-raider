using System.ComponentModel.DataAnnotations;

namespace Model.Gpt;

public class GptOptions
{
	public const string SectionKey = "Gpt";

	[Required]
	public required string ApiKey { get; set; }

	[Required]
	public required string Uri { get; set; }
}