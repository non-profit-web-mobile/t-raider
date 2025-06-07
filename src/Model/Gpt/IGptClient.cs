using Model.Domain;

namespace Model.Gpt;

public interface IGptClient
{
    Task<INewsProcessorResult> ProcessNewsAsync(string newsUrl, int sourceReliability);
}