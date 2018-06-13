using Osmo.Core.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Osmo.UI
{
    /// <summary>
    /// Interaction logic for MaterialMessageBox.xaml
    /// </summary>
    public partial class MaterialMessageBox : DockPanel
    {
        #region Title
        [Category("Message Box")]
        [Description("A string that specifies the title to display")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MaterialMessageBox), new PropertyMetadata(""));
        #endregion

        #region Description
        [Category("Message Box")]
        [Description("A string that specifies the text to display")]
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MaterialMessageBox), new PropertyMetadata(""));
        #endregion

        #region ButtonOneTtext
        public string ButtonOneText
        {
            get { return (string)GetValue(ButtonOneTextProperty); }
            set { SetValue(ButtonOneTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonOneText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonOneTextProperty =
            DependencyProperty.Register("ButtonOneText", typeof(string), typeof(MaterialMessageBox), new PropertyMetadata("Yes"));
        #endregion

        #region ButtonTwoText
        public string ButtonTwoText
        {
            get { return (string)GetValue(ButtonTwoTextProperty); }
            set { SetValue(ButtonTwoTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonTwoText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTwoTextProperty =
            DependencyProperty.Register("ButtonTwoText", typeof(string), typeof(MaterialMessageBox), new PropertyMetadata("No"));
        #endregion

        #region ButtonThreeText
        public string ButtonThreeText
        {
            get { return (string)GetValue(ButtonThreeTextProperty); }
            set { SetValue(ButtonThreeTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonThreeText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonThreeTextProperty =
            DependencyProperty.Register("ButtonThreeText", typeof(string), typeof(MaterialMessageBox), new PropertyMetadata("Cancel"));
        #endregion

        #region Buttons
        [Category("Message Box")]
        [Description("Sets the available buttons")]
        public MessageBoxButton Buttons
        {
            get { return (MessageBoxButton)GetValue(ButtonsProperty); }
            set
            {
                SetValue(ButtonsProperty, value);
                switch (value)
                {
                    case MessageBoxButton.OK:
                        ButtonOneText = "";
                        ButtonTwoText = "";
                        ButtonThreeText = "OK";
                        break;
                    case MessageBoxButton.OKCancel:
                        ButtonOneText = "";
                        ButtonTwoText = "Cancel";
                        ButtonThreeText = "OK";
                        break;
                    case MessageBoxButton.YesNo:
                        ButtonOneText = "";
                        ButtonTwoText = "No";
                        ButtonThreeText = "Yes";
                        break;
                    case MessageBoxButton.YesNoCancel:
                        ButtonOneText = "Cancel";
                        ButtonTwoText = "No";
                        ButtonThreeText = "Yes";
                        break;
                }
            }
        }

        // Using a DependencyProperty as the backing store for Buttons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register("Buttons", typeof(MessageBoxButton), typeof(MaterialMessageBox), new PropertyMetadata(MessageBoxButton.YesNoCancel));
        #endregion

        #region Result
        [Category("Message Box")]
        [Description("Gets the result of the message box")]
        public MessageBoxResult Result
        {
            get { return (MessageBoxResult)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(MessageBoxResult), typeof(MaterialMessageBox), new PropertyMetadata(MessageBoxResult.None));
        #endregion

        private MaterialMessageBox()
        {
            InitializeComponent();
        }

        //TODO: Implement a property which selects a default choice
        public static MaterialMessageBox Show(string title, string description, MessageBoxButton buttons)
        {
            return new MaterialMessageBox()
            {
                Title = title,
                Description = description,
                Buttons = buttons
            }; ;
        }

        private void ButtonOne_Click(object sender, RoutedEventArgs e)
        {
            if (Buttons == MessageBoxButton.YesNoCancel)
            {
                Result = MessageBoxResult.Cancel;
            }
        }

        private void ButtonTwo_Click(object sender, RoutedEventArgs e)
        {
            switch (Buttons)
            {
                case MessageBoxButton.OKCancel:
                    Result = MessageBoxResult.Cancel;
                    break;
                case MessageBoxButton.YesNo:
                case MessageBoxButton.YesNoCancel:
                    Result = MessageBoxResult.No;
                    break;
            }
        }

        private void ButtonThree_Click(object sender, RoutedEventArgs e)
        {
            switch (Buttons)
            {
                case MessageBoxButton.OK:
                case MessageBoxButton.OKCancel:
                    Result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.YesNo:
                case MessageBoxButton.YesNoCancel:
                    Result = MessageBoxResult.Yes;
                    break;
            }
        }
    }
}
