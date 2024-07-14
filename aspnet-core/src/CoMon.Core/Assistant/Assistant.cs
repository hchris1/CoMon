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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Threading.Tasks;

namespace CoMon.Assistant
{
    public class Assistant(ISettingManager _settingManager, IRepository<Asset, long> _assetRepository,
        IRepository<Group, long> _groupRepository, IRepository<Package, long> _packageRepository, IRepository<Status, long> _statusRepository,
        IObjectMapper _mapper, ILogger<Assistant> _logger) : ISingletonDependency, IShouldInitialize
    {
        private Kernel _kernel;
        private readonly MemoryCache _historyCache = new(new MemoryCacheOptions()
        {
            SizeLimit = 100
        });
        private readonly OpenAIPromptExecutionSettings _promptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        private readonly object _cacheLock = new();

        public void Initialize()
        {
            KernelPluginCollection pluginCollection = [];
            pluginCollection.AddFromObject(new AssetPlugin(_assetRepository, _groupRepository, _mapper));
            pluginCollection.AddFromObject(new GroupPlugin(_assetRepository, _groupRepository, _mapper));
            pluginCollection.AddFromObject(new PackagePlugin(_assetRepository, _packageRepository, _statusRepository, _mapper));
            pluginCollection.AddFromObject(new StatusPlugin(_statusRepository, _mapper));

            _kernel = new Kernel(plugins: pluginCollection);
        }

        private void WriteHistoryToCache(Guid id, ChatHistory chatHistory)
        {
            lock (_cacheLock)
            {
                // Maximum duration of 1 hour and sliding expiration of 5 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                .SetSize(1);

                _historyCache.Set(id, chatHistory, cacheEntryOptions);
            }
        }

        private static OpenAIChatCompletionService CreateCompletionService(string openAiKey)
        {
            if (string.IsNullOrEmpty(openAiKey))
                throw new ArgumentException("OpenAI key is not set");

            return new OpenAIChatCompletionService("gpt-4o", openAiKey);
        }

        private static ChatHistory InitializeHistory(long? assetId = null, long? groupId = null, long? statusId = null, bool isRoot = false)
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("You are an assistant for a monitoring system named CoMon. Always ask for user confirmation when data is created or updated.");
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

            return chatHistory;
        }

        public async Task<AssistantAnswer> GetAnswer(Guid id, string input, long? assetId = null, long? groupId = null, long? statusId = null, bool isRoot = false)
        {
            var openAiKey = _settingManager.GetSettingValue(AppSettingNames.OpenAiKey);

            if (string.IsNullOrEmpty(openAiKey))
                return new AssistantAnswer { ChatId = id, Message = "The OpenAI key is not set. Please go to the settings to configure it." };

            IChatCompletionService chatCompletionService;
            try
            {
                chatCompletionService = CreateCompletionService(_settingManager.GetSettingValue(AppSettingNames.OpenAiKey));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing the completion service: {message}", ex.Message);
                return new AssistantAnswer { ChatId = id, Message = string.Format("Sorry, an error occurred while initializing the completion service. {0}", ex.Message) };
            }

            if (!_historyCache.TryGetValue(id, out ChatHistory chatHistory))
            {
                chatHistory = InitializeHistory(assetId, groupId, statusId, isRoot);
            }

            chatHistory.AddUserMessage(input);
            WriteHistoryToCache(id, chatHistory);

            try
            {
                var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, _promptExecutionSettings, _kernel);
                chatHistory.AddSystemMessage(response.ToString());
                WriteHistoryToCache(id, chatHistory);

                return new AssistantAnswer
                {
                    ChatId = id,
                    Message = response.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while processing the request: {message}", ex.Message);
                return new AssistantAnswer { ChatId = id, Message = "Sorry, an error occurred while processing the request. " + ex.Message };
            }
        }
    }
}
