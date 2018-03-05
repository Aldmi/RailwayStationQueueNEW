using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using Communication.Annotations;
using Library.Logs;
using NAudio.Wave;



namespace Sound
{
    public class SoundQueue : INotifyPropertyChanged, IDisposable
    {
        #region field

        private readonly Timer _timerInvokeSoundQueue;

        private readonly Log _loggerSound = new Log("Sound.SoundQueue");

        #endregion




        #region prop

        public ISoundPlayer Player { get; set; }
        public ISoundNameService SoundNameService { get; set; }

        private Queue<SoundTemplate> Queue { get; } = new Queue<SoundTemplate>();
        public IEnumerable<SoundTemplate> GetQueue => Queue;
        public SoundTemplate CurrentSoundMessagePlaying { get; set; }
        public string CurrentFilePlaying { get; set; }
        public bool IsWorking { get; private set; }

        #endregion




        #region ctor

        public SoundQueue(ISoundPlayer soundPlayer, ISoundNameService soundNameService, double timerQueueInterval)
        {
            Player = soundPlayer;
            SoundNameService = soundNameService;
            _timerInvokeSoundQueue= new Timer(timerQueueInterval);
            _timerInvokeSoundQueue.Elapsed += TimerInvokeSoundQueue_Elapsed;
            _timerInvokeSoundQueue.Start();
        }



        #endregion




        #region Events

        private void TimerInvokeSoundQueue_Elapsed(object sender, ElapsedEventArgs e)
        {
            Invoke();
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

            _loggerSound.Info($"AddItem: {item.Name}");
            Queue.Enqueue(item);
            OnPropertyChanged("Queue");
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
            OnPropertyChanged("Queue");
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
        private void Invoke()
        {
            if (!IsWorking)
                return;

            try
            {
                var status = Player.GetStatus();

                //Разматывание очереди. Определение проигрываемого файла-----------------------------------------------------------------------------
                if (status != PlaybackState.Playing)
                {
                    if (Queue.Any())
                    {
                        if (CurrentSoundMessagePlaying == null)
                        {
                            CurrentSoundMessagePlaying = Queue.Peek();
                        }

                        if (!CurrentSoundMessagePlaying.FileNameQueue.Any())
                        {
                            Queue.Dequeue();
                            CurrentSoundMessagePlaying = null;
                            OnPropertyChanged("Queue");
                        }
                    }

                    if (CurrentSoundMessagePlaying == null)
                       return;
                    
                    var soundFile= CurrentSoundMessagePlaying.FileNameQueue.Any() ? CurrentSoundMessagePlaying.FileNameQueue.Dequeue() : null;
                    if (soundFile?.Contains(".wav") == false)
                        soundFile = SoundNameService?.GetFileName(soundFile);

                    if(string.IsNullOrEmpty(soundFile) || string.IsNullOrWhiteSpace(soundFile))
                        return;

                    _loggerSound.Info($"PlayFile: {soundFile}");
                    Player.PlayFile(soundFile);
                 }
            }
            catch (Exception ex)
            {
                _loggerSound.Error($"SoundQueue/Invoke  {ex}");
            }
        }

        #endregion



        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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