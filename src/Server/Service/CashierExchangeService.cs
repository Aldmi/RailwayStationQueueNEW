﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Communication.SerialPort;
using Library.Logs;
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
        private readonly string _logName;
        private readonly Log _loggerCashierInfo;

        #endregion




        #region ctor

        public CashierExchangeService(List<DeviceCashier> deviceCashiers, DeviceCashier adminCashier, ushort timeRespone, string logName)
        {
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

                        switch (cashierInfo.Handling)
                        {
                            case CashierHandling.IsSuccessfulHandling:
                                if (!devCashier.Cashier.CanHandling)
                                    break;
                                devCashier.Cashier.SuccessfulHandling();
                                break;


                            case CashierHandling.IsErrorHandling:
                                if (!devCashier.Cashier.CanHandling)
                                    break;
                                devCashier.Cashier.ErrorHandling();
                                break;


                            case CashierHandling.IsStartHandling:
                                await WriteTicketTransaction(devCashier, cashierInfo.Handling, port, ct);
                                break;


                            case CashierHandling.IsSuccessfulAndStartHandling:
                                devCashier.Cashier.SuccessfulHandling();
                                await WriteTicketTransaction(devCashier, cashierInfo.Handling, port, ct);
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
                                    await WriteTicketTransaction(devCashier, cashierInfo.Handling, port, ct);
                                }
                                break;


                            case CashierHandling.IsErrorAndStartHandling:
                                devCashier.Cashier.ErrorHandling();
                                await WriteTicketTransaction(devCashier, cashierInfo.Handling, port, ct);
                                break;


                            case CashierHandling.IsRedirectHandling:
                                if (_adminCashier != null)
                                {
                                    if (!devCashier.Cashier.CanHandling)
                                        break;
                                    var redirectTicket = devCashier.Cashier.CurrentTicket;
                                    if (redirectTicket != null)
                                    {
                                        _adminCashier.Cashier.AddRedirectedTicket(redirectTicket);
                                    }
                                    devCashier.Cashier.SuccessfulHandling();
                                }
                                break;

                            default:
                                item = null;
                                break;
                        }            
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerCashierInfo.Info($"EXCEPTION CashierExchangeService:   {ex.ToString()}");
            }
        }


        /// <summary>
        /// Транзакция на передачу билета кассиру.
        /// </summary>
        private async Task<bool> WriteTicketTransaction(DeviceCashier devCashier, CashierHandling handling, MasterSerialPort port, CancellationToken ct)
        {
            var item = devCashier.Cashier.StartHandling();         
            if (item == null)                         //БЛОКИРОВКА ТРАНЗАКЦИИ (билет уже используется в транзакции)
                return false;

            var writeProvider = new Server2CashierWriteDataProvider(devCashier.AddresDevice, _logName) { InputData = item };
            await port.DataExchangeAsync(_timeRespone, writeProvider, ct);        
            if (writeProvider.IsOutDataValid)        //ТРАНЗАКЦИЯ ЗАВЕРШЕНА УСПЕШНО  (извлечем билет из очереди и присвоим кассиру)         
            {
                devCashier.Cashier.StartHandlingSuccessful();
            }
            else                                     //ТРАНЗАКЦИЯ ЗАВЕРШЕНА С ОШИБКОЙ (снимем блокировку)
            {
                _loggerCashierInfo.Info($"НЕ УДАЧНАЯ ТРАНЗАКЦИЯ ПЕРЕДАЧИ БИЛЕТА КАССИРУ: Id= {devCashier.Cashier.Id}   НА КОМАНДУ={handling}");
                devCashier.Cashier.StartHandlingFaild();
                return false;
            }          
            return true;
        }


        #endregion
    }
}