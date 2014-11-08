using System;
using System.Data.Common;

//using System.Data.SqlClient;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

namespace ADO.NET_Connection_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var builder = new DbConnectionStringBuilder();
            builder.Add("Server", "localhost");
            builder.Add("Port", "3306");
            builder.Add("Uid", "root");
            builder.Add("Pwd", "idclipon");
            // Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;            
            using (var con = new MySql.Data.MySqlClient.MySqlConnection())
            {
                con.ConnectionString = builder.ConnectionString;
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "show databases";
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = -1;
                        while (reader.Read())
                        {
                            count = reader.FieldCount;
                            var fields = new List<string>(count);
                            for (int i = 0; i < count; ++i)
                            {
                                var o = reader[i];
                                var t = reader.GetFieldType(i);
                                if (t != typeof(string))
                                {
                                    o = Convert.ChangeType(o, typeof(string), Thread.CurrentThread.CurrentCulture);
                                }
                                string str = (string)o;
                                fields.Add(str);
                            }
                            listView1.Items.Add(new ListViewItem(fields.ToArray()));
                        }
                        listView1.Columns.Clear();
                        for (int i = 0; i < count; ++i)
                        {
                            string name = reader.GetName(i);
                            listView1.Columns.Add(name);
                        }
                        reader.Close();
                        listView1.AutoResizeColumns (ColumnHeaderAutoResizeStyle.ColumnContent);
                    }
                    con.Close();
                }
            }
        }
    }
}
