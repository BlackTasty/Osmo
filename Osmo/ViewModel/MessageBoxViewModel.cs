using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class MessageBoxViewModel : ViewModelBase
    {
        private string mTitle;
        private string mDescription;
        private string mButtonOneText = "_CANCEL";
        private string mButtonTwoText = "_NO";
        private string mButtonThreeText = "_YES";
        private double mWidth = 300;

        public string Title
        {
            get => mTitle;
            set
            {
                mTitle = value;
                InvokePropertyChanged("Title");
            }
        }

        public string Description
        {
            get => mDescription;
            set
            {
                mDescription = value;
                InvokePropertyChanged("Description");
            }
        }

        public string ButtonOneText
        {
            get => mButtonOneText;
            set
            {
                mButtonOneText = value;
                InvokePropertyChanged("ButtonOneText");
                InvokePropertyChanged("ButtonOneVisible");
            }
        }

        public bool ButtonOneVisible
        {
            get => !string.IsNullOrWhiteSpace(mButtonOneText);
        }

        public string ButtonTwoText
        {
            get => mButtonTwoText;
            set
            {
                mButtonTwoText = value;
                InvokePropertyChanged("ButtonTwoText");
                InvokePropertyChanged("ButtonTwoVisible");
            }
        }

        public bool ButtonTwoVisible
        {
            get => !string.IsNullOrWhiteSpace(mButtonTwoText);
        }

        public string ButtonThreeText
        {
            get => mButtonThreeText;
            set
            {
                mButtonThreeText = value;
                InvokePropertyChanged("ButtonThreeText");
            }
        }

        public double Width
        {
            get => mWidth;
            set
            {
                mWidth = value;
                InvokePropertyChanged("Width");
            }
        }
    }
}
