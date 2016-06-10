using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseProject
{
    /// <summary>
    /// Homework_And_Weight.xaml 的交互逻辑
    /// </summary>
    public partial class Homework_And_Weight_Control : UserControl
    {
        public int Homework_ID;
        public Homework_And_Weight_Control(int hid)
        {
            InitializeComponent();
            Homework_ID = hid;
            Update_Data();
        }
        void Update_Data()
        {
            Detail_Grid.max_text_length = Main_Grid.ActualWidth - 160;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homeworks WHERE ID = " + Homework_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        Detail_Grid.Homework_Title = (string)tb.Rows[0]["title"];
                        Detail_Grid.Homework_Content = (string)tb.Rows[0]["content"];
                        Detail_Grid.Homework_Time = (DateTime)tb.Rows[0]["time"];
                        Totalscore_TextBlock.Text = ((int)tb.Rows[0]["totalscore"]).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }

        public string weight_string
        {
            get
            {
                return Weight_TextBox.Text;
            }
            set
            {
                Weight_TextBox.Text = value;
            }
        }

        public int homework_totalscore
        {
            get
            {
                return Convert.ToInt32(Totalscore_TextBlock.Text);
            }
        }

        private void Main_Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update_Data();
        }
    }
}
