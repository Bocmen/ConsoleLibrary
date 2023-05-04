using WorkInvoker.Models;

namespace WorkInvoker.Interfaces
{
    public interface IGetWorkInfo
    {
        WorkInfo GetWorkInfo(object @params);
    }
}
