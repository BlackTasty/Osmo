using System.Windows.Input;

namespace Osmo.Core
{
    interface IHotkeyHelper
    {
        /// <summary>
        /// This method is used to forward <see cref="KeyEventArgs"/> to child controls
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> which should be forwarded</param>
        /// <returns>Returns true if the key combination was handled.</returns>
        bool ForwardKeyboardInput(KeyEventArgs e);
    }
}
