using System.ComponentModel;
using System.Runtime.CompilerServices;
using Communication.Annotations;

namespace Server.Entitys
{
    public class DeviceCashier : INotifyPropertyChanged
    {
        private const byte MaxCountFaildRespowne = 2;
        private byte _countFaildRespowne;

        public byte AddresDevice { get; }
        public Сashier Cashier { get; }
        public string Port { get; }
        public int LastSyncLabel { get; set; } //Метка синхронизации времени (сбрасывается при отсутствии связи)

        private bool _isConnect;
        public bool IsConnect
        {
            get { return _isConnect; }
            set
            {
                RaiseNotConnect2IsConnect = (!_isConnect && value);
                RaiseIsConnect2NotConnect = (_isConnect && !value);

                if (_isConnect == value) return;
                _isConnect = value;
                OnPropertyChanged();
            }
        }


        public bool RaiseNotConnect2IsConnect { get; private set; } // State. Небыл на свзяи -> на связи
        public bool RaiseIsConnect2NotConnect { get; private set; }// State. был на свзяи -> не на связи


        private bool _dataExchangeSuccess;
        public bool DataExchangeSuccess
        {
            get { return _dataExchangeSuccess; }
            set
            {
                _dataExchangeSuccess = value;
                if (_dataExchangeSuccess)
                {
                   _countFaildRespowne = 0;
                   IsConnect = true;
                }
                else
                {
                    if (_countFaildRespowne++ >= MaxCountFaildRespowne)
                    {
                       _countFaildRespowne = 0;
                        IsConnect = false;
                    }
                }
                OnPropertyChanged(); //TODO: вызывается очень часто раз в 250мс!!!!!!!!!!!!!!
            }
        }


        public DeviceCashier(byte addresDevice, Сashier cashier, string port)
        {
            AddresDevice = addresDevice;
            Cashier = cashier;
            Port = port;
        }



        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}