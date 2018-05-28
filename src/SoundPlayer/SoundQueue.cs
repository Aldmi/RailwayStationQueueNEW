using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using Communication.Annotations;
using Library.Logs;
using NAudio.Wave;
using Timer = System.Timers.Timer;


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

        private ConcurrentQueue<SoundTemplate> Queue { get; set; } = new ConcurrentQueue<SoundTemplate>();
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
            _timerInvokeSoundQueue = new Timer(timerQueueInterval);
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


            var agregateStr = item.FileNameQueue.Aggregate((i, j) => i + " " + j);//DEBUG
            _loggerSound.Error($"AddItem: {item.Name}     agregateStr= {agregateStr}     Thread= {Thread.CurrentThread.ManagedThreadId}");
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
            Queue = new ConcurrentQueue<SoundTemplate>();
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
                    if (!Queue.IsEmpty)
                    {
                        if (CurrentSoundMessagePlaying == null)
                        {
                            SoundTemplate outVal;
                            if (Queue.TryPeek(out outVal))
                            {
                                CurrentSoundMessagePlaying = outVal;
                            }
                            //CurrentSoundMessagePlaying = Queue.Peek();
                        }

                        if (!CurrentSoundMessagePlaying.FileNameQueue.Any())
                        {
                            SoundTemplate outVal;
                            if (Queue.TryDequeue(out outVal))
                            {
                                CurrentSoundMessagePlaying = null;
                                OnPropertyChanged("Queue");
                            }
                        }
                    }

                    if (CurrentSoundMessagePlaying == null)
                        return;

                    var soundFile = CurrentSoundMessagePlaying.FileNameQueue.Any() ? CurrentSoundMessagePlaying.FileNameQueue.Dequeue() : null;
                    if (soundFile?.Contains(".wav") == false)
                        soundFile = SoundNameService?.GetFileName(soundFile);

                    if (string.IsNullOrEmpty(soundFile) || string.IsNullOrWhiteSpace(soundFile))
                    {
                        _loggerSound.Info($"PlayFile: IsNullOrEmpty {soundFile}");
                        return;
                    }

                    _loggerSound.Info($"PlayFile: {soundFile}");
                    Player.PlayFile(soundFile);
                }
            }
            catch (Exception ex)
            {
                _loggerSound.Error($"SoundQueue/Invoke  {ex.Message}");
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