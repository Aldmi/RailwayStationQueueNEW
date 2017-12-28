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
        private readonly DeviceCashier _adminCashier;
        private readonly ushort _timeRespone;

        private int _lastSyncLabel;

        #endregion




        #region ctor

        public CashierExchangeService(List<DeviceCashier> deviceCashiers, DeviceCashier adminCashier, ushort timeRespone)
        {
            _deviceCashiers = deviceCashiers;
            _adminCashier = adminCashier;
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
                var readProvider = new Server2CashierReadDataProvider(devCashier.AddresDevice);
                devCashier.DataExchangeSuccess = await port.DataExchangeAsync(_timeRespone, readProvider, ct);

                if (!devCashier.IsConnect)
                {
                    devCashier.LastSyncLabel = 0;
                    continue;
                }

                if (readProvider.IsOutDataValid)
                {
                    TicketItem item;
                    var cashierInfo = readProvider.OutputData;

                    //Если устойство было не на связи, то Отправка запроса синхронизации времени раз в час, будет произведенна мгновенно.
                    if (devCashier.LastSyncLabel != DateTime.Now.Hour)
                    {
                        devCashier.LastSyncLabel = DateTime.Now.Hour;
                        var syncTimeProvider = new Server2CashierSyncTimeDataProvider();
                        await port.DataExchangeAsync(_timeRespone, syncTimeProvider, ct);
                    }

                    if (!cashierInfo.IsWork)
                    {
                        //Если кассир быстро закрыла сессию (до того как опрос порта дошел до нее), то билет из обработки надо убрать.
                        if (devCashier.Cashier.CurrentTicket != null)
                            devCashier.Cashier.SuccessfulHandling();

                        continue;
                    }

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
                            var writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice) { InputData = item };
                            await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                            if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                            {
                                devCashier.Cashier.SuccessfulStartHandling();
                            }
                            break;

                        case CashierHandling.IsRedirectHandling:
                            if (_adminCashier != null)
                            {
                                var redirectTicket = devCashier.Cashier.CurrentTicket;
                                if (redirectTicket != null)
                                {
                                    _adminCashier.Cashier.AddRedirectedTicket(redirectTicket);
                                }
                                devCashier.Cashier.SuccessfulHandling();
                            }
                            break;

                        case CashierHandling.IsSuccessfulAndStartHandling:
                            devCashier.Cashier.SuccessfulHandling();
                            item = devCashier.Cashier.StartHandling();
                            writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice) { InputData = item };
                            await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                            if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                            {
                                devCashier.Cashier.SuccessfulStartHandling();
                            }
                            break;

                        case CashierHandling.IsRedirectAndStartHandling:
                            if (_adminCashier != null)
                            {
                                var redirectTicket = devCashier.Cashier.CurrentTicket;
                                if (redirectTicket != null)
                                {
                                    _adminCashier.Cashier.AddRedirectedTicket(redirectTicket);
                                }
                                devCashier.Cashier.SuccessfulHandling();

                                item = devCashier.Cashier.StartHandling();
                                writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice) { InputData = item };
                                await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                                if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                                {
                                    devCashier.Cashier.SuccessfulStartHandling();
                                }
                            }
                            break;

                        case CashierHandling.IsErrorAndStartHandling:
                            devCashier.Cashier.ErrorHandling();
                            item = devCashier.Cashier.StartHandling();
                            writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice) { InputData = item };
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
        }

        #endregion
    }
}