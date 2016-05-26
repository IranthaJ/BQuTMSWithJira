using System;
using System.Windows.Threading;
using Microsoft.Data.Odbc;
using System.Data;

namespace BQuTMSWithJira
{
    class GetIdleTime
    {
        private DispatcherTimer dispatcherTimer;
        private ClientIdleHandler _clientIdleHandler;
        bool flag = true;
        DateTime stime;
        public int idletimeinerval = 35;// idel time minuts hemal
        public int highestidlle = 500;// idel time minuts

        public void callForIdletime()
        {
            _clientIdleHandler = new ClientIdleHandler();
            _clientIdleHandler.Start();
            //start timer
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += TimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 10);
            dispatcherTimer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (_clientIdleHandler.IsActive)//active
            {
                //reset IsActive flag
                _clientIdleHandler.IsActive = false;
                if (!flag && (DateTime.Now).Subtract(stime).Minutes >= idletimeinerval && (DateTime.Now).Subtract(stime).TotalMinutes < highestidlle && CheckLoginStatus())
                {
                    IdleTimeTrack it2 = new IdleTimeTrack();
                    it2.STIME = stime;
                    it2.update();
                    it2.Show();
                }
                flag = true;//reset flag
            }
            else
            {
                if (flag)
                {
                    stime = Util.getDateTime();
                    flag = false;
                }
            }
        }

        private bool CheckLoginStatus()
        {
            try
            {
                Connection.tempConnection.Open();
                if (Connection.tempConnection.State == ConnectionState.Open)
                {
                    using (OdbcCommand MyCommand = new OdbcCommand("select tms_logsheet.logsheet_id from tms_logsheet inner join jos_users on tms_logsheet.employee_id =jos_users.id where jos_users.username='" + UserConfig.configlist[0] + "' and tms_logsheet.date_year='" + DateTime.Now.Year + "' and tms_logsheet.date_month='" + DateTime.Now.Month + "' and tms_logsheet.date_day=' " + DateTime.Now.Day + "' and (tms_logsheet.out_time is null or tms_logsheet.out_time='" + String.Empty + "')", Connection.tempConnection))
                    {
                        if (Convert.ToInt32(MyCommand.ExecuteScalar()) != 0)
                        {
                            Connection.tempConnection.Close();
                            return true;
                        }
                        else
                        {
                            Connection.tempConnection.Close();
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (Connection.tempConnection.State == ConnectionState.Open)
                {
                    Connection.tempConnection.Close();
                }
                return false;
            }
            return false;
        }
    }
}
