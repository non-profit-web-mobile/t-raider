namespace Model.Kafka;

public interface ITopicInfoProvider
{
	TopicInfo GetRawNewsTopicInfo();

	TopicInfo GetHypothesesTopicInfo();

	TopicInfo GetHypothesesForUsersTopicInfo();
	
	TopicInfo GetAdminSignalsTopicInfo();
}