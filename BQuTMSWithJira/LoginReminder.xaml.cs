using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.Odbc;
using System.Data;
using System.Threading;

namespace BQuTMSWithJira
{
    /// <summary>
    /// Interaction logic for LoginReminder.xaml
    /// </summary>
    public partial class LoginReminder : Window
    {
        private int lgid;
        string outtime = string.Empty;
        bool isUpdated = false;
        public LoginReminder()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            outt_TPicker.Value = Convert.ToDateTime(Login.etimehh + ":" + Login.etimemm);
            using (OdbcCommand MyCommand = new OdbcCommand("select date_year,date_month,date_day from tms_logsheet where logsheet_id='" + lgid + "'", Connection.MyConnection))
            {
                OdbcDataReader MyDataReader;
                MyDataReader = MyCommand.ExecuteReader();
                MyDataReader.Read();
                load_lab.Content = "You haven't sign-out on " + (MyDataReader.GetInt32(2).ToString()) + "/" + (MyDataReader.GetInt32(1).ToString()) + "/" + (MyDataReader.GetInt32(0).ToString()) + ".";
                MyDataReader.Close();
            }
        }

        public int LogSheetId
        {
            get
            {
                return lgid;
            }
            set
            {
                lgid = value;
            }
        }

        private void ok_but_Click(object sender, RoutedEventArgs e)
        {
            isUpdated = true;
            if (Connection.MyConnection.State == ConnectionState.Closed)
            {
                Connection.MyConnection.Open();
            }
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                outtime = outt_TPicker.Value.ToString();
                //Thread t = new Thread(new ThreadStart(InsertThread));
                //t.Start();
                //this.Close();
                using (OdbcCommand MyCommand = new OdbcCommand("update tms_logsheet set out_time='" + Convert.ToDateTime(outtime).ToString("HH:mm") + "' where logsheet_id='" + lgid + "' ", Connection.MyConnection))
                {
                    MyCommand.ExecuteNonQuery();
                }
                this.Close();
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. You need to restart the application", "BQuTMSWithJira Signout Reminder", MessageBoxButton.OK, MessageBoxImage.Stop);
                Environment.Exit(0);
            }
        }


        void InsertThread()
        {
            //Thread.Sleep(5000);
            //if (Connection.tempConnection.State == ConnectionState.Closed)
            //{
            //    Connection.tempConnection.Open();
            //}
            //if (Connection.tempConnection.State == ConnectionState.Open)
            //{
            //    using (OdbcCommand MyCommand = new OdbcCommand("update tms_logsheet set out_time='" + Convert.ToDateTime(outtime).ToString("HH:mm") + "' where logsheet_id='" + lgid + "' ", Connection.tempConnection))
            //    {
            //        MyCommand.ExecuteNonQuery();
            //    }
            //    Connection.tempConnection.Close();
            //}

            Thread.Sleep(5000);
            if (Connection.MyConnection.State == ConnectionState.Closed)
            {
                Connection.MyConnection.Open();
            }
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                using (OdbcCommand MyCommand = new OdbcCommand("update tms_logsheet set out_time='" + Convert.ToDateTime(outtime).ToString("HH:mm") + "' where logsheet_id='" + lgid + "' ", Connection.MyConnection))
                {
                    MyCommand.ExecuteNonQuery();
                }
                // Connection.tempConnection.Close();
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. You need to restart the application", "BQuTMSWithJira Signout Reminder", MessageBoxButton.OK, MessageBoxImage.Stop);
                Environment.Exit(0);
            }
        }
   

        private void ign_but_Click(object sender, RoutedEventArgs e)
        {
            if (Connection.MyConnection.State == ConnectionState.Closed)
            {
                Connection.MyConnection.Open();
            }
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                outtime = Login.etimehh + ":" + Login.etimemm;
                //Thread t = new Thread(new ThreadStart(InsertThread));
                //t.Start();
                //this.Close();
                using (OdbcCommand MyCommand = new OdbcCommand("update tms_logsheet set out_time='" + Convert.ToDateTime(outtime).ToString("HH:mm") + "' where logsheet_id='" + lgid + "' ", Connection.MyConnection))
                {
                    MyCommand.ExecuteNonQuery();
                }
                this.Close();
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. You need to restart the application", "BQuTMSWithJira Signout Reminder", MessageBoxButton.OK, MessageBoxImage.Stop);
                Environment.Exit(0);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isUpdated)
            {
            if (Microsoft.Windows.Controls.MessageBox.Show("Do you want to enter defult time?", "BQuTMSWithJira Signout Reminder", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Connection.MyConnection.State == ConnectionState.Closed)
                {
                    Connection.MyConnection.Open();
                }
                if (Connection.MyConnection.State == ConnectionState.Open)
                {
                    outtime = Login.etimehh + ":" + Login.etimemm;
                    //Thread t = new Thread(new ThreadStart(InsertThread));
                    //t.Start();
                    //this.Close();
                    using (OdbcCommand MyCommand = new OdbcCommand("update tms_logsheet set out_time='" + Convert.ToDateTime(outtime).ToString("HH:mm") + "' where logsheet_id='" + lgid + "' ", Connection.MyConnection))
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                   // this.Close();
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. You need to restart the application", "BQuTMSWithJira Signout Reminder", MessageBoxButton.OK, MessageBoxImage.Stop);
                    Environment.Exit(0);
                }
            }
        }
        }
    }
}
