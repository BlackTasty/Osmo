namespace Osmo.ViewModel
{
    class NewSkinViewModel : ViewModelBase
    {
        private string mName;
        private string mAuthor;
        private bool mAddDummyFiles = true; //
        
        internal OsmoViewModel Master { get; set; }

        public string Name
        {
            get => mName;
            set
            {
                mName = value;
                InvokePropertyChanged("Name");
                InvokePropertyChanged("IsConfirmEnabled");
            }
        }

        public string Author
        {
            get => mAuthor;
            set
            {
                mAuthor = value;
                InvokePropertyChanged("Author");
                InvokePropertyChanged("IsConfirmEnabled");
            }
        }

        public bool AddDummyFiles
        {
            get => mAddDummyFiles;
            set
            {
                mAddDummyFiles = value;
                InvokePropertyChanged("AddDummyFiles");
            }
        }
    }
}
