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
    /// Teacher.xaml 的交互逻辑
    /// </summary>
    public partial class Teacher : Window
    {
        DataTable Courses_DataTable = new DataTable();
        bool Form_Load_Complete = false;
        public Teacher()
        {
            InitializeComponent();
            this.Title += Consts.login_nickname;
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
                    string cmdstring = "SELECT name, COUNT(students_id) AS count, ID FROM courses FULL OUTER JOIN students_has_courses ON courses_id = ID WHERE courses.teachers_ID = " + Consts.login_id.ToString();
                    if (Course_Chosen_Search_TextBox.Text != "") cmdstring += " AND CHARINDEX(@name, courses.name) <> 0";
                    cmdstring += " GROUP BY name, ID";
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
        }
        private void Add_New_Course_Button_Click(object sender, RoutedEventArgs e)
        {
            Consts.t_lock = true;
            Consts.t_id = Consts.login_id;
            Consts.title_register = "";
            New_Course newcourseform = new New_Course();
            newcourseform.ShowDialog();
            if (Consts.t_id == -1) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    int max_id = 0;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(Consts.get_max_ID("courses"), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        if (tb.Rows.Count > 0)
                            max_id = (int)tb.Rows[0]["mm"];
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO courses(ID, name, teachers_ID) VALUES(@id, @name, @tid)", sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@id", max_id + 1);
                        cmd.Parameters.AddWithValue("@name", Consts.title_register);
                        cmd.Parameters.AddWithValue("@tid", Consts.login_id);
                        cmd.ExecuteNonQuery();
                    }
                }
                Update_Courses();
                System.Windows.Forms.MessageBox.Show("课程创建成功！");
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
        private void Get_In_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Courses_ListView.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("未选择任何项！");
                return;
            }
            int course_id = (int)((DataRowView)Courses_ListView.SelectedItem)["ID"];
            Course_Detail coursedetailform = new Course_Detail(course_id);
            this.Hide();
            coursedetailform.ShowDialog();
            this.Show();
        }
    }
}
