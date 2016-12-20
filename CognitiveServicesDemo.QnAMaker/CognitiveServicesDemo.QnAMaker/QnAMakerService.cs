using CognitiveServicesDemo.QnAMaker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.QnAMaker
{
    public class QnAMakerService
    {
        private readonly string knowledgeBaseId = "your kbid";
        private readonly string subscriptionKey = "your subscription key";
        string uriString;

        public QnAMakerService()
        {
            uriString = $"https://westus.api.cognitive.microsoft.com/qnamaker/v1.0/knowledgebases/{knowledgeBaseId}/generateAnswer";
        }

        public async Task<Answer> GetAnswer(string question)
        {
            var uri = new Uri(uriString, UriKind.Absolute);

            var entity = new Question(question);

            var json = JsonConvert.SerializeObject(entity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                HttpResponseMessage response = null;

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    var answer = JsonConvert.DeserializeObject<Answer>(result);
                    answer.Text = System.Net.WebUtility.HtmlDecode(answer.Text);

                    return answer;
                }
            }

            return null;
        }
    }
}
