using System;
using System.Data.SqlClient;
using System.Windows.Forms;

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
            SqlConnection con = new SqlConnection();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "."; // Server Name
            builder.InitialCatalog = "libs"; // Database Name
            builder.IntegratedSecurity = true;
            con.ConnectionString = builder.ConnectionString;
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Books.Name, Authors.LastName + ' ' + Authors.FirstName, Themes.Name, Categories.Name, Books.Pages, Books.YearPress, Press.Name, Books.Quantity FROM Books INNER JOIN Authors ON Books.Id_Author=Authors.Id INNER JOIN Themes ON Books.Id_Themes=Themes.Id INNER JOIN Categories ON Books.Id_Category=Categories.Id INNER JOIN Press ON Books.Id_Press=Press.Id", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                listView1.Items.Add(new ListViewItem(new string[] { reader[0].ToString(),
                                                                    reader[1].ToString(),
                                                                    reader[2].ToString(),
                                                                    reader[3].ToString(),
                                                                    reader[4].ToString(),
                                                                    reader[5].ToString(),
                                                                    reader[6].ToString(),
                                                                    reader[7].ToString()}));
            }
            reader.Close();
            con.Close();
        }
    }
}
