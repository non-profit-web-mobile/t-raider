namespace Model.NewsProcessing;

public interface IRawNewsProcessor
{
    Task ProcessAsync(RawNewsMessage rawNewsMessage);
}