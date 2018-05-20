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

        public AudioEngine()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }

        public void PlayAudio(string path)
        {
            _stream = Bass.BASS_StreamCreateFile(path, 0, 0, BASSFlag.BASS_DEFAULT);
            if (_stream != 0)
                Bass.BASS_ChannelPlay(_stream, false);
            else
                MessageBox.Show("Unable to play the selected audio file!");
        }

        public void StopAudio()
        {
            Bass.BASS_StreamFree(_stream);
        }

        ~AudioEngine()
        {
            Bass.BASS_StreamFree(_stream);
            Bass.BASS_Free();
        }
    }
}
