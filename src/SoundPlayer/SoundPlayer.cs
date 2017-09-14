using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;

namespace Sound
{
    public class SoundPlayer : IDisposable
    {
        #region prop

        private MediaPlayer Player { get;} = new MediaPlayer();

        public double Volume
        {
            get { return Player.Volume; }
            set { Player.Volume = value; }
        }

        public PlaybackState GetState => Player.State;

        #endregion





        #region Methode

        public async Task Play(string filePath)
        {
            await Player.LoadAsync(filePath);
            Player.Play();
        }


        public void Stop()
        {
            if (GetState == PlaybackState.Playing)
            {
                Player.Stop();
            }
        }
    

        public void Pause()
        {
            if (GetState == PlaybackState.Playing)
            {
                Player.Pause();
            }
        }

        #endregion





        public void Dispose()
        {
            Player?.Dispose();
        }
    }
}
