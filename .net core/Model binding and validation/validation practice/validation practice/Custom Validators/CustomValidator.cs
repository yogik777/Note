using System.ComponentModel.DataAnnotations;

namespace validation_practice.Custom_Validators
{
    public class CustomValidator: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            //return ValidationResult.Success;
            return new ValidationResult(ErrorMessage);
        }
    }
}
