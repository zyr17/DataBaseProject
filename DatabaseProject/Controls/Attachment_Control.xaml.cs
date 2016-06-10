using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseProject
{
    /// <summary>
    /// Attachment_Control.xaml 的交互逻辑
    /// </summary>
    public partial class Attachment_Control : System.Windows.Controls.UserControl
    {
        private string db_table_att;
        private string db_table_fa;
        public int update_id;
        private DataTable Attachment_DataTable = new DataTable();
        private ObservableCollection<string> Attachment_List = new ObservableCollection<string>();
        public Attachment_Control()
        {
            InitializeComponent();
            Attachment_ListBox.ItemsSource = Attachment_List;
        }

        public string db_table_string
        {
            get
            {
                return db_table_att;
            }
            set
            {
                if (value == "homework_attachments")
                {
                    db_table_att = value;
                    db_table_fa = "homeworks";
                }
                else if (value == "homework_entity_attachments")
                {
                    db_table_att = value;
                    db_table_fa = "homework_entities";
                }
                else throw new Exception();
            }
        }

        public string title_textblock_string
        {
            get
            {
                return Title_TextBlock.Text;
            }
            set
            {
                Title_TextBlock.Text = value;
            }
        }

        public double font_size
        {
            get
            {
                return Title_TextBlock.FontSize;
            }
            set
            {
                Title_TextBlock.FontSize = Insert_Button.FontSize = Delete_Button.FontSize = Download_Button.FontSize = value;
            }
        }

        public bool Can_Edit
        {
            set
            {
                Insert_Button.IsEnabled = Delete_Button.IsEnabled = value;
            }
        }

        public void Update_Attachment_List()
        {
            using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
            {
                sqlcon.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM " + db_table_att + " WHERE " + db_table_fa + "_ID = " + update_id.ToString(), sqlcon))
                {
                    Attachment_DataTable.Clear();
                    adapter.Fill(Attachment_DataTable);
                    Attachment_List.Clear();
                    foreach (DataRow row in Attachment_DataTable.Rows)
                        Attachment_List.Add((string)row["filename"]);
                }
            }
        }

        private void Insert_Attachment_Button_Click(object sender, RoutedEventArgs e)
        {
            Consts.Save_Attachment_To_DB(db_table_att, update_id);
            Update_Attachment_List();
        }

        private void Delete_Attachment_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Attachment_ListBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("未选择任何项！");
                return;
            }
            int select_id = (int)Attachment_DataTable.Rows[Attachment_ListBox.SelectedIndex]["ID"];
            MessageBoxButtons btn = MessageBoxButtons.YesNo;
            if (System.Windows.Forms.MessageBox.Show("你确定要删除这个附件吗？这个操作无法撤销！", "警告", btn) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection sqlcon = new SqlConnection(Consts.constring))
                    {
                        sqlcon.Open();
                        string filepos = "";
                        using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT attachment FROM " + db_table_att + " WHERE ID = " + select_id.ToString(), sqlcon))
                        {
                            DataTable tb = new DataTable();
                            adapter.Fill(tb);
                            filepos = (string)tb.Rows[0]["attachment"];
                        }
                        if (File.Exists(Consts.attachment_save_directory + filepos))
                            File.Delete(Consts.attachment_save_directory + filepos);
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM " + db_table_att + " WHERE ID = " + select_id.ToString(), sqlcon))
                            cmd.ExecuteNonQuery();
                    }
                    System.Windows.Forms.MessageBox.Show("删除附件成功！");
                    Update_Attachment_List();
                }
                catch (Exception ex)
                {
                    Consts.Output_Database_Err(ex);
                }
            }
        }

        private void Download_Attachment_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Attachment_ListBox.SelectedItem == null)
            {
                System.Windows.Forms.MessageBox.Show("未选择任何项！");
                return;
            }
            int select_id = (int)Attachment_DataTable.Rows[Attachment_ListBox.SelectedIndex]["ID"];
            Consts.Load_Attachment_To_File(db_table_att, select_id);
        }
    }
}
