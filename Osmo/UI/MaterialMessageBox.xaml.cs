using Osmo.Core;
using Osmo.Core.Objects;
using Osmo.ViewModel;
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
        string buttonOneCustomText,
            buttonTwoCustomText,
            buttonThreeCustomText;

        #region Buttons
        [Category("Message Box")]
        [Description("Sets the available buttons")]
        public OsmoMessageBoxButton Buttons
        {
            get { return (OsmoMessageBoxButton)GetValue(ButtonsProperty); }
            set
            {
                SetValue(ButtonsProperty, value);
                MessageBoxViewModel vm = DataContext as MessageBoxViewModel;
                switch (value)
                {
                    case OsmoMessageBoxButton.OK:
                        vm.ButtonOneText = "";
                        vm.ButtonTwoText = "";
                        vm.ButtonThreeText = Helper.FindString("ok");
                        break;
                    case OsmoMessageBoxButton.OKCancel:
                        vm.ButtonOneText = "";
                        vm.ButtonTwoText = Helper.FindString("cancel");
                        vm.ButtonThreeText = Helper.FindString("ok");
                        break;
                    case OsmoMessageBoxButton.YesNo:
                        vm.ButtonOneText = "";
                        vm.ButtonTwoText = Helper.FindString("no");
                        vm.ButtonThreeText = Helper.FindString("yes");
                        break;
                    case OsmoMessageBoxButton.YesNoCancel:
                        vm.ButtonOneText = Helper.FindString("cancel");
                        vm.ButtonTwoText = Helper.FindString("no");
                        vm.ButtonThreeText = Helper.FindString("yes");
                        break;
                    case OsmoMessageBoxButton.Custom:
                        vm.ButtonOneText = buttonOneCustomText;
                        vm.ButtonTwoText = buttonTwoCustomText;
                        vm.ButtonThreeText = buttonThreeCustomText;
                        break;
                }
            }
        }

        // Using a DependencyProperty as the backing store for Buttons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register("Buttons", typeof(OsmoMessageBoxButton), typeof(MaterialMessageBox), new PropertyMetadata(OsmoMessageBoxButton.YesNoCancel));
        #endregion

        #region Result
        [Category("Message Box")]
        [Description("Gets the result of the message box")]
        public OsmoMessageBoxResult Result
        {
            get { return (OsmoMessageBoxResult)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Result.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(OsmoMessageBoxResult), typeof(MaterialMessageBox), new PropertyMetadata(OsmoMessageBoxResult.None));
        #endregion

        private MaterialMessageBox(string title, string description)
        {
            InitializeComponent();
            MessageBoxViewModel vm = DataContext as MessageBoxViewModel;
            vm.Title = title;
            vm.Description = description;
        }

        private MaterialMessageBox(string title, string description, double width) : this(title, description)
        {
            MessageBoxViewModel vm = DataContext as MessageBoxViewModel;
            vm.Width = width;
        }

        //TODO: Implement a property which selects a default choice
        /// <summary>
        /// Shows a new message box with the given title, description and pre-defined button labels.
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="description">The description text of the message box.</param>
        /// <param name="buttons">Select one of the pre-defined button templates.</param>
        /// <returns></returns>
        public static MaterialMessageBox Show(string title, string description, OsmoMessageBoxButton buttons)
        {
            return new MaterialMessageBox(title, description)
            {
                Buttons = buttons
            };
        }

        //TODO: Implement a property which selects a default choice
        /// <summary>
        /// Shows a new message box with the given title, description and custom button labels.
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="description">The description text of the message box.</param>
        /// <param name="buttonRightText">The custom text for the right button.</param>
        /// <param name="buttonMiddleText">Optional: The custom text for the middle button. Provide null to hide this button</param>
        /// <param name="buttonLeftText">Optional: The custom text for the left button. Provide null to hide this button</param>
        /// <param name="width">Optional: Custom width for the window</param>
        /// <returns></returns>
        public static MaterialMessageBox Show(string title, string description, string buttonRightText, 
            string buttonMiddleText = null, string buttonLeftText = null, double width = 300)
        {
            if (buttonRightText == null)
            {
                throw new ArgumentNullException("buttonRightText", "This property cannot be null!");
            }

            return new MaterialMessageBox(title, description, width)
            {
                buttonOneCustomText = buttonLeftText,
                buttonTwoCustomText = buttonMiddleText,
                buttonThreeCustomText = buttonRightText,
                Buttons = OsmoMessageBoxButton.Custom
            };
        }

        private void ButtonOne_Click(object sender, RoutedEventArgs e)
        {
            if (Buttons == OsmoMessageBoxButton.YesNoCancel)
            {
                Result = OsmoMessageBoxResult.Cancel;
            }
            else
            {
                Result = OsmoMessageBoxResult.CustomActionLeft;
            }
        }

        private void ButtonTwo_Click(object sender, RoutedEventArgs e)
        {
            switch (Buttons)
            {
                case OsmoMessageBoxButton.OKCancel:
                    Result = OsmoMessageBoxResult.Cancel;
                    break;
                case OsmoMessageBoxButton.YesNo:
                case OsmoMessageBoxButton.YesNoCancel:
                    Result = OsmoMessageBoxResult.No;
                    break;
                case OsmoMessageBoxButton.Custom:
                    Result = OsmoMessageBoxResult.CustomActionMiddle;
                    break;
            }
        }

        private void ButtonThree_Click(object sender, RoutedEventArgs e)
        {
            switch (Buttons)
            {
                case OsmoMessageBoxButton.OK:
                case OsmoMessageBoxButton.OKCancel:
                    Result = OsmoMessageBoxResult.OK;
                    break;
                case OsmoMessageBoxButton.YesNo:
                case OsmoMessageBoxButton.YesNoCancel:
                    Result = OsmoMessageBoxResult.Yes;
                    break;
                case OsmoMessageBoxButton.Custom:
                    Result = OsmoMessageBoxResult.CustomActionRight;
                    break;
            }
        }
    }
}
