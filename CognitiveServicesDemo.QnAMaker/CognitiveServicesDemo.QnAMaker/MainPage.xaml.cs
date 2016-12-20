using CognitiveServicesDemo.QnAMaker.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CognitiveServicesDemo.QnAMaker
{


    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            this.BindingContext = new MainPageViewModel();
        }

        //private async void BtnMakeQuestion_Clicked(object sender, EventArgs e)
        //{
        //    Indicator.IsRunning = true;

        //    //await Task.Delay(TimeSpan.FromSeconds(5));

        //    var uri = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0/knowledgebases/fd985fb6-0c4d-4d6a-97e4-24976a4ddfc0/generateAnswer", UriKind.Absolute);

        //    var QnA = new QnAQuestion(editorQuestion.Text);

        //    var json = JsonConvert.SerializeObject(QnA);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1ec61daac44c4859a1d8f8ab545f7152");

        //        HttpResponseMessage response = null;

        //        response = await client.PostAsync(uri, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = await response.Content.ReadAsStringAsync();

        //            TextResult.Text = result;
        //        }
        //    }


        //    Indicator.IsRunning = false;
        //}
    }
}
