using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SafeStringAttribute : ValidationAttribute
    {
        public SafeStringAttribute()
            : base("The value was not deemed safe")
        {
        }

        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (!(value is string valueToValidate))
            {
                return new ValidationResult(ErrorMessageString);
            }

            if (valueToValidate.Contains("<script>", StringComparison.OrdinalIgnoreCase))
            {
                var logger = (ILogger<SafeStringAttribute>)validationContext.GetService(typeof(ILogger<SafeStringAttribute>));
                logger.LogWarning($"Failing validation because the value was not deemed safe for { validationContext.MemberName }");
                return new ValidationResult(ErrorMessageString);
            }

            return ValidationResult.Success;
        }
    }
}
