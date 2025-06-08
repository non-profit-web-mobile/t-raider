using Model.Domain;

namespace Model.MessageClicks;

public interface IMessageClickEncoder
{
	MessageClick Decode(string messageKey, string redirectUri);
	string Encode(string clickType, string tactics);
}