using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using MaterialDesignThemes.Wpf;
using Osmo.Core.Objects;
using Osmo.UI;
using System;
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

        public static string GetDate(char splitter = '.')
        {
            return DateTime.Now.ToString(string.Format("dd{0}MM{0}yyyy", splitter));
        }

        public static string GetTime(char splitter = ':')
        {
            return DateTime.Now.ToString(string.Format("HH{0}mm{0}ss", splitter));
        }
        public static string GetDateAndTime()
        {
            return GetDate() + ", " + GetTime();
        }

        public static string GetDateAndTime(char mainSplitter, char splitter)
        {
            return GetDate(splitter) + mainSplitter + GetTime(splitter);
        }

        public static double GetDirectorySize(string directory)
        {
            double size = 0;
            foreach (FileInfo fi in new DirectoryInfo(directory).GetFiles())
            {
                size += fi.Length;
            }

            return Math.Round((size / 1024) /1024, 1);
        }

        public static string ApplyForumTemplate(ForumTemplate template, Skin skin)
        {
            return ApplyForumTemplate(template.Content, skin);
        }

        public static string ApplyForumTemplate(string templateText, Skin skin)
        {
            if (skin != null && !skin.IsEmpty)
            {
                return templateText.Replace("[NAME]", skin.Name)
                           .Replace("[AUTHOR]", skin.Author)
                           .Replace("[DATE]", GetDate())
                           .Replace("[SIZE]", GetDirectorySize(skin.Path).ToString())
                           .Replace("[VERSION]", skin.Version);
            }
            else
            {
                return "";
            }
        }

        public static async void ExportSkin(string exportPath, int selectedIndex, bool skipDialog)
        {
            OsmoMessageBoxResult result;

            if (!skipDialog)
            {
                var msgBox = MaterialMessageBox.Show(FindString("export_saveFirstTitle"),
                    FindString("export_saveFirstDescription"),
                    OsmoMessageBoxButton.YesNoCancel);

                await DialogHost.Show(msgBox);
                result = msgBox.Result;
            }
            else
            {
                result = OsmoMessageBoxResult.No;
            }

            if (result != OsmoMessageBoxResult.Cancel)
            {
                if (selectedIndex == FixedValues.EDITOR_INDEX)
                {
                    SkinEditor.Instance.ExportSkin(exportPath, result == OsmoMessageBoxResult.Yes);
                }
                else if (selectedIndex == FixedValues.MIXER_INDEX)
                {
                    SkinMixer.Instance.ExportSkin(exportPath, result == OsmoMessageBoxResult.Yes);
                }
            }
        }

        public static void ExportSkin(string exportPath, int selectedIndex)
        {
            bool skipDialog = false;
            if (selectedIndex == FixedValues.EDITOR_INDEX)
            {
                skipDialog = !SkinEditor.Instance.LoadedSkin.UnsavedChanges;
            }
            else if (selectedIndex == FixedValues.MIXER_INDEX)
            {
                skipDialog = !SkinMixer.Instance.LoadedSkin.UnsavedChanges;
            }

            ExportSkin(exportPath, selectedIndex, skipDialog);
        }

        public static string FindString(string targetName)
        {
            try
            {
                return (string)Application.Current.FindResource(targetName);
            }
            catch
            {
                return "NO STRING FOUND!";
            }
        }
    }
}
