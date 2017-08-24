namespace Server.Entitys
{
    public class DeviceCashier
    {
        private const byte MaxCountFaildRespowne = 5;
        private byte _countFaildRespowne;

        public Сashier Cashier { get; }
        public bool IsConnect { get; private set; } = false;


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


        public DeviceCashier(Сashier cashier)
        {
            Cashier = cashier;
        }
    }
}