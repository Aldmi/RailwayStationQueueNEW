using System;

namespace Sound
{
    public enum SoundFileStatus
    {
        Error,
        Stop,
        Playing,
        Paused,
    };

    public interface ISoundPlayer : IDisposable
    {
        bool PlayFile(string file);
        void Pause();
        void Play();
        int GetVolume();
        void SetVolume(int volume);

        int GetCurrentPosition();
        float GetDuration();

        SoundFileStatus GetStatus();
    }
}