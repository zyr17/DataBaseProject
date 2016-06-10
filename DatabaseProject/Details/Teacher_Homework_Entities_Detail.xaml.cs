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
using System.Windows.Shapes;

namespace DatabaseProject
{
    /// <summary>
    /// Teacher_Homework_Entities_Detail.xaml 的交互逻辑
    /// </summary>
    public partial class Teacher_Homework_Entities_Detail : Window
    {
        int Homework_ID;
        public Teacher_Homework_Entities_Detail(int hid)
        {
            InitializeComponent();
            Homework_ID = hid;
            Update_Homework();
            Update_Homework_Entities_Teacher();
        }

        private void Update_Homework_Entities_Teacher()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT ID FROM homework_entities WHERE homeworks_ID = " + Homework_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow row in tb.Rows)
                        {
                            Homework_Entities_Teacher_Control hetctrl = new Homework_Entities_Teacher_Control();
                            hetctrl.Homework_Entities_ID = (int)row["ID"];
                            Homework_Entities_StackPanel.Children.Add(hetctrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }

        private void Update_Homework()
        {
            try
            {
                this.Title = "课程 ";
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT name FROM homeworks, courses WHERE courses_ID = courses.ID AND homeworks.ID = " + Homework_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        this.Title += (string)tb.Rows[0]["name"] + " 作业 ";
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homeworks WHERE ID = " + Homework_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        Homework_Title_TextBlock.Text = (string)tb.Rows[0]["title"];
                        this.Title += Homework_Title_TextBlock.Text + " 的详细信息";
                        Homework_Content_TextBlock.Text = (string)tb.Rows[0]["content"];
                        Homework_Score_TextBlock.Text = ((int)tb.Rows[0]["totalscore"]).ToString();
                        DateTime time = (DateTime)tb.Rows[0]["time"];
                        Homework_Time_TextBlock.Text = time == Consts.Unlimited_Time ? "无期限" : time.ToString("yyyyy/MM/dd hh:mm:ss");
                        Homework_Attachment.update_id = Homework_ID;
                        Homework_Attachment.db_table_string = "homework_attachments";
                        Homework_Attachment.Update_Attachment_List();
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }

        private void Edit_Homework_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    int course_id = -1;
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT courses_ID, title FROM homeworks WHERE ID = " + Homework_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        course_id = (int)tb.Rows[0]["courses_ID"];
                        Consts.t_id = Homework_ID;
                        Consts.title_register = (string)tb.Rows[0]["title"];
                        New_Homework newhomeworkform = new New_Homework(course_id, false);
                        newhomeworkform.ShowDialog();
                        if (Consts.t_id == -1) return;
                    }
                }
                Update_Homework();
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
    }
}
