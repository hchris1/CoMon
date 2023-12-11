using CoMon.Groups.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoMon.Groups
{
    public interface IGroupAppService
    {
        Task<GroupDto> GetRoot();
        //Task<List<GroupDto>> GetAll();
        Task<GroupDto> Get(long id);
        Task<long> Create(CreateGroupDto input);
        Task UpdateName(long id, string name);
        Task UpdateParent(long id, long? parentId);
        Task Delete(long id);
    }
}
