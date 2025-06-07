namespace Model.Domain;

public class NewsProcessorErrorResultFactory
{
    public NewsProcessorErrorResult Create(Exception exception)
    {
        return new NewsProcessorErrorResult(exception.Message); 
    }
}