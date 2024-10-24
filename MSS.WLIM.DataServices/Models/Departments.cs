using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Models
{
    public class Departments : DepartmentDTO
    {
        public ICollection<Users> Users { get; set; }

    }
    public class DepartmentDTO : AuditData
    {
        public string Name { get; set; }
    }
    public class DepartmentCreateDTO
    {
        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Name { get; set; }
    }
    public class DepartmentUpdateDTO : DepartmentCreateDTO
    {
        public string Id { get; set; }
    }
}
