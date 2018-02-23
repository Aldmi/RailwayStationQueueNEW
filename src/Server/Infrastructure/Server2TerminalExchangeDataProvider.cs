using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Communication.Annotations;
using Communication.Interfaces;
using Terminal.Infrastructure;

namespace Server.Infrastructure
{
    //Один объект DataProvider используется для нескольких ClientTcpIp
    public class Server2TerminalExchangeDataProvider : IExchangeDataProvider<TerminalInData, TerminalOutData>
    {


        public Server2TerminalExchangeDataProvider(bool isSynchronized)
        {
            IsSynchronized = isSynchronized;
        }




        #region prop

        public bool IsSynchronized { get; }
        public object SyncRoot { get; } = new object();


        public int CountSetDataByte => 25;
        public int CountGetDataByte => 16;

        private TerminalInData _inputData;
        public TerminalInData InputData                       //данные полученные от клиента. Т.е. номер очереди и действие
        {
            get { return _inputData; }
            set
            {
                _inputData = value;
                OnPropertyChanged();
            }
        }

        public TerminalOutData OutputData { get; set; }        //данные отправляемые клиенту. Т.е. номер очереди и номер элемента, кол-во элементов в очереди

        public bool IsOutDataValid { get; private set; }

        #endregion




        #region Methode

        /// <summary>
        /// формат ответа сервера клиенту:
        /// байт[0]= 0хAA
        /// байт[1]= 0хBB
        /// байт[2]= префикс очереди
        /// байт[3]= префикс очереди
        /// байт[4]= номер элемента в очереди (Б0)
        /// байт[5]= номер элемента в очереди (Б1)
        /// байт[6]= кол-во элементов в очереди  (Б0)     
        /// байт[7]= кол-во элементов в очереди  (Б1)
        /// байт[8]= дата и время (Б0)
        /// байт[9]= дата и время (Б1)
        /// байт[10]= дата и время (Б2)
        /// байт[11]= дата и время (Б3)
        /// байт[12]= дата и время (Б4)
        /// байт[13]= дата и время (Б5)
        /// байт[14]= дата и время (Б6)
        /// байт[15]= дата и время (Б7)
        /// </summary>
        public byte[] GetDataByte()
        {

            var buff = new byte[CountGetDataByte];

            var encoding = Encoding.Unicode;
            var prefixQueueBytes = encoding.GetBytes(OutputData.PrefixQueue).Take(2).ToArray();

            buff[0] = 0xAA;
            buff[1] = 0xBB;
            buff[2] = prefixQueueBytes[0];
            buff[3] = prefixQueueBytes[1];

            var idElementBuff = BitConverter.GetBytes(OutputData.NumberElement);
            idElementBuff.CopyTo(buff, 4);

            var countElementBuff = BitConverter.GetBytes(OutputData.CountElement);
            countElementBuff.CopyTo(buff, 6);

            var dateAddedBuff = BitConverter.GetBytes(OutputData.AddedTime.Ticks);
            dateAddedBuff.CopyTo(buff, 8);

            return buff;

        }


        /// <summary>
        /// формат запроса от клиента:
        /// байт[0]= 0хAA
        /// байт[1]= 0хBB
        /// байт[2]= действие
        /// байт[3]= префикс очереди (кирилица)
        /// байт[4]= префикс очереди (кирилица)
        /// байт[5]= Название очереди (макс 10 симолов = 20 байт)
        /// байт[6]= Название очереди
        /// байт[7]= Название очереди
        /// байт[8]= Название очереди
        /// байт[9]= Название очереди
        /// байт[10]= Название очереди
        /// байт[11]= Название очереди
        /// байт[12]= Название очереди
        /// байт[13]= Название очереди
        /// байт[14]= Название очереди
        /// байт[15]= Название очереди
        /// байт[16]= Название очереди
        /// байт[17]= Название очереди
        /// байт[18]= Название очереди
        /// байт[19]= Название очереди
        /// байт[20]= Название очереди
        /// байт[21]= Название очереди
        /// байт[22]= Название очереди
        /// байт[23]= Название очереди
        /// байт[24]= Название очереди
        /// </summary>
        public bool SetDataByte(byte[] data)
        {

            IsOutDataValid = false;

            if (data == null || data.Count() < CountSetDataByte)
                return IsOutDataValid;

            if (data[0] == 0xAA &&
                data[1] == 0xBB)
            {
                string nameQueue;
                string prefixQueue;
                try
                {
                    var encoding = Encoding.Unicode;
                    nameQueue = encoding.GetString(data, 5, 20);
                    nameQueue = nameQueue.TrimEnd();
                    prefixQueue = encoding.GetString(data, 3, 2);
                }
                catch (Exception ex)
                {
                    IsOutDataValid = false;
                    return false;
                }

                InputData = new TerminalInData { NameQueue = nameQueue, PrefixQueue = prefixQueue, Action = (TerminalAction)data[2] };
                IsOutDataValid = true;
            }
            else
            {
                IsOutDataValid = false;
            }

            return IsOutDataValid;

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
    }
}