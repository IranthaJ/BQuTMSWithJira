using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.Odbc;
using System.Data;
using System.Threading;

namespace BQuTMSWithJira
{
    public partial class IdleTimeTrack : Window
    {
        private DateTime stime;
        string commnet = String.Empty;
        string etime = String.Empty;

        public IdleTimeTrack()
        {
            InitializeComponent();
        }

        //when show this dialog box capture times and recode this one into database at this time
        public void update()
        {
            stime_lab.Content = stime.ToString("hh:mm tt");
            etime_lab.Content = DateTime.Now.ToString("hh:mm tt");
            TimeSpan span = (DateTime.Now).Subtract(stime);
            heding_tBlock.Text = "You were away for " + Math.Round(Convert.ToDecimal(span.TotalMinutes)) + " minutes. Please comment.";
            try
            {
                Connection.tempConnection.Open();
                if (Connection.tempConnection.State == ConnectionState.Open)
                {
                    using (OdbcCommand MyCommand = new OdbcCommand("insert into tms_idle_time (user_id,idle_stime,idle_etime,date,comment) values ('" + Login.userid + "','" + stime.ToString("HH:mm").ToString() + "','" + DateTime.Now.ToString("HH:mm") + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + (comment_tBox.Text).Replace("'", "\\'") + "')  ", Connection.tempConnection))
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {

            }
            if (Connection.tempConnection.State == ConnectionState.Open)
            {
                Connection.tempConnection.Close();
            }
        }

        public DateTime STIME
        {
            get
            {
                return stime;
            }
            set
            {
                stime = value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, comment_tBox);
        }

        //when click apply button update reason feild in the database table for idle 
        private void submit_but_Click(object sender, RoutedEventArgs e)
        {
            commnet = comment_tBox.Text;
            etime = etime_lab.Content.ToString();
            Thread t = new Thread(new ThreadStart(UpdateThread));
            t.Start();
            this.Hide();
        }

        void UpdateThread()
        {
            Thread.Sleep(100);
            Connection.tempConnection.Open();
            if (Connection.tempConnection.State == ConnectionState.Open)
            {
                using (OdbcCommand MyCommand = new OdbcCommand("update tms_idle_time set comment='" + (commnet).Replace("'", "\\'") + "' where user_id= '" + Login.userid + "' and idle_stime ='" + stime.ToString("HH:mm").ToString() + "' and idle_etime='" + Convert.ToDateTime(etime).ToString("HH:mm") + "' and date='" + DateTime.Now.ToString("yyyy-MM-dd") + "' ", Connection.tempConnection))
                {
                    MyCommand.ExecuteNonQuery();
                }
                Connection.tempConnection.Close();
            }
        }

        private void comment_tBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusManager.SetFocusedElement(this, submit_but);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            submit_but_Click(null, null);
        }
    }
}
