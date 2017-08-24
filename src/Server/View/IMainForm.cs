using System;
using System.Drawing;
using Server.Model;

namespace Server.View
{
    public interface IMainForm : IView
    {
        ServerModel ServerModel { set; } //DEBUG

        Color BackgroundColorDataGrid { set; }
        string ErrorString {set; }

        void AddRow(string col1, string col2);
        void RemoveRow(string id);
    }
}