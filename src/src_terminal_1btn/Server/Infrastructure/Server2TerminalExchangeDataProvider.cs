using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Communication.Annotations;
using Communication.Interfaces;
using Terminal.Infrastructure;

namespace Server.Infrastructure
{
    public class Server2TerminalExchangeDataProvider : IExchangeDataProvider<TerminalInData, TerminalOutData>
    {
        #region prop

        public int CountSetDataByte => 4;
        public int CountGetDataByte => 15;

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
        /// байт[2]= номер очереди
        /// байт[3]= номер элемента в очереди (Б0)
        /// байт[4]= номер элемента в очереди (Б1)
        /// байт[5]= кол-во элементов в очереди  (Б0)     
        /// байт[6]= кол-во элементов в очереди  (Б1)
        /// байт[7]= дата и время (Б0)
        /// байт[8]= дата и время (Б1)
        /// байт[9]= дата и время (Б2)
        /// байт[10]= дата и время (Б3)
        /// байт[11]= дата и время (Б4)
        /// байт[12]= дата и время (Б5)
        /// байт[13]= дата и время (Б6)
        /// байт[14]= дата и время (Б7)
        /// </summary>
        public byte[] GetDataByte()
        {
            var buff = new byte[CountGetDataByte];

            buff[0]= 0xAA;
            buff[1]= 0xBB;
            buff[2] = OutputData.NumberQueue;   

            var idElementBuff = BitConverter.GetBytes(OutputData.NumberElement);
            idElementBuff.CopyTo(buff, 3);

            var countElementBuff = BitConverter.GetBytes(OutputData.CountElement);
            countElementBuff.CopyTo(buff, 5);

            var dateAddedBuff = BitConverter.GetBytes(OutputData.AddedTime.Ticks);
            dateAddedBuff.CopyTo(buff, 7);

            return buff;
        }


        /// <summary>
        /// формат запроса от клиента:
        /// байт[0]= 0хAA
        /// байт[1]= 0хBB
        /// байт[2]= номер очереди
        /// байт[3]= действие
        /// </summary>
        public bool SetDataByte(byte[] data)
        {
            IsOutDataValid = false;

            if (data == null || data.Count() < CountSetDataByte)
                return IsOutDataValid;

            if (data[0] == 0xAA &&
                data[1] == 0xBB)
            {
                InputData= new TerminalInData { NumberQueue = data[2], Action = (TerminalAction) data[3] };
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