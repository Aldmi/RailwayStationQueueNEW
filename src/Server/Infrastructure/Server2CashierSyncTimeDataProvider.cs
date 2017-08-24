using Communication.Interfaces;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Communication.Annotations;
using Library.Library;


namespace Server.Infrastructure
{
    class Server2CashierSyncTimeDataProvider :  IExchangeDataProvider<byte, byte>
    {
        #region field

        private const ushort StartAddresWrite = 0x0003;
        private const ushort NWriteRegister = 0x0002;

        #endregion





        #region prop

        public int CountGetDataByte { get; } = 9 + (NWriteRegister * 2);
        public int CountSetDataByte { get; } = 8;

        public byte InputData { get; set; }

        public bool IsOutDataValid { get; set; }

        public byte OutputData { get; }

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





        #region Methode

        /// <summary>
        /// Данные запроса по записи времени (функц 0x10):
        /// байт[0]= 0x00  - широковещательная посылка
        /// байт[1]= 0x10
        /// байт[2]= Адр. Ст.
        /// байт[3]= Адр. Мл.
        /// байт[4]= Кол-во. рег. Ст.
        /// байт[5]= Кол-во. рег. Мл.
        /// байт[6]= Кол-во. байт
        /// байт[7]= Время Ст.байт Ст.слова (seconds + minutes*60 + hours*3600) 
        /// байт[8]= Время Мл.байт Ст.слова (seconds + minutes*60 + hours*3600) 
        /// байт[9]= Время Ст.байт Мл.слова (seconds + minutes*60 + hours*3600) 
        /// байт[10]= Время Мл.байт Мл.слова (seconds + minutes*60 + hours*3600) 
        /// байт[11]= CRC Мл.
        /// байт[12]= CRC Ст.
        /// </summary>
        public byte[] GetDataByte()
        {
            byte[] buff = new byte[CountGetDataByte];

            buff[0] = 0x00;
            buff[1] = 0x10;

            var addrBuff = BitConverter.GetBytes(StartAddresWrite).Reverse().ToArray();
            addrBuff.CopyTo(buff, 2);

            var numberBuff = BitConverter.GetBytes(NWriteRegister).Reverse().ToArray();
            numberBuff.CopyTo(buff, 4);

            buff[6] = (NWriteRegister * 2);

            uint time = (uint) (DateTime.Now.Hour*3600 + DateTime.Now.Minute*60 + DateTime.Now.Second);
            var timeBuff = BitConverter.GetBytes(time).Reverse().ToArray();
            timeBuff.CopyTo(buff, 7);

            var crc = Crc16.ModRTU_CRC(buff, CountGetDataByte - 2);
            crc.CopyTo(buff, CountGetDataByte - 2);

            return buff;
        }


        public bool SetDataByte(byte[] data)
        {
            return true;
        }

        #endregion
    }
}
