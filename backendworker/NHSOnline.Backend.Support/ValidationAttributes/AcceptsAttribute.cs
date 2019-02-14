using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NHSOnline.Backend.Support.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AcceptsAttribute : ValidationAttribute
    {
        public object Value { get; }

        public object[] OtherValues { get; }

        public AcceptsAttribute(object value, params object[] otherValues)
        {
            Value = value;
            OtherValues = otherValues ?? new object[] { null };
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var expectedValues = new[] { Value }
                .Concat(OtherValues)
                .ToArray();

            if (expectedValues.Any(x => Equals(x, value)))
                return ValidationResult.Success;

            var expectedValuesText = string.Join(", ", expectedValues.Select(x => x ?? "null"));

            return new ValidationResult($"'{value}' is not one of the accepted values ({expectedValuesText})");
        }
    }
}