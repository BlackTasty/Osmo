using System.Windows.Controls;

namespace Installer_Online.UI
{
    /// <summary>
    /// Interaktionslogik für Agreement.xaml
    /// </summary>
    public partial class Agreement : UserControl
    {
        public Agreement()
        {
            InitializeComponent();
            richTextBox.Text = Properties.Resources.license;
            richTextBox.Focus();
        }
    }
}
