using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    public partial class Form1 : Form
    {
        public SqlConnection sqlCon;

        public List<String> GraphNames { get; set; } // Stored Procedure needs to split into two : one for names, and one that returns flags for each name

        public Form1()
        {
            InitializeComponent();

            sqlCon = this.MakeSQLConnection();

            GraphNames = this.GetGraphNames(sqlCon);

            //todo: getdata should actually happen when clicking on a specific graph name
        }

        public SqlConnection MakeSQLConnection()
        {
            var server = ConfigurationManager.AppSettings["SERVER"];
            var database = ConfigurationManager.AppSettings["DATABASE"];
            String strConnect = $"Server={server};Database={database};Trusted_Connection=True;";
            SqlConnection sqlCon = new SqlConnection(strConnect);
            sqlCon.Open();

            return sqlCon;

        }

        private List<String> GetGraphNames(SqlConnection sqlCon)
        {
            SqlCommand getAllGraphs = new SqlCommand("spGetGraphNames", sqlCon);
            getAllGraphs.CommandType = CommandType.StoredProcedure;
            getAllGraphs.ExecuteNonQuery();
            SqlDataAdapter da1 = new SqlDataAdapter(getAllGraphs);
            DataSet dataset1 = new DataSet();
            da1.Fill(dataset1, "Graphs");


            List<String> GraphNames = new List<String>();

            var nrGraphs = dataset1.Tables["Graphs"].Rows.Count;

            for (int row = 0; row < nrGraphs; ++row)
            {
                GraphNames.Add((String)dataset1.Tables["Graphs"].Rows[row].ItemArray[0]);
            }

            return GraphNames;
        
        }
    }
}
