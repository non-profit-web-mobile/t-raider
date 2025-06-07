namespace Model.Kafka;

public interface ITopicInfoProvider
{
	TopicInfo GetItemsTopicInfo();
	
	TopicInfo GetRawNewsTopicInfo();
}