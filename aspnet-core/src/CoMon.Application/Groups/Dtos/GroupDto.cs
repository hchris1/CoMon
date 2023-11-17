using Abp.AutoMapper;
using CoMon.Assets.Dtos;
using System.Collections.Generic;

namespace CoMon.Groups.Dtos
{
    [AutoMapFrom(typeof(Group))]
    public class GroupDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<AssetDto> Assets { get; set; }
        public GroupPreviewDto Parent { get; set; }
        public List<GroupPreviewDto> SubGroups { get; set; }
    }
}
