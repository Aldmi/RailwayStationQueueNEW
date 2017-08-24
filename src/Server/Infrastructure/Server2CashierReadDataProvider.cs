using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Communication.Annotations;
using Communication.Interfaces;
using Library.Library;
using Server.Entitys;

namespace Server.Infrastructure
{
    public enum CashierHandling : byte { IsNotHandling, IsStartHandling, IsSuccessfulHandling, IsErrorHandling, IsSuccessfulAndStartHandling, IsErrorAndStartHandling }


    /// <summary>
    /// Данные полученные в ответ от кассира
    /// </summary>
    public class CashierOutData
    {
        public bool IsWork { get; set; }                     // Работа/Перерыв
        public CashierHandling Handling { get; set; }        // Активная кнопка обработки клиента
        public string NameTicket { get; set; }                // Название билета (4 символа) 
    }



    /// <summary>
    /// Modbus Функция 0x03 
    /// </summary>
    public class Server2CashierReadDataProvider : IExchangeDataProvider<byte, CashierOutData>
    {
        #region field

        private const ushort StartAddresRead = 0x0000;
        private const ushort NReadRegister = 0x0003;

        #endregion




        #region prop

        public int CountGetDataByte { get; } = 0x08;                            // N байт запроса
        public int CountSetDataByte { get; } = (0x05 + NReadRegister * 2);      // N байт ответа

        public byte InputData { get; set; }                                      //Номер кассира для опроса
        public CashierOutData OutputData { get; private set; }                   //Номер кнопки нажатой кассиром
        public bool IsOutDataValid { get; private set; }

        #endregion




        #region Methode

        /// <summary>
        /// Данные запроса функц 0x03:
        /// байт[0]= InputData
        /// байт[1]= 0x03
        /// байт[2]= Адр. Ст.
        /// байт[3]= Адр. Мл.
        /// байт[4]= Кол-во. Ст. (0x00)
        /// байт[5]= Кол-во. Мл. (0x03) 
        /// байт[6]= CRC Ст.
        /// байт[7]= CRC Мл.
        /// </summary>
        public byte[] GetDataByte()
        {
            byte[] buff = new byte[CountGetDataByte];

            buff[0] = InputData;
            buff[1] = 0x03;

            var addrBuff = BitConverter.GetBytes(StartAddresRead).Reverse().ToArray();
            addrBuff.CopyTo(buff, 2);

            var numberBuff = BitConverter.GetBytes(NReadRegister).Reverse().ToArray();
            numberBuff.CopyTo(buff, 4);

            var crc = Crc16.ModRTU_CRC(buff, CountGetDataByte - 2);
            crc.CopyTo(buff, 6);

            return buff;
        }

        /// <summary>
        /// Данные ответа:
        /// байт[0]= InputData
        /// байт[1]= 0x03
        /// байт[2]= кол-во байт  (0x06)
        /// байт[3]= Статутс Ст.
        /// байт[4]= Статутс  Мл.
        /// байт[5]= Счетчик обработанных клиентов Ст.
        /// байт[6]= Счетчик обработанных клиентов Мл.
        /// байт[7]= № билета 0-9бит (0-1023) - номер билета
        /// байт[8]= № билета 0-9бит (0-1023) - номер билета
        /// байт[9]= CRC Мл.
        /// байт[10]= CRC Ст.
        /// 
        /// Статус (байт[4])
        /// бит 0: Станция в работе
        /// бит 1: Запрос нового пасажира от сервера
        /// бит 2: Идет обработка пассажира
        /// бит 3: Последний пассажир обработанн
        /// бит 4: Результат обработки - успешно
        /// бит 5: Результат обработки - отсутсвует (обратно в очередь)
        /// бит 6: Запрос нового пасажира от сервера ПОЛУЧЕНН.
        /// </summary>
        public bool SetDataByte(byte[] data)
        {
            if (data == null || data.Length != CountSetDataByte)
            {
                IsOutDataValid = false;
                return false;
            }

            byte[] dataBuffer = null;
            if (data[0] == InputData &&
                data[1] == 0x03 &&
                data[2] == (NReadRegister * 2) &&
                Crc16.CheckCrc(data))
            {
                dataBuffer = data.Skip(3).Take((NReadRegister * 2)).ToArray();
            }

            if (dataBuffer != null)
            {
                OutputData = new CashierOutData { IsWork = (dataBuffer[1] & 0x01) != 0x00, Handling = CashierHandling.IsNotHandling };

                if ((dataBuffer[1] & 0x04) == 0x00)        // НЕ идет обработка пассажира
                {
                    if ((dataBuffer[1] & 0x02) != 0x00)    
                    {
                        OutputData.Handling = CashierHandling.IsStartHandling;                      // приглашен новый пассажир.

                        if ((dataBuffer[1] & 0x08) != 0x00 && (dataBuffer[1] & 0x10) != 0x00)       //приглашен новый пассажир + Последний пассажир обработан и обработанн успешно                           
                        {
                            OutputData.Handling = CashierHandling.IsSuccessfulAndStartHandling;
                        }
                        else if ((dataBuffer[1] & 0x08) != 0x00 && (dataBuffer[1] & 0x20) != 0x00)  //приглашен новый пассажир + Последний пассажир обработан и обработанн НЕ успешно                         
                        {
                            OutputData.Handling = CashierHandling.IsErrorAndStartHandling;
                        }
                    }
                    else
                    {
                        if ((dataBuffer[1] & 0x08) != 0x00 && (dataBuffer[1] & 0x10) != 0x00)        //Последний пассажир обработан и обработанн успешно                           
                        {
                            OutputData.Handling = CashierHandling.IsSuccessfulHandling;
                        }
                        else if ((dataBuffer[1] & 0x08) != 0x00 && (dataBuffer[1] & 0x20) != 0x00)  //Последний пассажир обработан и обработанн НЕ успешно                         
                        {
                            OutputData.Handling = CashierHandling.IsErrorHandling;
                        }
                    }
                }


                IsOutDataValid = true;
                return true;
            }

            return false;
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