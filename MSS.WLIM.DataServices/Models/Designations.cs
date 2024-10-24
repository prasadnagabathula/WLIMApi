using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Models
{
    public class Designations : DesignationDTO
    {
        public ICollection<Users> Users { get; set; }
    }
    public class DesignationDTO : AuditData
    {
        public string Name { get; set; }
    }
    public class DesignationCreateDTO
    {
        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Name { get; set; }
    }
    public class DesignationUpdateDTO : DesignationCreateDTO
    {
        public string Id { get; set; }
    }
}
