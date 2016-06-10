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
    /// New_Course.xaml 的交互逻辑
    /// </summary>
    public partial class New_Course : Window
    {
        public New_Course()
        {
            InitializeComponent();
            Teacher_ID_TextBox.IsEnabled = !Consts.t_lock;
            if (Consts.t_id != -1) Teacher_ID_TextBox.Text = Consts.t_id.ToString();
            Consts.t_id = -1;
            Course_Name_TextBox.Text = Consts.title_register;
            if (Consts.title_register != "")
            {
                Title_TextBlock.Text = "修改课程";
            }
            Consts.title_register = "";
        }

        private void Teacher_ID_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToInt32(Teacher_ID_TextBox.Text);
            }
            catch
            {
                if (Teacher_ID_TextBox.Text.Length == 0)
                {
                    Teacher_UserName_TextBlock.Text = "";
                    Teacher_NickName_TextBlock.Text = "";
                    return;
                }
                Teacher_ID_TextBox.Text = Teacher_ID_TextBox.Text.Remove(Teacher_ID_TextBox.Text.Length - 1, 1);
                Teacher_ID_TextBox.SelectionStart = Teacher_ID_TextBox.Text.Length;
                return;
            }
            Teacher_UserName_TextBlock.Text = "";
            Teacher_NickName_TextBlock.Text = "";
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM teachers WHERE ID = " + Teacher_ID_TextBox.Text, sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        if (tb.Rows.Count != 0)
                        {
                            Teacher_UserName_TextBlock.Text = (string)tb.Rows[0]["username"];
                            Teacher_NickName_TextBlock.Text = (string)tb.Rows[0]["nickname"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Teacher_UserName_TextBlock.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("教师ID错误，新建失败！");
                return;
            }
            if (Course_Name_TextBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("未填写课程名，新建失败！");
                return;
            }
            Consts.t_id = Convert.ToInt32(Teacher_ID_TextBox.Text);
            Consts.title_register = Course_Name_TextBox.Text;
            this.Close();
        }

    }
}
