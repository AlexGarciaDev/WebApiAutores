using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validations
{
    public class LetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value==null || String.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primerLetra = value.ToString()[0].ToString();

            if (primerLetra!=primerLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;
        }
    }
}
