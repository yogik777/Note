using ModelValidationsExample.CustomValidators;
using System.ComponentModel.DataAnnotations;

namespace ModelValidationsExample.Models
{
    public class Person : IValidatableObject
    {
        [Required(ErrorMessage ="{0}can't be empty or null")]//the property name automaticaly get passed as an argument 
        [Display(Name ="Person Name")]//we can change the property name from here if we want
        public string? PersonName { get; set; }

        [Required]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public double? Price { get; set; }


        [MinimumYearvalidation(2005, ErrorMessage ="Date of Birth should not be newer that Jan 01, {0}")]
        public DateTime? DateOfBirth { get; set; }

        public DateTime? FromDate { get; set; }

        [DateRangeValidator("FromDate", ErrorMessage= "'From Date {0}' should be older that or equal to 'To date {1}'")]
        public DateTime? ToDate { get; set; }

        public int? Age { get; set; }

        public List<string?> Tags { get; set; } = new List<string?>();
        public override string ToString()
        {
            return $"Person object - " +
                $"Person name: {PersonName}," +
                $"Email: {Email}," +
                $"Phone: {Phone}," +
                $"Password: {Password}," +
                $"ConfirmPassword: {ConfirmPassword}," +
                $"Price: {Price}";
        }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if(DateOfBirth.HasValue == false && Age.HasValue == false)
            {
                yield return  new ValidationResult("Either of Date of Birth or Age must be supplied", new[] { nameof(Age) });
            }
		}
	}
}
