using Installer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Installer.ViewModel
{
    class AbortingViewModel : ViewModelBase
    {
        private static AbortingViewModel instance;

        public static AbortingViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AbortingViewModel();
                }

                return instance;
            }
        }

        private bool mIsDialogOpen;
        private object mDialogContent;

        public event EventHandler<EventArgs> AbortInstallationClicked;

        public ICommand OpenDialogCommand { get; }
        public ICommand CancelDialogCommand { get; }
        public ICommand AbortInstallationCommand { get; }

        public bool IsDialogOpen
        {
            get => mIsDialogOpen;
            set
            {
                if (mIsDialogOpen == value) return;

                mIsDialogOpen = value;
                InvokePropertyChanged();
            }
        }

        public object DialogContent
        {
            get => mDialogContent;
            set
            {
                if (mDialogContent == value) return;

                mDialogContent = value;
                InvokePropertyChanged();
            }
        }

        private AbortingViewModel()
        {
            OpenDialogCommand = new RelayCommand(ShowDialog);
            CancelDialogCommand = new RelayCommand(CancelDialog);
            AbortInstallationCommand = new RelayCommand(AbortInstallation);
        }

        private void ShowDialog(object obj)
        {
            DialogContent = new AbortingDialog();
            IsDialogOpen = true;
        }

        private void CancelDialog(object obj)
        {
            IsDialogOpen = false;
        }

        private void AbortInstallation(object obj)
        {
            OnAbortInstallationClicked(EventArgs.Empty);
        }

        protected virtual void OnAbortInstallationClicked(EventArgs e)
        {
            AbortInstallationClicked?.Invoke(this, e);
        }
    }
}
