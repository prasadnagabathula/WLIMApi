using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MSS.WLIM.DataServices.Data;

namespace MSS.WLIM.DataServices.Models
{
    public class Users : UserDTO
    {
        [StringLength(36)]
        public string? DesignationId { get; set; }
        public string? DepartmentId { get; set; }

        [ForeignKey("DesignationId")]
        public Designations Designation { get; set; }

        [ForeignKey("DepartmentId")]
        public Departments Department { get; set; }

        [ForeignKey("Role")]
        public Roles Roles { get; set; }

        [ForeignKey("ReportingTo")]
        public Users? ReportingToUser { get; set; }
        public ICollection<Users> Subordinates { get; set; }
    }

    public class UserProfileDTO
    {
        public string? Id { get; set; }
        [FileExtensions(Extensions = "doc,docx,pdf", ErrorMessage = "Profile must be a .doc, .docx, or .pdf file.")]
        public IFormFile Profile { get; set; }

    }
    public class UserDTO : AuditData
    {
        public string Name { get; set; }
        public string? Designation { get; set; }
        public string EmployeeID { get; set; }
        public string EmailId { get; set; }
        public string? Department { get; set; }
        public string? Password { get; set; }
        public string PhoneNo { get; set; }
        public string? Role { get; set; }
        public string? ReportingTo { get; set; }
        public string? Profile { get; set; }

    }
    public class UserCreateDTO
    {

        [Required(ErrorMessage = "The Name field is required.")]
        [MinLength(3)]
        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "The Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Special characters and Digits are not allowed.")]
        public string Name { get; set; }
        [StringLength(36)]
        public string? Designation { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(8)]
        [RegularExpression(@"^[0-9\s]+$", ErrorMessage = "Only digits are allowed.")]
        public string EmployeeID { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        [EmailDomain("miraclesoft.com", ErrorMessage = "Email must contain 'miraclesoft.com'.")]
        public string EmailId { get; set; }
        public string? Department { get; set; }
        public string? ReportingTo { get; set; }
        [Required]
        // Password Validation: minimum length of 8 characters with at least 1 uppercase, 1 lowercase, 1 digit, and 1 special character
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? Password { get; set; }
        public string? Profile { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits long.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number must be numeric.")]
        public string PhoneNo { get; set; }
        [Required]
        public string? Role { get; set; }

    }
    public class UserUpdateDTO : UserCreateDTO
    {
        public string Id { get; set; }
    }

    // Custom Email Domain Validation Attribute
    public class EmailDomainAttribute : ValidationAttribute
    {
        private readonly string _domainName;

        public EmailDomainAttribute(string domainName)
        {
            _domainName = domainName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();
                if (email.Contains(_domainName))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage ?? $"Email must contain the domain '{_domainName}'");
                }
            }
            return new ValidationResult("Email is required.");
        }
        // Custom Validation Attribute for PhoneNo Uniqueness
        public class UniquePhoneNumberAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var context = (DataBaseContext)validationContext.GetService(typeof(DataBaseContext));
                string phoneNo = value.ToString();
                bool exists = context.WHTblUser.Any(e => e.PhoneNo == phoneNo);

                if (exists)
                {
                    return new ValidationResult("Phone number must be unique.");
                }

                return ValidationResult.Success;
            }
        }
    }
}
