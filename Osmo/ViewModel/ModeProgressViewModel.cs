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
            get => string.Format("{0}%", mProgress);
        }

        public double ProgressHD
        {
            get => mProgressHD;
            set
            {
                mProgressHD = value;
                InvokePropertyChanged("ProgressHD");
                InvokePropertyChanged("ProgressHDText");
            }
        }

        public string ProgressHDText
        {
            get => string.Format("HD: {0}%", mProgressHD);
        }
    }
}
