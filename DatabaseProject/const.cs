using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
namespace DatabaseProject
{
    public class Consts
    {
        public static string constring = @"server=(LocalDB)\v11.0;database=master;integrated security=true";
        public static string attachment_save_directory = @"Attachments\";
        public static int login_id = -1;
        public static string login_username = "";
        public static string login_nickname = "";
        public static string username_register;
        public static string password_register;
        public static string nickname_register;
        public static string title_register;
        public static int t_id;
        public static bool t_lock;
        public static SolidColorBrush[] Time_Colors = new SolidColorBrush[5];
        public static FontWeight[] Time_FontWeights = new FontWeight[5];
        public static DateTime Unlimited_Time = new System.DateTime(9999, 12, 31, 23, 59, 59);
        public static void Color_Initialize()
        {
            Time_Colors[0] = new SolidColorBrush(Color.FromRgb(0, 128, 0));
            Time_Colors[1] = new SolidColorBrush(Color.FromRgb(255, 165, 0));
            Time_Colors[2] = new SolidColorBrush(Color.FromRgb(255, 69, 0));
            Time_Colors[3] = new SolidColorBrush(Color.FromRgb(178, 34, 34));
            Time_Colors[4] = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            Time_FontWeights[0] = FontWeights.Normal;
            Time_FontWeights[1] = FontWeights.Bold;
            Time_FontWeights[2] = FontWeights.Bold;
            Time_FontWeights[3] = FontWeights.Bold;
            Time_FontWeights[4] = FontWeights.Normal;
        }
        public static void Output_Database_Err(Exception ex)
        {
            System.Windows.Forms.MessageBox.Show("数据库错误!\n" + ex.ToString());
        }
        public static bool Check_Username(string s)
        {
            return Check_Username(s, "用户名包含非法字符！用户名仅能包含字母，数字及下划线");
        }
        public static bool Check_Username(string s, string output)
        {
            int re = 0;
            foreach (var i in s)
                if (i >= 'a' && i <= 'z') re++;
                else if (i >= 'A' && i <= 'Z') re++;
                else if (i >= '0' && i <= '9') re++;
                else if (i == '_') re++;
            if (re != s.Length) System.Windows.Forms.MessageBox.Show(output);
            return re != s.Length;
        }
        public static string get_hash(string s)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            return BitConverter.ToString(sha256.ComputeHash(Encoding.Default.GetBytes(s))).Replace("-", "");
        }
        public static string get_max_ID(string s)
        {
            return "SELECT * FROM (SELECT MAX(ID) AS mm FROM " + s + ") AS T WHERE mm IS NOT NULL";
        }
        public static void SQL_No_Result()
        {
            System.Windows.Forms.MessageBox.Show("未查询到符合要求的数据！");
        }
        public static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxButtons btn = MessageBoxButtons.YesNo;
            var res = System.Windows.Forms.MessageBox.Show("你确认要退出吗？", "警告", btn);
            if (!(res == System.Windows.Forms.DialogResult.Yes)) e.Cancel = true;
        }
        public static string Get_Random_String(string input)
        {
            return get_hash(input + DateTime.Now.ToString("+add time: yyyy/MM/dd hh:mm:ss.fffffff"));
        }
        public static string Get_Random_String()
        {
            return Get_Random_String("");
        }
        public static void Save_Attachment_To_DB(string DBTable_Name, int ID)
        {
            string h_string = "homeworks_ID";
            if (DBTable_Name == "homework_entity_attachments")
                h_string = "homework_entities_ID";
            OpenFileDialog Attachment_Select_Dialog = new OpenFileDialog();
            Attachment_Select_Dialog.Filter = "All files (*.*)|*.*";
            Attachment_Select_Dialog.FilterIndex = 0;
            if (Attachment_Select_Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(Attachment_Select_Dialog.FileName))
                {
                    try
                    {
                        using (SqlConnection sqlcon = new SqlConnection(constring))
                        {
                            sqlcon.Open();
                            string filename_random = Get_Random_String(Attachment_Select_Dialog.FileName);
                            try
                            {
                                File.Copy(Attachment_Select_Dialog.FileName, attachment_save_directory + filename_random);
                            }
                            catch
                            {
                                System.Windows.Forms.MessageBox.Show("复制失败");
                                return;
                            }
                            int max_id = 0;
                            using (SqlDataAdapter adapter = new SqlDataAdapter(get_max_ID(DBTable_Name), sqlcon))
                            {
                                DataTable tb = new DataTable();
                                adapter.Fill(tb);
                                if (tb.Rows.Count != 0)
                                    max_id = (int)tb.Rows[0]["mm"];
                            }
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO " + DBTable_Name + "(ID, attachment, filename, " + h_string + ") VALUES(@id, @att, @name, @hid)", sqlcon))
                            {
                                cmd.Parameters.AddWithValue("@id", max_id + 1);
                                cmd.Parameters.AddWithValue("@att", filename_random);
                                cmd.Parameters.AddWithValue("@name", Attachment_Select_Dialog.SafeFileName);
                                cmd.Parameters.AddWithValue("@hid", ID);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        System.Windows.Forms.MessageBox.Show("上传成功");
                    }
                    catch (Exception ex)
                    {
                        Output_Database_Err(ex);
                    }
                }
            }
        }
        public static void Load_Attachment_To_File(string DBTable_Name, int ID){
            SaveFileDialog Attachment_Save_Dialog = new SaveFileDialog();
            string filename = "";
            string savename = "";
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + DBTable_Name + " WHERE ID = " + ID.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        filename = (string)tb.Rows[0]["filename"];
                        savename = (string)tb.Rows[0]["attachment"];
                    }
                }
            }
            catch (Exception ex)
            {
                Output_Database_Err(ex);
            }
            string suffix = Regex.Match(filename,@"(?<=\.)[^\.]*$").Value;
            Attachment_Save_Dialog.Filter = suffix + " files (*." + suffix + ")|*." + suffix + "|All files (*.*)|*.*";
            Attachment_Save_Dialog.FilterIndex = 0;
            Attachment_Save_Dialog.FileName = filename;
            if (Attachment_Save_Dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(attachment_save_directory + savename, Attachment_Save_Dialog.FileName, true);
                    System.Windows.Forms.MessageBox.Show("下载完成！");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
        }
        public static double MeasureTextWidth(string text, double fontSize, string fontFamily, FontWeight weight)
        {
            FormattedText formattedText = new FormattedText(text,
                                                            System.Globalization.CultureInfo.InvariantCulture,
                                                            System.Windows.FlowDirection.LeftToRight,
                                                            new Typeface(fontFamily.ToString()),
                                                            fontSize,
                                                            Brushes.Black
            );
            formattedText.SetFontWeight(weight);
            return formattedText.WidthIncludingTrailingWhitespace;
        }
        public static void String_Initialize()
        {
            if (File.Exists("settings.cfg"))
            {
                System.Windows.Forms.MessageBox.Show("找到配置文件，从其中载入数据库连接字符串及附件保存位置。");
                System.IO.StreamReader s = File.OpenText("settings.cfg");
                constring = s.ReadLine();
                attachment_save_directory = s.ReadLine();
            }
        }
    }
}