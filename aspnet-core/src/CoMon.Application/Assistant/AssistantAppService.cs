using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoMon.Assistant
{
    public class AssistantAppService(Assistant assistantWorker)
        : CoMonAppServiceBase
    {
        private readonly Assistant _assistantWorker = assistantWorker;

        public async Task<AssistantAnswer> GetAnswer(Guid? id, string message, long? assetId = null, long? groupId = null, long? statusId = null, bool isRoot = false)
        {
            if (!id.HasValue)
                id = Guid.NewGuid();

            return await _assistantWorker.GetAnswer(id.Value, message, assetId, groupId, statusId, isRoot);
        }
    }
}
