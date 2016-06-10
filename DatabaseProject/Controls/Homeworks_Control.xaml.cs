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
    /// Homeworks_Control.xaml 的交互逻辑
    /// </summary>
    public partial class Homeworks_Control : UserControl
    {
        public Homeworks_Control()
        {
            InitializeComponent();
        }
        private void Outer_Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Inner_Grid.Height = Math.Max(0, Outer_Button.ActualHeight - Inner_Grid.Margin.Top - Inner_Grid.Margin.Bottom - 5);
            Inner_Grid.Width = Math.Max(0, Outer_Button.ActualWidth - Inner_Grid.Margin.Left - Inner_Grid.Margin.Right - 5);
            Homework_Detail_Grid.max_text_length = Inner_Grid.Width;
            this.Homeworks_ID = hid;
            //System.Windows.Forms.MessageBox.Show(Inner_Grid.ActualHeight.ToString() + "|" + Inner_Grid.ActualWidth.ToString());
        }

        public string Homework_Title
        {
            get
            {
                return Homework_Detail_Grid.Homework_Title;
            }
            set
            {
                Homework_Detail_Grid.Homework_Title = value;
            }
        }
        public string Homework_Content
        {
            get
            {
                return Homework_Detail_Grid.Homework_Content;
            }
            set
            {
                Homework_Detail_Grid.Homework_Content = value;
            }
        }
        public System.DateTime Homework_Time
        {
            get
            {
                return Homework_Detail_Grid.Homework_Time;
            }
            set
            {
                Homework_Detail_Grid.Homework_Time = value;
            }
        }
        private int hid = -1;
        public int Homeworks_ID
        {
            get
            {
                return hid;
            }
            set
            {
                if (value == -1) return;
                //System.Windows.Forms.MessageBox.Show(value.ToString() + "|" + max_text_length.ToString());
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT title, content, time FROM homeworks WHERE ID = " + value.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        Homework_Title = (string)tb.Rows[0]["title"];
                        Homework_Content = (string)tb.Rows[0]["content"];
                        Homework_Time = (System.DateTime)tb.Rows[0]["time"];
                    }
                }
                hid = value;
            }
        }
        private void Outer_Button_Click(object sender, RoutedEventArgs e)
        {
            Teacher_Homework_Entities_Detail form = new Teacher_Homework_Entities_Detail(hid);
            form.ShowDialog();
            Homeworks_ID = hid;
        }
    }
}
