using AuditLogDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuditLogDemo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        IAuditLogService _auditLogService;
        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }
        // GET: api/<AuditLogController>
        [HttpGet]
        public IEnumerable<Models.AuditInfo> Get()
        {
            return _auditLogService.GetAll();
            //return new string[] { "value1", "value2" };
        }

        // GET api/<AuditLogController>/5
        [HttpGet("{id}")]
        public Models.AuditInfo Get(int id)
        {
            return _auditLogService.Get(id);
        }

        // POST api/<AuditLogController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuditLogController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuditLogController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        
    }
}
