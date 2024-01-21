using Abp.Configuration;
using Abp.Dependency;
using Abp.UI;
using CoMon.Configuration;
using OpenAI_API;
using OpenAI_API.Models;
using System.Threading.Tasks;

namespace CoMon.Assistant
{
    public class Assistant(ISettingManager settingManager) : ITransientDependency
    {
        private readonly ISettingManager _settingManager = settingManager;

        public async Task<string> GetSummary(string input)
        {
            var systemMessage = "I have a list of statuses from a condition monitoring system in JSON format. The data represents various groups and assets along with their respective packages and statuses. Please provide a short and concise summary using bullet points that includes the overall health of the system, highlighting any critical issues and the general status of the groups and assets. Criticality 1 means Healthy, 3 means Warning, 5 means Alert. Return plain html. Ignore the WorstStatus property. Do not mention when information is not given in the input data.";
            return await GetResponse(systemMessage, input);
        }

        public async Task<string> GetRecommendations(string input)
        {
            var systemMessage = "I have a status update from a condition monitoring system in JSON format, which represents the current state of package. Based on this information, could you provide any recommendations or actions that should be taken to maintain or improve the system’s condition? Please provide a short and concise summary using bullet points. Criticality 1 means Healthy, 3 means Warning, 5 means Alert. Return plain html. Ignore the WorstStatus property. Do not mention when information is not given in the input data.";
            return await GetResponse(systemMessage, input);
        }

        private async Task<string> GetResponse(string systemMessage, string userInput)
        {
            var openAiKey = await _settingManager.GetSettingValueAsync(AppSettingNames.OpenAiKey);

            if (string.IsNullOrEmpty(openAiKey))
                throw new UserFriendlyException("Your OpenAI API key is empty. Please enter it in the settings.");

            var api = new OpenAIAPI(openAiKey);
            var chat = api.Chat.CreateConversation();
            chat.Model = Model.GPT4_Turbo;
            chat.RequestParameters.Temperature = 0;

            chat.AppendSystemMessage(systemMessage);
            chat.AppendUserInput(userInput);

            return await chat.GetResponseFromChatbotAsync();
        }
    }
}
