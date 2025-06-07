using System.Text.Json;
using Model.Domain;

public static class NewsProcessorErrorResultFactory
{
    public static NewsProcessorErrorResult Create(Exception exception)
    {
        var errorInfo = SerializeException(exception);

        var serialized = JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return new NewsProcessorErrorResult(serialized);
    }

    private static object? SerializeException(Exception? exception)
    {
        if (exception == null) return null;

        var dataDictionary = new Dictionary<string, object>();

        foreach (var key in exception.Data.Keys)
        {
            if (key != null)
            {
                dataDictionary[key.ToString()!] = exception.Data[key];
            }
        }

        return new
        {
            Type = exception.GetType().FullName,
            Message = exception.Message,
            StackTrace = exception.StackTrace,
            Source = exception.Source,
            Data = dataDictionary.Count > 0 ? dataDictionary : null,
            InnerException = SerializeException(exception.InnerException)
        };
    }
}