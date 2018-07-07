using System.Threading.Tasks;
using System.Windows.Input;

namespace Osmo.Core
{
    interface IAsyncShortcutHelper
    {
        /// <summary>
        /// This method is used to forward <see cref="KeyEventArgs"/> to child controls
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> which should be forwarded</param>
        /// <returns>Returns true if the key combination was handled.</returns>
        Task<bool> ForwardKeyboardInput(KeyEventArgs e);
    }
}
