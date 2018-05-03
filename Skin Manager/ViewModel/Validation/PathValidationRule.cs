using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace Osmo.ViewModel.Validation
{
    class PathValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Directory.Exists((value ?? "").ToString()) ?
                ValidationResult.ValidResult :
                new ValidationResult(false, "Invalid directory!");
        }
    }
}
