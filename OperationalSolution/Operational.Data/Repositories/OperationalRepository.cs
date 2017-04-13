using Operational.Data.IContract;
using Operational.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;

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

        public async Task<IEnumerable<OperatorReport>> Get(string device , string website, string from, string to)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@pWebsite", website);
                p.Add("@pDevice", device);
                p.Add("@pTo", to);
                p.Add("@pFrom", from);
                var result = await ProcessAsync<OperatorReport>("[dbo].[OperatorProductivityFiltered]", p, commandType: CommandType.StoredProcedure);
                foreach (var x in result)
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

        public async Task<IEnumerable<string>> GetDistinctDevices()
        {
            try
            {
                var result = await ProcessAsync<string>("[dbo].[DistinctDevices]", null, commandType: CommandType.StoredProcedure);
                
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetDistinctWebsites()
        {
            try
            {
                var result = await ProcessAsync<string>("[dbo].[DistinctWebsites]", null, commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
