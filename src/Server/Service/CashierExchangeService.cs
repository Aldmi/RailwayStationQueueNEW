﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Communication.SerialPort;
using Library.Logs;
using Server.Actions;
using Server.Entitys;
using Server.Infrastructure;

namespace Server.Service
{
    public class CashierExchangeService
    {
        #region field

        private readonly ActionQueue _actionCashierQueue;
        private readonly List<DeviceCashier> _deviceCashiers;
        private readonly DeviceCashier _adminCashier;
        private readonly ushort _timeRespone;
        private int _lastSyncLabel;
        private readonly string _logName;
        private readonly Log _loggerCashierInfo;

        #endregion




        #region ctor

        public CashierExchangeService(ActionQueue actionCashierQueue, List<DeviceCashier> deviceCashiers, DeviceCashier adminCashier, ushort timeRespone, string logName)
        {
            _actionCashierQueue = actionCashierQueue;
            _deviceCashiers = deviceCashiers;
            _adminCashier = adminCashier;
            _timeRespone = timeRespone;
            _logName = logName;
            _loggerCashierInfo = new Log(_logName);
        }

        #endregion




        #region Methode

        public async Task ExchangeService(MasterSerialPort port, CancellationToken ct)
        {
            if (port == null)
                return;

            try
            {
                foreach (var devCashier in _deviceCashiers)              //Запуск опроса кассиров
                {
                    _loggerCashierInfo.Info($"---------------------------КАССИР: Id= {devCashier.Cashier.Id}   CurrentTicket= {(devCashier.Cashier.CurrentTicket != null ? devCashier.Cashier.CurrentTicket.Prefix + devCashier.Cashier.CurrentTicket.NumberElement.ToString("000") : "НЕТ")}----------------------------------");//LOG;

                    var readProvider = new Server2CashierReadDataProvider(devCashier.AddresDevice, _logName);
                    devCashier.DataExchangeSuccess = await port.DataExchangeAsync(_timeRespone, readProvider, ct);

                    if (!devCashier.IsConnect)
                    {
                        _loggerCashierInfo.Info($"кассир НЕ на связи: Id= {devCashier.Cashier.Id}");//LOG;
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
                            var syncTimeProvider = new Server2CashierSyncTimeDataProvider(_logName);
                            await port.DataExchangeAsync(_timeRespone, syncTimeProvider, ct);
                        }

                        //TODO: проверить
                        if (!cashierInfo.IsWork)
                        {
                            //Если кассир быстро закрыла сессию (до того как опрос порта дошел до нее), то билет из обработки надо убрать.
                            if (devCashier.Cashier.CurrentTicket != null)
                            {
                                _loggerCashierInfo.Info($"Команда от кассира: Id= {devCashier.Cashier.Id}   Handling=\"Если кассир быстро закрыла сессию(до того как опрос порта дошел до нее). НО У НЕЕ БЫЛ ТЕКУЩИЙ ОБРАБАТЫВАЕМЫЙ БИЛЕТ\"    NameTicket= {cashierInfo.NameTicket}");//LOG;
                                //devCashier.Cashier.SuccessfulHandling();
                            }
                            continue;
                        }

                        Func<CancellationToken, Task> cashierAct;
                        switch (cashierInfo.Handling)
                        {
                            case CashierHandling.IsSuccessfulHandling:
                                if(!devCashier.Cashier.CanHandling)
                                    break;

                                cashierAct = async (ctQueue) =>
                                {
                                    devCashier.Cashier.SuccessfulHandling();
                                    await Task.CompletedTask;
                                };
                                var act = new ActionFromCashier(cashierAct);           //создаем действие
                                _actionCashierQueue.Enqueue(act);                      //помещаем в очередь действий
                                await act.MarkerEndAction();                           //дожидаемся завершения действия на очереди
                                break;


                            case CashierHandling.IsErrorHandling:
                                if (!devCashier.Cashier.CanHandling)
                                    break;
                                cashierAct = async (ctQueue) =>
                                {
                                    devCashier.Cashier.ErrorHandling();
                                    await Task.CompletedTask;
                                };
                                act = new ActionFromCashier(cashierAct);
                                _actionCashierQueue.Enqueue(act);
                                await act.MarkerEndAction();
                                break;


                            case CashierHandling.IsStartHandling:
                                cashierAct = async (ctQueue) =>
                                {
                                    item = devCashier.Cashier.StartHandling();
                                    if (item == null)
                                        return;

                                    var writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
                                    await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                                    if (writeProvider.IsOutDataValid)                //завершение транзакции (успешная передача билета кассиру)
                                    {
                                        devCashier.Cashier.SuccessfulStartHandling();
                                    }
                                };
                                act = new ActionFromCashier(cashierAct);
                                _actionCashierQueue.Enqueue(act);
                                await act.MarkerEndAction();
                                break;


                            case CashierHandling.IsRedirectHandling:
                                if (_adminCashier != null)
                                {
                                    if (!devCashier.Cashier.CanHandling)
                                        break;
                                    cashierAct = async (ctQueue) =>
                                    {
                                        var redirectTicket = devCashier.Cashier.CurrentTicket;
                                        if (redirectTicket != null)
                                        {
                                            _adminCashier.Cashier.AddRedirectedTicket(redirectTicket);
                                        }
                                        devCashier.Cashier.SuccessfulHandling();
                                        await Task.CompletedTask;
                                    };
                                    act = new ActionFromCashier(cashierAct);
                                    _actionCashierQueue.Enqueue(act);
                                    await act.MarkerEndAction();
                                }
                                break;


                            case CashierHandling.IsSuccessfulAndStartHandling:
                                cashierAct = async (ctQueue) =>
                                {
                                    devCashier.Cashier.SuccessfulHandling();
                                    item = devCashier.Cashier.StartHandling();
                                    if (item == null)
                                        return;

                                    var writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
                                    await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                                    if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                                    {
                                        devCashier.Cashier.SuccessfulStartHandling();
                                    }
                                };
                                act = new ActionFromCashier(cashierAct);
                                _actionCashierQueue.Enqueue(act);
                                await act.MarkerEndAction();
                                break;


                            case CashierHandling.IsRedirectAndStartHandling:
                                if (_adminCashier != null)
                                {
                                    cashierAct = async (ctQueue) =>
                                    {
                                        var redirectTicket = devCashier.Cashier.CurrentTicket;
                                        if (redirectTicket != null)
                                        {
                                            _adminCashier.Cashier.AddRedirectedTicket(redirectTicket);
                                        }
                                        devCashier.Cashier.SuccessfulHandling();

                                        item = devCashier.Cashier.StartHandling();
                                        if (item == null)
                                           return;

                                       var writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
                                        await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                                        if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                                        {
                                            devCashier.Cashier.SuccessfulStartHandling();
                                        }
                                    };
                                    act = new ActionFromCashier(cashierAct);
                                    _actionCashierQueue.Enqueue(act);
                                    await act.MarkerEndAction();
                                }
                                break;


                            case CashierHandling.IsErrorAndStartHandling:
                                cashierAct = async (ctQueue) =>
                                {
                                    devCashier.Cashier.ErrorHandling();
                                    item = devCashier.Cashier.StartHandling();
                                    if (item == null)
                                      return;

                                    var writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
                                    await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                                    if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                                    {
                                        devCashier.Cashier.SuccessfulStartHandling();
                                    }
                                };
                                act = new ActionFromCashier(cashierAct);
                                _actionCashierQueue.Enqueue(act);
                                await act.MarkerEndAction();
                                break;


                            default:
                                item = null;
                                break;
                        }


                        //switch (cashierInfo.Handling)
                        //{
                        //    case CashierHandling.IsSuccessfulHandling:
                        //        //DEBUG--------------------------------------------------------------------
                        //        Func<CancellationToken, Task> cashierAct = async (ctQueue) =>
                        //        {
                        //           // await devCashier.Cashier.SuccessfulHandlingAsync(ctQueue);  //TODO: все методы переделать на возврат Task
                        //            devCashier.Cashier.SuccessfulHandling();
                        //            await Task.CompletedTask;
                        //        };
                        //        var act = new ActionFromCashier(CashierHandling.IsSuccessfulHandling, cashierAct);
                        //        _actionCashierQueue.Enqueue(act);
                        //        var exception= await act.MarkerEndAction();
                        //        //-------------------------------------------------------------------------

                        //        devCashier.Cashier.SuccessfulHandling();
                        //        break;

                        //    case CashierHandling.IsErrorHandling:
                        //        devCashier.Cashier.ErrorHandling();
                        //        break;

                        //    case CashierHandling.IsStartHandling:
                        //        item = devCashier.Cashier.StartHandling();
                        //        if(item == null)
                        //            break;

                        //        var writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
                        //        await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                        //        if (writeProvider.IsOutDataValid)                //завершение транзакции (успешная передача билета кассиру)
                        //        {
                        //            devCashier.Cashier.SuccessfulStartHandling();
                        //        }
                        //        break;

                        //    case CashierHandling.IsRedirectHandling:
                        //        if (_adminCashier != null)
                        //        {
                        //            var redirectTicket = devCashier.Cashier.CurrentTicket;
                        //            if (redirectTicket != null)
                        //            {
                        //                _adminCashier.Cashier.AddRedirectedTicket(redirectTicket);
                        //            }
                        //            devCashier.Cashier.SuccessfulHandling();
                        //        }
                        //        break;

                        //    case CashierHandling.IsSuccessfulAndStartHandling:
                        //        devCashier.Cashier.SuccessfulHandling();
                        //        item = devCashier.Cashier.StartHandling();
                        //        if (item == null)
                        //            break;

                        //        writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
                        //        await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                        //        if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                        //        {
                        //            devCashier.Cashier.SuccessfulStartHandling();
                        //        }
                        //        break;

                        //    case CashierHandling.IsRedirectAndStartHandling:
                        //        if (_adminCashier != null)
                        //        {
                        //            var redirectTicket = devCashier.Cashier.CurrentTicket;
                        //            if (redirectTicket != null)
                        //            {
                        //                _adminCashier.Cashier.AddRedirectedTicket(redirectTicket);
                        //            }
                        //            devCashier.Cashier.SuccessfulHandling();

                        //            item = devCashier.Cashier.StartHandling();
                        //            if (item == null)
                        //                break;

                        //            writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
                        //            await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                        //            if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                        //            {
                        //                 devCashier.Cashier.SuccessfulStartHandling();
                        //            }
                        //        }
                        //        break;

                        //    case CashierHandling.IsErrorAndStartHandling:
                        //        devCashier.Cashier.ErrorHandling();
                        //        item = devCashier.Cashier.StartHandling();
                        //        if (item == null)
                        //            break;

                        //        writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
                        //        await port.DataExchangeAsync(_timeRespone, writeProvider, ct);
                        //        if (writeProvider.IsOutDataValid)                //завершение транзакции ( успешная передача билета кассиру)
                        //        {
                        //            devCashier.Cashier.SuccessfulStartHandling();
                        //        }
                        //        break;

                        //    default:
                        //        item = null;
                        //        break;
                        //}            
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerCashierInfo.Info($"EXCEPTION CashierExchangeService:   {ex.ToString()}");
            }
        }

        #endregion
    }
}