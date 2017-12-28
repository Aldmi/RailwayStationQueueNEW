using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Communication.Annotations;
using Communication.Interfaces;
using Library.Library;
using Server.Entitys;

namespace Server.Infrastructure
{
    /// <summary>
    /// Modbus Функция 0x10 
    /// </summary>
    public class Server2CashierWriteDataProvider : IExchangeDataProvider<TicketItem, bool>
    {
        #region field

        private const ushort StartAddresWrite = 0x0002;
        private const ushort NWriteRegister = 0x0001;

        private readonly byte _addressDevice;
        #endregion




        #region ctor

        public Server2CashierWriteDataProvider(byte addressDevice)
        {
            _addressDevice = addressDevice;
        }

        #endregion




        #region prop

        public int CountGetDataByte { get; } = 9 + (NWriteRegister * 2);
        public int CountSetDataByte { get; } = 8;

        public TicketItem InputData { get; set; }          //Номер обрабатываемого билета
        public bool OutputData { get; }
        public bool IsOutDataValid { get; set; }

        #endregion




        #region Methode

        /// <summary>
        /// Данные запроса по записи информации о билете (функц 0x10):
        /// байт[0]= I_addressDevice
        /// байт[1]= 0x10
        /// байт[2]= Адр. Ст.
        /// байт[3]= Адр. Мл.
        /// байт[4]= Кол-во. рег. Ст.
        /// байт[5]= Кол-во. рег. Мл.
        /// байт[6]= Кол-во. байт
        /// байт[7]= Название билета (0...9 бит - число 0...1023) 
        /// байт[8]= Название билета (10...15 бит - буква 0-A  Z-25) 
        /// байт[9]= CRC Мл.
        /// байт[10]= CRC Ст.
        /// </summary>
        /// 
        public byte[] GetDataByte()
        {
            if (InputData == null)
                return null;

            byte[] buff = new byte[CountGetDataByte];


            buff[0] = _addressDevice;
            buff[1] = 0x10;

            var addrBuff = BitConverter.GetBytes(StartAddresWrite).Reverse().ToArray();
            addrBuff.CopyTo(buff, 2);

            var numberBuff = BitConverter.GetBytes(NWriteRegister).Reverse().ToArray();
            numberBuff.CopyTo(buff, 4);

            buff[6] = (NWriteRegister * 2);

            //Название билета(буква - кирилицы, число - 0...999)
            var encoding = Encoding.Unicode;
            var prefixQueueBytes = encoding.GetBytes(InputData.Prefix).Take(2).ToArray();
            ushort prefix = (ushort) (BitConverter.ToUInt16(prefixQueueBytes, 0) - 0x3F6); //prefix= 0x1A - А, 0x1B - Б
            ushort formatNameTicket = (ushort)(InputData.NumberElement & 0x3FF);                           
            formatNameTicket |= (ushort)(prefix << 10);
            var ticketNameBuff = BitConverter.GetBytes(formatNameTicket).Reverse().ToArray();
            ticketNameBuff.CopyTo(buff, 7);

            var crc = Crc16.ModRTU_CRC(buff, CountGetDataByte - 2);
            crc.CopyTo(buff, CountGetDataByte - 2);

            return buff;
        }



        /// <summary>
        /// Обработка ответа на данные записи информации о билете (функц 0x10):
        /// байт[0]= _addressDevice
        /// байт[1]= 0x10
        /// байт[2]= Адр. Ст.
        /// байт[3]= Адр. Мл.
        /// байт[4]= Кол-во. рег. Ст.
        /// байт[5]= Кол-во. рег. Мл.
        /// байт[6]= CRC Мл.
        /// байт[7]= CRC Ст.
        /// </summary>
        public bool SetDataByte(byte[] data)
        {
            if (data == null || data.Length != CountSetDataByte)
            {
                IsOutDataValid = false;
                return false;
            }

            if (data[0] == _addressDevice &&
                data[1] == 0x10 &&
                Crc16.CheckCrc(data))
            {
                IsOutDataValid = true;
                return true;
            }

            IsOutDataValid = false;      
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