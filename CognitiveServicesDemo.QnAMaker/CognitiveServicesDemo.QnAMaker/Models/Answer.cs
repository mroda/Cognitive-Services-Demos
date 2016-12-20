using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.QnAMaker.Models
{
    public class Answer
    {
        [JsonProperty("answer")]
        public string Text { get; set; }

        [JsonProperty("score")]
        public string Score { get; set; }
    }
}
