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
    /// Join_Course.xaml 的交互逻辑
    /// </summary>
    public partial class Join_Course : Window
    {
        DataTable Courses_DataTable = new DataTable();
        public Join_Course()
        {
            InitializeComponent();
            Consts.t_id = -1;
            Courses_ListView.ItemsSource = Courses_DataTable.DefaultView;
            Update_Courses();
        }
        private void Update_Courses()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    string cmdstring = "SELECT name, nickname, courses.ID FROM courses, teachers WHERE teachers_ID = teachers.ID AND courses.ID IN (SELECT ID FROM courses EXCEPT SELECT courses_ID AS ID FROM students_has_courses WHERE students_ID = " + Consts.login_id.ToString() + ")";
                    if (Course_Chosen_Search_TextBox.Text != "") cmdstring += " AND CHARINDEX(@name, courses.name) <> 0";
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand(cmdstring, sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@name", Course_Chosen_Search_TextBox.Text);
                            adapter.SelectCommand = cmd;
                            Courses_DataTable.Clear();
                            adapter.Fill(Courses_DataTable);
                            if (Courses_DataTable.Rows.Count == 0)
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

        private void Accept_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Courses_ListView.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("未选择任何项！");
                return;
            }
            MessageBoxButtons btn = MessageBoxButtons.YesNo;
            if (System.Windows.Forms.MessageBox.Show("你确定要选这门课吗？", "警告", btn) == System.Windows.Forms.DialogResult.No) return;
            DataRowView row = (DataRowView)Courses_ListView.SelectedItem;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    int t_id = (int)row["ID"];
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO students_has_courses(students_ID, courses_ID) VALUES(@sid, @tid)", sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@sid", Consts.login_id);
                        cmd.Parameters.AddWithValue("@tid", t_id);
                        cmd.ExecuteNonQuery();
                    }
                    Consts.t_id = t_id;
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
                return;
            }
            this.Close();
        }
    }
}
