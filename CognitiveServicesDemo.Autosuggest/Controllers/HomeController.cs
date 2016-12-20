using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net;

namespace CognitiveServicesDemo.Autosuggest.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<BingAPIConfig> bingAPIconfig;

        public HomeController(IOptions<BingAPIConfig> bingAPIconfig)
        {
            this.bingAPIconfig = bingAPIconfig;
        }

        public async Task<IActionResult> Index(string query)
        {
            query = System.Net.WebUtility.UrlEncode("miguel");
            if (!string.IsNullOrEmpty(query))
            {
                var autosuggestKey = bingAPIconfig.Value.AutosuggestKey;
                

                var client = new HttpClient();
                
                //Encabezados del Request
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{autosuggestKey}");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "mkt=en-us");
                

                //Parametros del Request
                var uri = "https://api.cognitive.microsoft.com/bing/v5.0/suggestions/?q=" + $"{ query}";

                

                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                
                
                var json = await response.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse(json);

                if (data.suggestionGroups != null && data.suggestionGroups.Count > 0 && data.suggestionGroups[0].searchSuggestions != null)
                {
                    for (int i = 0; i < data.suggestionGroups[0].searchSuggestions.Count; i++)
                    {
                        //suggestions.Add(data.suggestionGroups[0].searchSuggestions[i].displayText.Value);
                    }
                }
            }

            return View();
        }

        private void responsecallback(IAsyncResult ar)
        {
            //Stream resStream = response.GetResponseStream();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
