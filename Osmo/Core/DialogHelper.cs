using MaterialDesignThemes.Wpf;
using Osmo.Core.Objects;
using Osmo.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Osmo.Core
{
    class DialogHelper
    {
        private static DialogHelper instance;

        public static DialogHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DialogHelper();
                }

                return instance;
            }
        }

        public bool IsOpen { get; private set; }

        private DialogHelper()
        {

        }

        public async Task<OsmoMessageBoxResult> ShowDialog(MaterialMessageBox msgBox)
        {
            while (IsOpen)
            {
                await Task.Delay(100);
            }

            IsOpen = true;
            await DialogHost.Show(msgBox);
            IsOpen = false;

            return msgBox.Result;
        }
    }
}
