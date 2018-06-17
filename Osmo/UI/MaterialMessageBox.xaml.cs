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
        #region Buttons
        [Category("Message Box")]
        [Description("Sets the available buttons")]
        public MessageBoxButton Buttons
        {
            get { return (MessageBoxButton)GetValue(ButtonsProperty); }
            set
            {
                SetValue(ButtonsProperty, value);
                MessageBoxViewModel vm = DataContext as MessageBoxViewModel;
                switch (value)
                {
                    case MessageBoxButton.OK:
                        vm.ButtonOneText = "";
                        vm.ButtonTwoText = "";
                        vm.ButtonThreeText = "OK";
                        break;
                    case MessageBoxButton.OKCancel:
                        vm.ButtonOneText = "";
                        vm.ButtonTwoText = "Cancel";
                        vm.ButtonThreeText = "OK";
                        break;
                    case MessageBoxButton.YesNo:
                        vm.ButtonOneText = "";
                        vm.ButtonTwoText = "No";
                        vm.ButtonThreeText = "Yes";
                        break;
                    case MessageBoxButton.YesNoCancel:
                        vm.ButtonOneText = "Cancel";
                        vm.ButtonTwoText = "No";
                        vm.ButtonThreeText = "Yes";
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

        private MaterialMessageBox(string title, string description)
        {
            InitializeComponent();
            MessageBoxViewModel vm = DataContext as MessageBoxViewModel;
            vm.Title = title;
            vm.Description = description;
        }

        //TODO: Implement a property which selects a default choice
        public static MaterialMessageBox Show(string title, string description, MessageBoxButton buttons)
        {
            return new MaterialMessageBox(title, description)
            {
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
