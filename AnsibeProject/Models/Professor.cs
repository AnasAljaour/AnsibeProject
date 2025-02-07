﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnsibeProject.Models
{
    public class Professor
    {
        [Required(ErrorMessage = "File Number is required")]
        [Key]
        [Display(Name ="File Number")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FileNumber { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Z a-z]+$", ErrorMessage = "First name must contain only alphabetic characters.")]
        [Display(Name ="First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Middle name is required.")]
        [MaxLength (100)]
        [RegularExpression(@"^[A-Z a-z]+$", ErrorMessage = "Middle name must contain only alphabetic characters.")]
        [Display(Name ="Middle Name")]
        public string MiddleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Z a-z]+$", ErrorMessage = "Last name must contain only alphabetic characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [Display(Name = "Date of birth")]
        public DateOnly DateOfBirth { get; set; }

       
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[\d\+\-\(\)]+$", ErrorMessage = "Invalid phone Number.")]
        [Display(Name ="Phone Number")]
        [MaxLength(100)]
        public string PhoneNumber { get; set; } = string.Empty;

        
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Email Address")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Speciality {  get; set; } = string.Empty;

        [Required(ErrorMessage = "الأسم باللغة العربية مطلوب")]
        [Display(Name = "الأسم باللغة العربية")]
        [MaxLength(255)]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$", ErrorMessage = "يرجى إدخال أحرف عربية فقط.")]
        public string FullNameInArabic {  get; set; } = string.Empty;

        [Required(ErrorMessage ="Rank is required")]
        public Rank Rank { get; set; }

        [Required(ErrorMessage = "Active State is required")]
        [Display(Name ="Active state")]
        public ActiveState ActiveState { get; set; }

        [Required]
        public Contract Contract { get; set; }




    }
}
