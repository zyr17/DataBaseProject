using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
using System.Xml;

namespace DatabaseProject
{
    /// <summary>
    /// Admin.xaml 的交互逻辑
    /// </summary>
    public partial class Admin : Window
    {
        private DataTable Teacher_DataTable = new DataTable();
        private DataTable Student_DataTable = new DataTable();
        private DataTable Course_DataTable = new DataTable();
        private DataTable Sql_DataTable = new DataTable();
        public Admin()
        {
            InitializeComponent();
            this.Title = "欢迎！管理员：" + Consts.login_nickname;
            Teacher_ListView.ItemsSource = Teacher_DataTable.DefaultView;
            Student_ListView.ItemsSource = Student_DataTable.DefaultView;
            Course_ListView.ItemsSource = Course_DataTable.DefaultView;
            Sql_ListView.ItemsSource = Sql_DataTable.DefaultView;
            this.Closing += Consts.Window_Closing;
        }
        void Read_From_XML(string xml_filename)
        {
            XmlDocument xmlreader = new XmlDocument();
            xmlreader.Load(xml_filename);
            XmlNode xmlroot = xmlreader.SelectSingleNode("root");
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO admins(ID, username, password, nickname) VALUES(@id, @username, @password, @nickname)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("admins").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(xml_element.Attributes["ID"].Value));
                            cmd.Parameters.AddWithValue("@username", xml_element.Attributes["username"].Value);
                            cmd.Parameters.AddWithValue("@password", xml_element.Attributes["password"].Value);
                            cmd.Parameters.AddWithValue("@nickname", xml_element.Attributes["nickname"].Value);
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO students(ID, username, password, nickname) VALUES(@id, @username, @password, @nickname)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("students").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(xml_element.Attributes["ID"].Value));
                            cmd.Parameters.AddWithValue("@username", xml_element.Attributes["username"].Value);
                            cmd.Parameters.AddWithValue("@password", xml_element.Attributes["password"].Value);
                            cmd.Parameters.AddWithValue("@nickname", xml_element.Attributes["nickname"].Value);
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO teachers(ID, username, password, nickname) VALUES(@id, @username, @password, @nickname)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("teachers").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(xml_element.Attributes["ID"].Value));
                            cmd.Parameters.AddWithValue("@username", xml_element.Attributes["username"].Value);
                            cmd.Parameters.AddWithValue("@password", xml_element.Attributes["password"].Value);
                            cmd.Parameters.AddWithValue("@nickname", xml_element.Attributes["nickname"].Value);
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO courses(ID, name, teachers_ID) VALUES(@id, @name, @tid)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("courses").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(xml_element.Attributes["ID"].Value));
                            cmd.Parameters.AddWithValue("@name", xml_element.Attributes["name"].Value);
                            cmd.Parameters.AddWithValue("@tid", Convert.ToInt32(xml_element.Attributes["teachers_ID"].Value));
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO students_has_courses(students_ID, courses_ID) VALUES(@sid, @cid)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("students_has_courses").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@sid", Convert.ToInt32(xml_element.Attributes["students_ID"].Value));
                            cmd.Parameters.AddWithValue("@cid", Convert.ToInt32(xml_element.Attributes["courses_ID"].Value));
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO homeworks(ID, time, title, content, totalscore, courses_ID) VALUES(@id, @time, @title, @content, @totalscore, @cid)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("homeworks").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(xml_element.Attributes["ID"].Value));
                            cmd.Parameters.AddWithValue("@time", Convert.ToDateTime(xml_element.Attributes["time"].Value));
                            cmd.Parameters.AddWithValue("@title", xml_element.Attributes["title"].Value);
                            cmd.Parameters.AddWithValue("@content", xml_element.Attributes["content"].Value);
                            cmd.Parameters.AddWithValue("@totalscore", Convert.ToInt32(xml_element.Attributes["totalscore"].Value));
                            cmd.Parameters.AddWithValue("@cid", Convert.ToInt32(xml_element.Attributes["courses_ID"].Value));
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO homework_entities(ID, text, score, comment, is_submitted, is_returned, homeworks_ID, students_ID) VALUES(@id, @text, @score, @comment, @isub, @iret, @hid, @sid)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("homework_entities").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(xml_element.Attributes["ID"].Value));
                            cmd.Parameters.AddWithValue("@text", xml_element.Attributes["text"].Value);
                            cmd.Parameters.AddWithValue("@score", Convert.ToInt32(xml_element.Attributes["score"].Value));
                            cmd.Parameters.AddWithValue("@comment", xml_element.Attributes["comment"].Value);
                            cmd.Parameters.Add("@isub", SqlDbType.Bit).Value = Convert.ToInt32(xml_element.Attributes["is_submitted"].Value);
                            cmd.Parameters.Add("@iret", SqlDbType.Bit).Value = Convert.ToInt32(xml_element.Attributes["is_returned"].Value);
                            cmd.Parameters.AddWithValue("@hid", Convert.ToInt32(xml_element.Attributes["homeworks_ID"].Value));
                            cmd.Parameters.AddWithValue("@sid", Convert.ToInt32(xml_element.Attributes["students_ID"].Value));
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO homework_attachments(ID, attachment, filename, homeworks_ID) VALUES(@id, @attachment, @filename, @hid)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("homework_attachments").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(xml_element.Attributes["ID"].Value));
                            cmd.Parameters.AddWithValue("@attachment", xml_element.Attributes["attachment"].Value);
                            cmd.Parameters.AddWithValue("@filename", xml_element.Attributes["filename"].Value);
                            cmd.Parameters.AddWithValue("@hid", Convert.ToInt32(xml_element.Attributes["homeworks_ID"].Value));
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO homework_entity_attachments(ID, attachment, filename, homework_entities_ID) VALUES(@id, @attachment, @filename, @heid)", sqlcon))
                    {
                        XmlNode xml_element = xmlroot.SelectSingleNode("homework_entity_attachments").FirstChild;
                        while (xml_element != null)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(xml_element.Attributes["ID"].Value));
                            cmd.Parameters.AddWithValue("@attachment", xml_element.Attributes["attachment"].Value);
                            cmd.Parameters.AddWithValue("@filename", xml_element.Attributes["filename"].Value);
                            cmd.Parameters.AddWithValue("@heid", Convert.ToInt32(xml_element.Attributes["homework_entities_ID"].Value));
                            cmd.ExecuteNonQuery();
                            xml_element = xml_element.NextSibling;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        void Save_To_XML(string xml_filename)
        {
            XmlDocument newxml = new XmlDocument();
            XmlElement xmlroot = newxml.CreateElement("root");
            newxml.AppendChild(xmlroot);
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM admins", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("admins");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("ID");
                            attr.InnerText = ((int)i["ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("username");
                            attr.InnerText = (string)i["username"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("password");
                            attr.InnerText = (string)i["password"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("nickname");
                            attr.InnerText = (string)i["nickname"];
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM students", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("students");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("ID");
                            attr.InnerText = ((int)i["ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("username");
                            attr.InnerText = (string)i["username"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("password");
                            attr.InnerText = (string)i["password"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("nickname");
                            attr.InnerText = (string)i["nickname"];
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM teachers", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("teachers");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("ID");
                            attr.InnerText = ((int)i["ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("username");
                            attr.InnerText = (string)i["username"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("password");
                            attr.InnerText = (string)i["password"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("nickname");
                            attr.InnerText = (string)i["nickname"];
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM courses", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("courses");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("ID");
                            attr.InnerText = ((int)i["ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("name");
                            attr.InnerText = (string)i["name"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("teachers_ID");
                            attr.InnerText = ((int)i["teachers_ID"]).ToString();
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM students_has_courses", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("students_has_courses");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("students_ID");
                            attr.InnerText = ((int)i["students_ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("courses_ID");
                            attr.InnerText = ((int)i["courses_ID"]).ToString();
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homeworks", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("homeworks");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("ID");
                            attr.InnerText = ((int)i["ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("time");
                            attr.InnerText = ((System.DateTime)i["time"]).ToString("G");
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("title");
                            attr.InnerText = (string)i["title"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("content");
                            attr.InnerText = (string)i["content"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("totalscore");
                            attr.InnerText = ((int)i["totalscore"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("courses_ID");
                            attr.InnerText = ((int)i["courses_ID"]).ToString();
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homework_entities", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("homework_entities");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("ID");
                            attr.InnerText = ((int)i["ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("text");
                            attr.InnerText = (string)i["text"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("score");
                            attr.InnerText = ((int)i["score"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("comment");
                            attr.InnerText = (string)i["comment"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("is_submitted");
                            attr.InnerText = ((bool)i["is_submitted"] ? 1 : 0).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("is_returned");
                            attr.InnerText = ((bool)i["is_returned"] ? 1 : 0).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("homeworks_ID");
                            attr.InnerText = ((int)i["homeworks_ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("students_ID");
                            attr.InnerText = ((int)i["students_ID"]).ToString();
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homework_attachments", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("homework_attachments");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("ID");
                            attr.InnerText = ((int)i["ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("attachment");
                            attr.InnerText = (string)i["attachment"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("filename");
                            attr.InnerText = (string)i["filename"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("homeworks_ID");
                            attr.InnerText = ((int)i["homeworks_ID"]).ToString();
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homework_entity_attachments", sqlcon))
                    {
                        XmlElement xml_element = newxml.CreateElement("homework_entity_attachments");
                        xmlroot.AppendChild(xml_element);
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        foreach (DataRow i in tb.Rows)
                        {
                            XmlElement row = newxml.CreateElement("row");
                            XmlAttribute attr = newxml.CreateAttribute("ID");
                            attr.InnerText = ((int)i["ID"]).ToString();
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("attachment");
                            attr.InnerText = (string)i["attachment"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("filename");
                            attr.InnerText = (string)i["filename"];
                            row.Attributes.Append(attr);
                            attr = newxml.CreateAttribute("homework_entities_ID");
                            attr.InnerText = ((int)i["homework_entities_ID"]).ToString();
                            row.Attributes.Append(attr);
                            xml_element.AppendChild(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
                return;
            }
            try
            {
                newxml.Save(xml_filename);
                System.Windows.Forms.MessageBox.Show("导出成功！");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("文件错误！\n" + ex.Message);
            }
        }
        private void XML_In_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog XML_Select_Dialog = new OpenFileDialog();

            XML_Select_Dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            XML_Select_Dialog.FilterIndex = 0;

            if (XML_Select_Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(XML_Select_Dialog.FileName))
                {
                    // Insert code to read the stream here.
                    MessageBoxButtons msgb = MessageBoxButtons.OKCancel;
                    if (System.Windows.Forms.MessageBox.Show("确定要从这个xml导入数据吗？这将清空现在的数据库！", "确认导入", msgb) == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                            {
                                sqlcon.Open();
                                string nowname = "", nowpass = "";
                                MessageBoxButtons msgb1 = MessageBoxButtons.YesNo;
                                if (System.Windows.Forms.MessageBox.Show("管理员信息也将导入！为了不影响登录，是否保留当前账号的用户名及密码？若保留，则ID会发生变化，且备份中已存在相同用户名用户的话将会被目前用户覆盖。", "确认导入", msgb1) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM admins WHERE ID = " + Consts.login_id.ToString(), sqlcon))
                                    {
                                        DataTable tb = new DataTable(); ;
                                        adapter.Fill(tb);
                                        nowname = (string)tb.Rows[0]["username"];
                                        nowpass = (string)tb.Rows[0]["password"];
                                    }
                                }
                                using (SqlCommand cmd = new SqlCommand("drop_all", sqlcon))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                sqlcon.Close();
                                Read_From_XML(XML_Select_Dialog.FileName);
                                sqlcon.Open();
                                if (nowname.Length > 0)
                                {
                                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM admins WHERE username='" + nowname + "'", sqlcon))
                                    {
                                        DataTable tb = new DataTable();
                                        adapter.Fill(tb);
                                        if (tb.Rows.Count > 0)
                                        {
                                            using (SqlCommand cmd = new SqlCommand("UPDATE admins SET password = '" + nowpass + "' WHERE username = '" + nowname + "'", sqlcon))
                                            {
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            using (SqlCommand cmd = new SqlCommand("INSERT INTO admins(ID, username, password) VALUES(@id, @username, @password)", sqlcon))
                                            {
                                                int max_id = 0;
                                                using (SqlDataAdapter adapter2 = new SqlDataAdapter(Consts.get_max_ID("admins"), sqlcon))
                                                {
                                                    adapter2.Fill(tb);
                                                    if (tb.Rows.Count != 0)
                                                        max_id = (int)tb.Rows[0]["mm"];
                                                }
                                                cmd.Parameters.AddWithValue("@id", max_id + 1);
                                                cmd.Parameters.AddWithValue("@username", nowname);
                                                cmd.Parameters.AddWithValue("@password", nowpass);
                                                cmd.ExecuteNonQuery();
                                            }
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
                        System.Windows.Forms.MessageBox.Show("导入成功！");
                    }
                }
            }
        }
        private void XML_Out_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog XML_Select_Dialog = new SaveFileDialog();

            XML_Select_Dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            XML_Select_Dialog.FilterIndex = 0;
            if (XML_Select_Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                if (File.Exists(XML_Select_Dialog.FileName))
                    Save_To_XML(XML_Select_Dialog.FileName);
        }
        private void Teacher_Search_Button_Click(object sender, RoutedEventArgs e)
        {
            int id = - 1;
            try
            {
                if (Teacher_Search_ID.Text.Length > 0)
                    id = Convert.ToInt32(Teacher_Search_ID.Text);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("ID输入有误！");
                return;
            }
            if (Consts.Check_Username(Teacher_Search_Name.Text)) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    Teacher_DataTable.Clear();
                    string sqlquery = "SELECT * FROM teachers";
                    string idquery = "";
                    string namequery = "";
                    if (Teacher_Search_ID.Text != "") idquery = " ID = " + id.ToString();
                    if (Teacher_Search_Name.Text != "") namequery = " CHARINDEX('" + Teacher_Search_Name.Text + "', username) <> 0";
                    if (idquery + namequery != "") sqlquery += " WHERE";
                    if (idquery != "" && namequery != "") idquery += " AND";
                    sqlquery += idquery + namequery;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, sqlcon))
                        adapter.Fill(Teacher_DataTable);
                    if (Teacher_DataTable.Rows.Count == 0)
                        Consts.SQL_No_Result();
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
        private void Teacher_Insert_Button_Click(object sender, RoutedEventArgs e)
        {
            Consts.password_register = Consts.username_register = Consts.nickname_register = "";
            Consts.title_register = "添加用户";
            Register registerform = new Register();
            registerform.ShowDialog();
            if (Consts.password_register.Length != 64) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    bool flag = false;
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM teachers WHERE username = '" + Consts.username_register + "'", sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        flag = tb.Rows.Count != 0;
                    }
                    if (flag)
                    {
                        MessageBoxButtons btn1 = MessageBoxButtons.YesNo;
                        if (System.Windows.Forms.MessageBox.Show("此用户名的教师已存在！是否更新此教师的信息？", "警告", btn1) == System.Windows.Forms.DialogResult.Yes)
                        {
                            using (SqlCommand cmd = new SqlCommand("UPDATE teachers SET password = '" + Consts.password_register + "', nickname = @nickname WHERE username = '" + Consts.username_register + "'", sqlcon))
                            {
                                cmd.Parameters.AddWithValue("@nickname", Consts.nickname_register);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else return;
                    }
                    else
                    {
                        int max_id = - 1;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(Consts.get_max_ID("teachers"), sqlcon))
                        {
                            using (DataTable tb = new DataTable())
                            {
                                adapter.Fill(tb);
                                if (tb.Rows.Count != 0)
                                    max_id = (int)tb.Rows[0]["mm"];
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO teachers(ID, username, password, nickname) VALUES(@id, @username, @password, @nickname)", sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@id", max_id + 1);
                            cmd.Parameters.AddWithValue("@username", Consts.username_register);
                            cmd.Parameters.AddWithValue("@password", Consts.password_register);
                            cmd.Parameters.AddWithValue("@nickname", Consts.nickname_register);
                            cmd.ExecuteNonQuery();
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM teachers WHERE ID = " + (max_id + 1).ToString(), sqlcon))
                        {
                            adapter.Fill(Teacher_DataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
            System.Windows.Forms.MessageBox.Show("添加成功！");
        }
        private void Teacher_Update_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Teacher_ListView.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("当前未选择任何项！");
                return;
            }
            DataRowView row = (DataRowView)Teacher_ListView.SelectedItem;
            Consts.password_register = "";
            Consts.username_register = (string)row["username"];
            Consts.nickname_register = (string)row["nickname"];
            Consts.title_register = "用户修改";
            Register registerwindow = new Register();
            registerwindow.ShowDialog();
            if (Consts.password_register.Length != 64) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE teachers SET username = '" + Consts.username_register + "', password = '" + Consts.password_register + "', nickname = @nickname WHERE ID = " + row["ID"].ToString(), sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@nickname", Consts.nickname_register);
                        cmd.ExecuteNonQuery();
                    }
                    row["username"] = Consts.username_register;
                    row["password"] = Consts.password_register;
                    row["nickname"] = Consts.nickname_register;
                    Teacher_ListView.SelectedItem = row;
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
                return;
            }
            System.Windows.Forms.MessageBox.Show("修改成功！");
        }
        private void Teacher_Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Teacher_ListView.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("当前未选择任何项！");
                return;
            }
            MessageBoxButtons btn1 = MessageBoxButtons.YesNo;
            if (System.Windows.Forms.MessageBox.Show("你确认要删除这条信息吗？", "警告", btn1) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                    {
                        sqlcon.Open();
                        DataRowView row = (DataRowView)Teacher_ListView.SelectedItem;
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM teachers WHERE ID = " + ((int)row["ID"]).ToString(), sqlcon))
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
                Teacher_DataTable.Rows.RemoveAt(Teacher_ListView.SelectedIndex);
                System.Windows.Forms.MessageBox.Show("删除成功！");
            }
        }
        private void Student_Search_Button_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            try
            {
                if (Student_Search_ID.Text.Length > 0)
                    id = Convert.ToInt32(Student_Search_ID.Text);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("ID输入有误！");
                return;
            }
            if (Consts.Check_Username(Student_Search_Name.Text)) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    Student_DataTable.Clear();
                    string sqlquery = "SELECT * FROM students";
                    string idquery = "";
                    string namequery = "";
                    if (Student_Search_ID.Text != "") idquery = " ID = " + id.ToString();
                    if (Student_Search_Name.Text != "") namequery = " CHARINDEX('" + Student_Search_Name.Text + "', username) <> 0";
                    if (idquery + namequery != "") sqlquery += " WHERE";
                    if (idquery != "" && namequery != "") idquery += " AND";
                    sqlquery += idquery + namequery;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlquery, sqlcon))
                        adapter.Fill(Student_DataTable);
                    if (Student_DataTable.Rows.Count == 0)
                        Consts.SQL_No_Result();
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
        private void Student_Insert_Button_Click(object sender, RoutedEventArgs e)
        {
            Consts.password_register = Consts.username_register = Consts.nickname_register = "";
            Consts.title_register = "添加用户";
            Register registerform = new Register();
            registerform.ShowDialog();
            if (Consts.password_register.Length != 64) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    bool flag = false;
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM students WHERE username = '" + Consts.username_register + "'", sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        flag = tb.Rows.Count != 0;
                    }
                    if (flag)
                    {
                        MessageBoxButtons btn1 = MessageBoxButtons.YesNo;
                        if (System.Windows.Forms.MessageBox.Show("此用户名的学生已存在！是否更新此学生的信息？", "警告", btn1) == System.Windows.Forms.DialogResult.Yes)
                        {
                            using (SqlCommand cmd = new SqlCommand("UPDATE students SET password = '" + Consts.password_register + "', nickname = @nickname WHERE username = '" + Consts.username_register + "'", sqlcon))
                            {
                                cmd.Parameters.AddWithValue("@nickname", Consts.nickname_register);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else return;
                    }
                    else
                    {
                        int max_id = -1;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(Consts.get_max_ID("students"), sqlcon))
                        {
                            using (DataTable tb = new DataTable())
                            {
                                adapter.Fill(tb);
                                if (tb.Rows.Count != 0)
                                    max_id = (int)tb.Rows[0]["mm"];
                            }
                        }
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO students(ID, username, password, nickname) VALUES(@id, @username, @password, @nickname)", sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@id", max_id + 1);
                            cmd.Parameters.AddWithValue("@username", Consts.username_register);
                            cmd.Parameters.AddWithValue("@password", Consts.password_register);
                            cmd.Parameters.AddWithValue("@nickname", Consts.nickname_register);
                            cmd.ExecuteNonQuery();
                        }
                        using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM students WHERE ID = " + (max_id + 1).ToString(), sqlcon))
                        {
                            adapter.Fill(Student_DataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
            System.Windows.Forms.MessageBox.Show("添加成功！");
        }
        private void Student_Update_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Student_ListView.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("当前未选择任何项！");
                return;
            }
            DataRowView row = (DataRowView)Student_ListView.SelectedItem;
            Consts.password_register = "";
            Consts.username_register = (string)row["username"];
            Consts.nickname_register = (string)row["nickname"];
            Consts.title_register = "用户修改";
            Register registerwindow = new Register();
            registerwindow.ShowDialog();
            if (Consts.password_register.Length != 64) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE students SET username = '" + Consts.username_register + "', password = '" + Consts.password_register + "', nickname = @nickname WHERE ID = " + row["ID"].ToString(), sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@nickname", Consts.nickname_register);
                        cmd.ExecuteNonQuery();
                    }
                    row["username"] = Consts.username_register;
                    row["password"] = Consts.password_register;
                    row["nickname"] = Consts.nickname_register;
                    Student_ListView.SelectedItem = row;
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
                return;
            }
            System.Windows.Forms.MessageBox.Show("修改成功！");
        }
        private void Student_Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Student_ListView.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("当前未选择任何项！");
                return;
            }
            MessageBoxButtons btn1 = MessageBoxButtons.YesNo;
            if (System.Windows.Forms.MessageBox.Show("你确认要删除这条信息吗？", "警告", btn1) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                    {
                        sqlcon.Open();
                        DataRowView row = (DataRowView)Student_ListView.SelectedItem;
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM students WHERE ID = " + ((int)row["ID"]).ToString(), sqlcon))
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
                Student_DataTable.Rows.RemoveAt(Student_ListView.SelectedIndex);
                System.Windows.Forms.MessageBox.Show("删除成功！");
            }
        }
        private void Course_Search_Button_Click(object sender, RoutedEventArgs e)
        {
            int id = -1, teacher_id = -1;
            try
            {
                if (Course_Search_ID.Text.Length > 0)
                    id = Convert.ToInt32(Course_Search_ID.Text);
                if (Course_Teacher_Search_ID.Text.Length > 0)
                    teacher_id = Convert.ToInt32(Course_Teacher_Search_ID.Text);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("ID输入有误！");
                return;
            }
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    Course_DataTable.Clear();
                    string sqlquery = "SELECT courses.ID, name, teachers_ID, username, nickname FROM courses, teachers WHERE teachers_ID = teachers.ID";
                    string idquery = "";
                    string namequery = "";
                    string teacheridquery = "";
                    string teachernamequery = "";
                    if (Course_Search_ID.Text != "") idquery = " AND courses.ID = " + id.ToString();
                    if (Course_Search_Name.Text != "") namequery = " AND CHARINDEX(@name, name) <> 0";
                    if (Course_Teacher_Search_ID.Text != "") teacheridquery = " AND teachers.ID = " + teacher_id.ToString();
                    if (Course_Teacher_Search_Name.Text != "") teachernamequery = " AND CHARINDEX(@teachername, username) <> 0";
                    sqlquery += idquery + namequery + teacheridquery + teachernamequery;
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlquery, sqlcon))
                        {
                            if (Course_Search_Name.Text != "") cmd.Parameters.AddWithValue("@name", Course_Search_Name.Text);
                            if (Course_Teacher_Search_Name.Text != "") cmd.Parameters.AddWithValue("@teachername", Course_Teacher_Search_Name.Text);
                            adapter.SelectCommand = cmd;
                            adapter.Fill(Course_DataTable);
                            if (Course_DataTable.Rows.Count == 0)
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
        private void Course_Insert_Button_Click(object sender, RoutedEventArgs e)
        {
            Consts.t_id = -1;
            Consts.title_register = "";
            Consts.t_lock = false;
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
                        if (tb.Rows.Count != 0)
                            max_id = (int)tb.Rows[0]["mm"];
                    }
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO courses(ID, name, teachers_ID) VALUES(@id, @name, @tid)", sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@id", max_id + 1);
                        cmd.Parameters.AddWithValue("@name", Consts.title_register);
                        cmd.Parameters.AddWithValue("@tid", Consts.t_id);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT courses.ID, name, teachers_ID, username, nickname FROM courses, teachers WHERE teachers_ID = teachers.ID AND courses.ID = " + (max_id + 1).ToString(), sqlcon))
                    {
                        adapter.Fill(Course_DataTable);
                    }
                }
                System.Windows.Forms.MessageBox.Show("添加成功！");
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
        private void Course_Update_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Course_ListView.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("当前未选择任何项！");
                return;
            }
            DataRowView row = (DataRowView)Course_ListView.SelectedItem;
            Consts.t_id = (int)row["teachers_ID"];
            Consts.title_register = (string)row["name"];
            Consts.t_lock = false;
            New_Course coursewindow = new New_Course();
            coursewindow.ShowDialog();
            if (Consts.t_id == -1) return;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlCommand cmd = new SqlCommand("UPDATE courses SET teachers_ID = " + Consts.t_id.ToString() + ", name = @name WHERE ID = " + ((int)row["ID"]).ToString(), sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@name", Consts.title_register);
                        cmd.ExecuteNonQuery();
                    }
                    row["teachers_ID"] = Consts.t_id;
                    row["name"] = Consts.title_register;
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT username FROM teachers WHERE ID = " + Consts.t_id.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        row["username"] = tb.Rows[0]["username"];
                    }
                    Course_ListView.SelectedItem = row;
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
                return;
            }
            System.Windows.Forms.MessageBox.Show("修改成功！");
        }
        private void Course_Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Course_ListView.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("当前未选择任何项！");
                return;
            }
            MessageBoxButtons btn1 = MessageBoxButtons.YesNo;
            if (System.Windows.Forms.MessageBox.Show("你确认要删除这条信息吗？", "警告", btn1) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                    {
                        sqlcon.Open();
                        DataRowView row = (DataRowView)Course_ListView.SelectedItem;
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM courses WHERE ID = " + ((int)row["ID"]).ToString(), sqlcon))
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
                Course_DataTable.Rows.RemoveAt(Course_ListView.SelectedIndex);
                System.Windows.Forms.MessageBox.Show("删除成功！");
            }
        }
        private void Sql_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (char i in Sql_Input.Text)
            {
                if (!(i >= 0 && i < 128))
                {
                    MessageBoxButtons btn = MessageBoxButtons.YesNo;
                    System.Windows.Forms.DialogResult res = System.Windows.Forms.MessageBox.Show("包含非ASCII字符！直接执行可能会导致非ASCII字符（如汉字）插入时出错，是否确认要执行？", "警告", btn);
                    if (res == System.Windows.Forms.DialogResult.Yes) break;
                    if (res == System.Windows.Forms.DialogResult.No) return;
                }
            }
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(Sql_Input.Text, sqlcon))
                    {
                        Sql_DataTable.Columns.Clear();
                        Sql_DataTable.Rows.Clear();
                        Sql_DataTable.Clear();
                        adapter.Fill(Sql_DataTable);
                        //System.Windows.Forms.MessageBox.Show(Sql_DataTable.Rows.Count.ToString());
                        /*Sql_DataGrid.Columns.Clear();
                        foreach (DataColumn col in Sql_DataTable.Columns)
                        {
                            System.Windows.Forms.MessageBox.Show(col.ColumnName);
                            DataGridTextColumn dgcol = new DataGridTextColumn();
                            dgcol.Binding = new System.Windows.Data.Binding(col.ColumnName);
                            dgcol.Header = col.ColumnName;
                            dgcol.Width = 50;
                            Sql_DataGrid.Columns.Add(dgcol);
                        }*/
                        ((GridView)Sql_ListView.View).Columns.Clear();
                        foreach (DataColumn col in Sql_DataTable.Columns)
                        {
                            GridViewColumn gcol = new GridViewColumn();
                            gcol.Header = col.ColumnName;
                            gcol.DisplayMemberBinding = new System.Windows.Data.Binding(col.ColumnName);
                            ((GridView)Sql_ListView.View).Columns.Add(gcol);
                        }
                    }
                }
                System.Windows.Forms.MessageBox.Show("成功执行");
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }
        private void Hide_All()
        {
            Student_Management_Grid.Visibility = System.Windows.Visibility.Hidden;
            Teacher_Management_Grid.Visibility = System.Windows.Visibility.Hidden;
            Course_Management_Grid.Visibility = System.Windows.Visibility.Hidden;
            XML_Grid.Visibility = System.Windows.Visibility.Hidden;
            SQL_Grid.Visibility = System.Windows.Visibility.Hidden;
        }
        private void Teacher_Management_button_Click(object sender, RoutedEventArgs e)
        {
            Hide_All();
            Teacher_Management_Grid.Visibility = System.Windows.Visibility.Visible;
            Title_Textblock.Text = (string)Teacher_Management_button.Content;
        }
        private void Student_Management_button_Click(object sender, RoutedEventArgs e)
        {
            Hide_All();
            Student_Management_Grid.Visibility = System.Windows.Visibility.Visible;
            Title_Textblock.Text = (string)Student_Management_button.Content;
        }
        private void Course_Management_button_Click(object sender, RoutedEventArgs e)
        {
            Hide_All();
            Course_Management_Grid.Visibility = System.Windows.Visibility.Visible;
            Title_Textblock.Text = (string)Course_Management_button.Content;
        }
        private void XML_button_Click(object sender, RoutedEventArgs e)
        {
            Hide_All();
            XML_Grid.Visibility = System.Windows.Visibility.Visible;
            Title_Textblock.Text = (string)XML_button.Content;
        }
        private void SQL_button_Click_1(object sender, RoutedEventArgs e)
        {
            Hide_All();
            SQL_Grid.Visibility = System.Windows.Visibility.Visible;
            Title_Textblock.Text = (string)SQL_button.Content;
        }
    }
}
