using System;
using System.Windows;
using System.Windows.Input;
using System.Data;
using Microsoft.Data.Odbc;

namespace BQuTMSWithJira
{
    /// <summary>
    /// Interaction logic for TaskReminderWindow.xaml
    /// </summary>
    public partial class TaskReminderWindow : Window
    {
        private int remid = 0;
        public TaskReminderWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Connection.MyConnection.State == ConnectionState.Broken)
                Connection.MyConnection.Open();
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                using (OdbcCommand MyCommand = new OdbcCommand("SELECT duedate,priority,reminder,minute,note FROM tms_reminders WHERE rid='" + TaskReminderCount.rid + "'", Connection.MyConnection))
                {
                    OdbcDataReader MyDataReader;
                    MyDataReader = MyCommand.ExecuteReader();
                    MyDataReader.Read();
                    duedateLab.Content = MyDataReader.GetDateTime(0).ToString("yyyy-MM-dd hh:mm tt ");
                    priolab.Content = MyDataReader.GetString(1);
                    title_Lab.Content = MyDataReader.GetString(2);
                    noofhourlab.Content = TimeDisplay(MyDataReader.GetInt32(3));
                    string tempcomment = MyDataReader.GetString(4);
                    if (tempcomment.Length == 0)
                    {
                        commentlab.Text = "<no descrption>";
                    }
                    else
                    {
                        commentlab.Text = MyDataReader.GetString(4);
                    }
                    MyDataReader.Close();
                }
            }
        }

        private String TimeDisplay(int mins)
        {
            int hours = (mins - mins % 60) / 60;
            string smin = (mins - hours * 60).ToString();
            if (smin.Length != 2)
            {
                smin = "0" + smin;
            }
            return (hours + "." + smin).ToString();
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public int RemID
        {
            get
            {
                return remid;
            }
            set
            {
                remid = value;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
