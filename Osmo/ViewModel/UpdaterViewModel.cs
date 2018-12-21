using Osmo.Core;
using Osmo.Core.Patcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class UpdaterViewModel : ViewModelBase
    {
        private UpdateManager updateManager;
        private double mDownloadSize;
        private double mDownloadCurrent;
        private bool mIsSearching;
        private string mTopText;
        private string mBottomText;
        private string mRightButtonText;
        private bool mShowButton;
        private bool mIsIdle = true;
        private bool mShowTopTextOnly;
        private bool mIsInstalling;

        public event EventHandler<string> ToolTipChanged;

        public bool UpdatesReady
        {
            get => updateManager.UpdatesReady;
        }

        public bool IsIdle
        {
            get => mIsIdle;
            set
            {
                if (mIsIdle != value)
                {
                    mIsIdle = value;
                    InvokePropertyChanged();
                }
            }
        }

        public bool IsInstalling
        {
            get => mIsInstalling;
            set
            {
                if (mIsInstalling != value)
                {
                    mIsInstalling = value;
                    InvokePropertyChanged();
                }
            }
        }

        public string TopText
        {
            get => mTopText;
            set
            {
                mTopText = value;
                OnToolTipChanged(value);
                InvokePropertyChanged();
            }
        }

        public string BottomText
        {
            get => mBottomText;
            set
            {
                mBottomText = value;
                InvokePropertyChanged();
            }
        }

        public string RightButtonText
        {
            get => mRightButtonText;
            set
            {
                mRightButtonText = value;
                InvokePropertyChanged();
            }
        }

        //True = Buttons to install/restart/retry are visible
        //False = Animation and progress bar are visible
        public bool ShowButton
        {
            get => mShowButton;
            set
            {
                if (mShowButton != value)
                {
                    mShowButton = value;
                    InvokePropertyChanged();
                }
            }
        }

        public bool ShowTopTextOnly
        {
            get => mShowTopTextOnly;
            set
            {
                if (mShowTopTextOnly != value)
                {
                    mShowTopTextOnly = value;
                    InvokePropertyChanged();
                }
            }
        }

        public UpdateStatus Status
        {
            get => updateManager.Status;
        }

        public double DownloadSize
        {
            get => mDownloadSize;
            set
            {
                mDownloadSize = value;
                InvokePropertyChanged();
            }
        }

        public double DownloadCurrent
        {
            get => mDownloadCurrent;
            set
            {
                mDownloadCurrent = value;
                InvokePropertyChanged();
            }
        }

        public bool IsSearching {
            get => mIsSearching;
            private set
            {
                mIsSearching = value;
                InvokePropertyChanged();
            }
        }

        public UpdaterViewModel()
        {
            TopText = Helper.FindString("updater_searching");
            RightButtonText = Helper.FindString("download");
            updateManager = new UpdateManager();
            updateManager.DownloadProgressChanged += UpdateManager_DownloadProgressChanged;
            updateManager.UpdateFound += UpdateManager_UpdateFound;
            updateManager.UpdateFailed += UpdateManager_UpdateFailed;
            updateManager.SearchStatusChanged += UpdateManager_SearchStatusChanged;
            updateManager.StatusChanged += UpdateManager_StatusChanged;
        }

        public void DownloadUpdate()
        {

        }

        private void UpdateManager_StatusChanged(object sender, UpdateStatus e)
        {
            IsIdle = e == UpdateStatus.IDLE;
            ShowTopTextOnly = e == UpdateStatus.UPDATES_FOUND || e == UpdateStatus.READY;
            ShowButton = ShowTopTextOnly || e == UpdateStatus.ERROR;
            IsInstalling = e == UpdateStatus.INSTALLING;

            switch (e)
            {
                case UpdateStatus.SEARCHING:
                    TopText = Helper.FindString("updater_searching");
                    break;
                case UpdateStatus.UPDATES_FOUND:
                    TopText = Helper.FindString("Updates found!");
                    RightButtonText = Helper.FindString("download");
                    break;
                case UpdateStatus.DOWNLOADING:
                    TopText = Helper.FindString("updater_downloading");
                    BottomText = string.Format("{0} 0 MB {1} 0 MB (0 kb/s)", 
                        Helper.FindString("updater_status1"), 
                        Helper.FindString("updater_status2"));
                    break;
                case UpdateStatus.EXTRACTING:
                case UpdateStatus.INSTALLING:
                    IsInstalling = true;
                    TopText = Helper.FindString("updater_installing");
                    break;
                case UpdateStatus.READY:
                    TopText = Helper.FindString("updater_done");
                    RightButtonText = Helper.FindString("restart");
                    break;
            }
        }

        private void UpdateManager_SearchStatusChanged(object sender, bool e)
        {
            IsSearching = e;
        }

        private void UpdateManager_UpdateFailed(object sender, UpdateFailedEventArgs e)
        {
            TopText = Helper.FindString("updater_failed1");
            BottomText = Helper.FindString("updater_failed2");
            RightButtonText = Helper.FindString("retry");
            ShowButton = true;
        }

        private void UpdateManager_UpdateFound(object sender, UpdateFoundEventArgs e)
        {
            TopText = Helper.FindString("updater_done1");
            BottomText = Helper.FindString("updater_done2");
            RightButtonText = Helper.FindString("restart");
            ShowButton = true;
        }

        private void UpdateManager_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            BottomText = Helper.FindString("updater_status1") + Math.Round((e.BytesReceived / 1024d) / 1024d, 2).ToString("0.00") +
                Helper.FindString("updater_status2") +  Math.Round((e.TotalBytesToReceive / 1024d) / 1024d, 2).ToString("0.00") + 
                " (" + updateManager.CalculateSpeed(e.BytesReceived) + ")";
            DownloadSize = e.TotalBytesToReceive;
            DownloadCurrent = e.BytesReceived;
        }

        protected virtual void OnToolTipChanged(string e)
        {
            ToolTipChanged?.Invoke(this, e);
        }
    }
}
