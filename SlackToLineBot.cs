#r "Newtonsoft.Json"
#r "System.Web"

using System.Net;

using System.Text;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections.Specialized;

// Line Messaging API EndPoint
public static string LineBotApiEndpoint = "https://api.line.me/v2/bot/message/push";
// Channel Secret of LINE Bot
public static string ChennelSecret = "{Enter your secret}";
public static string ChennelId = "{Enter your Chennel Id}";
// Channel ID of LINE Bot
public static string ChannelMid = "{Enter your Mid}";
// Channel Access Token of LINE Bot
public static string ChannelAccessToken = "{Enter your Channel Access Token}";
// Target Identifier (ID of SMC group)
public static string TargetIdentifier = "{Enter target id (user or group)}";

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    var Content = await req.Content.ReadAsStringAsync();
    log.Info("Request Received : " + Content);

    // Parse parameters
    NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(Content);
    string token = queryString.Get("token");
    string team_id = queryString.Get("team_id");
    string team_domain = queryString.Get("team_domain");
    string channel_id = queryString.Get("channel_id");
    string channel_name = queryString.Get("channel_name");
    string timestamp = queryString.Get("timestamp");
    string user_id = queryString.Get("user_id");
    string user_name = queryString.Get("user_name");
    string text = queryString.Get("text");

    if (token != null){
        string message = user_name + " Ç™ " + channel_name + ", Ç…ìäçe : \n" + text;
        log.Info(message);

        var data = new  { to=TargetIdentifier, 
            messages=new [] { new{type="text", text=message}}
            };
        string json = JsonConvert.SerializeObject(data);
        log.Info("Send JSON : " + json);

        // Create POST requset to Line Bot API
        // Refer to https://developers.line.me/bot-api/api-reference#sending_message
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(LineBotApiEndpoint);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.Headers.Set("Authorization", "Bearer " + ChannelAccessToken);
        
        byte[] byteArray = Encoding.UTF8.GetBytes (json);
        request.ContentLength = byteArray.Length;

        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(byteArray, 0, byteArray.Length);
        }

        string responseContent = null;

        try {
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr99 = new StreamReader(stream))
                    {
                        responseContent = sr99.ReadToEnd();
                        log.Info("Response : " + responseContent);
                    }
                }
            }
        } catch(Exception e) {
                        log.Info("Exception : " + e.Message);
        }
    }
    return token == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        : req.CreateResponse(HttpStatusCode.OK, "Hello ");
}
