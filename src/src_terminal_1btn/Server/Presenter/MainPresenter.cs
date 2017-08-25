using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Communication.TcpIp;
using Server.Entitys;
using Server.Model;
using Server.View;


namespace Server.Presenter
{
    public class MainPresenter : IPresenter, IDisposable
    {
        #region field

        private readonly IMainForm _view;
        private readonly ServerModel _model;
        private readonly Task _mainTask;

        #endregion




        #region ctor

        public MainPresenter(IMainForm view)
        {
            _view = view;

            _model = new ServerModel();
            _model.PropertyChanged += _model_PropertyChanged;

            _model.LoadSetting();
            foreach (var cashier in _model.Сashiers)
            {
                cashier.PropertyChanged += Cashier_PropertyChanged;
            }

            if (_model.Listener != null)
            {
                _model.Listener.PropertyChanged += Listener_PropertyChanged;               
                _mainTask = _model.Start();

                _view.ServerModel = _model;//DEBUG
            }
        }

        #endregion




        #region EventHandler

        private async void Cashier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var c = sender as Сashier;
            if (c != null)
            {
                if (e.PropertyName == "CurrentTicket")
                {
                    if (c.CurrentTicket != null)      //добавить элемент к списку
                    {
                        _view.AddRow($"Талон {c.CurrentTicket.Prefix}{c.CurrentTicket.NumberElement.ToString("000")}", "Касса " + c.CurrentTicket.Сashbox);
                        var task = _model.LogTicket?.Add(c.CurrentTicket.ToString());
                        if (task != null) await task;
                    }
                    else                             //удалить элемент из списка
                    {
                        _view.RemoveRow(c.Id.ToString());
                    }
                }
            }
        }


        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var listener = sender as ListenerTcpIp;
            if (listener != null)
            {
                if (e.PropertyName == "IsConnect")
                {
                    _view.BackgroundColorDataGrid = listener.IsConnect ? Color.DimGray : Color.Brown;
                }
            }
        }


        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var server = sender as ServerModel;
            if (server != null)
            {
                if (e.PropertyName == "ErrorString")
                {
                    _view.ErrorString = server.ErrorString;
                }
            }
        }

        #endregion




        #region Methode

        public void Run()
        {
            _view.Show();
        }

        #endregion





        #region IDisposable

        public void Dispose()
        {
           _model.Dispose();
        }

        #endregion
    }
}