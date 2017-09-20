using System;
using System.Collections.Generic;
using System.Linq;

namespace Sound
{
    public class SoundQueue : IDisposable
    {
        #region prop

        public ISoundPlayer Player { get; set; }
        public ISoundNameService SoundNameService { get; set; }

        private Queue<SoundTemplate> Queue { get; } = new Queue<SoundTemplate>();
        public SoundTemplate CurrentSoundMessagePlaying { get; set; }
        public string CurrentFilePlaying { get; set; }

        public bool IsWorking { get; private set; }

        #endregion




        #region ctor

        public SoundQueue(ISoundPlayer soundPlayer, ISoundNameService soundNameService)
        {
            Player = soundPlayer;
            SoundNameService = soundNameService;
        }

        #endregion




        #region Methode

        /// <summary>
        /// Добавить элемент в очередь
        /// </summary>
        public void AddItem(SoundTemplate item)
        {
            if (item == null)
                return;
        }



        public void StartQueue()
        {
            IsWorking = true;
        }



        public void StopQueue()
        {
            IsWorking = false;
        }



        /// <summary>
        /// Очистить очередь
        /// </summary>
        public void Clear()
        {
            Queue?.Clear();
            CurrentSoundMessagePlaying = null;
            CurrentFilePlaying = null;
        }


        public void PausePlayer()
        {
            StopQueue();
            Player.Pause();
        }



        public void PlayPlayer()
        {
            StartQueue();
            Player.Play();
        }



        public void Erase()
        {
            Clear();
            Player.PlayFile(string.Empty);
        }



        /// <summary>
        /// Разматывание очереди, внешним кодом
        /// </summary>
        public void Invoke()
        {
            if (!IsWorking)
                return;

            try
            {
                var status = Player.GetStatus();

                //Разматывание очереди. Определение проигрываемого файла-----------------------------------------------------------------------------
                if (status != SoundFileStatus.Playing)
                {
                    if (Queue.Any())
                    {
                        CurrentSoundMessagePlaying = Queue.Dequeue();
                    }

                    if (CurrentSoundMessagePlaying == null)
                        return;

                    var soundFile= CurrentSoundMessagePlaying.FileNameQueue.Any() ? CurrentSoundMessagePlaying.FileNameQueue.Dequeue() : null;
                    if (soundFile?.Contains(".wav") == false)
                        soundFile = SoundNameService?.GetFileName(soundFile);

                    if(string.IsNullOrEmpty(soundFile) || string.IsNullOrWhiteSpace(soundFile))
                        return;

                    Player.PlayFile(soundFile);
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine($"Invoke = {ex.ToString()}");//DEBUG
            }
        }

        #endregion





            #region IDisposable

        public void Dispose()
        {
          Player?.Dispose();
        }

        #endregion
    }
}