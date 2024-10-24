using Microsoft.EntityFrameworkCore;
using MSS.WLIM.DataServices.Data;
using MSS.WLIM.DataServices.Models;
using MSS.WLIM.DataServices.Repositories;

namespace MSS.WLIM.User.API.Services
{
    public class UserService : IUserService
    {
        private readonly DataBaseContext _context;
        private readonly IRepository<Users> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(DataBaseContext context, IRepository<Users> repository, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var employees = await _context.WHTblUser
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Roles)
                .Include(e => e.ReportingToUser)
                .ToListAsync();

            var empDtos = employees.Select(employee => new UserDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                Designation = employee.Designation?.Name,
                EmployeeID = employee.EmployeeID,
                EmailId = employee.EmailId,
                Department = employee.Department?.Name,
                ReportingTo = employee.ReportingToUser?.Name,
                IsActive = employee.IsActive,
                CreatedBy = employee.CreatedBy,
                CreatedDate = employee.CreatedDate,
                UpdatedBy = employee.UpdatedBy,
                UpdatedDate = employee.UpdatedDate,
                Profile = employee.Profile,
                PhoneNo = employee.PhoneNo,
                Role = employee.Roles?.RoleName
            }).ToList();

            return empDtos;
        }

        public async Task<UserDTO> Get(string id)
        {
            var employee = await _context.WHTblUser
                  .Include(e => e.Department)
                  .Include(e => e.Designation)
                  .Include(e => e.Roles)
                  .Include(e => e.ReportingToUser)
                  .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return null;

            return new UserDTO
            {
                Id = employee.Id,
                Name = employee.Name,
                Designation = employee.Designation?.Name,
                EmployeeID = employee.EmployeeID,
                EmailId = employee.EmailId,
                Department = employee.Department?.Name,
                ReportingTo = employee.ReportingToUser?.Name,
                IsActive = employee.IsActive,
                CreatedBy = employee.CreatedBy,
                CreatedDate = employee.CreatedDate,
                UpdatedBy = employee.UpdatedBy,
                UpdatedDate = employee.UpdatedDate,
                Profile = employee.Profile,
                PhoneNo = employee.PhoneNo,
                Role = employee.Roles?.RoleName
            };
        }

        public async Task<UserDTO> Add(UserDTO empDto)
        {
            //var employeeName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;

            // Check if Employee name is unique
            var existingEmployeeName = await _context.WHTblUser
                .FirstOrDefaultAsync(e => e.Name == empDto.Name);

            if (existingEmployeeName != null)
            {
                throw new ArgumentException("Employee Name must be unique. This Employee Name is already in use.");
            }

            // Check if EmployeeID is unique
            var existingEmployeeID = await _context.WHTblUser
                .FirstOrDefaultAsync(e => e.EmployeeID == empDto.EmployeeID);

            if (existingEmployeeID != null)
            {
                throw new ArgumentException("EmployeeID must be unique. This EmployeeID is already in use.");
            }

            // Check if EmailId is unique
            var existingEmailId = await _context.WHTblUser
                .FirstOrDefaultAsync(e => e.EmailId == empDto.EmailId);

            if (existingEmailId != null)
            {
                throw new ArgumentException("EmailId must be unique. This EmailId is already in use.");
            }

            // Check if phone number is unique
            var existingEmployeeWithSamePhoneNo = await _context.WHTblUser
                .FirstOrDefaultAsync(e => e.PhoneNo == empDto.PhoneNo);

            if (existingEmployeeWithSamePhoneNo != null)
            {
                throw new ArgumentException("Phone number must be unique. This phone number is already in use.");
            }

            var employee = new Users();
            //----------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Designation))
            {
                var designation = await _context.WHTblDesignation
                    .FirstOrDefaultAsync(d => d.Name == empDto.Designation);
                if (designation == null)
                {
                    throw new ArgumentException($"Invalid designation. Please enter a valid designation.");
                }
                employee.DesignationId = designation.Id;
            }
            else
            {
                employee.DepartmentId = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Department))
            {
                var department = await _context.WHTblDepartment
                    .FirstOrDefaultAsync(d => d.Name == empDto.Department);
                if (department == null)
                {
                    throw new ArgumentException($"Invalid department name. Please enter a valid department name.");
                }

                employee.DepartmentId = department.Id;
            }
            else
            {
                employee.DepartmentId = null;
            }
            //------------------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.ReportingTo))
            {
                var reportingTo = await _context.WHTblUser
                    .FirstOrDefaultAsync(d => d.Name == empDto.ReportingTo);
                if (reportingTo == null)
                {
                    throw new ArgumentException($"Invalid ReportingTo name. Please enter a valid ReportingTo name.");
                }
                employee.ReportingTo = reportingTo.Id;
            }
            else
            {
                employee.ReportingTo = null;
            }
           
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Profile))
            {
                employee.Profile = empDto.Profile;
            }
            else
            {
                employee.Profile = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.PhoneNo))
            {
                employee.PhoneNo = empDto.PhoneNo;
            }
            else
            {
                employee.PhoneNo = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Role))
            {
                var role = await _context.WHTblRole
                    .FirstOrDefaultAsync(d => d.RoleName == empDto.Role);
                if (role == null)
                {
                    throw new ArgumentException($"Invalid Role. Please enter a valid Role.");
                }
                employee.Role = role.Id;
            }
            else
            {
                employee.ReportingTo = null;
            }
            //---------------------------------------------------------
            employee.Name = empDto.Name;
            employee.EmployeeID = empDto.EmployeeID;
            employee.EmailId = empDto.EmailId;
            employee.IsActive = true;
            employee.CreatedBy = "SYSTEM";
            employee.CreatedDate = DateTime.Now;
            employee.Password = PasswordHasher.HashPassword(empDto.Password);
            _context.WHTblUser.Add(employee);
            await _context.SaveChangesAsync();

            empDto.Id = employee.Id;

            return empDto;
        }

        public async Task<string> UploadFileAsync(UserProfileDTO employeeProfile)
        {
            string filePath = "";
            try
            {
                // Check if the file is not empty
                if (employeeProfile.Profile.Length > 0)
                {
                    var file = employeeProfile.Profile;
                    filePath = Path.GetFullPath($"C:\\Users\\mshaik5\\Desktop\\UploadProfiles\\{file.FileName}");

                    // Save file to the specified path
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Update employee's profile if ID is provided
                    if (!string.IsNullOrEmpty(employeeProfile.Id))
                    {
                        var employee = await Get(employeeProfile.Id);

                        if (employee != null)
                        {
                            employee.Profile = file.FileName;
                            await Update(employee);
                        }
                    }
                    else
                    {
                        return file.FileName;
                    }
                }
                else
                {
                    throw new Exception("The uploaded file is empty.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while uploading the file: " + ex.Message);
            }

            return filePath;
        }

        public async Task<UserDTO> Update(UserDTO empDto)
        {
            //var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("EmployeeName")?.Value;

            var employee = await _context.WHTblUser.FindAsync(empDto.Id);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            //----------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Designation))
            {
                var designation = await _context.WHTblDesignation
                    .FirstOrDefaultAsync(d => d.Name == empDto.Designation);
                if (designation == null)
                {
                    throw new ArgumentException($"Invalid designation. Please enter a valid designation.");
                }
                employee.DesignationId = designation.Id;
            }
            else
            {
                employee.DepartmentId = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Department))
            {
                var department = await _context.WHTblDepartment
                    .FirstOrDefaultAsync(d => d.Name == empDto.Department);
                if (department == null)
                {
                    throw new ArgumentException($"Invalid department name. Please enter a valid department name.");
                }

                employee.DepartmentId = department.Id;
            }
            else
            {
                employee.DepartmentId = null;
            }
            //------------------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.ReportingTo))
            {
                var reportingTo = await _context.WHTblUser
                    .FirstOrDefaultAsync(d => d.Name == empDto.ReportingTo);
                if (reportingTo == null)
                {
                    throw new ArgumentException($"Invalid ReportingTo name. Please enter a valid ReportingTo name.");
                }
                employee.ReportingTo = reportingTo.Id;
            }
            else
            {
                employee.ReportingTo = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Profile))
            {
                employee.Profile = empDto.Profile;
            }
            else
            {
                employee.Profile = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.PhoneNo))
            {
                employee.PhoneNo = empDto.PhoneNo;
            }
            else
            {
                employee.PhoneNo = null;
            }
            //---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(empDto.Role))
            {
                var role = await _context.WHTblRole
                    .FirstOrDefaultAsync(d => d.RoleName == empDto.Role);
                if (role == null)
                {
                    throw new ArgumentException($"Invalid Role. Please enter a valid Role.");
                }
                employee.Role = role.Id;
            }
            else
            {
                employee.ReportingTo = null;
            }
            //---------------------------------------------------------
            employee.Name = empDto.Name;
            employee.EmployeeID = empDto.EmployeeID;
            employee.EmailId = empDto.EmailId;
            employee.IsActive = true;
            employee.UpdatedBy = empDto.UpdatedBy;
            employee.UpdatedDate = DateTime.Now;
            employee.Password = PasswordHasher.HashPassword(empDto.Password);

            // Set the Profile property if a file is uploaded
            if (!string.IsNullOrEmpty(empDto.Profile))
            {
                employee.Profile = empDto.Profile;
            }

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return empDto;
        }

        public async Task<bool> Delete(string id)
        {
            /*            var employee = await _context.TblEmployee.FindAsync(id);
                        if (employee == null) return false;
                        _context.TblEmployee.Remove(employee);
                        await _context.SaveChangesAsync();
                        return true;*/

            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"with ID {id} not found.");
            }
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}
