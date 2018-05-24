namespace Osmo.ViewModel
{
    class NewSkinViewModel : ViewModelBase
    {
        private string mName;
        private string mAuthor;
        private bool mAddDummyFiles = true; //

        public string Name
        {
            get => mName;
            set
            {
                mName = value;
                InvokePropertyChanged("Name");
            }
        }

        public string Author
        {
            get => mAuthor;
            set
            {
                mAuthor = value;
                InvokePropertyChanged("Author");
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
