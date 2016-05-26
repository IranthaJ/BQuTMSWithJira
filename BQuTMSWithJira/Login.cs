using System;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Data.Odbc;
using System.Windows;
using System.Configuration;
using System.Data;

namespace BQuTMSWithJira
{
   
        class Login
        {
            OdbcCommand MyCommand2;
            public static int userid;
            public static int userdepid;
            public static string userfname;
            public static DateTime current_date;
            public static int etimehh;
            public static int etimemm;
            int lastsigninday;
            int lastsigninmonth;
            int lastsigninyear;
            public static DateTime tsUpdateLastDay = DateTime.Now;

            GetIdleTime git = new GetIdleTime();

            //check username and password form database
            public bool checkuser(string uname, string pword)
            {
                OdbcCommand MyCommand = new OdbcCommand("SELECT `jos_users`.`password` FROM `jos_users` INNER JOIN `jos_comprofiler` ON `jos_users`.`id`=`jos_comprofiler`.`id` WHERE `jos_users`.`username`='" + uname + "' AND `jos_comprofiler`.`cb_employeestatus`=1", Connection.MyConnection);
                string userpword = (string)MyCommand.ExecuteScalar();
                if (userpword != null)
                {
                    if (userpword.Substring(0, 32).Equals(Encripting(pword + (userpword.Substring(33, 32)))))
                    {
                        return true;
                    }
                    else
                    {
                        Microsoft.Windows.Controls.MessageBox.Show("Your password is incorrect", "BQuTMSWithJira Password Incorrect.", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return false;
                    }
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Username is not found.", "BQuTMSWithJira Username Incorrect", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return false;
                }
            }

            //md5hash code
            public string Encripting(string upassword)
            {
                // Create a new instance of the MD5CryptoServiceProvider object.
                MD5 md5Hasher = MD5.Create();
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(upassword));
                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();
                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                // Return the hexadecimal string.
                string md5password = sBuilder.ToString();
                return md5password;
            }

            //login method
            public void userLogin()
            {
                //if (checkuser(UserConfig.configlist[0], UserConfig.configlist[1]) == true)
                //{
                MyCommand2 = new OdbcCommand("Select id from jos_users where username='" + UserConfig.configlist[0] + "'", Connection.MyConnection);

                    userid = Convert.ToInt32(MyCommand2.ExecuteScalar());
                    MyCommand2.CommandText = "Select name from jos_users where id='" + userid + "' ";
                    userfname = (string)MyCommand2.ExecuteScalar();
                    //etimehh = Notification.GetEndTimeHH();
                    //etimemm = Notification.GetEndTimeMM();
                    //loginReminder(1);
                    //MyCommand2.CommandText = "insert into tms_logsheet(employee_id,date_year,date_month,date_day,in_time) values('" + userid + "','" + current_date.Year + "','" + current_date.Month + "','" + current_date.Day + "','" + string.Format("{0:HH:mm}", current_date) + "')";
                    //MyCommand2.ExecuteNonQuery();
                    TaskReminderCount.TaskCount();
                    if (TaskReminderCount.rid != 0)
                    {
                        TaskReminderCount.startTaskRemindTimer();//task reminder timer on
                    }
                    git.callForIdletime();//idle time timer on
                    //LatearriArlyDep(Convert.ToInt32(Login.current_date.Hour), Convert.ToInt32(Login.current_date.Minute), Notification.GetStartingTimeHH(), Notification.GetStartingTimeMM(), "Late arrival");
                //}
            }

            public void userLoginAgain()
            {
                if (checkuser(UserConfig.configlist[0], UserConfig.configlist[1]) == true)
                {
                    MyCommand2.CommandText = "Select id from jos_users where username='" + UserConfig.configlist[0] + "'";
                    userid = Convert.ToInt32(MyCommand2.ExecuteScalar());
                    MyCommand2.CommandText = "Select name from jos_users where id='" + userid + "' ";
                    userfname = (string)MyCommand2.ExecuteScalar();

                    TimeSpan timeSpentAwayFromOffice = calculateAwayTimeFromOffice(userid.ToString(), DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString(), DateTime.Now.Second.ToString());
                    updateSignedOutTimeAndTimeSpentAwayFromOffice(userid.ToString(), DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), timeSpentAwayFromOffice);

                    TaskReminderCount.TaskCount();
                    if (TaskReminderCount.rid != 0)
                    {
                        TaskReminderCount.startTaskRemindTimer();//task reminder timer on
                    }
                    git.callForIdletime();//idle time timer on
                }
            }

