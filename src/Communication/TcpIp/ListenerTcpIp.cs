﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Communication.Annotations;
using Communication.Interfaces;
using Communication.Settings;

namespace Communication.TcpIp
{
    public class ListenerTcpIp : INotifyPropertyChanged, IDisposable
    {
        #region fields

        private readonly int _ipPort;                                                           //порт

        private string _statusString;
        private bool _isConnect;

        readonly object _lock = new object();
        readonly List<Task> _hotTasks = new List<Task>();                                       //все задачи всех клиентов не завершенные в данный момент
        readonly ObservableCollection<Client> _clients = new ObservableCollection<Client>();    //все подключенные клиенты


        private List<Client> _getClients;
        public List<Client> GetClients
        {
            get { return _getClients; }
            private set
            {
                _getClients = value;
                OnPropertyChanged();
            }
        }

        #endregion





        #region ctor

        public ListenerTcpIp(int ipPort)
        {
            _ipPort = ipPort;
            _clients.CollectionChanged += (sender, args) =>
            {
                var collect = sender as ObservableCollection<Client>;
                GetClients = _clients.ToList();
                IsConnect = (collect != null && collect.Any());                                 //при наличии хотя бы одного клиента IsConnect = true;
            };
        }

        public ListenerTcpIp(XmlListenerSettings settings) : this(settings.Port)
        {
        }

        #endregion


    


        #region prop   

        public TcpListener TcpListener { get; set; }

        public CancellationTokenSource Cts { get; set; } = new CancellationTokenSource();

        public string StatusString
        {
            get { return _statusString; }
            set
            {
                if (value == _statusString) return;
                _statusString = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnect
        {
            get { return _isConnect; }
            set
            {
                if (value == _isConnect) return;
                _isConnect = value;
                OnPropertyChanged();
            }
        }

        #endregion




        #region Method

        /// <summary>
        /// каждому типу клиента назначается свой порт и свой поставшик данных.
        /// </summary>
        public async Task RunServer(IExchangeDataProviderBase dataProvider)
        {
            if (dataProvider == null)
                return;

            OnPropertyChanged(nameof(IsConnect));

            var token = Cts.Token;
            await Task.Run(async () =>
            {
                TcpListener = TcpListener.Create(_ipPort);
                TcpListener.Start();

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var tcpClient = await TcpListener.AcceptTcpClientAsync();
                        var task = ProcessClient(tcpClient, dataProvider, token);

                        if (task.IsFaulted)
                            task.Wait(token);
                    }
                    catch (ObjectDisposedException)                               //срабоатет при TcpListener.Stop() и если был вызван токен отмены то выйдем из цикла прослушки. 
                    {
                        continue;
                    }
                }
            }, token);
        }


        private async Task ProcessClient(TcpClient c, IExchangeDataProviderBase dataProvider, CancellationToken token)
        {
            using (var client = new Client(c))
            {
                // Если Ip нового клиента уже есть в списке, значит этот клиент выполнил аварийный реконект (без вызова Dispose),
                //Т.е. просто отключение терминала по питанию. 
                //Вручную удалим старого клиента.
               var fatalDisposeClient= _clients.FirstOrDefault(cl => cl.Ip == client.Ip);
               if (fatalDisposeClient != null)
               {
                   fatalDisposeClient.Dispose();
                   _clients.Remove(fatalDisposeClient);
               }

                _clients.Add(client);
                while (c.Connected && !token.IsCancellationRequested)
                {
                    var exchangeTask = client.ProcessAsync(dataProvider, token);

                    lock (_lock)
                    _hotTasks.Add(exchangeTask);

                    // обработка ошибок для клиентского потока
                    try
                    {
                        await exchangeTask;
                    }
                    catch (Exception)
                    {
                        // log
                    }
                    finally
                    {
                        lock (_lock)
                        _hotTasks.Remove(exchangeTask);
                    }
                }
                _clients.Remove(client);
            }
        }

        #endregion




        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion




        #region NestedClass

        public class Client : IDisposable
        {
            readonly NetworkStream _stream;
            public string Ip { get; }



            public Client(TcpClient client)
            {
                _stream = client.GetStream();
                Ip = ((IPEndPoint) client.Client.RemoteEndPoint).Address.MapToIPv4().ToString();
            }


            public async Task ProcessAsync(IExchangeDataProviderBase dataProvider, CancellationToken token)
            {
                //Ожидание получение информации из потока
                var actionBuffer = await ReadFromStreamAsync(dataProvider.CountSetDataByte, token);

                byte[] writeBuffer=null;
                if (dataProvider.IsSynchronized)
                {
                    lock (dataProvider.SyncRoot)
                    {
                        if (dataProvider.SetDataByte(actionBuffer))//если полученные от клиента данные валидны, то отправим ему ответ
                        {
                            writeBuffer = dataProvider.GetDataByte();
                        }
                    }
                }
                else
                {
                    if (dataProvider.SetDataByte(actionBuffer))//если полученные от клиента данные валидны, то отправим ему ответ
                    {
                        writeBuffer = dataProvider.GetDataByte();
                    }
                }

                //Отправка ответа в поток
                if (writeBuffer != null)
                {
                    await WriteInStreamAsync(writeBuffer, token);
                }       
            }

            private async Task<byte[]> ReadFromStreamAsync(int nbytes, CancellationToken token)
            {
                var buf = new byte[nbytes];
                var readpos = 0;
                while (readpos < nbytes)
                    readpos += await _stream.ReadAsync(buf, readpos, nbytes - readpos, token);
                return buf;
            }

            private async Task WriteInStreamAsync(byte[] buffer, CancellationToken token)
            {
                await _stream.WriteAsync(buffer, 0, buffer.Length, token);
            }


            public void Dispose()
            {
                _stream.Dispose();
            }
        }

        #endregion




        #region Disposable

        public void Dispose()
        {
            Cts.Cancel();
            TcpListener?.Stop();

            if (_hotTasks != null)
            {
                lock (_lock)
                {
                    _hotTasks.Clear();
                }
            }

            if (_clients != null)
            {
                foreach (var c in _clients)
                {
                    c.Dispose();
                }
            }
        }

        #endregion
    }
}
