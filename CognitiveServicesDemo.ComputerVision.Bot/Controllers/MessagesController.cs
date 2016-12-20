using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.IO;

namespace CognitiveServicesDemo.ComputerVision.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IVisionService captionService = new VisionService();
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                string message;

                try
                {
                    message = await this.GetCaptionAsync(activity, connector);
                }
                catch (ArgumentException e)
                {
                    message = "Me enviaste una foto?. Intenta enviarme una foto.";

                    Trace.TraceError(e.ToString());
                }
                catch (Exception e)
                {
                    message = "Oops! Algo anduvo mal. Intenta de nuevo mas tarde.";

                    Trace.TraceError(e.ToString());
                }

                Activity reply = activity.CreateReply(message);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                await this.HandleSystemMessage(activity);
            }

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                // Greet the user the first time the bot is added to a conversation.
                if (message.MembersAdded.Any(m => m.Id == message.Recipient.Id))
                {
                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));

                    var response = message.CreateReply();
                    response.Text = "Hola yo soy un robot e intentare entender el contenido de cualquier foto que me envies.";

                    await connector.Conversations.ReplyToActivityAsync(response);
                }
                
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private async Task<string> GetCaptionAsync(Activity activity, ConnectorClient connector)
        {
            var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
            if (imageAttachment != null)
            {
                using (var stream = await GetImageStream(connector, imageAttachment))
                {
                    return await this.captionService.GetCaptionAsync(stream);
                }
            }

            string url;
            if (TryParseAnchorTag(activity.Text, out url))
            {
                return await this.captionService.GetCaptionAsync(url);
            }

            if (Uri.IsWellFormedUriString(activity.Text, UriKind.Absolute))
            {
                return await this.captionService.GetCaptionAsync(activity.Text);
            }
            
            throw new ArgumentException("El activity no contiene una imagen adjunta valilda.");
        }

        private static async Task<Stream> GetImageStream(ConnectorClient connector, Attachment imageAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(imageAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                }
                else
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(imageAttachment.ContentType));
                }

                return await httpClient.GetStreamAsync(uri);
            }
        }

        private static bool TryParseAnchorTag(string text, out string url)
        {
            var regex = new Regex("^<a href=\"(?<href>[^\"]*)\">[^<]*</a>$", RegexOptions.IgnoreCase);
            url = regex.Matches(text).OfType<Match>().Select(m => m.Groups["href"].Value).FirstOrDefault();
            return url != null;
        }

        private static async Task<string> GetTokenAsync(ConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
            {
                return await credentials.GetTokenAsync();
            }

            return null;
        }
        
    }
}