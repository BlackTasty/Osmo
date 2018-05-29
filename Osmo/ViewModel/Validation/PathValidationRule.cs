using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace Osmo.ViewModel.Validation
{
    class PathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Runtime)
            {
                return Directory.Exists((value ?? "").ToString()) ?
                    ValidationResult.ValidResult :
                    new ValidationResult(false, "Invalid directory!");
            }
            else return ValidationResult.ValidResult;
        }
    }
}
