using Microsoft.EntityFrameworkCore;
using MSS.WLIM.DataServices.Data;
using MSS.WLIM.DataServices.Models;

namespace MSS.WLIM.Role.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly DataBaseContext _context;

        public RoleService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Roles>> GetAll()
        {
            var roles = await _context.WHTblRole.ToListAsync();
            var roleDtos = new List<Roles>();

            foreach (var r in roles)
            {
                roleDtos.Add(new Roles
                {
                    Id = r.Id,
                    RoleName = r.RoleName
                });
            }

            return roleDtos;
        }

        public async Task<Roles> Add(Roles _object)
        {
            // Check if the Role name already exists
            var existingRole = await _context.WHTblRole
                .FirstOrDefaultAsync(t => t.RoleName == _object.RoleName);

            if (existingRole != null)
                throw new ArgumentException("A role with the same name already exists.");

            var role = new Roles
            {
                RoleName = _object.RoleName
            };

            _context.WHTblRole.Add(role);
            await _context.SaveChangesAsync();

            _object.Id = role.Id;
            return _object;
        }
    }
}
