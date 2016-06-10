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
    /// Homework_Entities_Control.xaml 的交互逻辑
    /// </summary>
    public partial class Homework_Entities_Control : UserControl
    {
        private int Homework_Status_int = 1;
        public Homework_Entities_Control()
        {
            InitializeComponent();
        }
        private void Outer_Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Inner_Grid.Height = Math.Max(0, Outer_Button.ActualHeight - Inner_Grid.Margin.Top - Inner_Grid.Margin.Bottom - 5);
            Inner_Grid.Width = Math.Max(0, Outer_Button.ActualWidth - Inner_Grid.Margin.Left - Inner_Grid.Margin.Right - 5);
            Homework_Detail_Grid.max_text_length = Inner_Grid.Width - 80;
            this.Homework_Entities_ID = heid;
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
        public int Homework_Status
        {
            get
            {
                return Homework_Status_int;
            }
            set
            {
                if (value < 0 || value > 2) throw new Exception();
                Homework_Status_int = value;
                if (value == 0)
                {
                    Homework_Status_TextBlock.Text = "未完成";
                    Homework_Status_TextBlock.Foreground = Brushes.Red;
                }
                else if (value == 1)
                {
                    Homework_Status_TextBlock.Text = "已完成";
                    Homework_Status_TextBlock.Foreground = Brushes.Green;
                }
                else
                {
                    Homework_Status_TextBlock.Text = "已返还";
                    Homework_Status_TextBlock.Foreground = Brushes.Blue;
                }
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
        private int heid = -1;
        public int Homework_Entities_ID
        {
            get
            {
                return heid;
            }
            set
            {
                if (value == -1) return;
                //System.Windows.Forms.MessageBox.Show(value.ToString() + "|" + max_text_length.ToString());
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT title, content, is_submitted, is_returned, time FROM homework_entities, homeworks WHERE homeworks_ID = homeworks.ID AND homework_entities.ID = " + value.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        Homework_Title = (string)tb.Rows[0]["title"];
                        Homework_Content = (string)tb.Rows[0]["content"];
                        Homework_Time = (System.DateTime)tb.Rows[0]["time"];
                        if ((bool)tb.Rows[0]["is_submitted"] == false) Homework_Status = 0;
                        else if ((bool)tb.Rows[0]["is_returned"] == false) Homework_Status = 1;
                        else if ((bool)tb.Rows[0]["is_returned"] == true) Homework_Status = 2;
                    }
                }
                heid = value;
            }
        }
        private void Outer_Button_Click(object sender, RoutedEventArgs e)
        {
            
            Do_Homework dhform = new Do_Homework(heid);
            if (Homework_Status_int == 2) dhform.Read_Mode();
            else dhform.Assign_Mode();
            dhform.ShowDialog();
            Homework_Entities_ID = heid;
        }
    }
}
