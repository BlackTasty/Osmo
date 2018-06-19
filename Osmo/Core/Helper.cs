using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using MaterialDesignThemes.Wpf;
using Osmo.UI;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Osmo.Core
{
    public static class Helper
    {

        public static BitmapImage LoadImage(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                BitmapImage bmp = new BitmapImage();

                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = fs;
                    bmp.EndInit();
                }

                return bmp;
            }
            else return null;
        }

        public static string GetTextAtCurrentLine(TextEditor textEditor)
        {
            int offset = textEditor.CaretOffset;
            DocumentLine line = textEditor.Document.GetLineByOffset(offset);
            return textEditor.Document.GetText(line.Offset, line.Length);
        }

        public static async void ExportSkin(int selectedIndex, bool skipDialog)
        {
            MessageBoxResult result;

            if (!skipDialog)
            {
                var msgBox = MaterialMessageBox.Show("Save changes first?",
                    "Do you wish to save your skin first?",
                    MessageBoxButton.YesNoCancel);

                await DialogHost.Show(msgBox);
                result = msgBox.Result;
            }
            else
            {
                result = MessageBoxResult.No;
            }

            if (result != MessageBoxResult.Cancel)
            {
                using (var dlg = new System.Windows.Forms.FolderBrowserDialog()
                {
                    Description = "Select the directory you want to export your skin to"
                })
                {
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (selectedIndex == 2)
                        {
                            SkinEditor.Instance.ExportSkin(dlg.SelectedPath, result == MessageBoxResult.Yes);
                        }
                        else if (selectedIndex == 3)
                        {
                            SkinMixer.Instance.ExportSkin(dlg.SelectedPath, result == MessageBoxResult.Yes);
                        }
                    }
                }
            }
        }

        public static void ExportSkin(int selectedIndex)
        {
            ExportSkin(selectedIndex, false);
        }
    }
}
