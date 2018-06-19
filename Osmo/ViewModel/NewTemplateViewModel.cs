namespace Osmo.ViewModel
{
    class NewTemplateViewModel : ViewModelBase
    {
        private string mName;

        public string Name
        {
            get => mName;
            set
            {
                mName = value;
                InvokePropertyChanged("Name");
            }
        }
    }
}
