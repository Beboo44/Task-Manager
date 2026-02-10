using System.ComponentModel.DataAnnotations;

namespace TaskManager.Validation
{
    /// <summary>
    /// Validates that a DateTime value is not in the past
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        private readonly bool _allowToday;

        public FutureDateAttribute(bool allowToday = true)
        {
            _allowToday = allowToday;
            ErrorMessage = allowToday 
                ? "Deadline cannot be in the past" 
                : "Deadline must be in the future";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is DateTime deadline)
            {
                var now = DateTime.Now;
                var compareDate = _allowToday ? now.Date : now;

                if (deadline < compareDate)
                {
                    return new ValidationResult(ErrorMessage);
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid date format");
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is DateTime deadline)
            {
                var now = DateTime.Now;
                var compareDate = _allowToday ? now.Date : now;
                return deadline >= compareDate;
            }

            return false;
        }
    }
}
