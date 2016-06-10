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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DatabaseProject
{
    /// <summary>
    /// Do_Homework.xaml 的交互逻辑
    /// </summary>
    public partial class Do_Homework : Window
    {
        public int Homework_Entity_ID;
        private int Edit_Mode;
        public Do_Homework(int heid)
        {
            InitializeComponent();
            Homework_Attachments.Can_Edit = false;
            Homework_Entity_ID = heid;
            Update_Data();
        }

        private void Update_Data()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT homeworks.ID AS hid, nickname, name, title, content, time, totalscore, score, text, comment FROM teachers, courses, homeworks, homework_entities WHERE teachers_ID = teachers.ID AND courses_ID = courses.ID AND homeworks_ID = homeworks.ID AND homework_entities.ID = " + Homework_Entity_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        DataRow i = tb.Rows[0];
                        Course_Title_TextBlock.Text = (string)i["name"];
                        Teacher_Name_TextBlock.Text = (string)i["nickname"];
                        Homework_Title_TextBlock.Text = (string)i["title"];
                        Homework_Content_TextBox.Text = (string)i["content"];
                        Teacher_Answer_TextBox.Text = (string)i["comment"];
                        Totalscore_TextBlock.Text = ((int)i["totalscore"]).ToString();
                        Score_TextBox.Text = ((int)i["score"]).ToString();
                        DateTime ttt = (DateTime)i["time"];
                        Time_TextBlock.Text = ttt == Consts.Unlimited_Time ? "无期限" : ttt.ToString("yyyy/MM/dd hh:mm:ss");
                        Homework_Entity_Text_TextBlock.Text = (string)i["text"];
                        Homework_Attachments.update_id = (int)i["hid"];
                        Homework_Attachments.db_table_string = "homework_attachments";
                        Homework_Attachments.Update_Attachment_List();
                        Homework_Entity_Attachments.update_id = Homework_Entity_ID;
                        Homework_Entity_Attachments.db_table_string = "homework_entity_attachments";
                        Homework_Entity_Attachments.Update_Attachment_List();
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }

        public void Assign_Mode()
        {
            Edit_Mode = 0;
            this.Title = "提交作业";
            Submit_Button.Content = "确认提交";
            Score_TextBox.IsReadOnly = true;
            Teacher_Answer_CheckBox.IsChecked = false;
            Teacher_Answer_CheckBox.IsEnabled = false;
        }

        public void Check_Mode()
        {
            Edit_Mode = 1;
            this.Title = "批改作业";
            Submit_Button.Content = "确认批改";
            Homework_Entity_Text_TextBlock.IsReadOnly = true;
            Homework_Entity_Attachments.Can_Edit = false;
        }

        public void Read_Mode()
        {
            Edit_Mode = 2;
            this.Title = "查看作业";
            Submit_Button.Content = "关闭";
            Score_TextBox.IsReadOnly = true;
            Homework_Entity_Text_TextBlock.IsReadOnly = true;
            Homework_Entity_Attachments.Can_Edit = false;
            Teacher_Answer_TextBox.IsReadOnly = true;
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (Student_Answer_Grid != null) //Student_Answer_Grid.Visibility = (bool)Student_Answer_CheckBox.IsChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                Student_Answer_Grid.Height = (bool)Student_Answer_CheckBox.IsChecked ? 280 : 0;
            if (Teacher_Answer_ScrollViewer != null) //Teacher_Answer_TextBox.Visibility = (bool)Teacher_Answer_CheckBox.IsChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                Teacher_Answer_ScrollViewer.Height = (bool)Teacher_Answer_CheckBox.IsChecked ? 200 : 0;
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Edit_Mode == 2)
            {
                this.Close();
                return;
            }
            int score;
            try
            {
                score = Convert.ToInt32(Score_TextBox.Text);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("分数格式错误！");
                return;
            }
            MessageBoxButtons btn = MessageBoxButtons.YesNo;
            if (System.Windows.Forms.MessageBox.Show(Edit_Mode == 1 ? "你确认要批改吗？这次作业的得分为 " + Score_TextBox.Text + "/" + Totalscore_TextBlock.Text + " ，一经确认无法撤销！" : "你确认要提交吗？老师一旦批改将无法再修改！", "警告", btn) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                    {
                        sqlcon.Open();
                        if (Edit_Mode == 0)
                        {
                            using (SqlCommand cmd = new SqlCommand("UPDATE homework_entities SET text = @text, is_submitted = 1 WHERE ID = " + Homework_Entity_ID.ToString(), sqlcon))
                            {
                                cmd.Parameters.AddWithValue("@text", Homework_Entity_Text_TextBlock.Text);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("UPDATE homework_entities SET score = @score, comment = @comment, is_returned = 1 WHERE ID = " + Homework_Entity_ID.ToString(), sqlcon))
                            {
                                cmd.Parameters.AddWithValue("@score", score);
                                cmd.Parameters.AddWithValue("@comment", Teacher_Answer_TextBox.Text);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        System.Windows.Forms.MessageBox.Show("操作成功！");
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    Consts.Output_Database_Err(ex);
                }
            }
        }
    }
}
