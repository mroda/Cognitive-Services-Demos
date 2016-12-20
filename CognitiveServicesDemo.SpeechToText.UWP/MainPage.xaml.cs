using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CognitiveServicesDemo.SpeechToText.UWP
{
    public sealed partial class MainPage : Page
    {
        MediaCapture capture;
        InMemoryRandomAccessStream buffer;
        
        private readonly string subscriptionKey = "{your subscription key}";

        public MainPage()
        {
            this.InitializeComponent();
        }

        public async Task<string> GetTextFromAudioAsync(Stream audiostream)
        {
            Authentication auth = new Authentication(subscriptionKey);

            var requestUri = @"https://speech.platform.bing.com/recognize?scenarios=ulm&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5&locale=es-ES&device.os=WinOs&form=BCSSTT&version=3.0&format=json&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3&requestid=" + Guid.NewGuid();
            
            using (var client = new HttpClient())
            {
                var token = auth.GetAccessToken();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                using (var binaryContent = new ByteArrayContent(StreamToBytes(audiostream)))
                {
                    binaryContent.Headers.TryAddWithoutValidation("content-type", "audio/wav; codec=\"audio/pcm\"; samplerate=16000");

                    var response = await client.PostAsync(requestUri, binaryContent);
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(responseString);
                    return data.header.name;
                }
            }
        }

        private static byte[] StreamToBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private async void btnStartRecord_Click(object sender, RoutedEventArgs e)
        {
            btnStartRecord.IsEnabled = !btnStartRecord.IsEnabled;
            btnStopRecord.IsEnabled = !btnStopRecord.IsEnabled;

            await RecordProcess();
            await capture.StartRecordToStreamAsync(MediaEncodingProfile.CreateWav(AudioEncodingQuality.High), buffer);
        }

        private async void btnStopRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnStopRecord.IsEnabled = !btnStopRecord.IsEnabled;

                this.ProgressRing.IsActive = true;

                await capture.StopRecordAsync();

                var text = await GetTextFromAudioAsync(buffer.AsStream());
            }
            catch (Exception ex)
            {
                var dialog = new Windows.UI.Popups.MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            this.ProgressRing.IsActive = false;
            btnStartRecord.IsEnabled = !btnStartRecord.IsEnabled;
        }

        private async Task<bool> RecordProcess()
        {
            if (buffer != null)
            {
                buffer.Dispose();
            }
            buffer = new InMemoryRandomAccessStream();
            if (capture != null)
            {
                capture.Dispose();
            }
            try
            {
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio
                };
                capture = new MediaCapture();
                await capture.InitializeAsync(settings);
                capture.RecordLimitationExceeded += (MediaCapture sender) =>
                {
                    throw new Exception("Record Limitation Exceeded ");
                };
                capture.Failed += (MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs) =>
                {
                    throw new Exception(string.Format("Code: {0}. {1}", errorEventArgs.Code, errorEventArgs.Message));
                };
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }

    }
}
