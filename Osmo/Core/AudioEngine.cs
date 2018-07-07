using MaterialDesignThemes.Wpf;
using Osmo.Core.Objects;
using Osmo.UI;
using Osmo.ViewModel;
using System;
using Un4seen.Bass;

namespace Osmo.Core
{
    class AudioEngine
    {
        private int _stream;
        private BASSTimer _timer;
        private bool paused;

        private AudioViewModel vm;

        public bool EnableSliderChange { get; set; }

        public AudioEngine(AudioViewModel vm)
        {
            this.vm = vm;
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            _timer = new BASSTimer(100);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!EnableSliderChange)
                vm.AudioPosition = Bass.BASS_ChannelGetPosition(_stream);
        }

        public bool PlayAudio(string path)
        {
            if (!paused)
            {
                _stream = Bass.BASS_StreamCreateFile(path, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT);
                if (_stream != 0)
                {
                    Bass.BASS_ChannelPlay(_stream, false);
                    vm.AudioLength = Bass.BASS_ChannelGetLength(_stream, BASSMode.BASS_POS_BYTES);
                }
                else
                {
                    var msgBox = MaterialMessageBox.Show(Helper.FindString("dlg_invalidAudioTitle"),
                        Helper.FindString("dlg_invalidAudioDescription"),
                        OsmoMessageBoxButton.OK);

                    DialogHost.Show(msgBox);
                    return false;
                }
            }
            else
            {
                paused = false;
                Bass.BASS_ChannelPlay(_stream, false);
            }

            return true;
        }

        public void PauseAudio()
        {
            paused = true;
            Bass.BASS_ChannelPause(_stream);
        }

        public void StopAudio()
        {
            Bass.BASS_StreamFree(_stream);
            paused = false;
        }

        public void SetVolume(double volume)
        {
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, (float)volume);
        }

        public void SetPosition(double position)
        {
            Bass.BASS_ChannelPause(_stream);
            Bass.BASS_ChannelSetPosition(_stream, position);
            Bass.BASS_ChannelPlay(_stream, false);
            vm.AudioPosition = position;
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
