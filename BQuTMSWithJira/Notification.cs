using System;
using Microsoft.Data.Odbc;
using System.Windows.Threading;
using Microsoft.Windows.Controls;
using System.Threading;
using System.ComponentModel;

namespace BQuTMSWithJira
{
    class Notification
    {
        public static event Action<string> MessageReceived;
        internal static void Broadcast(string message)
        {
            if (MessageReceived != null)
            {
                MessageReceived(message);
            }
        }
        int initialCount = 0;
        public static int GetStartingTimeHH()
        {
            OdbcCommand MyCommand = new OdbcCommand("select day_start_time_hh from tms_company_profile", Connection.MyConnection);
            return Convert.ToInt32(MyCommand.ExecuteScalar());
        }

        public static int GetStartingTimeMM()
        {
            OdbcCommand MyCommand = new OdbcCommand("select day_start_time_mm from tms_company_profile", Connection.MyConnection);
            return Convert.ToInt32(MyCommand.ExecuteScalar());
        }

        public static int GetEndTimeHH()
        {
            OdbcCommand MyCommand = new OdbcCommand("select day_end_hh from tms_company_profile", Connection.MyConnection);
            return Convert.ToInt32(MyCommand.ExecuteScalar());
        }

        public static int GetEndTimeMM()
        {
            OdbcCommand MyCommand = new OdbcCommand("select day_end_mm from tms_company_profile", Connection.MyConnection);
            return Convert.ToInt32(MyCommand.ExecuteScalar());
        }

        public void startNotificationDisplayer()
        {
            initialCount = getLeaveRequestsCount();
            while (true)
            {
                Thread.Sleep(5000);
                int numberOfRows = getLeaveRequestsCount();
                if (numberOfRows > initialCount)
                {
                    
                    Broadcast("ABC");
                }
                initialCount = numberOfRows;
            }
        }

        private int getLeaveRequestsCount()
        {
            int rowcount = 0;
            Connection.MyConnection2.Open();
            OdbcCommand MyCommand = new OdbcCommand("SELECT COUNT(leave_id) FROM tms_leaves", Connection.MyConnection2);
            try
            {
                rowcount = Convert.ToInt32(MyCommand.ExecuteScalar());
            }
            catch
            {
                return initialCount;
            }

            Connection.MyConnection2.Close();
            return rowcount;
        }

        //get real time from online server
        //public static DateTime GetRealTime(bool convertToLocalTime)
        //{
        //    //check weather net connection
        //    DateTime date = DateTime.Now;
        //    if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        //    {
        //        Random ran = new Random(Login.current_date.Millisecond);
        //        string serverResponse = string.Empty;
        //        // Represents the list of NIST servers
        //        string[] servers = new string[] {
        //         "64.90.182.55",                                                    "128.138.188.172",
        //          "128.138.188.172"
        //                 };
        //        // Try each server in random order to avoid blocked requests due to too frequent request
        //        for (int i = 0; i < 3; i++)
        //        {
        //            try
        //            {
        //                // Open a StreamReader to a random time server
        //                System.IO.StreamReader reader = new System.IO.StreamReader(new System.Net.Sockets.TcpClient(servers[ran.Next(0, servers.Length)], 13).GetStream());
        //                serverResponse = reader.ReadToEnd();
        //                reader.Close();
        //                // Check to see that the signiture is there
        //                if (serverResponse.Length > 47 && serverResponse.Substring(38, 9).Equals("UTC(NIST)"))
        //                {
        //                    // Parse the date
        //                    int jd = int.Parse(serverResponse.Substring(1, 5));
        //                    int yr = int.Parse(serverResponse.Substring(7, 2));
        //                    int mo = int.Parse(serverResponse.Substring(10, 2));
        //                    int dy = int.Parse(serverResponse.Substring(13, 2));
        //                    int hr = int.Parse(serverResponse.Substring(16, 2));
        //                    int mm = int.Parse(serverResponse.Substring(19, 2));
        //                    int sc = int.Parse(serverResponse.Substring(22, 2));
        //                    if (jd > 51544)
        //                        yr += 2000;
        //                    else
        //                        yr += 1999;
        //                    date = new DateTime(yr, mo, dy, hr, mm, sc);
        //                    // Convert it to the current timezone if desired
        //                    if (convertToLocalTime)
        //                        //date = date.AddHours(5);
        //                        date = date.AddMinutes(330);
        //                    //    System.Windows.MessageBox.Show(date.ToString());
        //                    // Exit the loop
        //                    break;
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                /* Do Nothing...try the next server */
        //                Microsoft.Windows.Controls.MessageBox.Show("can't get real date and time", "BQuTMSWithJira can't get real time", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        //            }
        //        }
        //    }
        //    return date;
        //}

    }
}