            internal TimeSpan calculateAwayTimeFromOffice(string employeeID, string Year, string Month, string Day, string Hour, string Minute, string Second)
            {
                DateTime nowTime = Convert.ToDateTime(Hour + ":" + Minute + ":" + Second);
                DateTime signedOutTime;
                MyCommand2.CommandText = "SELECT out_time FROM tms_logsheet WHERE employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day;
                MyCommand2.CommandType = CommandType.Text;
                string times = (string)MyCommand2.ExecuteScalar();
                if (times == "")
                {
                    signedOutTime = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day), 0, 0, 0);
                }
                else
                {
                    signedOutTime = Convert.ToDateTime(times);
                }

                TimeSpan span = nowTime.Subtract(signedOutTime);
                return span;
            }

            internal void updateSignedOutTimeAndTimeSpentAwayFromOffice(string employeeID, string Year, string Month, string Day, TimeSpan timeSpentAwayFromOffice)
            {
                //empty da signed out time and add the time spent time for existing time

                DateTime time;
                TimeSpan timeAway;
                MyCommand2.CommandText = "SELECT away_time FROM tms_logsheet WHERE employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day;
                MyCommand2.CommandType = CommandType.Text;
                string times = (MyCommand2.ExecuteScalar() == DBNull.Value) ? string.Empty : MyCommand2.ExecuteScalar().ToString();
                if (times != "")
                {
                    time = Convert.ToDateTime(times);
                    timeAway = new TimeSpan(time.Hour, time.Minute, time.Second);
                }
                else
                {
                    timeAway = new TimeSpan(0, 0, 0);
                }
                TimeSpan newAwayTime = timeAway.Add(timeSpentAwayFromOffice);

                MyCommand2.CommandText = "update tms_logsheet set out_time='',away_time = '" + newAwayTime.Hours.ToString("00") + ":" + newAwayTime.Minutes.ToString("00") + "' where employee_id = " + employeeID + " AND date_year =" + Year + " AND date_month =" + Month + " AND date_day =" + Day;
                MyCommand2.CommandType = CommandType.Text;
                MyCommand2.ExecuteNonQuery();
            }

            //eraly departure and late arrival check method
            public void LatearriArlyDep(int max_hh, int max_mm, int min_hh, int min_mm, string text)
            {
                //value ekak table 1ta yanna balapana eka max vidihata tharaganna.
                int defferance = 0;
                if (max_hh >= min_hh)
                {
                    if (max_mm > min_mm)
                    {
                        defferance = (max_mm - min_mm) + (max_hh - min_hh) * 60;
                    }

                    else if (max_mm < min_mm)
                    {
                        defferance = ((max_mm + 60) - min_mm) + ((max_hh - 1) - min_hh) * 60;
                    }

                    if (defferance > 15 && defferance < 120)
                    {
                        MyCommand2.CommandText = "insert into tms_lte_arr_erly_dep(date,time,type,employee_id) values('" + current_date.ToString("yyyy-MM-dd") + "','" + defferance + "','" + text + "','" + userid + "')";
                        MyCommand2.ExecuteNonQuery();
                    }
                }
            }

            public void Logout()//logout method
            {
                OdbcCommand MyCommand3 = new OdbcCommand("update tms_logsheet set out_time='" + string.Format("{0:HH:mm}", DateTime.Now) + "' where employee_id='" + userid + "' and date_year='" + DateTime.Now.Year + "' and date_month='" + DateTime.Now.Month + "' and date_day='" + DateTime.Now.Day + "'", Connection.MyConnection);
                if (MyCommand3.ExecuteNonQuery() == 1)
                {
                    LatearriArlyDep(Login.etimehh, Login.etimemm, Convert.ToInt32(Login.current_date.Hour), Convert.ToInt32(Login.current_date.Minute), "Early departure");
                }
                Connection.MyConnection.Close();
            }

            //check whether user login in already 
            public int checklog()
            {
                current_date = DateTime.Now;
                MyCommand2 = new OdbcCommand("select tms_logsheet.logsheet_id from tms_logsheet inner join jos_users on tms_logsheet.employee_id =jos_users.id where jos_users.username='" + UserConfig.configlist[0] + "' and tms_logsheet.date_year='" + current_date.Year + "' and tms_logsheet.date_month='" + current_date.Month + "' and tms_logsheet.date_day=' " + current_date.Day + "' and (tms_logsheet.out_time is null or tms_logsheet.out_time='" + String.Empty + "')", Connection.MyConnection);
                if (Convert.ToInt32(MyCommand2.ExecuteScalar()) != 0)
                {
                    MyCommand2.CommandText = "Select id from jos_users where username='" + UserConfig.configlist[0] + "' ";
                    userid = Convert.ToInt32(MyCommand2.ExecuteScalar());
                    MyCommand2.CommandText = "Select cb_departmentid from jos_comprofiler where user_id='" + userid + "' ";
                    userdepid = Convert.ToInt32(MyCommand2.ExecuteScalar());
                    MyCommand2.CommandText = "Select name from jos_users where id='" + userid + "' ";
                    userfname = (string)MyCommand2.ExecuteScalar();
                    etimehh = Notification.GetEndTimeHH();
                    etimemm = Notification.GetEndTimeMM();
                    TaskReminderCount.TaskCount();
                    if (TaskReminderCount.rid != 0)
                    {
                        TaskReminderCount.startTaskRemindTimer();
                    }
                    git.callForIdletime();
                    return 1;
                }
                MyCommand2.CommandText = "select tms_logsheet.logsheet_id from tms_logsheet inner join jos_users on tms_logsheet.employee_id =jos_users.id where jos_users.username='" + UserConfig.configlist[0] + "' and tms_logsheet.date_year='" + current_date.Year + "' and tms_logsheet.date_month='" + current_date.Month + "' and tms_logsheet.date_day=' " + current_date.Day + "' and (tms_logsheet.out_time is not null or tms_logsheet.out_time!='" + String.Empty + "')";
                if (Convert.ToInt32(MyCommand2.ExecuteScalar()) != 0)
                {
                    MessageBoxResult result = Microsoft.Windows.Controls.MessageBox.Show("Hi " + UserConfig.configlist[0] + ", our system tracked that you have already signed out for the day. Do you want to continue?", "BQuTMSWithJira Sign out Alert", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        MyCommand2.CommandText = "Select id from jos_users where username='" + UserConfig.configlist[0] + "' ";
                        userid = Convert.ToInt32(MyCommand2.ExecuteScalar());
                        MyCommand2.CommandText = "Select name from jos_users where id='" + userid + "' ";
                        userfname = (string)MyCommand2.ExecuteScalar();
                        return 2;//login alredy to system
                    }
                    else
                    {
                        Connection.MyConnection.Close();
                        Environment.Exit(0);
                        return 3;
                    }
                }

                MyCommand2 = new OdbcCommand("select tms_logsheet.logsheet_id from tms_logsheet inner join jos_users on tms_logsheet.employee_id =jos_users.id where jos_users.username='" + UserConfig.configlist[0] + "' and tms_logsheet.date_year='" + current_date.Year + "' and tms_logsheet.date_month='" + current_date.Month + "' and tms_logsheet.date_day=' " + current_date.Day + "'", Connection.MyConnection);
                if (Convert.ToInt32(MyCommand2.ExecuteScalar()) == 0)
                {
                    return 0;
                }
                else
                {
                    return 4;
                }
            }

            public void getServerDetails()//call for connection class
            {
                // bool serverconnection = Connection.Connect(ConfigurationManager.AppSettings["sip"], ConfigurationManager.AppSettings["susername"], Util.DecryptDES(ConfigurationManager.AppSettings["spassword"]), ConfigurationManager.AppSettings["sport"], ConfigurationManager.AppSettings["dbname"]); //create connection in this
                bool serverconnection = Connection.Connect(UserConfig.configlist[3], UserConfig.configlist[5], Util.DecryptDES(UserConfig.configlist[6]), UserConfig.configlist[4], UserConfig.configlist[7]); //create connection in this
                if (serverconnection == true)
                {
                    ///////////////// MessageBox.Show("serverconnection sucess");
                }
            }

            public void loginReminder(int x)//if x=1=> check before signin,x=2=> check after sign in
            {
                bool tempflag = false;
                int lid = 0;
                if (x == 1)
                {
                    MyCommand2.CommandText = "select logsheet_id,out_time,date_year,date_month,date_day from tms_logsheet where employee_id='" + userid + "' order by date_year DESC,date_month DESC,date_day DESC limit 1";
                    OdbcDataReader MyDataReader = MyCommand2.ExecuteReader();
                    while (MyDataReader.Read())
                    {
                        try
                        {
                            lastsigninyear = MyDataReader.GetInt32(2);
                            lastsigninmonth = MyDataReader.GetInt32(3);
                            lastsigninday = MyDataReader.GetInt32(4);
                            if (MyDataReader.GetString(1) == String.Empty)
                            {
                                tempflag = true;
                                lid = MyDataReader.GetInt32(0);
                            }
                        }
                        catch (Exception)
                        {
                            tempflag = true;
                            lid = MyDataReader.GetInt32(0);
                        }
                    }
                    MyDataReader.Close();
                    //if (tempflag)
                    //{
                    //    LoginReminder lr = new LoginReminder();
                    //    lr.LogSheetId = lid;
                    //    lr.ShowDialog();
                    //}
                }
                else
                {
                    int check = 0;
                    MyCommand2.CommandText = "select logsheet_id,out_time,date_year,date_month,date_day from tms_logsheet where employee_id='" + userid + "' order by date_year DESC,date_month DESC,date_day DESC limit 2";
                    OdbcDataReader MyDataReader = MyCommand2.ExecuteReader();
                    while (MyDataReader.Read())
                    {
                        check++;
                        try
                        {
                            lastsigninyear = MyDataReader.GetInt32(2);
                            lastsigninmonth = MyDataReader.GetInt32(3);
                            lastsigninday = MyDataReader.GetInt32(4);
                            if (check == 2 && MyDataReader.GetString(1) == String.Empty)
                            {
                                tempflag = true;
                                lid = MyDataReader.GetInt32(0);
                            }
                        }
                        catch (Exception)
                        {
                            if (check == 2)
                            {
                                tempflag = true;
                                lid = MyDataReader.GetInt32(0);
                            }
                        }
                    }
                    MyDataReader.Close();
                    //if (tempflag)
                    //{
                    //    LoginReminder lr = new LoginReminder();
                    //    lr.LogSheetId = lid;
                    //    lr.ShowDialog();
                    //}
                }
            }

            internal void UpdateVersion(string newversion, string username)
            {
                if (Connection.MyConnection.State == ConnectionState.Open)
                {

                    using (OdbcCommand MyCommand = new OdbcCommand("update jos_users set tms_app_version='" + newversion + "' where username='" + username + "'", Connection.MyConnection))
                    {
                        MyCommand.ExecuteNonQuery();
                    }
                }
                
            }

            internal String checkTimesheet()
            {
                using (OdbcCommand MyCommand = new OdbcCommand("SELECT `tms_timesheet`.`date_year`,`tms_timesheet`.`date_month`,`tms_timesheet`.`date_day` FROM `tms_timesheet` WHERE `tms_timesheet`.`employee_id`='" + Login.userid + "' ORDER BY `tms_timesheet`.`date_year` DESC,`tms_timesheet`.`date_month` DESC,`tms_timesheet`.`date_day` DESC LIMIT 1", Connection.MyConnection))
                {
                    OdbcDataReader MyDataReader = MyCommand.ExecuteReader();
                    while (MyDataReader.Read())
                    {
                        IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
                        DateTime dtSigninLastDate = DateTime.Parse(lastsigninday + "/" + lastsigninmonth + "/" + lastsigninyear, culture, System.Globalization.DateTimeStyles.AssumeLocal);

                        DateTime dtTimesheetLastDate = DateTime.Parse(MyDataReader.GetInt32(2) + "/" + MyDataReader.GetInt32(1) + "/" + MyDataReader.GetInt32(0), culture, System.Globalization.DateTimeStyles.AssumeLocal);//last timesheet update date


                        if (dtTimesheetLastDate == DateTime.Now.Date)
                        {
                            MyDataReader.Close();
                            tsUpdateLastDay = DateTime.Now.Date;
                            return "true";
                        }
                        else if (dtTimesheetLastDate == dtSigninLastDate)
                        {
                            MyDataReader.Close();
                            tsUpdateLastDay = DateTime.Now.Date;
                            return "true";
                        }
                        else if ((dtTimesheetLastDate > dtSigninLastDate) && (dtTimesheetLastDate < DateTime.Now))
                        {
                            MyDataReader.Close();
                            tsUpdateLastDay = DateTime.Now.Date;
                            return "true";
                        }
                        else
                        {
                            MyDataReader.Close();
                            tsUpdateLastDay = dtSigninLastDate;
                            return lastsigninyear + "-" + lastsigninmonth + "-" + lastsigninday;
                        }
                    }

                    MyDataReader.Close();
                    return "true";

                }

            }

        }
    
}
