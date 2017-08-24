using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Communication.SerialPort;
using Server.Entitys;
using Server.Infrastructure;

namespace Server.Service
{
    public class CashierExchangeService
    {
        #region field

        private readonly List<DeviceCashier> _deviceCashiers;
        private readonly ushort _timeRespone;

        private int _lastSyncLabel;

        #endregion



        #region ctor

        public CashierExchangeService(List<DeviceCashier> deviceCashiers, ushort timeRespone)
        {
            _deviceCashiers = deviceCashiers;
            _timeRespone = timeRespone;
        }

        #endregion



        #region Methode

        public async Task ExchangeService(MasterSerialPort port, CancellationToken ct)
        {
            if (port == null)
                return;

            foreach (var devCashier in _deviceCashiers)              //Запуск опроса кассиров
            {
                var readProvider= new Server2CashierReadDataProvider { InputData = devCashier.Cashier.Id };
                devCashier.DataExchangeSuccess= await port.DataExchangeAsync(_timeRespone, readProvider, ct);                        //TODO: можно добавить кассирам свойство IsConnect. И выставлять его как резульат обмена.

                if (!devCashier.IsConnect)
                {
                 devCashier.Cashier.DisconectHandling();
                 continue;
                }

                if (readProvider.IsOutDataValid)
                {
                    TicketItem item;
                    var cashierInfo = readProvider.OutputData;

                    if (!cashierInfo.IsWork)
                        continue;

                    switch (cashierInfo.Handling)
                    {
                        case CashierHandling.IsSuccessfulHandling:
                            devCashier.Cashier.SuccessfulHandling();
                            break;

                        case CashierHandling.IsErrorHandling:
                            devCashier.Cashier.ErrorHandling();
                            break;

                        case CashierHandling.IsStartHandling:
                            item = devCashier.Cashier.StartHandling();
                            var writeProvider = new Server2CashierWriteDataProvider { InputData = item };
                            await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                            if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                            {
                             devCashier.Cashier.SuccessfulStartHandling();
                            }
                            break;

                        case CashierHandling.IsSuccessfulAndStartHandling:
                            devCashier.Cashier.SuccessfulHandling();

                            item = devCashier.Cashier.StartHandling();
                            writeProvider = new Server2CashierWriteDataProvider { InputData = item };
                            await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                            if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                            {
                             devCashier.Cashier.SuccessfulStartHandling();
                            }
                            break;

                        case CashierHandling.IsErrorAndStartHandling:
                            devCashier.Cashier.ErrorHandling();

                            item = devCashier.Cashier.StartHandling();
                            writeProvider = new Server2CashierWriteDataProvider { InputData = item };
                            await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                            if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                            {
                              devCashier.Cashier.SuccessfulStartHandling();
                            }
                            break;

                        default:
                            item = null;
                            break;
                    }
                }
            }

            //Отправка запроса синхронизации времени раз в час
            if (_lastSyncLabel != DateTime.Now.Hour)
            {
                _lastSyncLabel = DateTime.Now.Hour;

                var syncTimeProvider = new Server2CashierSyncTimeDataProvider();
                await port.DataExchangeAsync(_timeRespone, syncTimeProvider, ct);
            }
        }

        #endregion
    }
}