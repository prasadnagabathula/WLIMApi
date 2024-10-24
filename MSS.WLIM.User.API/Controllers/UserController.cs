using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.WLIM.DataServices.Data;
using MSS.WLIM.DataServices.Models;
using MSS.WLIM.User.API.Services;

namespace MSS.WLIM.User.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _Service;
        private readonly ILogger<UserController> _logger;
        private readonly DataBaseContext _context;

        public UserController(IUserService Service, ILogger<UserController> logger, DataBaseContext context)
        {
            _Service = Service;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            _logger.LogInformation("Fetching all employees");
            var users = await _Service.GetAll();
            /*if (User.IsInRole("Admin"))
            {
                return Ok(users); // Admin can see all data
            }
            else
            {
                return Ok(users.Where(d => d.IsActive)); // Non-admins see only active data
            }*/
            return Ok(users);
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin, Director, Project Manager, Team Lead, Team Member")]
        public async Task<ActionResult<UserDTO>> Get(string id)
        {
            _logger.LogInformation("Fetching employee with id: {Id}", id);
            var user = await _Service.Get(id);

            if (user == null)
            {
                _logger.LogWarning("Employee with id: {Id} not found", id);
                return NotFound();
            }
            // Check if the logged-in user has the "Admin" role
            /*if (User.IsInRole("Admin"))
            {
                return Ok(user); // Admin can see both active and inactive 
            }
            else if (user.IsActive)
            {
                return Ok(user); // Non-admins can only see active data
            }*/
            /*else
            {
                _logger.LogWarning("Employee with id: {Id} is inactive and user does not have admin privileges", id);
                return Forbid(); // Return forbidden if non-admin tries to access an inactive 
            }*/
            return Ok(user);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> CreateEmployee([FromBody] UserCreateDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating employee");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating a new employee");

            try
            {
                var userDto = new UserDTO
                {
                    Name = createDto.Name,
                    Designation = createDto.Designation,
                    EmployeeID = createDto.EmployeeID,
                    EmailId = createDto.EmailId,
                    Department = createDto.Department,
                    ReportingTo = createDto.ReportingTo,
                    Password = createDto.Password,
                    Profile = createDto.Profile,
                    PhoneNo = createDto.PhoneNo,
                    Role = createDto.Role
                };
                var createdUser = await _Service.Add(userDto);
                return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uploadFile")]
        //[Authorize(Roles = "Admin, Director, Project Manager")]
        public async Task<IActionResult> UploadFile(UserProfileDTO userProfile)
        {
            try
            {
                var filePath = await _Service.UploadFileAsync(userProfile);
                return Ok(new { message = "Your File is uploaded successfully.", path = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin, Director, Project Manager, Team Lead")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating employee");
                return BadRequest(ModelState);
            }

            if (id != updateDto.Id)
            {
                _logger.LogWarning("Employee id: {Id} does not match with the id in the request body", id);
                return BadRequest("Employee ID mismatch.");
            }

            try
            {
                var userDto = new UserDTO
                {
                    Id = id,
                    Name = updateDto.Name,
                    Designation = updateDto.Designation,
                    EmployeeID = updateDto.EmployeeID,
                    EmailId = updateDto.EmailId,
                    Department = updateDto.Department,
                    ReportingTo = updateDto.ReportingTo,
                    Password = updateDto.Password,
                    Profile = updateDto.Profile,
                    PhoneNo = updateDto.PhoneNo,
                    Role = updateDto.Role
                };
                await _Service.Update(userDto);
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
            _logger.LogInformation("Deleting employee with id: {Id}", id);
            var success = await _Service.Delete(id);

            if (!success)
            {
                _logger.LogWarning("Employee with id: {Id} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
