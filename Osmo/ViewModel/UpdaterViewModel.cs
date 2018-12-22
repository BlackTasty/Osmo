using MaterialDesignThemes.Wpf;
using Osmo.Core;
using Osmo.Core.Patcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private string mLeftButtonText;
        private string mRightButtonText;
        private bool mShowButton = true;
        private bool mShowLeftButton = false;
        private bool mShowTopTextOnly;
        private bool mIsInstalling;
        private bool mShowRightButton = true;
        private PackIconKind mIcon = PackIconKind.CloudDownload;

        public event EventHandler<FrontendChangedEventArgs> FrontendChanged;
        public event EventHandler<bool> RunAnimation;

        public UpdateManager UpdateManager { get => updateManager; }

        public bool ShowLeftButton
        {
            get => mShowLeftButton;
            set
            {
                if (mShowLeftButton != value)
                {
                    mShowLeftButton = value;
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

        public PackIconKind Icon
        {
            get => mIcon;
            set
            {
                mIcon = value;
                OnFrontendChanged(new FrontendChangedEventArgs(TopText, value));
                InvokePropertyChanged();
            }
        }

        public string TopText
        {
            get => mTopText;
            set
            {
                if (mTopText != value)
                {
                    mTopText = value;
                    OnFrontendChanged(new FrontendChangedEventArgs(value, Icon));
                    InvokePropertyChanged();
                }
            }
        }

        public string BottomText
        {
            get => mBottomText;
            set
            {
                if (mBottomText != value)
                {
                    mBottomText = value;
                    InvokePropertyChanged();
                }
            }
        }

        public string LeftButtonText
        {
            get => mLeftButtonText;
            set
            {
                if (mLeftButtonText != value)
                {
                    mLeftButtonText = value;
                    InvokePropertyChanged();
                }
            }
        }

        public string RightButtonText
        {
            get => mRightButtonText;
            set
            {
                if (mRightButtonText != value)
                {
                    mRightButtonText = value;
                    InvokePropertyChanged();
                }
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

        public bool ShowRightButton
        {
            get => mShowRightButton;
            set
            {
                if (mShowRightButton != value)
                {
                    mShowRightButton = value;
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
                if (mDownloadSize != value)
                {
                    mDownloadSize = value;
                    InvokePropertyChanged();
                }
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
            TopText = Helper.FindString("updater_idle");
            RightButtonText = Helper.FindString("search");
            updateManager = new UpdateManager();
            updateManager.DownloadProgressChanged += UpdateManager_DownloadProgressChanged;
            updateManager.UpdateFound += UpdateManager_UpdateFound;
            updateManager.UpdateFailed += UpdateManager_UpdateFailed;
            updateManager.SearchStatusChanged += UpdateManager_SearchStatusChanged;
            updateManager.StatusChanged += UpdateManager_StatusChanged;

            //if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Runtime)
            //{
            //    updateManager.CheckForUpdates();
            //}
        }

        private void UpdateManager_StatusChanged(object sender, UpdateStatus e)
        {
            bool leftAndRightButton = e == UpdateStatus.UPDATES_FOUND || e == UpdateStatus.READY;

            ShowLeftButton = e != UpdateStatus.IDLE || leftAndRightButton;
            ShowTopTextOnly = e == UpdateStatus.IDLE;
            ShowRightButton = e == UpdateStatus.ERROR || ShowTopTextOnly || leftAndRightButton;

            ShowButton = ShowTopTextOnly || ShowRightButton;
            IsInstalling = e == UpdateStatus.INSTALLING;

            switch (e)
            {
                case UpdateStatus.IDLE:
                    TopText = Helper.FindString("updater_idle");
                    RightButtonText = Helper.FindString("search");
                    break;
                case UpdateStatus.SEARCHING:
                    TopText = Helper.FindString("updater_searching");
                    OnRunAnimation(true);
                    break;
                case UpdateStatus.UPDATES_FOUND:
                    TopText = Helper.FindString("updater_ready1");
                    BottomText = Helper.FindString("updater_ready2");
                    LeftButtonText = Helper.FindString("later");
                    RightButtonText = Helper.FindString("download");
                    break;
                case UpdateStatus.DOWNLOADING:
                    ShowLeftButton = false;
                    TopText = Helper.FindString("updater_downloading");
                    BottomText = string.Format("{0} 0 MB {1} 0 MB (0 kb/s)", 
                        Helper.FindString("updater_status1"), 
                        Helper.FindString("updater_status2"));
                    break;
                case UpdateStatus.EXTRACTING:
                case UpdateStatus.INSTALLING:
                    TopText = Helper.FindString("updater_installing");
                    break;
                case UpdateStatus.READY:
                    OnRunAnimation(false);
                    TopText = Helper.FindString("updater_done1");
                    BottomText = Helper.FindString("updater_done2");
                    LeftButtonText = Helper.FindString("later");
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
            LeftButtonText = Helper.FindString("retry");
            ShowRightButton = false;
            ShowButton = true;
        }

        private void UpdateManager_UpdateFound(object sender, UpdateFoundEventArgs e)
        {
            TopText = Helper.FindString("updater_ready1");
            BottomText = Helper.FindString("updater_ready2");
            LeftButtonText = Helper.FindString("later");
            RightButtonText = Helper.FindString("download");
            ShowButton = true;
        }

        private void UpdateManager_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            BottomText = Helper.FindString("updater_status1") + " " + 
                Math.Round((e.BytesReceived / 1024d) / 1024d, 2).ToString("0.00") + " " + 
                Helper.FindString("updater_status2") + " " + 
                Math.Round((e.TotalBytesToReceive / 1024d) / 1024d, 2).ToString("0.00") + 
                " (" + updateManager.CalculateSpeed(e.BytesReceived) + ")";
            DownloadSize = e.TotalBytesToReceive;
            DownloadCurrent = e.BytesReceived;
        }

        protected virtual void OnFrontendChanged(FrontendChangedEventArgs e)
        {
            FrontendChanged?.Invoke(this, e);
        }

        protected virtual void OnRunAnimation(bool runAnimation)
        {
            RunAnimation?.Invoke(this, runAnimation);
        }
    }
}
