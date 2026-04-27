using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Telinha.Views
{
    public partial class Sobre : Form
    {
        public Sobre()
        {
            InitializeComponent();
            Load += Sobre_Load;
        }

        private void Sobre_Load(object sender, EventArgs e)
        {
            var restSharp = Assembly.Load("RestSharp");
            var freeSQL = Assembly.Load("FreeSql");
            var mysqlConnector = Assembly.Load("FreeSql.Provider.MySqlConnector");
            var deeplNet = Assembly.Load("DeepLNet");

            RestSharpLabel.Text = $"RestSharp: {restSharp.GetName().Version}";
            FreeSQLLabel.Text = $"FreeSQL: {freeSQL.GetName().Version}";


        }
    }
}
