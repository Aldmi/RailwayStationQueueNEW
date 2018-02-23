using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
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
        public string PrefixQueue { get; set; }
        public string NameQueue { get; set; }
        public TerminalAction Action { get; set; } //Узнать информацию об очереди или добавить элемент
    }


    /// <summary>
    /// Данные полученные в ответ от сервера
    /// </summary>
    public class TerminalOutData
    {
        public DateTime AddedTime { get; set; }
        public ushort NumberElement { get; set; }
        public string PrefixQueue { get; set; }
        public ushort CountElement { get; set; }    
    }


    public class Terminal2ServerExchangeDataProvider : IExchangeDataProvider<TerminalInData, TerminalOutData>
    {
        #region prop

        public bool IsSynchronized { get; } = false; // Без внешней синхронизации
        public object SyncRoot { get; } = new object();

        public int CountSetDataByte => 16;
        public int CountGetDataByte => 25;

        public TerminalInData InputData { get; set; }  
        public TerminalOutData OutputData { get; set; }

        public bool IsOutDataValid { get; set; }

        #endregion




        #region Methode

        /// <summary>
        /// формат запроса серверу:
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
        public byte[] GetDataByte()
        {
            var buffer = new List<byte>
            {
                0xAA,
                0xBB,
               (byte)InputData.Action
            };

            try
            {
                var encoding = Encoding.Unicode;

                var prefixQueueBytes= encoding.GetBytes(InputData.PrefixQueue).Take(2).ToArray();
                buffer.AddRange(prefixQueueBytes);

                var padSpace = InputData.NameQueue.PadRight(10); //дополнить пробелами справа до 10 симолов.
                var nameQueueBytes = encoding.GetBytes(padSpace);
                buffer.AddRange(nameQueueBytes);
            }
            catch (Exception ex)
            {
                return null;
            }

            return buffer.ToArray();
        }


        /// <summary>
        /// формат ответа от сервера:
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
        public bool SetDataByte(byte[] data)
        {
            IsOutDataValid = false;

            if (data == null || data.Count() < CountSetDataByte)
                return IsOutDataValid;

            var encoding = Encoding.Unicode;
            var prefixQueueBytes = encoding.GetBytes(InputData.PrefixQueue).Take(2).ToArray();

            if (data[0] == 0xAA &&
                data[1] == 0xBB &&
                data[2] == prefixQueueBytes[0] &&
                data[3] == prefixQueueBytes[1])
            {
                OutputData = new TerminalOutData
                {
                    PrefixQueue = encoding.GetString(data, 2, 2),
                    NumberElement = BitConverter.ToUInt16(data, 4),
                    CountElement = BitConverter.ToUInt16(data, 6),
                    AddedTime = new DateTime(BitConverter.ToInt64(data, 8))
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