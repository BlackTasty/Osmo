using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows.Media;

namespace Osmo.Core.Objects
{
    class CompletionData : ICompletionData
    {
        public CompletionData(string text, string description)
        {
            Text = text;
            Description = description;
        }

        public ImageSource Image => null;

        public string Text { get; private set; }

        public object Content => Text;

        public object Description { get; private set; }

        public double Priority => 1;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(textArea.Caret.Offset - textArea.Caret.Column, 
                textArea.Caret.Column, Text + ": ");
            //textArea.Document.Replace(completionSegment, Text + ":");
            //textArea.Caret.Offset = textArea.Caret.Offset - (1 + this.Text.Length + 2);
        }
    }
}
