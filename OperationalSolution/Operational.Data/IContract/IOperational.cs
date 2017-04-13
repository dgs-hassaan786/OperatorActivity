using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operational.Data.IContract
{
    public interface IOperational<T>
    {
        Task<IEnumerable<T>> Get();

        Task<IEnumerable<T>> Get(string device, string website, string from, string to);

        Task<IEnumerable<string>> GetDistinctWebsites();

        Task<IEnumerable<string>> GetDistinctDevices();
    }
}
