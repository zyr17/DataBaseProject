using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DatabaseProject
{
    /// <summary>
    /// New_Homework.xaml 的交互逻辑
    /// </summary>
    public partial class New_Homework : Window
    {
        private static DateTime Max_DateTime = new DateTime(2999, 12, 31, 23, 59, 59);
        private string Last_Correct_Input_Time = "00:00:00";
        private int update_id = -1;
        private bool is_new_homework = false;
        private int Course_ID;
        public New_Homework(int c_id, bool Is_New_Homework)
        {
            InitializeComponent();
            is_new_homework = Is_New_Homework;
            Course_ID = c_id;
            this.Title = "为课程 " + Consts.title_register + " ";
            if (is_new_homework) this.Title += "添加新作业";
            else this.Title = "修改作业";
            Date_DatePicker.DisplayDateEnd = Max_DateTime;
            Update_Data(Consts.t_id);
            Consts.t_id = -1;
        }
        private void Update_Data(int hid){
            update_id = Attachment_Control_Grid.update_id = hid;
            Attachment_Control_Grid.db_table_string = "homework_attachments";
            Attachment_Control_Grid.Update_Attachment_List();
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homeworks WHERE ID = " + hid.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        Homework_Title_TextBox.Text = (string)tb.Rows[0]["title"];
                        Homework_Content_TextBox.Text = (string)tb.Rows[0]["content"];
                        Homework_Score_TextBox.Text = ((int)tb.Rows[0]["totalscore"]).ToString();
                        DateTime dt = (DateTime)tb.Rows[0]["time"];
                        if (dt.Equals(Consts.Unlimited_Time))
                        {
                            Unlimited_CheckBox.IsChecked = true;
                        }
                        else
                        {
                            Date_DatePicker.Text = dt.ToString("yyyy/MM/dd");
                            Time_TextBlock.Text = Last_Correct_Input_Time = dt.ToString("HH:mm:ss");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
        private void Time_TextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                DateTime dt = DateTime.Parse("2000/1/1 " + Time_TextBlock.Text);
                Last_Correct_Input_Time = dt.ToString("HH:mm:ss");
            }
            catch
            {

            }
        }
        private void Time_TextBlock_LostFocus(object sender, RoutedEventArgs e)
        {
            Time_TextBlock.Text = Last_Correct_Input_Time;
        }
        private void Post_Homework_Button_Click(object sender, RoutedEventArgs e)
        {
            int score = -1;
            try
            {
                score = Convert.ToInt32(Homework_Score_TextBox.Text);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("作业总分格式错误！");
                Homework_Score_TextBox.Focus();
                return;
            }
            if (score < 1)
            {
                System.Windows.Forms.MessageBox.Show("作业总分不得小于1！");
                return;
            }
            try
            {
                if (!(bool)Unlimited_CheckBox.IsChecked)
                {
                    DateTime ttt = Convert.ToDateTime(Date_DatePicker.Text + " " + Time_TextBlock.Text);
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("日期输入错误！");
                return;
            }
            string errstring = "";
            if (Homework_Title_TextBox.Text == "") errstring = "作业标题";
            if (Homework_Content_TextBox.Text == "") errstring += (errstring == "" ? "" : "及") + "作业内容";
            MessageBoxButtons btn = MessageBoxButtons.YesNo;
            if (errstring == "" || System.Windows.Forms.MessageBox.Show(errstring + "为空！确认什么都不填写吗？", "警告", btn) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                    {
                        sqlcon.Open();
                        string timestring = Consts.Unlimited_Time.ToString();
                        if (!(bool)Unlimited_CheckBox.IsChecked) timestring = Date_DatePicker.Text + " " + Time_TextBlock.Text;
                        using (SqlCommand cmd = new SqlCommand("UPDATE homeworks SET title = @title, content = @content, time = @time, totalscore = @score WHERE ID = " + update_id.ToString(), sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@title", Homework_Title_TextBox.Text);
                            cmd.Parameters.AddWithValue("@content", Homework_Content_TextBox.Text);
                            cmd.Parameters.AddWithValue("@time", Convert.ToDateTime(timestring));
                            cmd.Parameters.AddWithValue("@score", score);
                            cmd.ExecuteNonQuery();
                        }
                        Consts.t_id = update_id;
                        if (is_new_homework) System.Windows.Forms.MessageBox.Show("添加作业成功！");
                        else System.Windows.Forms.MessageBox.Show("更新作业成功！");
                    }
                    this.Close();
                }
                catch (Exception ex)
                {
                    Consts.Output_Database_Err(ex);
                }
            }
        }
        private void Unlimited_CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            Time_TextBlock.IsEnabled = Date_DatePicker.IsEnabled = !(bool)Unlimited_CheckBox.IsChecked;
        }
    }
}
