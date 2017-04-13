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

        [HttpGet]
        [Route("websites")]
        public async Task<IHttpActionResult> GetWebsite()
        {
            try
            {
                var result = await _Operational.GetDistinctWebsites();
                return Ok(result);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("devices")]
        public async Task<IHttpActionResult> GetDevices()
        {
            try
            {
                var result = await _Operational.GetDistinctDevices();
                return Ok(result);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody] AdvanceSearchVM model)
        {
            try
            {
                var result = await _Operational.Get(model.Device,model.Website, model.From,model.To);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

    }


    public class AdvanceSearchVM
    {
        public string Device { get; set; }
        public string Website { get; set; }
        public string To { get; set; }
        public string From { get; set; }
    }
}