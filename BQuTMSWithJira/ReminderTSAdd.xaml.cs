using System;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Globalization;
using Microsoft.Data.Odbc;
using System.Data;

namespace BQuTMSWithJira
{
    /// <summary>
    /// Interaction logic for ReminderTSAdd.xaml
    /// </summary>
    public partial class ReminderTSAdd : Window
    {
        public ReminderTSAdd()
        {
            InitializeComponent();
        }

        private int id;
        private string note;
        private string hour;
        private int proid;
        private int catid;
        public static bool checkflag = false;

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public String NOTE
        {
            get
            {
                return note;
            }
            set
            {
                note = value;
            }
        }

        public String HOUR
        {
            get
            {
                return hour;
            }
            set
            {
                hour = value;
            }
        }

        public void Load()
        {
            Thread.CurrentThread.CurrentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone(); Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "ddd, MMM dd, yyyy";
            select_dPicker.SelectedDate = DateTime.Now;
            note_tBox.Text = note;
            time_TPicker.Value = Convert.ToDateTime(hour.Replace(".", ":"));
            if (Connection.MyConnection.State == ConnectionState.Broken)
                Connection.MyConnection.Open();
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                using (OdbcCommand MyCommand = new OdbcCommand("SELECT `tms_projects`.`project_name`,`tms_reminders`.`project` ,`tms_category`.`category_name` ,`tms_reminders`.`category` FROM `tms_reminders` INNER JOIN `tms_projects` ON `tms_projects`.`project_id`=`tms_reminders`.`project` INNER JOIN `tms_category` ON `tms_category`.`category_id`=`tms_reminders`.`category` WHERE `tms_reminders`.`rid`='" + id + "'", Connection.MyConnection))
                {
                    OdbcDataReader MyDataReader;
                    MyDataReader = MyCommand.ExecuteReader();
                    MyDataReader.Read();
                    proname_wTBox.Text = MyDataReader.GetString(0);
                    proid = MyDataReader.GetInt32(1);
                    catename_wTBox.Text = MyDataReader.GetString(2);
                    catid = MyDataReader.GetInt32(3);
                    MyDataReader.Close();
                }
            }
        }

        private void add_but_Click(object sender, RoutedEventArgs e)
        {
            if (select_dPicker.SelectedDate < DateTime.Now)
            {
                int time = Convert.ToDateTime(time_TPicker.Value).Hour * 60 + Convert.ToDateTime(time_TPicker.Value).Minute;
                if (time > 0)
                {
                    int status = (sendApprovedcBox.IsChecked == true) ? 0 : 1;

                    //quary = "INSERT INTO tms_timesheet (project_id,project_name, issue_id,category_name,employee_id,date_year,date_month,date_day,work_time,category_id,category_type_name,start_time,end_time,status) VALUES ('" + projectId + "','" + projname + "','" + issue_id + "','" + issue + "','" + Login.userid + "','" + Convert.ToDateTime(date).Year + "','" + Convert.ToDateTime(date).Month + "','" + Convert.ToDateTime(date).Day + "','" + time + "','" + catId + "','" + categoryname + "','" + Convert.ToDateTime(pstime).ToString("HH:mm:ss") + "','" + Convert.ToDateTime(petime).ToString("HH:mm:ss") + "','" + status + "')";
                    //"INSERT INTO tms_timesheet (project_id, category_name,employee_id,date_year,date_month,date_day,work_time,category_id,status) VALUES ('" + proid + "','" + note_tBox.Text + "','" + Login.userid + "','" + select_dPicker.SelectedDate.Value.Year + "','" + select_dPicker.SelectedDate.Value.Month + "','" + select_dPicker.SelectedDate.Value.Day + "','" + time + "','" + catid + "','" + status + "')"
                    
                    using (OdbcCommand MyCommand = new OdbcCommand("INSERT INTO tms_timesheet (project_id, category_name,employee_id,date_year,date_month,date_day,work_time,category_id,status) VALUES ('" + proid + "','" + note_tBox.Text + "','" + Login.userid + "','" + select_dPicker.SelectedDate.Value.Year + "','" + select_dPicker.SelectedDate.Value.Month + "','" + select_dPicker.SelectedDate.Value.Day + "','" + time + "','" + catid + "','" + status + "')", Connection.MyConnection))
                    {
                        MyCommand.ExecuteNonQuery();//insert
                        MyCommand.CommandText = "DELETE FROM tms_reminders WHERE rid='" + id + "'";
                        MyCommand.ExecuteNonQuery();
                        checkflag = true;
                        sendApprovedcBox.IsChecked = false;
                        this.Hide();
                    }
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Invalid time entering", "BQuTMSWithJira Missing Field(s)", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Sorry, you cannot select future date.", "BQuTMSWithJira Incorrect Date Selection", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void cancalbut_Click(object sender, RoutedEventArgs e)
        {
            checkflag = false;
            sendApprovedcBox.IsChecked = false;
            this.Hide();
        }

        private void close_but_Click(object sender, RoutedEventArgs e)
        {
            checkflag = false;
            sendApprovedcBox.IsChecked = false;
            this.Hide();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                base.OnMouseLeftButtonDown(e);
                DragMove();
            }
            catch (Exception)
            {
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            checkflag = false;
            sendApprovedcBox.IsChecked = false;
            this.Hide();
        }
    }
}
