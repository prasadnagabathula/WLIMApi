using Microsoft.EntityFrameworkCore;
using MSS.WLIM.DataServices.Data;
using MSS.WLIM.DataServices.Models;
using MSS.WLIM.DataServices.Repositories;

namespace MSS.WLIM.Department.API.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Departments> _repository;
        private readonly DataBaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DepartmentService(IRepository<Departments> repository, DataBaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAll()
        {
            var departments = await _context.WHTblDepartment.ToListAsync();

            var departmentDTOs = new List<DepartmentDTO>();

            foreach (var d in departments)
            {
                departmentDTOs.Add(new DepartmentDTO
                {
                    Id = d.Id,
                    Name = d.Name,
                    IsActive = d.IsActive,
                    CreatedBy = d.CreatedBy,
                    CreatedDate = d.CreatedDate,
                    UpdatedBy = d.UpdatedBy,
                    UpdatedDate = d.UpdatedDate
                });
            }

            return departmentDTOs;
        }

        public async Task<DepartmentDTO> Get(string id)
        {
            var department = await _context.WHTblDepartment
                .FirstOrDefaultAsync(t => t.Id == id);

            if (department == null)
                return null;

            return new DepartmentDTO
            {
                Id = department.Id,
                Name = department.Name,
                IsActive = department.IsActive,
                CreatedBy = department.CreatedBy,
                CreatedDate = department.CreatedDate,
                UpdatedBy = department.UpdatedBy,
                UpdatedDate = department.UpdatedDate
            };
        }

        public async Task<DepartmentDTO> Add(DepartmentDTO _object)
        {
            // Check if the Department name already exists
            var existingDepartment = await _context.WHTblDepartment
                .FirstOrDefaultAsync(t => t.Name == _object.Name);

            if (existingDepartment != null)
                throw new ArgumentException("A department with the same name already exists.");

            //var employeeName = _httpContextAccessor.HttpContext?.User?.FindFirst("UserName")?.Value;
            var department = new Departments
            {
                Name = _object.Name,
                IsActive = true,
                CreatedBy = "SYSTEM",
                CreatedDate = DateTime.Now
            };

            _context.WHTblDepartment.Add(department);
            await _context.SaveChangesAsync();

            _object.Id = department.Id;
            return _object;
        }

        public async Task<DepartmentDTO> Update(DepartmentDTO _object)
        {

            //var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("UserName")?.Value;
            var department = await _context.WHTblDepartment.FindAsync(_object.Id);

            if (department == null)
                throw new KeyNotFoundException("Department not found");

            department.Name = _object.Name;
            department.UpdatedBy = _object.UpdatedBy;
            department.UpdatedDate = DateTime.Now;

            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            // Check if the technology exists
            var existingData = await _repository.Get(id);
            if (existingData == null)
            {
                throw new ArgumentException($"with ID {id} not found.");
            }

            // Call repository to delete the Department
            existingData.IsActive = false; // Soft delete
            await _repository.Update(existingData); // Save changes
            return true;
        }
    }
}
