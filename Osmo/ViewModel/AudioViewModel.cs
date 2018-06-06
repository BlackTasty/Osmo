using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osmo.ViewModel
{
    class AudioViewModel : ViewModelBase
    {
        private double mAudioPosition = 0;
        private double mAudioLength = 0;

        public double AudioLength
        {
            get => mAudioLength;
            set
            {
                mAudioLength = value;
                InvokePropertyChanged("AudioLength");
            }
        }

        public double AudioPosition
        {
            get => mAudioPosition;
            set
            {
                mAudioPosition = value;
                InvokePropertyChanged("AudioPosition");
            }
        }
    }
}
