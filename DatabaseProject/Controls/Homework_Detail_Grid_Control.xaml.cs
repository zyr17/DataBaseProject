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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseProject
{
    /// <summary>
    /// Homework_Detail_Grid_Control.xaml 的交互逻辑
    /// </summary>
    public partial class Homework_Detail_Grid_Control : UserControl
    {
        public double max_text_length = 0;
        public Homework_Detail_Grid_Control()
        {
            InitializeComponent();
        }
        public string Homework_Title
        {
            get
            {
                return Homework_Title_TextBlock.Text;
            }
            set
            {
                string s = "";
                if (Consts.MeasureTextWidth(value, Homework_Title_TextBlock.FontSize, "微软雅黑", Homework_Title_TextBlock.FontWeight) <= max_text_length)
                {
                    Homework_Title_TextBlock.Text = value;
                    return;
                }
                string s2 = "...";
                foreach (char i in value)
                {
                    s2 += i;
                    if (Consts.MeasureTextWidth(s2, Homework_Title_TextBlock.FontSize, "微软雅黑", Homework_Title_TextBlock.FontWeight) > max_text_length)
                    {
                        Homework_Title_TextBlock.Text = (s == "" ? value[0].ToString() : s) + "...";
                        return;
                    }
                    s += i;
                }
            }
        }
        public string Homework_Content
        {
            get
            {
                return Homework_Content_TextBlock.Text;
            }
            set
            {
                value = value.Replace(Environment.NewLine, " ");
                string s = "";
                if (Consts.MeasureTextWidth(value, Homework_Content_TextBlock.FontSize, "微软雅黑", Homework_Content_TextBlock.FontWeight) <= max_text_length)
                {
                    Homework_Content_TextBlock.Text = value;
                    return;
                }
                string s2 = "...";
                foreach (char i in value)
                {
                    s2 += i;
                    //System.Windows.Forms.MessageBox.Show(MeasureTextWidth(s2, 16, "微软雅黑").ToString());
                    if (Consts.MeasureTextWidth(s2, Homework_Content_TextBlock.FontSize, "微软雅黑", Homework_Content_TextBlock.FontWeight) > max_text_length)
                    {
                        Homework_Content_TextBlock.Text = (s == "" ? value[0].ToString() : s) + "...";
                        return;
                    }
                    s += i;
                }
            }
        }
        private System.DateTime h_time;
        public System.DateTime Homework_Time
        {
            get
            {
                return h_time;
            }
            set
            {
                h_time = value;
                string shortdate = value.ToString("yyyy.MM.dd");
                string date = value.ToString("yyyy年MM月dd日");
                string datetime = date + " " + value.ToString("hh:mm:ss");
                if (value == Consts.Unlimited_Time) shortdate = date = datetime = "无期限";
                string longtime = "作业截止时间：" + datetime;
                Homework_Time_TextBlock.Text = Consts.MeasureTextWidth(longtime, Homework_Time_TextBlock.FontSize, "微软雅黑", Homework_Time_TextBlock.FontWeight) <= max_text_length ? longtime :
                                               Consts.MeasureTextWidth(datetime, Homework_Time_TextBlock.FontSize, "微软雅黑", Homework_Time_TextBlock.FontWeight) <= max_text_length ? datetime :
                                               Consts.MeasureTextWidth(date, Homework_Time_TextBlock.FontSize, "微软雅黑", Homework_Time_TextBlock.FontWeight) <= max_text_length ? date :
                                               Consts.MeasureTextWidth(shortdate, Homework_Time_TextBlock.FontSize, "微软雅黑", Homework_Time_TextBlock.FontWeight) <= max_text_length ? shortdate : "...";
                double deldays = (value - DateTime.UtcNow).TotalDays;
                Homework_Time_TextBlock.Foreground = deldays > 30 ? Consts.Time_Colors[0] :
                                                     deldays > 14 ? Consts.Time_Colors[1] :
                                                     deldays > 0 ? Consts.Time_Colors[2] :
                                                     deldays > -7 ? Consts.Time_Colors[3] : Consts.Time_Colors[4];
                Homework_Time_TextBlock.FontWeight = deldays > 30 ? Consts.Time_FontWeights[0] :
                                                     deldays > 14 ? Consts.Time_FontWeights[1] :
                                                     deldays > 0 ? Consts.Time_FontWeights[2] :
                                                     deldays > -7 ? Consts.Time_FontWeights[3] : Consts.Time_FontWeights[4];
            }
        }
    }
}
