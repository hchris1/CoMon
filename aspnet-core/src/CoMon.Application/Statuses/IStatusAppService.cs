using Abp.Application.Services.Dto;
using CoMon.Statuses.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoMon.Statuses
{
    public interface IStatusAppService
    {
        Task<StatusDto> Get(long id);
        Task<StatusHistoryDto> GetHistory(long id);
        Task<StatusTableOptionsDto> GetStatusTableOptions();
        Task<PagedResultDto<StatusPreviewDto>> GetStatusTable(PagedResultRequestDto request,
            long? assetId, long? groupId, long? packageId, Criticality? criticality, bool latestOnly = true);
        Task<List<StatusPreviewDto>> GetLatestStatusPreviews(long assetId);
    }
}
