using Abp.AutoMapper;

namespace CoMon.Groups.Dtos
{
    [AutoMapFrom(typeof(Group))]
    public class GroupPreviewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public GroupPreviewDto Parent { get; set; }
    }
}