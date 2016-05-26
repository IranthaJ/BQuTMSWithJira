using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Data.Odbc;

namespace BQuTMSWithJira
{
    /// <summary>
    /// Interaction logic for LeaveRequest.xaml
    /// </summary>
    public partial class LeaveRequest : UserControl
    {
        public LeaveRequest()
        {
            InitializeComponent();
        }

        int halfFull, allleaves;
        double gotleaves;
        string blockmsg = String.Empty;
        string hoidaymsg = String.Empty;
        OdbcDataReader MyDataReader;
        List<string> leaveitem = new List<string>();//leave names add into list
        double no_of_lev = 0;
        Department dep = new Department();
        string sdpicker = string.Empty;
        string edpicker = string.Empty;
        string hfstring = string.Empty;
        string ltype = string.Empty;
        string comment = string.Empty;
        string emailto = string.Empty;
        OdbcCommand MyCommand1;
        List<AllLeaves> remine_all_leaves = new List<AllLeaves>();//add leave summery to this list
        List<Leave> empleaves = new List<Leave>();
        public static bool checkflaglr = true;

        void configureDTPicker()//configure datetimepickers with blockdates 
        {
            using (OdbcCommand MyCommand = new OdbcCommand("SELECT DISTINCT tms_special_blocked_dates.b_year,tms_special_blocked_dates.b_month, tms_special_blocked_dates.b_day from tms_special_blocked_dates inner join jos_comprofiler on tms_special_blocked_dates.blocked_by=jos_comprofiler.user_id inner join tms_emp_privilege on tms_special_blocked_dates.blocked_by=tms_emp_privilege.employee_id where ((jos_comprofiler.cb_departmentid='" + Login.userdepid + "' or tms_emp_privilege.pri_id='1') and(tms_special_blocked_dates.b_year='" + DateTime.Now.Year + "'or tms_special_blocked_dates.b_year='" + (DateTime.Now.Year + 1) + "' ))", Connection.MyConnection))
            {
                sday_dPicker.BlackoutDates.Clear();
                eday_dPicker.BlackoutDates.Clear();
                MyDataReader = MyCommand.ExecuteReader();
                while (MyDataReader.Read())
                {
                    sday_dPicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(MyDataReader.GetInt32(0), MyDataReader.GetInt32(1), MyDataReader.GetInt32(2))));//add date to datetimepicker
                    eday_dPicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(MyDataReader.GetInt32(0), MyDataReader.GetInt32(1), MyDataReader.GetInt32(2))));
                }
                MyDataReader.Close();
            }
        }



        private void timesheethistory_but_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (leaveHistoryLab.Content.ToString() == "Back")
            //{
            //    leaveRequst_grid.Visibility = Visibility.Visible;
            //    leaveHistory_grid.Visibility = Visibility.Hidden;
            //    leaveHistoryLab.Content = "Leave History";
            //}
            //else
            //{
            //    leaveRequst_grid.Visibility = Visibility.Hidden;
            //    leaveHistory_grid.Visibility = Visibility.Visible;
            //    leaveHistoryLab.Content = "Back";
            //}

            if (leaveHistoryLab.Content.ToString() == "Leave History")//check weather click on "Leave History" button
            {
                if (Connection.MyConnection.State == ConnectionState.Open)
                {
                    remine_all_leaves.Clear();
                    empleaves.Clear();
                    BackgroundWorker _worker;
                    _worker = new BackgroundWorker();
                    _worker.WorkerReportsProgress = true;
                    _worker.WorkerSupportsCancellation = true;
                    OdbcDataReader MyDataReader2;
                    _worker.DoWork += delegate(object s, DoWorkEventArgs args)
                    {
                        _worker.ReportProgress(0);
                        using (OdbcCommand MyCommand = new OdbcCommand("select DISTINCT tms_leaves.leave_id,tms_leave_description.leave_name , tms_leaves.s_day , tms_leaves.s_month , tms_leaves.s_year,tms_leaves.e_day , tms_leaves.e_month , tms_leaves.e_year , tms_leaves.status,tms_leaves.ld_half_full,tms_leaves.comment_from_admin from tms_leaves inner join tms_leave_description on tms_leaves.ld_id=tms_leave_description.ld_id where tms_leaves.employee_id='" + Login.userid + "' && tms_leaves.s_year='" + DateTime.Now.Year + "'  order by tms_leaves.leave_id desc", Connection.MyConnection))
                        {
                            MyDataReader2 = MyCommand.ExecuteReader();
                            while (MyDataReader2.Read())
                            {
                                empleaves.Add(new Leave(MyDataReader2.GetInt32(0), MyDataReader2.GetString(1), MyDataReader2.GetString(2) + "/" + MyDataReader2.GetString(3) + "/" + MyDataReader2.GetString(4), MyDataReader2.GetString(5) + "/" + MyDataReader2.GetString(6) + "/" + MyDataReader2.GetString(7), MyDataReader2.GetString(8), MyDataReader2.GetString(9), MyDataReader2.GetString(10)));
                            }
                            MyDataReader2.Close();
                            _worker.ReportProgress(1);
                            foreach (string ss in leaveitem)
                            {
                                ReminLeaves(ss);
                                if ((System.Text.RegularExpressions.Regex.Match(ss, "Sick").Success))
                                {
                                    remine_all_leaves.Add(new AllLeaves(ss, 0, gotleaves, 0));
                                }
                                else
                                {
                                    remine_all_leaves.Add(new AllLeaves(ss, allleaves, gotleaves, (allleaves - gotleaves)));
                                }
                            }
                            _worker.ReportProgress(2);
                            _worker.ReportProgress(3);
                            _worker.ReportProgress(4);
                            _worker.ReportProgress(9);
                        }
                    };
                    _worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
                    {
                        if (args.ProgressPercentage == 0)
                        {
                            ltype_cbox.IsEnabled = false;
                            fday_rBut.IsEnabled = false;
                            hday_rBut.IsEnabled = false;
                            sday_dPicker.IsEnabled = false;
                            eday_dPicker.IsEnabled = false;
                            commet_tBox.IsEnabled = false;
                            apply_but.IsEnabled = false;
                            leavehistory_btn_img.IsEnabled = false;
                            circularProgressBar.Visibility = Visibility.Visible;
                        }
                        else if (args.ProgressPercentage == 1)
                        {
                            leave_lView.ItemsSource = empleaves;
                        }
                        else if (args.ProgressPercentage == 2)
                        {
                            all_leave_lView.ItemsSource = remine_all_leaves;
                        }
                        else if (args.ProgressPercentage == 3)
                        {
                           // all_leave_lView.View = myGrid2;
                        }
                        else if (args.ProgressPercentage == 9)
                        {
                            /////HIDDING
                          //  request_lab.Visibility = Visibility.Hidden;
                        //    lt_lab.Visibility = Visibility.Hidden;
                        //    type_lab.Visibility = Visibility.Hidden;
                        //    textBlock6.Visibility = Visibility.Hidden;
                        //    textBlock2.Visibility = Visibility.Hidden;
                        //    textBlock3.Visibility = Visibility.Hidden;
                        //    textBlock4.Visibility = Visibility.Hidden;
                        //    textBlock5.Visibility = Visibility.Hidden;
                        // //   lr_lab.Visibility = Visibility.Hidden;
                        ////    laveshowlab.Visibility = Visibility.Hidden;
                        //    ltype_cbox.Visibility = Visibility.Hidden;
                        //    no_of_leave_lab.Visibility = Visibility.Hidden;
                        //    fday_rBut.Visibility = Visibility.Hidden;
                        //    hday_rBut.Visibility = Visibility.Hidden;
                        //    sday_dPicker.Visibility = Visibility.Hidden;
                        //    sdate_lab.Visibility = Visibility.Hidden;
                        //    eday_dPicker.Visibility = Visibility.Hidden;
                        //    edate_lab.Visibility = Visibility.Hidden;
                        //    commet_tBox.Visibility = Visibility.Hidden;
                        //    comment_lab.Visibility = Visibility.Hidden;
                        //    apply_but.Visibility = Visibility.Hidden;
                        //    ///////VISIBLE
                        //    leave_lView.Visibility = Visibility.Visible;
                        //    label2.Visibility = Visibility.Visible;
                        //    textBlock1.Visibility = Visibility.Visible;
                        //    grean_lab.Visibility = Visibility.Visible;
                        //    textBlock8.Visibility = Visibility.Visible;
                        //    label1.Visibility = Visibility.Visible;
                        //    textBlock7.Visibility = Visibility.Visible;
                        //    all_leave_lView.Visibility = Visibility.Visible;
                         //   sum_lab.Visibility = Visibility.Visible;
                         //   his_lab.Visibility = Visibility.Visible;
                            ///////change button content
                          //  leaveHistoryLab.Content = "Back";

                            leaveRequst_grid.Visibility = Visibility.Hidden;
                            leaveHistory_grid.Visibility = Visibility.Visible;
                            leaveHistoryLab.Content = "Back";
                        }
                    };
                    _worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        leave_lView.Items.Refresh();
                        all_leave_lView.Items.Refresh();
                        circularProgressBar.Visibility = Visibility.Hidden;
                        ltype_cbox.IsEnabled = true;
                        fday_rBut.IsEnabled = true;
                        hday_rBut.IsEnabled = true;
                        sday_dPicker.IsEnabled = true;
                        eday_dPicker.IsEnabled = true;
                        commet_tBox.IsEnabled = true;
                        apply_but.IsEnabled = true;
                        leavehistory_btn_img.IsEnabled = true;
                    };
                    _worker.RunWorkerAsync();
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. Please refresh (Help -> Refresh) and try again.", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                /////HIDDING
              //  lt_lab.Visibility = Visibility.Visible;
              //  type_lab.Visibility = Visibility.Visible;
              //  textBlock6.Visibility = Visibility.Visible;
              //  textBlock2.Visibility = Visibility.Visible;
              //  textBlock3.Visibility = Visibility.Visible;
              //  textBlock4.Visibility = Visibility.Visible;
              //  textBlock5.Visibility = Visibility.Visible;
              // // lr_lab.Visibility = Visibility.Visible;
              // // laveshowlab.Visibility = Visibility.Visible;
              //  ltype_cbox.Visibility = Visibility.Visible;
              //  no_of_leave_lab.Visibility = Visibility.Visible;
              //  fday_rBut.Visibility = Visibility.Visible;
              //  hday_rBut.Visibility = Visibility.Visible;
              //  sday_dPicker.Visibility = Visibility.Visible;
              //  sdate_lab.Visibility = Visibility.Visible;
              //  eday_dPicker.Visibility = Visibility.Visible;
              //  edate_lab.Visibility = Visibility.Visible;
              //  commet_tBox.Visibility = Visibility.Visible;
              //  comment_lab.Visibility = Visibility.Visible;
              //  apply_but.Visibility = Visibility.Visible;
              //  ///////VISIBLE
              //  leave_lView.Visibility = Visibility.Hidden;
              //  label2.Visibility = Visibility.Hidden;
              //  textBlock1.Visibility = Visibility.Hidden;
              //  grean_lab.Visibility = Visibility.Hidden;
              //  textBlock8.Visibility = Visibility.Hidden;
              //  label1.Visibility = Visibility.Hidden;
              //  textBlock7.Visibility = Visibility.Hidden;
              //  all_leave_lView.Visibility = Visibility.Hidden;
              // // sum_lab.Visibility = Visibility.Hidden;
              ////  his_lab.Visibility = Visibility.Hidden;
              //  fday_rBut.IsChecked = true;
              //  ///////change button content
              // // leaveHistoryLab.Content = "Leave History";
                leaveRequst_grid.Visibility = Visibility.Visible;
                leaveHistory_grid.Visibility = Visibility.Hidden;
                leaveHistoryLab.Content = "Leave History";
            }
        }

        //count remind leaves ffor the year
        public void ReminLeaves(string leaveName)
        {
            string quary = String.Empty;
            using (OdbcCommand MyCommand = new OdbcCommand(quary, Connection.MyConnection))
            {
                allleaves = 0;
                gotleaves = 0;
                quary = "select tms_leaves_for_employees.no_of_leaves from tms_leaves_for_employees inner join tms_leave_description on tms_leave_description.ld_id=tms_leaves_for_employees.leave_id where tms_leave_description.leave_name ='" + leaveName + "' && tms_leaves_for_employees.employee_id='" + Login.userid + "' && tms_leaves_for_employees.leave_year='" + DateTime.Now.Year + "'";
                MyCommand.CommandText = quary;
                allleaves = Convert.ToInt32(MyCommand.ExecuteScalar());
                quary = "select tms_leaves.s_year ,tms_leaves.s_month ,tms_leaves.s_day ,tms_leaves.e_year ,tms_leaves. e_month , tms_leaves.e_day, tms_leaves.ld_half_full from tms_leaves inner join tms_leave_description on tms_leaves.ld_id=tms_leave_description.ld_id where tms_leave_description.leave_name ='" + leaveName + "' && tms_leaves.employee_id='" + Login.userid + "' && tms_leaves.status='Accepted' && tms_leaves.s_year='" + DateTime.Now.Year + "'";
                MyCommand.CommandText = quary;
                int fullDays = 0;
                double halfDays = 0;
                DateTime staringdate, enddate;
                MyDataReader = MyCommand.ExecuteReader();
                while (MyDataReader.Read())
                {
                    if (MyDataReader.GetInt32(6) == 1)
                    {
                        staringdate = new DateTime(MyDataReader.GetInt32(0), MyDataReader.GetInt32(1), MyDataReader.GetInt32(2));
                        enddate = new DateTime(MyDataReader.GetInt32(3), MyDataReader.GetInt32(4), MyDataReader.GetInt32(5));
                        for (DateTime index = staringdate; index <= enddate; index = index.AddDays(1))//remove weekends
                        {
                            if (index.DayOfWeek != DayOfWeek.Sunday && index.DayOfWeek != DayOfWeek.Saturday)
                            {
                                fullDays++;
                            }
                        }
                    }
                    else if (MyDataReader.GetInt32(6) == 0)
                    {
                        halfDays += 1;
                    }
                }
                MyDataReader.Close();
                gotleaves = fullDays + (halfDays / 2);
            }
        }

        private void leave_lView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                DeleteLeave();
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. Please refresh (Help -> Refresh) and try again", "BQuTMSWithJira Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //delete leave
        void DeleteLeave()
        {
            var selectedStockObject = leave_lView.SelectedItems[0] as Leave;//get selected item
            if (selectedStockObject == null)
            {
                return;
            }
            string quary = String.Empty;
            using (OdbcCommand MyCommand = new OdbcCommand(quary, Connection.MyConnection))
            {
                int key = selectedStockObject.ID;//assign selected item ID to key
                quary = "select s_year,s_month,s_day,status from tms_leaves where leave_id='" + key + "'";
                MyCommand.CommandText = quary;
                MyDataReader = MyCommand.ExecuteReader();
                MyDataReader.Read();
                DateTime dt1 = new DateTime(MyDataReader.GetInt32(0), MyDataReader.GetInt32(1), MyDataReader.GetInt32(2));
                if (DateTime.Compare(dt1, DateTime.Now) < 0 || MyDataReader.GetString(3) == "Accepted")
                {
                    MyDataReader.Close();
                    Microsoft.Windows.Controls.MessageBox.Show("Please contact your admin", "Permission denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else
                {
                    MyDataReader.Close();
                    if (Microsoft.Windows.Controls.MessageBox.Show("Do you sure want to delete this leave?", "BQuTMSWithJira Confirmination", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        quary = "DELETE FROM tms_leaves WHERE leave_id='" + key + "'";
                        MyCommand.CommandText = quary;
                        MyCommand.ExecuteNonQuery();
                        empleaves.RemoveAt(leave_lView.SelectedIndex);
                        leave_lView.ItemsSource = empleaves;
                        leave_lView.Items.Refresh();
                    }
                }
            }
        }

        private void Menu_click(object sender, RoutedEventArgs e)
        {
            DeleteLeave();
        }

        private void ltype_cbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            //lr_lab.Visibility = Visibility.Visible;
         //   laveshowlab.Visibility = Visibility.Visible;
            no_of_leave_lab.Visibility = Visibility.Visible;
            no_of_leave_lab.Visibility = Visibility.Visible;
            leaveshowlab.Content = (sender as ComboBox).SelectedItem.ToString();
            if (System.Text.RegularExpressions.Regex.Match((sender as ComboBox).SelectedItem.ToString(), "Sick").Success)
            {
                no_of_leave_lab.Content = "-";//hide remind sick leaves
            }
            else
            {
                ReminLeaves((sender as ComboBox).SelectedItem.ToString());
                no_of_lev = allleaves - gotleaves;//calculate remind leaves
                no_of_leave_lab.Content = no_of_lev;
            }
            Mouse.OverrideCursor = null;
        }

        private void sday_dPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eday_dPicker.SelectedDate = Convert.ToDateTime(sday_dPicker.Text);
        }

        private void apply_but_Click(object sender, RoutedEventArgs e)
        {
            if ((Connection.MyConnection.State == ConnectionState.Open))
            {
                if ((ltype_cbox.Text != "") && (sday_dPicker.Text != ""))
                {
                    blockmsg = String.Empty;
                    hoidaymsg = String.Empty;
                    ltype = ltype_cbox.Text;
                    string quary = String.Empty;
                    BackgroundWorker _worker;
                    Department depm = new Department();
                    int department_id = Login.userdepid;
                    ////////////////////////////////////////
                    _worker = new BackgroundWorker();
                    _worker.WorkerReportsProgress = true;
                    _worker.WorkerSupportsCancellation = true;

                    int leave_id = 0;
                    sdpicker = sday_dPicker.SelectedDate.ToString();
                    edpicker = eday_dPicker.SelectedDate.ToString();

                    _worker.DoWork += delegate(object s, DoWorkEventArgs args)
                    {
                        _worker.ReportProgress(0);
                        bool checkBlockDate = checkdate("SELECT DISTINCT tms_special_blocked_dates.b_month, tms_special_blocked_dates.b_day,tms_special_blocked_dates.reason from tms_special_blocked_dates inner join jos_comprofiler on tms_special_blocked_dates.blocked_by=jos_comprofiler.user_id inner join tms_emp_privilege on tms_special_blocked_dates.blocked_by =tms_emp_privilege.employee_id where (jos_comprofiler.cb_departmentid='" + Login.userdepid + "' or tms_emp_privilege.pri_id='1' ) and (tms_special_blocked_dates.b_year='" + DateTime.Now.Year + "' or tms_special_blocked_dates.b_year='" + (DateTime.Now.Year + 1) + "'   )", 0);
                        bool checkHoliday = checkdate("select h_month,h_date,h_description from tms_holidays where h_year='" + DateTime.Now.Year + "'", 10);
                        if (checkBlockDate && checkHoliday)
                        {
                            quary = "select ld_id from tms_leave_description where leave_name='" + ltype + "'";
                            MyCommand1.CommandText = quary;
                            leave_id = Convert.ToInt32(MyCommand1.ExecuteScalar());
                            quary = "insert into tms_leaves(ld_id,ld_half_full,employee_id,s_year,s_month,s_day,e_year,e_month,e_day,reason_from_emp) values('" + leave_id + "','" + halfFull + "','" + Login.userid + "','" + Convert.ToDateTime(sdpicker).Year + "','" + Convert.ToDateTime(sdpicker).Month + "','" + Convert.ToDateTime(sdpicker).Day + "','" + Convert.ToDateTime(edpicker).Year + "','" + Convert.ToDateTime(edpicker).Month + "','" + Convert.ToDateTime(edpicker).Day + "','" + (comment).Replace("'", "\\'") + "')";
                            MyCommand1.CommandText = quary;
                            MyCommand1.ExecuteNonQuery();
                            _worker.ReportProgress(1);
                            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                            {
                                int department_head = depm.GetDepartmentHead(department_id);
                                try
                                {
                                    if (department_head == Login.userid)//he is head of department
                                    {
                                        quary = "select jos_users.email from jos_users inner join tms_emp_privilege on tms_emp_privilege.employee_id =jos_users.id inner join  jos_comprofiler on jos_comprofiler.user_id=jos_users.id where tms_emp_privilege.pri_id=1 and jos_comprofiler.cb_employeestatus= 1";
                                        MyCommand1.CommandText = quary;
                                        MyDataReader = MyCommand1.ExecuteReader();
                                        while (MyDataReader.Read())
                                        {
                                            if (!emailto.Equals(""))
                                            {
                                                emailto = emailto + "," + MyDataReader.GetString(0);
                                            }
                                            else
                                            {
                                                emailto = MyDataReader.GetString(0);
                                            }
                                        }
                                        MyDataReader.Close();
                                        //send mail
                                        //   ms.MailSendforEmp("<table width= '38%' border='0' align= 'center' cellpadding ='0' cellspacing='0' style='border:#e1e1e1 10px  solid'> <tr><td align='center'><table width='100%' border='0' align='center' cellpadding ='10' bgcolor='#D4D9DD' style ='border:#666666 3px  solid'><tr> <td ><h2><b>TMS - Leave Request</b></h2></td></tr><tr><td> Hi " + "," + "</td></tr><tr><td> <table width='98%' align='center' cellpadding='4' bgcolor='#e1e1e1' boder ='0' style='border:#666666 1px  solid'><tr><td><table width='100%' border='0' cellpadding='5'> <tr> <td > Employee name:</td><td>" + Login.userfname + "</td></tr><tr> <td >Leave type:</td><td>" + ltype + "</td></tr><tr> <td >Starting date:</td><td>" + sdpicker + "</td></tr> <tr><td >End date:</td><td>" + edpicker + "</td></tr> <tr><td >Available leaves:</td><td>" + no_of_lev + "</td></tr><tr><td >Half/Full day:</td><td>" + hfstring + "</td></tr><tr> <td >Comment:</td><td>" + comment + "</td></tr></table></td></tr></table></td></tr> <tr><td> <hr align='left' width='100%' noshade='noshade'/><b>Please, accept my Leave Request</b></td></tr></table></td></tr></table>", emailto, "Request Leave by " + Login.userfname);
                                        // //console.writeline(emailto);
                                        //  mailsend = true;
                                        _worker.ReportProgress(2);
                                    }
                                    else
                                    {
                                        quary = "select jos_users.email from jos_users inner join tms_emp_privilege on tms_emp_privilege.employee_id = jos_users.id inner join jos_comprofiler on jos_comprofiler.user_id = jos_users.id where tms_emp_privilege.pri_id=2  and jos_comprofiler.cb_departmentid='" + department_id + "' and jos_comprofiler.cb_employeestatus=1";
                                        MyCommand1.CommandText = quary;
                                        MyDataReader = MyCommand1.ExecuteReader();
                                        while (MyDataReader.Read())
                                        {
                                            if (!emailto.Equals(""))
                                            {
                                                emailto = emailto + "," + MyDataReader.GetString(0);
                                            }
                                            else
                                            {
                                                emailto = MyDataReader.GetString(0);
                                            }
                                        }
                                        MyDataReader.Close();
                                        // ms.MailSendforEmp("<table width= '38%' border='0' align= 'center' cellpadding ='0' cellspacing='0' style='border:#e1e1e1 10px  solid'> <tr><td align='center'><table width='100%' border='0' align='center' cellpadding ='10' bgcolor='#D4D9DD' style ='border:#666666 3px  solid'><tr> <td ><h2><b>TMS - Leave Request</b></h2></td></tr><tr><td> Hi " + "," + "</td></tr><tr><td> <table width='98%' align='center' cellpadding='4' bgcolor='#e1e1e1' boder ='0' style='border:#666666 1px  solid'><tr><td><table width='100%' border='0' cellpadding='5'> <tr> <td > Employee name:</td><td>" + Login.userfname + "</td></tr><tr> <td >Leave type:</td><td>" + ltype + "</td></tr><tr> <td >Starting date:</td><td>" + sdpicker + "</td></tr> <tr><td >End date:</td><td>" + edpicker + "</td></tr> <tr><td >Available leaves:</td><td>" + no_of_lev + "</td></tr><tr><td >Half/Full day:</td><td>" + hfstring + "</td></tr><tr> <td >Comment:</td><td>" + comment + "</td></tr></table></td></tr></table></td></tr> <tr><td> <hr align='left' width='100%' noshade='noshade'/><b>please, Accept my Leave Request</b></td></tr></table></td></tr></table>", emailto, "Request Leave by " + Login.userfname);
                                        // //console.writeline(emailto);
                                        // mailsend = true;
                                        _worker.ReportProgress(2);
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                        else
                        {
                            _worker.ReportProgress(3);
                        }
                    };
                    _worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
                    {
                        if (args.ProgressPercentage == 0)
                        {
                            ltype_cbox.IsEnabled = false;
                            fday_rBut.IsEnabled = false;
                            hday_rBut.IsEnabled = false;
                            sday_dPicker.IsEnabled = false;
                            eday_dPicker.IsEnabled = false;
                            commet_tBox.IsEnabled = false;
                            apply_but.IsEnabled = false;
                            leavehistory_btn_img.IsEnabled = false;
                            circularProgressBar.Visibility = Visibility.Visible;
                            comment = commet_tBox.Text;
                        }

                        else if (args.ProgressPercentage == 1)
                        {
                            if (blockmsg != String.Empty)
                            {
                                Microsoft.Windows.Controls.MessageBox.Show(blockmsg, "BQuTMSWithJira Blocked Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else if (hoidaymsg != String.Empty)
                            {
                                Microsoft.Windows.Controls.MessageBox.Show(hoidaymsg, "BQuTMSWithJira Holiday", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                               // request_lab.Visibility = Visibility.Visible;
                                Microsoft.Windows.Controls.MessageBox.Show("Request Submitted.", "BQuTMSWithJira Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }

                        else if (args.ProgressPercentage == 2)
                        {
                           // request_lab.Visibility = Visibility.Visible;

                            Microsoft.Windows.Controls.MessageBox.Show("Request Submitted.", "BQuTMSWithJira Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
                            mailsend = true;
                        }

                        else if (args.ProgressPercentage == 3)
                        {
                            if (blockmsg != String.Empty)
                            {
                                Microsoft.Windows.Controls.MessageBox.Show("Submitted successfully.", "BQuTMSWithJira Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else if (hoidaymsg != String.Empty)
                            {
                                Microsoft.Windows.Controls.MessageBox.Show(hoidaymsg, "BQuTMSWithJira Holiday", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    };
                    _worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        circularProgressBar.Visibility = Visibility.Hidden;
                        ltype_cbox.IsEnabled = true;
                        fday_rBut.IsEnabled = true;
                        hday_rBut.IsEnabled = true;
                        sday_dPicker.IsEnabled = true;
                        eday_dPicker.IsEnabled = true;
                        commet_tBox.IsEnabled = true;
                        apply_but.IsEnabled = true;
                        leavehistory_btn_img.IsEnabled = true;
                        Console.WriteLine("AAAAAA");
                        Console.WriteLine(emailto);
                        // MessageBox.Show(mailsend.ToString());
                        // if (mailsend == true)
                        // {
                        Console.WriteLine("BBBBB");
                        Thread ThreadOne = new Thread(new ThreadStart(LeaveMailSend));
                        ThreadOne.Start();
                        // }
                    };
                    //_worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    //{
                    //    circularProgressBar.Visibility = Visibility.Hidden;
                    //    ltype_cbox.IsEnabled = true;
                    //    fday_rBut.IsEnabled = true;
                    //    hday_rBut.IsEnabled = true;
                    //    sday_dPicker.IsEnabled = true;
                    //    eday_dPicker.IsEnabled = true;
                    //    commet_tBox.IsEnabled = true;
                    //    apply_but.IsEnabled = true;
                    //    leavehistory_btn_img.IsEnabled = true;
                    //    // MessageBox.Show(mailsend.ToString());
                    //    if (mailsend == true)
                    //    {
                    //        Thread ThreadOne = new Thread(new ThreadStart(LeaveMailSend));
                    //        ThreadOne.Start();
                    //    }
                    //};
                    _worker.RunWorkerAsync();
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Please select the field(s).", "BQuTMSWithJira Empty Field(s)", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. Please refresh (Help -> Refresh) and try again.", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        bool mailsend = false;
        private static Mutex MMutex = new Mutex(false, "MailMutex");

        void LeaveMailSend()
        {
            //  Thread.Sleep(1000);
            MMutex.WaitOne();
            // Thread.Sleep(1000);
            if (comment == String.Empty)
            {
                comment = "No comment";
            }
            //  comment=(comment==string.Empty)?"No comment";
            //http://www.bqutms.com/tms_email_script/BQuTMSEmail.php?ename=Irantha Jayasekara&ltype=Paid Leave&sdate=Fri, Jun 13, 2014 12:00:00 AM&edate=Fri, Jun 13, 2014 12:00:00 AM&avalleav=23.5&halful=Full day/days&emailto=bhagya.weerathunga@bquintelligence.com,bhagya.weerathunga@bquintelligence.com&comment=My testing email
            string URI = "http://www.bqutms.com/tms_email_script/BQuTMSEmail.php?ename=" + Login.userfname + "&ltype=" + ltype + "&sdate=" + sdpicker + "&edate=" + edpicker + "&avalleav=" + no_of_lev + "&halful=" + hfstring + "&emailto=" + emailto + "&comment=" + comment + "";
            Console.WriteLine("URIURI" + URI);
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            req.Proxy = null; //new System.Net.WebProxy("www.bqutms.com", true); //true means no proxy
            System.Net.WebResponse resp = req.GetResponse();
            //System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            // //console.writeline( sr.ReadToEnd().Trim());
            resp.Close();
            ////console.writeline("done");
            MMutex.ReleaseMutex();
            //   mailsend = false;
        }

        //date checking weather have holiday ,blockdate....etc
        bool checkdate(string command, int i)
        {
            MyCommand1 = new OdbcCommand(command, Connection.MyConnection);
            DateTime bDate;
            MyCommand1.CommandText = command;
            MyDataReader = MyCommand1.ExecuteReader();
            string dt = Convert.ToDateTime(sdpicker).ToString();
            while (MyDataReader.Read())
            {
                bDate = new DateTime(Convert.ToInt32(DateTime.Now.Year), MyDataReader.GetInt32(0), MyDataReader.GetInt32(1));
                if (Convert.ToDateTime(sdpicker) < bDate && Convert.ToDateTime(edpicker) > bDate)//check two date
                {
                    if (i == 0 || i == 1)
                    {
                        //   Microsoft.Windows.Controls.MessageBox.Show(MyDataReader.GetInt32(1) + "/" + MyDataReader.GetInt32(0) + "/" + DateTime.Now.Year + " date was blocked.Admin say " + MyDataReader.GetString(2).ToString() + ".", "BQuTMSWithJira Blocked Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                        blockmsg = MyDataReader.GetInt32(1) + "/" + MyDataReader.GetInt32(0) + "/" + DateTime.Now.Year + " date was blocked.Admin say " + MyDataReader.GetString(2).ToString() + ".";
                        i = 1;
                    }
                    else if (i == 10 || i == 20)
                    {
                        // Microsoft.Windows.Controls.MessageBox.Show(MyDataReader.GetInt32(1) + "/" + MyDataReader.GetInt32(0) + "/" + DateTime.Now.Year + " date is " + MyDataReader.GetString(2).ToString() + ".", "BQuTMSWithJira Holiday", MessageBoxButton.OK, MessageBoxImage.Warning);
                        hoidaymsg = MyDataReader.GetInt32(1) + "/" + MyDataReader.GetInt32(0) + "/" + DateTime.Now.Year + " date is " + MyDataReader.GetString(2).ToString() + ".";
                        i = 20;
                    }
                }
            }
            MyDataReader.Close();
            if (i == 1 || i == 20)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void eday_dPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int i;
            try
            {
                i = DateTime.Compare(Convert.ToDateTime(sday_dPicker.Text), Convert.ToDateTime(eday_dPicker.Text));
                if (i > 0)
                {
                    Microsoft.Windows.Controls.MessageBox.Show("End date should be greater than starting date", "BQuTMSWithJira Invalid Date Selection", MessageBoxButton.OK, MessageBoxImage.Error);
                    eday_dPicker.SelectedDate = Convert.ToDateTime(sday_dPicker.Text);
                }
            }
            catch (Exception ee)
            {
                Microsoft.Windows.Controls.MessageBox.Show(ee + "First  of all select starting date.", "BQuTMSWithJira Incorrect Date Selection", MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }

        private void hday_rBut_Checked(object sender, RoutedEventArgs e)
        {
            halfFull = 0;
            hfstring = "Half day";
            eday_dPicker.IsEnabled = false;
        }

        private void fday_rBut_Checked(object sender, RoutedEventArgs e)
        {
            halfFull = 1;
            hfstring = "Full day/days";
            eday_dPicker.IsEnabled = true;
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                commet_tBox.Text = "";
                no_of_leave_lab.Content = "";
               // request_lab.Visibility = Visibility.Hidden;
                if (checkflaglr)
                {
                    Thread.CurrentThread.CurrentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone(); Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "ddd, MMM dd, yyyy";//set date as custom format
                    using (OdbcCommand MyCommand = new OdbcCommand("select tms_leave_description.leave_name from tms_leave_description inner join tms_leaves_for_employees on tms_leave_description.ld_id =tms_leaves_for_employees.leave_id where tms_leaves_for_employees.employee_id ='" + Login.userid + "' && tms_leaves_for_employees.leave_year='" + DateTime.Now.Year + "' ", Connection.MyConnection))
                    {
                        MyDataReader = MyCommand.ExecuteReader();
                      //  createTables();
                        while (MyDataReader.Read())
                        {
                            ltype_cbox.Items.Add(MyDataReader.GetString(0));
                            leaveitem.Add(MyDataReader.GetString(0));
                        }
                        MyDataReader.Close();
                    }
                    configureDTPicker();//block date showing
                    checkflaglr = false;
                }
            }
        }
    }
}
