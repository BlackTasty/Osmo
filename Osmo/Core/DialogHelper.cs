using MaterialDesignThemes.Wpf;
using Osmo.Core.Objects;
using Osmo.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Osmo.Core
{
    class DialogHelper
    {
        private static DialogHelper instance;

        private DialogSession session;
        private Button dialogSender;

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

        public void NotifyDialogOpened(Button element)
        {
            dialogSender = element;
            IsOpen = true;
        }

        public void NotifyDialogOpened(DialogSession session)
        {
            this.session = session;
            IsOpen = true;
        }

        public void NotifyDialogClosed()
        {
            session = null;
            dialogSender = null;
            IsOpen = false;
        }

        public async Task<OsmoMessageBoxResult> ShowDialog(MaterialMessageBox msgBox)
        {
            while (IsOpen)
            {
                await Task.Delay(100);
            }

            IsOpen = true;
            await DialogHost.Show(msgBox, delegate(object sender, DialogOpenedEventArgs args)
            {
                session = args.Session;
            });
            IsOpen = false;
            session = null;

            return msgBox.Result;
        }

        public async Task<OsmoMessageBoxResult> ShowDialog(MaterialMessageBox msgBox, bool forceClose)
        {
            if (forceClose && IsOpen)
            {
                if (dialogSender != null)
                {
                    Helper.ExecuteDialogCloseCommand(dialogSender);
                    NotifyDialogClosed();
                }
                else if (session != null)
                {
                    session.Close(false);
                    NotifyDialogClosed();
                }
            }

            return await ShowDialog(msgBox);
        }
    }
}
