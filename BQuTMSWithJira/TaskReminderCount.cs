using System;
using System.Data;
using Microsoft.Data.Odbc;
using System.Windows.Threading;

namespace BQuTMSWithJira
{
    class TaskReminderCount
    {
        public static int rid;
        static DateTime duedate;
        static DispatcherTimer dispatcherTimer;
        static bool timerflag = false;

        public static void TaskCount()
        {
            rid = 0;
            try
            {
                Connection.tempConnection.Open();
                if (Connection.tempConnection.State == ConnectionState.Open)
                {
                    using (OdbcCommand MyCommand = new OdbcCommand("SELECT rid,duedate FROM tms_reminders WHERE assignedto='" + Login.userid + "' AND duedate> '" + Util.getDateTime().AddMinutes(1).ToString("yyyy-MM-dd HH:mm") + "' AND DATE(duedate)='" + Util.getDateTime().ToString("yyyy-MM-dd") + "' ORDER BY duedate LIMIT 1", Connection.tempConnection))
                    {
                        OdbcDataReader MyDataReader = MyCommand.ExecuteReader();
                        MyDataReader.Read();
                        rid = MyDataReader.GetInt32(0);
                        duedate = MyDataReader.GetDateTime(1);
                        MyDataReader.Close();
                        if (timerflag && rid == 0)
                        {
                            dispatcherTimer.Stop();
                        }
                    }
                    Connection.tempConnection.Close();
                }
            }
            catch (Exception)
            {
                if (Connection.tempConnection.State == ConnectionState.Open)
                {
                    Connection.tempConnection.Close();
                }
            }
        }

        public static void startTaskRemindTimer()
        {
            timerflag = true;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (duedate.ToString("yyyy-MM-dd HH:mm") == Util.getDateTime().ToString("yyyy-MM-dd HH:mm"))
            {
                TaskReminderWindow trw = new TaskReminderWindow();
                trw.RemID = rid;
                trw.Show();
                TaskCount(); //call again and get next task in this date            
                if (rid == 0)
                {
                    dispatcherTimer.Stop();
                }
            }
        }
    }
}
