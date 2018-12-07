using System;
using System.Windows.Input;

namespace Installer.ViewModel
{
    class AbortViewModel : ViewModelBase
    {
        private AbortingViewModel model;

        public event EventHandler<EventArgs> AbortInstallationClicked;

        public ICommand OpenDialogCommand { get => model?.OpenDialogCommand; }
        public ICommand CancelDialogCommand { get => model?.CancelDialogCommand; }
        public ICommand AbortInstallationCommand { get => model?.AbortInstallationCommand; }

        public bool IsDialogOpen {
            get => model?.IsDialogOpen ?? false;
            set
            {
                if (model != null)
                {
                    model.IsDialogOpen = value;
                    InvokePropertyChanged();
                }
            }
        }

        public object DialogContent
        {
            get => model?.DialogContent;
            set
            {
                if (model != null)
                {
                    model.DialogContent = value;
                    InvokePropertyChanged();
                }
            }
        }

        public AbortViewModel()
        {
            model = AbortingViewModel.Instance;
            model.AbortInstallationClicked += Model_AbortInstallationClicked;
        }

        private void Model_AbortInstallationClicked(object sender, EventArgs e)
        {
            AbortInstallationClicked?.Invoke(sender, e);
        }
    }
}
