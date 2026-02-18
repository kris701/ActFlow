using System.ComponentModel.DataAnnotations;

namespace ActFlow.Models.Attributes
{
	public class StictLowerCaseStringAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value?.ToString().Any(char.IsUpper) == true)
				return new ValidationResult("Only lower case is allowed in names!");
			if (value?.ToString().Contains(" ") == true)
				return new ValidationResult("No space characters allowed in names!");
			return ValidationResult.Success;
		}
	}
}
