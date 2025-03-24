using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliHub.Models
{
    public class ChatApiRequest
    {
        
        public static ChatCompletion Send(List<Message> msgs, string apikey, string Model = "gpt-3.5-turbo")
        {
            try
            {
                var client = new RestClient("https://api.chatanywhere.tech/v1/chat/completions");
                var request = new RestRequest("", method: Method.Post);
                request.AddHeader("Authorization", $"Bearer {apikey}");
                request.AddHeader("Content-Type", "application/json");

                var body = new ChatRequest();
                body.Model = Model;
                body.Messages = msgs;

                request.AddParameter("application/json", JsonConvert.SerializeObject(body, Formatting.Indented), ParameterType.RequestBody);

                var response = JsonConvert.DeserializeObject<ChatCompletion>(client.Execute(request).Content);

                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}
