using Abp;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using CoMon.Assets;
using CoMon.Assistant.Plugins;
using CoMon.Configuration;
using CoMon.Groups;
using CoMon.Packages;
using CoMon.Statuses;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoMon.Assistant
{
    public class Assistant(ISettingManager _settingManager, IRepository<Asset, long> _assetRepository,
        IRepository<Group, long> _groupRepository, IRepository<Package, long> _packageRepository, IRepository<Status, long> _statusRepository, IObjectMapper _mapper) : ISingletonDependency, IShouldInitialize
    {
        private Kernel _kernel;
        private readonly Dictionary<Guid, ChatHistory> _historyById = [];
        private readonly OpenAIPromptExecutionSettings _promptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        private bool _initialized = false;

        public void Initialize()
        {
            var openAiKey = _settingManager.GetSettingValue(AppSettingNames.OpenAiKey);

            if (string.IsNullOrEmpty(openAiKey))
                return;

            KernelPluginCollection pluginCollection = [];
            pluginCollection.AddFromObject(new AssetPlugin(_assetRepository, _groupRepository, _mapper));
            pluginCollection.AddFromObject(new GroupPlugin(_assetRepository, _groupRepository, _mapper));
            pluginCollection.AddFromObject(new PackagePlugin(_assetRepository, _packageRepository, _statusRepository, _mapper));
            pluginCollection.AddFromObject(new StatusPlugin(_statusRepository, _mapper));

            _kernel = new Kernel(plugins: pluginCollection);
        }

        private IChatCompletionService CreateCompletionService(string openAiKey)
        {
            if (string.IsNullOrEmpty(openAiKey))
                throw new ArgumentException("OpenAI key is not set");

            return new OpenAIChatCompletionService("gpt-4o", openAiKey);
        }


        public async Task<AssistantAnswer> GetAnswer(Guid id, string input, long? assetId = null, long? groupId = null, long? statusId = null, bool isRoot = false)
        {
            IChatCompletionService chatCompletionService;
            try
            {
                chatCompletionService = CreateCompletionService(_settingManager.GetSettingValue(AppSettingNames.OpenAiKey));
            }
            catch (Exception ex)
            {
                return new AssistantAnswer { ChatId = id, Message = string.Format("Sorry, an error occurred while processing the request. {message}", ex.Message) };
            }

            if (!_historyById.TryGetValue(id, out var chatHistory))
            {
                chatHistory = new();
                _historyById[id] = chatHistory;

                chatHistory.AddSystemMessage("You are an assistant for a monitoring system named CoMon.");
                chatHistory.AddSystemMessage("Criticality Types: Healthy = 1, Warning = 3, Alert = 5");
                chatHistory.AddSystemMessage("Package Types: Ping = 0, Http = 1, Rtsp = 2, External = 10");

                if (assetId.HasValue)
                    chatHistory.AddSystemMessage($"The user is currently looking at the asset with ID {assetId}");

                if (groupId.HasValue)
                    chatHistory.AddSystemMessage($"The user is currently looking at the group with ID {groupId}");

                if (statusId.HasValue)
                    chatHistory.AddSystemMessage($"The user is currently looking at the status with ID {statusId}");

                if (isRoot)
                    chatHistory.AddSystemMessage("The user is currently in the root group");
            }

            chatHistory.AddUserMessage(input);

            try
            {
                var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, _promptExecutionSettings, _kernel);
                return new AssistantAnswer
                {
                    ChatId = id,
                    Message = response.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AssistantAnswer { ChatId = id, Message = "Sorry, an error occurred while processing the request. " + ex.Message };
            }
        }
    }
}
