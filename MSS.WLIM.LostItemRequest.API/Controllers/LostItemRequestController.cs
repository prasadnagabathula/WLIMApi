using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.WLIM.DataServices.Models;
using MSS.WLIM.LostItemRequest.API.Services;

namespace MSS.WLIM.LostItemRequest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LostItemRequestController : ControllerBase
    {
        private readonly ILostItemRequestService _Service;
        private readonly ILogger<LostItemRequestController> _logger;

        public LostItemRequestController(ILostItemRequestService service, ILogger<LostItemRequestController> logger)
        {
            _Service = service;
            _logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<LostItemRequests>>> GetAll()
        {
            _logger.LogInformation("Fetching all LostItemRequests");
            var data = await _Service.GetAll();
            /*if (User.IsInRole("Admin"))
            {
                return Ok(data); // Admin can see all data
            }
            else
            {
                return Ok(data.Where(d => d.IsActive)); // Non-admins see only active data
            }*/
            return Ok(data);
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<LostItemRequests>> Get(string id)
        {
            _logger.LogInformation("Fetching LostItemRequests with id: {Id}", id);
            var data = await _Service.Get(id);

            if (data == null)
            {
                _logger.LogWarning("LostItemRequests with id: {Id} not found", id);
                return NotFound();
            }

            // Check if the logged-in user has the "Admin" role
            /*if (User.IsInRole("Admin"))
            {
                return Ok(data); // Admin can see both active and inactive 
            }
            else if (data.IsActive)
            {
                return Ok(data); // Non-admins can only see active data
            }*/
            /*else
            {
                _logger.LogWarning("Department with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }*/
            return Ok(data);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<LostItemRequests>> Add([FromBody] LostItemRequests createDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating LostItemRequests");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new LostItemRequests");

            try
            {
                //var departmentDto = new DepartmentDTO { Name = createDto.Name }; // Create a new DTO instance for the service
                var created = await _Service.Add(createDto);
                return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] LostItemRequests updateDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating LostItemRequests");
                return BadRequest(ModelState);
            }

            // Check if the ID in the route matches the ID in the body
            if (id != updateDto.Id)
            {
                _logger.LogWarning("LostItemRequests id: {Id} does not match with the id in the request body", id);
                return BadRequest("ID mismatch.");
            }

            try
            {
                // Map the updateDto back to the original DepartmentDTO
                //var departmentDto = new DepartmentDTO { Id = id, Name = updateDto.Name };
                await _Service.Update(updateDto);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Deleting with id: {Id}", id);
            var success = await _Service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
