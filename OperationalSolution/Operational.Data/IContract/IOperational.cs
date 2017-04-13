using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operational.Data.IContract
{
    public interface IOperational<T>
    {
        Task<IEnumerable<T>> Get();
    }
}
