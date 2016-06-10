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
    /// Course_Detail.xaml 的交互逻辑
    /// </summary>
    public partial class Course_Detail : Window
    {
        int Course_ID = -1;
        int student_num = 0;
        List<string> Student_Name_List = new List<string>();
        public Course_Detail(int c_id)
        {
            InitializeComponent();
            Course_ID = c_id;
            Update_Student_List_And_Title();
            Update_Homeworks();
        }
        private void Update_Student_List_And_Title()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT COUNT(students_ID) AS mm FROM students_has_courses WHERE courses_ID = " + Course_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        if (tb.Rows.Count > 0)
                            student_num = (int)tb.Rows[0]["mm"];
                    }
                    Student_Num_TextBlock.Text = "学生数量：" + student_num.ToString() + "人";
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT nickname FROM students, students_has_courses WHERE students.ID = students_ID AND courses_ID = " + Course_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        Student_Name_List.Clear();
                        foreach (DataRow i in tb.Rows)
                            Student_Name_List.Add((string)i["nickname"]);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT name FROM courses WHERE ID = " + Course_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        Title_TextBlock.Text = (string)tb.Rows[0]["name"];
                        this.Title = "课程：" + Title_TextBlock.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
            Student_Name_ListBox.ItemsSource = Student_Name_List;
        }
        private void Update_Homeworks()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    Homeworks_StackPanel.Children.Clear();
                    string cmdstring = "SELECT ID FROM homeworks WHERE courses_ID = " + Course_ID.ToString();
                    if (Homework_Search_TextBox.Text != "") cmdstring += "AND CHARINDEX(@title, title) <> 0";
                    cmdstring += " ORDER BY time DESC";
                    using (SqlDataAdapter adapter1 = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand(cmdstring, sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@title", Homework_Search_TextBox.Text);
                            adapter1.SelectCommand = cmd;
                            DataTable tb = new DataTable();
                            adapter1.Fill(tb);
                            foreach (DataRow rr in tb.Rows)
                            {
                                Homeworks_Control hctrl = new Homeworks_Control();
                                Homeworks_StackPanel.Children.Add(hctrl);
                                hctrl.Homeworks_ID = (int)rr["ID"];
                            }
                            if (tb.Rows.Count == 0)
                            {
                                TextBlock text = new TextBlock();
                                text.Text = Homework_Search_TextBox.Text == "" ? "这门课还没有作业" : "无结果";
                                text.Margin = new System.Windows.Thickness(0, 20, 0, 20);
                                text.Foreground = Brushes.Black;
                                text.FontSize = 18;
                                text.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                                text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                Homeworks_StackPanel.Children.Add(text);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            Update_Homeworks();
        }

        private void Add_New_Homework_Button_Click(object sender, RoutedEventArgs e)
        {
            Consts.t_id = -1;
            Consts.title_register = Title_TextBlock.Text;
            int now_id = -1;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    int max_id = 0;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(Consts.get_max_ID("homeworks"), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        if (tb.Rows.Count != 0)
                            max_id = (int)tb.Rows[0]["mm"];
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO homeworks(ID, title, content, time, totalscore, courses_ID) VALUES (" + (max_id + 1).ToString() + ", '', '', @time, 100, " + Course_ID.ToString() + ")", sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@time", DateTime.Today + new TimeSpan(7, 0, 0, 0));
                        cmd.ExecuteNonQuery();
                    }
                    now_id = max_id + 1;
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT students_ID FROM students_has_courses WHERE courses_ID = " + Course_ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow row in tb.Rows)
                        {
                            int i = (int)row["students_ID"];
                            int max_id_he = 0;
                            using (SqlDataAdapter adapter1 = new SqlDataAdapter(Consts.get_max_ID("homework_entities"), sqlcon))
                            {
                                DataTable tb1 = new DataTable();
                                adapter1.Fill(tb1);
                                if (tb1.Rows.Count != 0)
                                    max_id_he = (int)tb1.Rows[0]["mm"];
                            }
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO homework_entities(ID, text, score, comment, is_submitted, is_returned, homeworks_ID, students_ID) VALUES(@id, @text, @score, @comment, @isub, @iret, @hid, @sid)", sqlcon))
                            {
                                cmd.Parameters.AddWithValue("@id", max_id_he += 1);
                                cmd.Parameters.AddWithValue("@text", "");
                                cmd.Parameters.AddWithValue("score", 0);
                                cmd.Parameters.AddWithValue("@comment", "");
                                cmd.Parameters.Add("@isub", SqlDbType.Bit).Value = 0;
                                cmd.Parameters.Add("@iret", SqlDbType.Bit).Value = 0;
                                cmd.Parameters.AddWithValue("@hid", now_id);
                                cmd.Parameters.AddWithValue("@sid", i);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
                return;
            }
            Consts.t_id = now_id;
            New_Homework homeworkform = new New_Homework(Course_ID, true);
            homeworkform.ShowDialog();
            if (Consts.t_id == -1)
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                    {
                        sqlcon.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM homeworks WHERE ID = " + now_id.ToString(), sqlcon))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Consts.Output_Database_Err(ex);
                    return;
                }
            }
            Homework_Search_TextBox.Text = "";
            Update_Homeworks();
        }

        private void Course_Data_Detail_Button_Click(object sender, RoutedEventArgs e)
        {
            Course_Data_Detail cddform = new Course_Data_Detail(Course_ID);
            cddform.ShowDialog();
        }
    }
}
