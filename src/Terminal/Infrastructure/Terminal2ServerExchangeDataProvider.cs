using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Communication.Annotations;
using Communication.Interfaces;

namespace Terminal.Infrastructure
{
    public enum TerminalAction : byte { Info, Add, Delete}      

    /// <summary>
    /// Данные отправленные серверу
    /// </summary>
    public class TerminalInData
    {
        public byte NumberQueue { get; set; }
        public TerminalAction Action { get; set; } //Узнать информацию об очереди или добавить элемент
    }


    /// <summary>
    /// Данные полученные в ответ от сервера
    /// </summary>
    public class TerminalOutData
    {
        public DateTime AddedTime { get; set; }
        public ushort NumberElement { get; set; }
        public byte NumberQueue { get; set; }
        public ushort CountElement { get; set; }    
    }


    public class Terminal2ServerExchangeDataProvider : IExchangeDataProvider<TerminalInData, TerminalOutData>
    {
        #region prop

        public int CountSetDataByte { get { return 15; } }
        public int CountGetDataByte { get { return 4; } }

        public TerminalInData InputData { get; set; }  
        public TerminalOutData OutputData { get; set; }

        public bool IsOutDataValid { get; set; }

        #endregion




        #region Methode

        /// <summary>
        /// формат запроса серверу:
        /// байт[0]= 0хAA
        /// байт[1]= 0хBB
        /// байт[2]= номер очереди
        /// байт[3]= действие
        /// </summary>
        public byte[] GetDataByte()
        {
            var buffer = new byte[] { 0xAA, 0xBB, InputData.NumberQueue, (byte) InputData.Action };
            return buffer;
        }


        /// <summary>
        /// формат ответа от сервера:
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
        public bool SetDataByte(byte[] data)
        {
            IsOutDataValid = false;

            if (data == null || data.Count() < CountSetDataByte)
                return IsOutDataValid;

            if (data[0] == 0xAA &&
                data[1] == 0xBB &&
                data[2] == InputData.NumberQueue)
            {
                OutputData = new TerminalOutData
                {
                    NumberQueue = data[2],
                    NumberElement = BitConverter.ToUInt16(data, 3),
                    CountElement = BitConverter.ToUInt16(data, 5),
                    AddedTime = new DateTime(BitConverter.ToInt64(data, 7))
                };
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