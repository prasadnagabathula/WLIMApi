using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MSS.WLIM.DataServices.Models;
using MSS.WLIM.Role.API.Services;

namespace MSS.WLIM.Role.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _Service;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService Service, ILogger<RoleController> logger)
        {
            _Service = Service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roles>>> GetAll()
        {
            _logger.LogInformation("Fetching all Roles");
            var roles = await _Service.GetAll();
            return Ok(roles);
        }
        [HttpPost]
        public async Task<ActionResult<Roles>> Add([FromBody] Roles  roleObj)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating department");
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _Service.Add(roleObj);
                return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
