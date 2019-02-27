namespace Osmo.ViewModel
{
    class ModeProgressViewModel : ViewModelBase
    {
        private double mProgress;
        private double mProgressHD;

        public double Progress
        {
            get => mProgress;
            set
            {
                mProgress = value;
                InvokePropertyChanged("Progress");
                InvokePropertyChanged("ProgressText");
            }
        }

        public string ProgressText
        {
            get => string.Format("{0}%\nHD: {1}%", mProgress, mProgressHD);
        }

        public double ProgressHD
        {
            get => mProgressHD;
            set
            {
                mProgressHD = value;
                InvokePropertyChanged("ProgressHD");
                InvokePropertyChanged("ProgressText");
            }
        }
    }
}
