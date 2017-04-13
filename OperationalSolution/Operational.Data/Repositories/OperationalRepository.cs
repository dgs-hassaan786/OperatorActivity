using Operational.Data.IContract;
using Operational.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

namespace Operational.Data.Repositories
{
    public class OperationalRepository : DataRepositoryBase<OperatorReport>, IOperational<OperatorReport>
    {

        public OperationalRepository(string connectionString) : base(connectionString)
        {

        }

        public async Task<IEnumerable<OperatorReport>> Get()
        {
            try
            {
                var result = await ProcessAsync<OperatorReport>("[dbo].[OperatorProductivity]", null, commandType: CommandType.StoredProcedure);
                foreach(var x in result)
                {
                    if (x.ProactiveSent > 0)
                        x.ProactiveResponseRate = Convert.ToInt32(((double)x.ProactiveAnswered / (double)x.ProactiveSent) * 100);

                    if (x.ReactiveReceived > 0)
                        x.ReactiveResponseRate = Convert.ToInt32(((double)x.ReactiveAnswered / (double)x.ReactiveReceived) * 100);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
