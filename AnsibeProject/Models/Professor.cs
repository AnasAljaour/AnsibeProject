using System.ComponentModel.DataAnnotations;

namespace AnsibeProject.Models
{
    public class Professor
    {
        [Required(ErrorMessage = "File Number is required")]
        [Key]
        [Display(Name ="File Number")]
        int FileNumber { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name must contain only alphabetic characters.")]
        [Display(Name ="First Name")]
        string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Middle name is required.")]
        [MaxLength (100)]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Middle name must contain only alphabetic characters.")]
        [Display(Name ="Middle Name")]
        string MiddleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name must contain only alphabetic characters.")]
        [Display(Name = "Last Name")]
        string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [Display(Name = "Date of birth")]
        DateOnly DateOfBirth { get; set; }

       
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
        [Display(Name ="Phone Number")]
        [MaxLength(100)]
        string PhoneNumber { get; set; } = string.Empty;

        
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Email Address")]
        [MaxLength(255)]
        string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        string Speciality {  get; set; } = string.Empty;

        [Required(ErrorMessage = "Name in Arabic is required.")]
        [Display(Name = "Full Name In Arabic")]
        [MaxLength(255)]
        string FullNameInArabic {  get; set; } = string.Empty;



        [Required(ErrorMessage ="Contract Type is required.")]
        [Display(Name = "Contract Type")]
        ContractType ContractType { get; set; }



    }
}
