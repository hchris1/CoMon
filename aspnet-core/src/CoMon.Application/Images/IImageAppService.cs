using System.Threading.Tasks;

namespace CoMon.Images
{
    public interface IImageAppService
    {
        Task Delete(long id);
    }
}
