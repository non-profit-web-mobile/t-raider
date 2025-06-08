using System.Text;
using System.Web;
using Model.Domain;

namespace Model.MessageClicks;

public class MessageClickEncoder : IMessageClickEncoder
{
    public MessageClick Decode(string messageKey, string redirectUri)
    {
        var messageKeyData = Convert.FromBase64String(messageKey);
        var messageKeyDataString = Encoding.UTF8.GetString(messageKeyData);

        var messageKeyQueryParams = HttpUtility.ParseQueryString(messageKeyDataString);

        return new MessageClick(
            ClickType: messageKeyQueryParams["ct"]!,
            Tactics: messageKeyQueryParams["ts"]!,
            Link: redirectUri);
    }

    public string Encode(string clickType, string tactics)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        
        queryParams["ct"] = clickType;
        queryParams["ts"] = tactics;
        
        var queryString = queryParams.ToString();
        
        var queryData = Encoding.UTF8.GetBytes(queryString);
        var queryDataString = Convert.ToBase64String(queryData);

        return queryDataString;
    }
}