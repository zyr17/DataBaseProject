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
    /// Student.xaml 的交互逻辑
    /// </summary>
    public partial class Student : Window
    {
        private DataTable Courses_DataTable = new DataTable();
        private bool Form_Load_Complete = false;
        public Student()
        {
            InitializeComponent();
            this.Title = "欢迎！学生：" + Consts.login_nickname;
            Courses_ListView.ItemsSource = Courses_DataTable.DefaultView;
            Update_Courses();
            Form_Load_Complete = true;
            this.Closing += Consts.Window_Closing;
        }
        private void Update_Courses()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    string cmdstring = "SELECT name, nickname, courses.ID FROM courses, students_has_courses, teachers WHERE students_has_courses.students_ID = " + Consts.login_id.ToString() + " AND students_has_courses.courses_ID = courses.ID AND courses.teachers_ID = teachers.ID";
                    if (Course_Chosen_Search_TextBox.Text != "") cmdstring += " AND CHARINDEX(@name, courses.name) <> 0";
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand(cmdstring, sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@name", Course_Chosen_Search_TextBox.Text);
                            adapter.SelectCommand = cmd;
                            Courses_DataTable.Clear();
                            adapter.Fill(Courses_DataTable);
                            if (Courses_DataTable.Rows.Count == 0 && Form_Load_Complete)
                                Consts.SQL_No_Result();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
        private void Course_Search_Button_Click(object sender, RoutedEventArgs e)
        {
            Update_Courses();
            Homework_Entities_StackPanel.Children.Clear();
        }
        private void Course_Join_Button_Click(object sender, RoutedEventArgs e)
        {
            Join_Course joinform = new Join_Course();
            joinform.ShowDialog();
            if (Consts.t_id == -1) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    DataTable tb = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT ID FROM homeworks WHERE courses_ID = " + Consts.t_id.ToString(), sqlcon))
                    {
                        adapter.Fill(tb);
                    }
                    foreach (DataRow trow in tb.Rows)
                    {
                        int i = (int)trow["ID"];
                        int max_id = 0;
                        using (SqlDataAdapter adapter2 = new SqlDataAdapter(Consts.get_max_ID("homework_entities"), sqlcon))
                        {
                            DataTable tb1 = new DataTable();
                            adapter2.Fill(tb1);
                            if (tb1.Rows.Count != 0)
                                max_id = (int)tb1.Rows[0]["mm"];
                        }
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO homework_entities(ID, text, score, comment, is_submitted, is_returned, homeworks_ID, students_ID) VALUES(@id, @text, @score, @comment, @isub, @iret, @hid, @sid)", sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@id", max_id + 1);
                            cmd.Parameters.AddWithValue("@text", "");
                            cmd.Parameters.AddWithValue("score", 0);
                            cmd.Parameters.AddWithValue("@comment", "");
                            cmd.Parameters.Add("@isub", SqlDbType.Bit).Value = 0;
                            cmd.Parameters.Add("@iret", SqlDbType.Bit).Value = 0;
                            cmd.Parameters.AddWithValue("@hid", i);
                            cmd.Parameters.AddWithValue("@sid", Consts.login_id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
            System.Windows.Forms.MessageBox.Show("选课成功！");
            Course_Chosen_Search_TextBox.Text = "";
            Update_Courses();
        }
        private void Courses_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Courses_ListView.SelectedItem == null) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    int courses_id = (int)((DataRowView)Courses_ListView.SelectedItem)["ID"];
                    Homework_Entities_StackPanel.Children.Clear();
                    using (SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT homework_entities.ID FROM homeworks, homework_entities WHERE homeworks.ID = homeworks_ID AND students_ID = " + Consts.login_id.ToString() + " AND courses_ID = " + courses_id.ToString() + "ORDER BY time DESC", sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter1.Fill(tb);
                        foreach (DataRow rr in tb.Rows)
                        {
                            Homework_Entities_Control hectrl = new Homework_Entities_Control();
                            Homework_Entities_StackPanel.Children.Add(hectrl);
                            hectrl.Homework_Entities_ID = (int)rr["ID"];
                        }
                        if (tb.Rows.Count == 0)
                        {
                            TextBlock text = new TextBlock();
                            text.Text = "这门课还没有作业";
                            text.Margin = new System.Windows.Thickness(0, 20, 0, 20);
                            text.Foreground = Brushes.Black;
                            text.FontSize = 18;
                            text.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                            text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            Homework_Entities_StackPanel.Children.Add(text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
    }
}
