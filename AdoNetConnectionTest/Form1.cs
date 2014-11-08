using System;
using System.Configuration;
using System.Threading;
using System.Collections.Generic;
using System.Data.Common;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.IO;

namespace ADO.NET_Connection_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #if PROGRAM_FAILS_AND_DEBUG_IS_REQUIRED
        public static String GetAssemblyName(String typeName)
        {
            String assemblyName = null;
            Func<AssemblyName, Assembly> assemblyResolver = name => {
                assemblyName = name.FullName;
                return null;
            };

            var type = Type.GetType(typeName, assemblyResolver, null, false);
            if (type != null)
                return type.AssemblyQualifiedName;

            return assemblyName;
        }

        private void FillListViewWithProviders(DataTable table)
        {
            listView1.Items.Clear();
            listView1.Columns.Clear();
            for (int columnIndex = 0; columnIndex < table.Columns.Count; ++columnIndex)
            {
                string name = table.Columns[columnIndex].ColumnName;
                listView1.Columns.Add(name);
            }
            for (int rowIndex = 0; rowIndex < table.Rows.Count; ++rowIndex)
            {
                var fields = new List<string>(table.Columns.Count);
                for (int columnIndex = 0; columnIndex < table.Columns.Count; ++columnIndex)
                {
                    fields.Add(table.Rows[rowIndex][columnIndex].ToString());
                }        
                listView1.Items.Add(new ListViewItem(fields.ToArray()));
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void TryLoadAllAssemblies(DataTable table)
        {
            for (int rowIndex = 0; rowIndex < table.Rows.Count; ++rowIndex)
            {
                string name = (string)table.Rows[rowIndex]["AssemblyQualifiedName"];
                try
                {
                    var t = Type.GetType(name); //  GetType causes loading of the assembly specified in typeName.
                    if (t == null)
                    {
                        Trace.WriteLine("not found " + name);
                    }
                    
                }
                catch (FileNotFoundException)
                {
                    Trace.WriteLine("Assembly not found: " + name);
                }
            }
            Trace.WriteLine("Assembly check finished");
        }
        #endif
        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["MyConnectionString"];
                var connectionString = cs.ConnectionString;
                var providerName = cs.ProviderName;
                #if PROGRAM_FAILS_AND_DEBUG_IS_REQUIRED                
                var table = DbProviderFactories.GetFactoryClasses();
                Debug.Assert(table != null);
                FillListViewWithProviders(table);
                return;
                TryLoadAllAssemblies(table);
                return;
                #endif
                var factory = DbProviderFactories.GetFactory(providerName);
                using (var con = factory.CreateConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = ConfigurationManager.AppSettings["Query"];
                        using (var reader = cmd.ExecuteReader())
                        {
                            listView1.Items.Clear();
                            int columnsCount = -1;
                            while (reader.Read())
                            {
                                columnsCount = reader.FieldCount;
                                var fields = new List<string>(columnsCount);
                                for (int i = 0; i < columnsCount; ++i)
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
                            for (int i = 0; i < columnsCount; ++i)
                            {
                                string name = reader.GetName(i);
                                listView1.Columns.Add(name);
                            }
                            reader.Close();
                            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
