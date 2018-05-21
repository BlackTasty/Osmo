using Osmo.UI;
using Osmo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Un4seen.Bass;

namespace Osmo.Core
{
    class AudioEngine
    {
        private int _stream;
        private BASSTimer _timer;

        private SkinViewModel vm;

        public AudioEngine(SkinViewModel vm)
        {
            this.vm = vm;
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            _timer = new BASSTimer(100);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            vm.AudioPosition = Bass.BASS_ChannelGetPosition(_stream);
        }

        public void PlayAudio(string path)
        {
            _stream = Bass.BASS_StreamCreateFile(path, 0, 0, BASSFlag.BASS_DEFAULT);
            if (_stream != 0)
            {
                Bass.BASS_ChannelPlay(_stream, false);
                vm.AudioLength = Bass.BASS_ChannelGetLength(_stream);
            }
            else
            {
                MessageBox.Show("Unable to play the selected audio file!");
            }
        }

        public void StopAudio()
        {
            Bass.BASS_StreamFree(_stream);
        }

        public void ChangeVolume(double volume)
        {
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, (float)volume);
        }

        ~AudioEngine()
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick;
            _timer.Dispose();
            Bass.BASS_StreamFree(_stream);
            Bass.BASS_Free();
        }
    }
}
