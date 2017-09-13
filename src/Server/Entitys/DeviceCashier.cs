using System.ComponentModel;
using System.Runtime.CompilerServices;
using Communication.Annotations;

namespace Server.Entitys
{
    public class DeviceCashier : INotifyPropertyChanged
    {
        private const byte MaxCountFaildRespowne = 2;
        private byte _countFaildRespowne;

        public Сashier Cashier { get; }
        public string Port { get; }

        private bool _isConnect;
        public bool IsConnect
        {
            get { return _isConnect; }
            set
            {
                if (_isConnect == value) return;
                _isConnect = value;
                OnPropertyChanged();
            }
        }


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
            }
        }


        public DeviceCashier(Сashier cashier, string port)
        {
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