using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Models
{
    public class Roles : RolesDTO
    {
        public ICollection<Users> Users { get; set; }
    }
    public class RolesDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoleName { get; set; }
    }
}
