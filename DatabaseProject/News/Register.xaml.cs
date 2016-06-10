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
using System.Windows.Shapes;

namespace DatabaseProject
{
    /// <summary>
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
            this.Title = MainWindow_Title.Text = Consts.title_register;
            Register_Username.Text = Consts.username_register;
            Register_Nickname.Text = Consts.nickname_register;
            Consts.nickname_register = "";
            Consts.username_register = "";
            Register_Username.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
            if (Consts.Check_Username(Register_Username.Text)) return;
            Consts.username_register = Register_Username.Text;
            Consts.password_register = Consts.get_hash(Register_Password.Password);
            Consts.nickname_register = Register_Nickname.Text;
            this.Close();
        }
    }
}
