using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Course_Data_Detail.xaml 的交互逻辑
    /// </summary>
    public partial class Course_Data_Detail : Window
    {
        private DataTable Homeworks_DataTable = new DataTable();
        private DataTable Statistics_DataTable = new DataTable();
        private ObservableCollection<string> Homeworks_List = new ObservableCollection<string>();
        private DataTable Checked_DataTable = new DataTable();
        private DataTable Unchecked_DataTable = new DataTable();
        private DataTable Unsubmitted_DataTable = new DataTable();
        private int Homework_ID = -1;
        private int Course_ID;
        public Course_Data_Detail(int cid)
        {
            InitializeComponent();
            Course_ID = cid;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT name FROM courses WHERE ID = " + cid.ToString(), sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        this.Title = "课程 " + (string)tb.Rows[0]["name"] + " 的详细数据";
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
            Homeworks_ComboBox.ItemsSource = Homeworks_List;
            Checked_ListView.ItemsSource = Checked_DataTable.DefaultView;
            Unchecked_ListView.ItemsSource = Unchecked_DataTable.DefaultView;
            Unsubmitted_ListView.ItemsSource = Unsubmitted_DataTable.DefaultView;
            Statistics_ListView.ItemsSource = Statistics_DataTable.DefaultView;
            Update_ComboBox_And_Homework_List();
        }

        private void Update_ComboBox_And_Homework_List()
        {
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homeworks WHERE courses_ID = " + Course_ID.ToString(), sqlcon))
                        adapter.Fill(Homeworks_DataTable);
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
            foreach (DataRow i in Homeworks_DataTable.Rows)
            {
                Homeworks_List.Add((string)i["title"]);
                Homework_And_Weight_Control hwctrl = new Homework_And_Weight_Control((int)i["ID"]);
                Homeworks_StackPanel.Children.Add(hwctrl);
            }
        }

        private void Update_Student_Homework_Data()
        {
            Checked_DataTable.Clear();
            Unchecked_DataTable.Clear();
            Unsubmitted_DataTable.Clear();
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT nickname, score FROM homework_entities, students WHERE is_submitted = 1 AND is_returned = 1 AND students_ID = students.ID AND homeworks_ID = " + Homework_ID.ToString(), sqlcon))
                        adapter.Fill(Checked_DataTable);
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT nickname FROM homework_entities, students WHERE is_submitted = 1 AND is_returned = 0 AND students_ID = students.ID AND homeworks_ID = " + Homework_ID.ToString(), sqlcon))
                        adapter.Fill(Unchecked_DataTable);
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT nickname FROM homework_entities, students WHERE is_submitted = 0 AND is_returned = 0 AND students_ID = students.ID AND homeworks_ID = " + Homework_ID.ToString(), sqlcon))
                        adapter.Fill(Unsubmitted_DataTable);
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
            Checked_Num_TextBlock.Text = "总人数：" + Checked_DataTable.Rows.Count.ToString();
            Unchecked_Num_TextBlock.Text = "总人数：" + Unchecked_DataTable.Rows.Count.ToString();
            Unsubmitted_Num_TextBlock.Text = "总人数：" + Unsubmitted_DataTable.Rows.Count.ToString();
            double tot = 0;
            foreach (DataRow i in Checked_DataTable.Rows)
                tot += (int)i["score"];
            Average_Score_TextBlock.Text = "平均分：" + (Checked_DataTable.Rows.Count == 0 ? "-" : (tot / Checked_DataTable.Rows.Count).ToString());
            Total_Score_TextBlock.Text = "总分：" + ((int)Homeworks_DataTable.Rows[Homeworks_ComboBox.SelectedIndex]["totalscore"]).ToString();
        }

        private void Homeworks_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Homework_ID = (int)Homeworks_DataTable.Rows[Homeworks_ComboBox.SelectedIndex]["ID"];
            Update_Student_Homework_Data();
        }

        private void Statictic_Calculate_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var i in Homeworks_StackPanel.Children)
            {
                if (i is Homework_And_Weight_Control)
                {
                    try
                    {
                        double ttt = Convert.ToDouble(((Homework_And_Weight_Control)i).weight_string);
                    }
                    catch
                    {
                        ((Homework_And_Weight_Control)i).Focus();
                        System.Windows.Forms.MessageBox.Show("有作业未填入合法的权重！请填入合法的整数或浮点数");
                        return;
                    }
                }
            }
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                {
                    sqlcon.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM homework_entities WHERE is_submitted = 1 AND is_returned = 0 AND homeworks_ID IN (SELECT ID FROM homeworks WHERE courses_ID = " + Course_ID.ToString() + ")", sqlcon))
                    {
                        DataTable tb = new DataTable();
                        adapter.Fill(tb);
                        if (tb.Rows.Count != 0)
                            System.Windows.Forms.MessageBox.Show("你有 " + tb.Rows.Count.ToString() + " 项作业学生已提交但未批改，这些作业都将被作为0分处理！");
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT T1.ID, nickname, '0' AS score, COUNT(is_submitted) AS unsubmitted FROM (SELECT ID, nickname FROM students, students_has_courses WHERE ID = students_ID AND courses_ID = " + Course_ID.ToString() + ") AS T1 FULL OUTER JOIN (SELECT students_ID, is_submitted FROM homework_entities WHERE is_submitted = 0 AND homeworks_ID IN (SELECT ID FROM homeworks WHERE courses_ID = " + Course_ID.ToString() + ")) AS T2 ON T1.ID = students_ID GROUP BY T1.ID, nickname", sqlcon))
                    {
                        Statistics_DataTable.Clear();
                        adapter.Fill(Statistics_DataTable);
                    }
                    double total_score = 0, total_average = 0;
                    foreach (var i in Homeworks_StackPanel.Children)
                    {
                        if (i is Homework_And_Weight_Control)
                        {
                            total_score += Convert.ToDouble(((Homework_And_Weight_Control)i).weight_string) * ((Homework_And_Weight_Control)i).homework_totalscore;
                        }
                    }
                    foreach (DataRow row in Statistics_DataTable.Rows)
                    {
                        double score = 0, total_weight = 0;
                        int sid = (int)row["ID"];
                        foreach (var i in Homeworks_StackPanel.Children)
                        {
                            if (i is Homework_And_Weight_Control)
                            {
                                double ttt = Convert.ToDouble(((Homework_And_Weight_Control)i).weight_string);
                                total_weight += ttt;
                                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT score FROM homework_entities WHERE students_ID = " + sid.ToString() + " AND homeworks_ID = " + ((Homework_And_Weight_Control)i).Homework_ID.ToString(), sqlcon))
                                {
                                    DataTable tb = new DataTable();
                                    adapter.Fill(tb);
                                    if ((bool)RadioButton1.IsChecked) score += (int)tb.Rows[0]["score"] * ttt;
                                    else score += (int)tb.Rows[0]["score"] * 1.0 / ((Homework_And_Weight_Control)i).homework_totalscore * ttt;
                                }
                            }
                        }
                        if ((bool)RadioButton1.IsChecked)
                        {
                            row["score"] = score.ToString("f1");
                            total_average += score;
                        }
                        else
                        {
                            row["score"] = (score / total_weight * 100).ToString("f1");
                            total_average += score / total_weight * 100;
                        }
                    }
                    if ((bool)RadioButton1.IsChecked)
                    {
                        Statistics_Total_Score_TextBlock.Text = "总分：" + total_score.ToString();
                        Statistics_Average_Score_TextBlock.Text = "平均分：" + (total_average / Statistics_DataTable.Rows.Count).ToString("f1");
                    }
                    else
                    {
                        Statistics_Total_Score_TextBlock.Text = "总分：100";
                        Statistics_Average_Score_TextBlock.Text = "平均分：" + (total_average / Statistics_DataTable.Rows.Count).ToString("f1");
                    }
                }
            }
            catch (Exception ex)
            {
                Consts.Output_Database_Err(ex);
            }
        }

        private void Main_Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Main_TabControl.Width = this.ActualWidth - 37;
        }
    }
}
