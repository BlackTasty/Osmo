using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for TemplateEditor.xaml
    /// </summary>
    public partial class TemplateEditor : Grid
    {
        private static TemplateEditor instance;

        private CompletionWindow completionWindow;

        public static TemplateEditor Instance
        {
            get
            {
                if (instance == null)
                    instance = new TemplateEditor();

                return instance;
            }
        }

        private TemplateEditor()
        {
            InitializeComponent();
        }

        public void LoadTemplate(ForumTemplate template)
        {
            (DataContext as TemplateEditorViewModel).Template = template;
            textEditor.Document = null; // immediately remove old document
            TextDocument doc = new TextDocument(new StringTextSource(template.Content));
            doc.SetOwnerThread(Application.Current.Dispatcher.Thread);
            Application.Current.Dispatcher.BeginInvoke(
                  new Action(
                      delegate
                      {
                          textEditor.Document = doc;
                      }), DispatcherPriority.Normal);

        }

        public void SaveTemplate()
        {
            (DataContext as TemplateEditorViewModel).Template.Save(textEditor.Text);
        }

        private void TextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            completionWindow = new CompletionWindow(textEditor.TextArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            string line = Helper.GetTextAtCurrentLine(textEditor);
            foreach (CompletionData item in FixedValues.templateCompletionData)
            {
                if (item.Text.Contains(line))
                    data.Add(item);
            }

            if (data.Count > 0)
            {
                completionWindow.Show();
                completionWindow.Closed += delegate { completionWindow = null; };
            }
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow != null && !char.IsLetterOrDigit(e.Text[0]))
            {
                // Whenever a non-letter is typed while the completion window is open,
                // insert the currently selected element.
                completionWindow.CompletionList.RequestInsertion(e);
            }
        }

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
        }

        public void MakePreview(Skin skin)
        {
            TemplateEditorViewModel vm = DataContext as TemplateEditorViewModel;
            vm.TargetSkin = skin;
            if (vm.TargetSkin != null)
            {
                vm.PreviewText = textEditor.Text.Replace("[NAME]", vm.TargetSkin.Name)
                    .Replace("[AUTHOR]", vm.TargetSkin.Author)
                    .Replace("[DATE]", Helper.GetDate())
                    .Replace("[SIZE]", Helper.GetDirectorySize(vm.TargetSkin.Path).ToString())
                    .Replace("[VERSION]", vm.TargetSkin.Version);
            }
        }
    }
}
