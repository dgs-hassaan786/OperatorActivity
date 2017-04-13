using Operational.Data.IContract;
using Operational.Data.Repositories;
using Operational.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Operational.Services.Controllers
{
    [RoutePrefix("api/operational")]
    public class OperationalController : ApiController
    {
        IOperational<OperatorReport> _Operational;

        public OperationalController()
        {
            _Operational = new OperationalRepository(ConfigurationManager.AppSettings["DBConnectionString"]);
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var result = await _Operational.Get();
                return Ok(result);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
}