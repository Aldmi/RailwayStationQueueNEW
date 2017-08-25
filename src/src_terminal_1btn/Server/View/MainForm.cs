using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication.Interfaces;
using Communication.SerialPort;
using Communication.Settings;
using Communication.TcpIp;
using Library.Library;
using Library.Logs;
using Library.Xml;
using Server.Infrastructure;
using Server.Model;
using Server.Service;
using Server.Settings;
using Terminal.Infrastructure;


namespace Server.View
{
    public partial class MainForm : Form, IMainForm
    {
        #region prop

        delegate void EmptyMethodHandler();

        public ServerModel ServerModel { get; set; } //DEBUG

        public Color BackgroundColorDataGrid
        {
            set
            {
                dataGridView1.Invoke(new EmptyMethodHandler(() => { dataGridView1.BackgroundColor = value; }));
            }
        }

        public string ErrorString
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    MessageBox.Show(value);
            }
        }

        #endregion




        #region ctor

        public MainForm()
        {
            InitializeComponent();
        }

        #endregion




        #region Events



        #endregion




        #region Methods

        public new void Show()
        {
            Application.Run(this);
        }

        public void AddRow(string col1, string col2)
        {
            dataGridView1.Rows.Add(col1, col2);
        }

        public void RemoveRow(string id)
        {
            for (var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string val = dataGridView1.Rows[i].Cells[1].Value as string;
                if (val != null && val.Last().ToString() == id)
                {
                    dataGridView1.Rows.RemoveAt(i);
                }
            }
        }

        #endregion





        #region DEBUG

        private void btn_vilage_Click(object sender, EventArgs e)
        {
            var ticket = ServerModel.TicketFactoryVilage.Create((ushort)ServerModel.QueueVilage.Count);
            ServerModel.QueueVilage.Enqueue(ticket);
        }

        private void btn_long_Click(object sender, EventArgs e)
        {
            //var ticket = TicketFactoryLong.Create((ushort)QueueLong.Count);
            //QueueLong.Enqueue(ticket);
        }




        BlockClick blockClick1 = new BlockClick(1000);
        private void btn_V1_Add_Click(object sender, EventArgs e)
        {
            //if (blockClick1.IsClicHot) return;
            //blockClick1.BlockClickStart();

            ServerModel.Сashiers[0].StartHandling();
            ServerModel.Сashiers[0].SuccessfulStartHandling();
        }

        private void btn_V1_Sucsess_Click(object sender, EventArgs e)
        {
            ServerModel.Сashiers[0].SuccessfulHandling();
        }

        private void btn_V1_Error_Click(object sender, EventArgs e)
        {
            ServerModel.Сashiers[0].ErrorHandling();
        }


        private void btn_V2_Add_Click(object sender, EventArgs e)
        {
            ServerModel.Сashiers[1].StartHandling();
            ServerModel.Сashiers[1].SuccessfulStartHandling();
        }

        private void btn_V2_Sucsess_Click(object sender, EventArgs e)
        {
            ServerModel.Сashiers[1].SuccessfulHandling();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServerModel.Сashiers[1].ErrorHandling();
        }




        private void btn_V3_Add_Click(object sender, EventArgs e)
        {
            ServerModel.Сashiers[2].StartHandling();
            ServerModel.Сashiers[2].SuccessfulStartHandling();
        }

        private void btn_V3_Sucsess_Click(object sender, EventArgs e)
        {
            ServerModel.Сashiers[2].SuccessfulHandling();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ServerModel.Сashiers[2].ErrorHandling();
        }

        #endregion

    }
}
