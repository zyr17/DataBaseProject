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
    /// Homework_Entities_Teacher_Control.xaml 的交互逻辑
    /// </summary>
    public partial class Homework_Entities_Teacher_Control : UserControl
    {
        private double max_text_length;
        private int Homework_Status_int;
        public Homework_Entities_Teacher_Control()
        {
            InitializeComponent();
        }

        private void Outer_Button_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Inner_Grid.Height = Math.Max(0, Outer_Button.ActualHeight - Inner_Grid.Margin.Top - Inner_Grid.Margin.Bottom - 5);
            Inner_Grid.Width = Math.Max(0, Outer_Button.ActualWidth - Inner_Grid.Margin.Left - Inner_Grid.Margin.Right - 5);
            max_text_length = Inner_Grid.Width - 80;
            this.Homework_Entities_ID = heid;
            //System.Windows.Forms.MessageBox.Show(Inner_Grid.ActualHeight.ToString() + "|" + Inner_Grid.ActualWidth.ToString());
        }

        public string Student_Name
        {
            get
            {
                return Student_Name_TextBlock.Text;
            }
            set
            {
                Student_Name_TextBlock.Text = value;
            }
        }

        public string Homework_Content
        {
            get
            {
                return Homework_Content_TextBlock.Text;
            }
            set
            {
                value = value.Replace(Environment.NewLine, " ");
                string s = "";
                if (Consts.MeasureTextWidth(value, Homework_Content_TextBlock.FontSize, "微软雅黑", Homework_Content_TextBlock.FontWeight) <= max_text_length)
                {
                    Homework_Content_TextBlock.Text = value;
                    return;
                }
                string s2 = "...";
                foreach (char i in value)
                {
                    s2 += i;
                    if (Consts.MeasureTextWidth(s2, Homework_Content_TextBlock.FontSize, "微软雅黑", Homework_Content_TextBlock.FontWeight) > max_text_length)
                    {
                        Homework_Content_TextBlock.Text = (s == "" ? value[0].ToString() : s) + "...";
                        return;
                    }
                    s += i;
                }
            }
        }

        public string Homework_Score
        {
            get
            {
                return Homework_Score_TextBlock.Text;
            }
            set
            {
                value = value.Replace(Environment.NewLine, " ");
                string s = "";
                if (Consts.MeasureTextWidth(value, Homework_Score_TextBlock.FontSize, "微软雅黑", Homework_Score_TextBlock.FontWeight) <= max_text_length)
                {
                    Homework_Score_TextBlock.Text = value;
                    return;
                }
                string s2 = "...";
                foreach (char i in value)
                {
                    s2 += i;
                    if (Consts.MeasureTextWidth(s2, Homework_Score_TextBlock.FontSize, "微软雅黑", Homework_Score_TextBlock.FontWeight) > max_text_length)
                    {
                        Homework_Score_TextBlock.Text = (s == "" ? value[0].ToString() : s) + "...";
                        return;
                    }
                    s += i;
                }
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
                    Homework_Status_TextBlock.Text = "未提交";
                    Homework_Status_TextBlock.Foreground = Brushes.Gray;
                }
                else if (value == 1)
                {
                    Homework_Status_TextBlock.Text = "未批改";
                    Homework_Status_TextBlock.Foreground = Brushes.Red;
                }
                else
                {
                    Homework_Status_TextBlock.Text = "已批改";
                    Homework_Status_TextBlock.Foreground = Brushes.Green;
                }
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
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT nickname, text, score, is_submitted, is_returned FROM homework_entities, students WHERE students_ID = students.ID AND homework_entities.ID = " + value.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        Student_Name = (string)tb.Rows[0]["nickname"];
                        Homework_Content = (string)tb.Rows[0]["text"];
                        Homework_Score = "得分：" + ((int)tb.Rows[0]["score"]).ToString();
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
            if (Homework_Status_int == 0)
            {
                System.Windows.Forms.MessageBox.Show("学生还未提交作业，无法批改！");
                return;
            }
            Do_Homework dhform = new Do_Homework(heid);
            if (Homework_Status_int == 2) dhform.Read_Mode();
            else dhform.Check_Mode();
            dhform.ShowDialog();
            Homework_Entities_ID = heid;
        }
    }
}
