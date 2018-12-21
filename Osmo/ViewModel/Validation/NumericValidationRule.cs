using Osmo.Core;
using System.Globalization;
using System.Windows.Controls;

namespace Osmo.ViewModel.Validation
{
    class NumericValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Runtime)
            {
                if (Parser.TryParse(value.ToString(), -1) > 0)
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "Only numbers are allowed!");
                }
            }
            else return ValidationResult.ValidResult;
        }
    }
}
