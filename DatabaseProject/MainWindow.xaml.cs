using System;
using System.Collections.Generic;
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
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;


namespace DatabaseProject
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection sqlcon;
        public MainWindow()
        {
            InitializeComponent();
            Consts.Color_Initialize();
            Consts.String_Initialize();
            try
            {
                sqlcon = new SqlConnection(Consts.constring);
                sqlcon.Open();
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
            Login_Username.Focus();
        }
        private void show_login()
        {
            Register_Show_Button.Visibility = System.Windows.Visibility.Visible;
            Login_Show_Button.Visibility = System.Windows.Visibility.Hidden;
            Login_Grid.Visibility = System.Windows.Visibility.Visible;
            Register_Grid.Visibility = System.Windows.Visibility.Hidden;
            Login_Button.Visibility = System.Windows.Visibility.Visible;
            Register_Button.Visibility = System.Windows.Visibility.Hidden;
            Teacher_RadioButton.IsEnabled = true;
            Admin_RadioButton.IsEnabled = true;
            MainWindows_Title.Text = "用户登录";
            Register_Password.Password = "";
            Register_Password_Confirm.Password = "";
            Register_Nickname.Text = "";
            Register_Username.Text = "";
            Login_Username.Focus();
        }
        private void show_register()
        {
            Register_Show_Button.Visibility = System.Windows.Visibility.Hidden;
            Login_Show_Button.Visibility = System.Windows.Visibility.Visible;
            Login_Grid.Visibility = System.Windows.Visibility.Hidden;
            Register_Grid.Visibility = System.Windows.Visibility.Visible;
            Login_Button.Visibility = System.Windows.Visibility.Hidden;
            Register_Button.Visibility = System.Windows.Visibility.Visible;
            Student_RadioButton.IsChecked = true;
            Teacher_RadioButton.IsEnabled = false;
            Admin_RadioButton.IsEnabled = false;
            MainWindows_Title.Text = "新学生注册";
            Login_Password.Password = "";
            Login_Username.Text = "";
            Register_Username.Focus();
        }
        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)Teacher_RadioButton.IsChecked)
            {
                int login_id = -1;
                try
                {
                    DataTable tb = new DataTable();
                    if (Consts.Check_Username(Login_Username.Text)) return;
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM teachers WHERE username = '" + Login_Username.Text + "'", sqlcon))
                    {
                        adapter.Fill(tb);
                        if (tb.Rows.Count == 0)
                        {
                            System.Windows.Forms.MessageBox.Show("用户名不存在！");
                            return;
                        }
                        if ((string)tb.Rows[0]["password"] != Consts.get_hash(Login_Password.Password))
                        {
                            System.Windows.Forms.MessageBox.Show("密码错误！");
                            return;
                        }
                        login_id = (int)tb.Rows[0]["ID"];
                        Consts.login_nickname = (string)tb.Rows[0]["nickname"];
                    }
                }
                catch (Exception ex)
                {
                    Consts.Output_Database_Err(ex);
                }
                System.Windows.Forms.MessageBox.Show("登录ID：" + login_id.ToString() + "； 用户名：" + Login_Username.Text);
                Consts.login_id = login_id;
                Consts.login_username = Login_Username.Text;
                Teacher TeacherWindow = new Teacher();
                TeacherWindow.Show();
            }
            else if ((bool)Student_RadioButton.IsChecked)
            {
                int login_id = -1;
                try
                {
                    DataTable tb = new DataTable();
                    if (Consts.Check_Username(Login_Username.Text)) return;
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM students WHERE username = '" + Login_Username.Text + "'", sqlcon))
                    {
                        adapter.Fill(tb);
                        if (tb.Rows.Count == 0)
                        {
                            System.Windows.Forms.MessageBox.Show("用户名不存在！");
                            return;
                        }
                        if ((string)tb.Rows[0]["password"] != Consts.get_hash(Login_Password.Password))
                        {
                            System.Windows.Forms.MessageBox.Show("密码错误！");
                            return;
                        }
                        login_id = (int)tb.Rows[0]["ID"];
                        Consts.login_nickname = (string)tb.Rows[0]["nickname"];
                    }
                }
                catch (Exception ex)
                {
                    Consts.Output_Database_Err(ex);
                }
                System.Windows.Forms.MessageBox.Show("登录ID：" + login_id.ToString() + "； 用户名：" + Login_Username.Text);
                Consts.login_id = login_id;
                Consts.login_username = Login_Username.Text;
                Student StudentWindow = new Student();
                StudentWindow.Show();
            }
            else if ((bool)Admin_RadioButton.IsChecked)
            {
                int login_id = -1;
                try
                {
                    DataTable tb = new DataTable();
                    if (Consts.Check_Username(Login_Username.Text)) return;
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM admins WHERE username = '" + Login_Username.Text + "'", sqlcon))
                    {
                        adapter.Fill(tb);
                        if (tb.Rows.Count == 0)
                        {
                            System.Windows.Forms.MessageBox.Show("用户名不存在！");
                            return;
                        }
                        if ((string)tb.Rows[0]["password"] != Consts.get_hash(Login_Password.Password))
                        {
                            System.Windows.Forms.MessageBox.Show("密码错误！");
                            return;
                        }
                        login_id = (int)tb.Rows[0]["ID"];
                        Consts.login_nickname = (string)tb.Rows[0]["nickname"];
                    }
                }
                catch (Exception ex)
                {
                    Consts.Output_Database_Err(ex);
                }
                System.Windows.Forms.MessageBox.Show("登录ID：" + login_id.ToString() + "； 用户名：" + Login_Username.Text);
                Consts.login_id = login_id;
                Consts.login_username = Login_Username.Text;
                Admin AdminWindow = new Admin();
                AdminWindow.Show();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("你还没选你是谁呢");
                return;
            }
            this.Close();
        }
        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Register_Username.Text.Length < 3)
            {
                System.Windows.Forms.MessageBox.Show("用户名太短，至少3位");
                return;
            }
            if (Register_Nickname.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("请输入姓名！");
                return;
            }
            if (Register_Password.Password.Length < 6)
            {
                System.Windows.Forms.MessageBox.Show("密码太短，至少6位");
                return;
            }
            if (Register_Password.Password != Register_Password_Confirm.Password)
            {
                System.Windows.Forms.MessageBox.Show("两次输入密码不同！");
                return;
            }
            try
            {
                DataTable tb = new DataTable();
                if (Consts.Check_Username(Register_Username.Text)) return;
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT 1 FROM students WHERE username = '" + Register_Username.Text + "'", sqlcon))
                {
                    adapter.Fill(tb);
                    if (tb.Rows.Count != 0)
                    {
                        System.Windows.Forms.MessageBox.Show("用户名已存在！");
                        return;
                    }
                }
                using (SqlCommand com = sqlcon.CreateCommand())
                {
                    int max_id = 0;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(Consts.get_max_ID("students"), sqlcon))
                    {
                        adapter.Fill(tb);
                        if (tb.Rows.Count != 0)
                            max_id = (int)tb.Rows[0]["mm"];
                    }
                    com.CommandText = "INSERT INTO students(ID, username, password, nickname) VALUES(@id, @username, @password, @nickname)";
                    com.Parameters.AddWithValue("@id", max_id + 1);
                    com.Parameters.AddWithValue("@username", Register_Username.Text);
                    com.Parameters.AddWithValue("@password", Consts.get_hash(Register_Password.Password));
                    com.Parameters.AddWithValue("@nickname", Register_Nickname.Text);
                    com.ExecuteNonQuery();
                    System.Windows.Forms.MessageBox.Show("注册成功！");
                    Login_Username.Text = Register_Username.Text;
                    show_login();
                    Login_Password.Focus();
                }


            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
        private void Register_Show_Button_Click(object sender, RoutedEventArgs e)
        {
            show_register();
        }
        private void Login_Show_Button_Click(object sender, RoutedEventArgs e)
        {
            show_login();
        }
    }
}
