using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Sound
{
    public class SoundPlayer : ISoundPlayer
    {
        #region prop

        public IWavePlayer WaveOutDevice { get; set; } = new WaveOut();
        public AudioFileReader AudioFileReader { get; set; }

        #endregion





        #region Methode

        public bool PlayFile(string file)
        {
            if (AudioFileReader != null)
            {
                AudioFileReader.Dispose();
                AudioFileReader = null;
            }

            try
            {
                if (System.IO.File.Exists(file))
                {
                    AudioFileReader = new AudioFileReader(file);
                    WaveOutDevice.Init(AudioFileReader);

                    SetVolume(0.9f);
                    Play();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }



        public void Play()
        {
            if (AudioFileReader == null)
                return;

            try
            {
                if (WaveOutDevice.PlaybackState == PlaybackState.Paused ||
                    WaveOutDevice.PlaybackState == PlaybackState.Stopped)
                {
                    WaveOutDevice.Play();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        public void Pause()
        {
            if (AudioFileReader == null)
                return;

            try
            {
                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
                    WaveOutDevice.Pause();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        public void Stop()
        {
            if (AudioFileReader == null)
                return;

            try
            {
                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
                    WaveOutDevice.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        public float GetVolume()
        {
            return AudioFileReader?.Volume ?? 0f;
        }



        //1.0f is full volume
        public void SetVolume(float volume)
        {
            if (AudioFileReader != null)
            {
                AudioFileReader.Volume = volume;
            }
        }



        public long GetCurrentPosition()
        {
            return  AudioFileReader?.Position ?? 0;
        }



        public TimeSpan? GetDuration()
        {
            return AudioFileReader?.TotalTime ?? null;
        }



        public PlaybackState GetStatus()
        {
            return WaveOutDevice.PlaybackState;
        }

        #endregion





        #region IDisposable

        public void Dispose()
        {
            if (WaveOutDevice != null)
            {
                WaveOutDevice.Stop();
                WaveOutDevice.Dispose();
            }
        }

        #endregion
    }
}
