using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;

namespace Sound
{
    public class SoundPlayer : ISoundPlayer
    {
        #region prop



        #endregion





        #region Methode

        public bool PlayFile(string file)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int volume)
        {
            throw new NotImplementedException();
        }

        public int GetCurrentPosition()
        {
            throw new NotImplementedException();
        }

        public float GetDuration()
        {
            throw new NotImplementedException();
        }

        public SoundFileStatus GetStatus()
        {
            return SoundFileStatus.Playing;
        }

        #endregion




        public void Dispose()
        {
            throw new NotImplementedException();
        }


    }
}
