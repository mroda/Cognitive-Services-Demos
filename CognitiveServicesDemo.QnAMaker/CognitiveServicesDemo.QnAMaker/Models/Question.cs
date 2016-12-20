using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo.QnAMaker.Models
{
    public class Question
    {
        [JsonProperty("question")]
        public string Text { get; set; }

        public Question(string question)
        {
            this.Text = question;
        }
    }
}
