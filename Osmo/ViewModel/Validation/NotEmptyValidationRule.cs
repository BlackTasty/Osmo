using System.Globalization;
using System.Windows.Controls;

namespace Osmo.ViewModel.Validation
{
    class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Runtime)
            {
                return !string.IsNullOrWhiteSpace(value?.ToString() ?? "") ? ValidationResult.ValidResult :
                new ValidationResult(false, "This field cannot be empty!");
            }
            else return ValidationResult.ValidResult;
        }
    }
}
