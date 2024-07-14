using Abp.AutoMapper;
using CoMon.Statuses.Dtos;

namespace CoMon.Groups.Dtos
{
    [AutoMapFrom(typeof(Group))]
    public class GroupPreviewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public GroupPreviewDto Parent { get; set; }
        public StatusPreviewDto WorstStatus { get; set; }

    }
}