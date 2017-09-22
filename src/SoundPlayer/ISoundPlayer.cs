using System;
using NAudio.Wave;

namespace Sound
{
    public interface ISoundPlayer : IDisposable
    {
        bool PlayFile(string file);
        void Play();
        void Pause();
        void Stop();

        float GetVolume();
        void SetVolume(float volume);

        long GetCurrentPosition();
        TimeSpan? GetDuration();

        PlaybackState GetStatus();
    }
}