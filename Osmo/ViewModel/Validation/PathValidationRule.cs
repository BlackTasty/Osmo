using Osmo.Core;
using Osmo.Core.FileExplorer;
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
                if (value is IFilePickerEntry entry)
                {
                    return Directory.Exists(entry.Path) ?
                        ValidationResult.ValidResult :
                        new ValidationResult(false, Helper.FindString("validationError_file"));
                }
                else
                {
                    return Directory.Exists((value ?? "").ToString()) ?
                        ValidationResult.ValidResult :
                        new ValidationResult(false, Helper.FindString("validationError_directory"));
                }
            }
            else return ValidationResult.ValidResult;
        }
    }
}
