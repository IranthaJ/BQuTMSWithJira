using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Data.Odbc;
using System.Data;
using System.Threading;
using System.Globalization;
using System.Xml;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Net.Security;
using JIRA_Library;
using JIRA_Library.Entities.Issues;

namespace BQuTMSWithJira
{

    /// <summary>
    /// Interaction logic for TimeSheet.xaml
    /// </summary>
    public partial class TimeSheet : UserControl
    {

        List<AddTimesheet> timesheetlist = new List<AddTimesheet>();//add timesheet table items to this list
        OdbcDataReader MyDataReader;
        public static bool checkflag = true;
        XMLRedWri xrw = new XMLRedWri();
        DataTable detailTable = new DataTable();
        
        JiraManager manager;
        private string user;
        private string password;

        public TimeSheet()
        {
            InitializeComponent();
            detailTable.Columns.Add("IssueKey");
            detailTable.Columns.Add("Issue");
            detailTable.Columns.Add("IssueTypeId");
            detailTable.Columns.Add("IssueType");
            detailTable.Columns.Add("ProjectId");
            detailTable.Columns.Add("ProjectKey");
            detailTable.Columns.Add("ProjectName");
        }

        private void MenuItem_delete(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Microsoft.Windows.Controls.MessageBox.Show("Do you really want to delete this item?", "BQuTMSWithJira Delete confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    DeleteSelected();
                    Mouse.OverrideCursor = null;
                }
            }
            catch (Exception)
            {
                Microsoft.Windows.Controls.MessageBox.Show("Incorrect selection", "BQuTMSWithJira Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void timesheet_lab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            timeTracker_visible_grid.Visibility = Visibility.Hidden;
            timesheet_visible_grid.Visibility = Visibility.Visible;
            timesheet_tBlock_tab.Visibility = Visibility.Hidden;
            timetracker_tBlock_tab.Visibility = Visibility.Visible;
            select_dPicker.IsEnabled = true;
            prjName_cbox.IsEnabled = true;
            catogory_cBox.IsEnabled = true;
            start_TPicker.IsEnabled = true;
            start_but.Visibility = Visibility.Hidden;
            stop_but.Visibility = Visibility.Hidden;
            timer_pBar.IsIndeterminate = false;
            timer_pBar.Visibility = Visibility.Hidden;
            textBlock7.Visibility = Visibility.Hidden;
            stimelab.Visibility = Visibility.Hidden;
        }

        private void timeTracker_lab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            timesheet_visible_grid.Visibility = Visibility.Hidden;
            timeTracker_visible_grid.Visibility = Visibility.Visible;
            timesheet_tBlock_tab.Visibility = Visibility.Visible;
            timetracker_tBlock_tab.Visibility = Visibility.Hidden;

            try
            {
                DateTime dt1 = Convert.ToDateTime(xrw.FileRead("date"));
                if (((dt1.Year == DateTime.Now.Year) && (dt1.Month == DateTime.Now.Month) && (dt1.Day == DateTime.Now.Day)))
                {
                    stop_but.Visibility = Visibility.Visible;
                    timer_pBar.Visibility = Visibility.Visible;
                    timer_pBar.IsIndeterminate = true;
                    textBlock7.Visibility = Visibility.Visible;
                    stimelab.Visibility = Visibility.Visible;
                    stimelab.Content = Convert.ToString(xrw.FileRead("stime"));
                }
                else
                {
                    start_but.Visibility = Visibility.Visible;
                }

            }
            catch (Exception)
            {
                start_but.Visibility = Visibility.Visible;
            }
            select_dPicker.SelectedDate = DateTime.Now.Date;
            select_dPicker.IsEnabled = false;

        }

        private void prjName_cbox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            if (cmb.SelectedIndex.ToString() != "-1")
            {
                int selectedIndex = cmb.SelectedIndex;
                String selectedValue = cmb.SelectedValue.ToString();
                DataTable result = new DataTable();
                result.Columns.Add("tid");
                result.Columns.Add("t_name");
                var collection = (from details in detailTable.AsEnumerable() where details["ProjectId"].ToString() == selectedValue orderby details["IssueTypeId"] select new { id = details["IssueTypeId"], issue = details["IssueType"] }).Distinct();
                foreach (var item in collection)
                {
                    result.Rows.Add(item.id, item.issue);
                }
                issue_cbox.ItemsSource = null;
                catogory_cBox.ItemsSource = result.DefaultView;
                catogory_cBox.SelectedValuePath = result.Columns[0].ToString();
                catogory_cBox.DisplayMemberPath = result.Columns[1].ToString();
            }
        }

        private void add_but_Click(object sender, RoutedEventArgs e)
        {
            if (select_dPicker.Text != "" && prjName_cbox.Text != "" && catogory_cBox.Text != "" && issue_cbox.Text != "")
            {
                /* if ((select_dPicker.SelectedDate == DateTime.Now.Date) || checkLastDate())
                 {
                 */

                if (select_dPicker.SelectedDate.Value.Date <= DateTime.Now.Date)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    string commentString = comment.Text;
                    commentString = commentString.Replace("'", "''");
                    InsertTable(prjName_cbox.SelectedValue.ToString(), prjName_cbox.Text, catogory_cBox.SelectedValue.ToString(), catogory_cBox.Text, issue_cbox.SelectedValue.ToString(),(issue_cbox.Text).Replace("'", "\\'"), Convert.ToDateTime("00:00").ToString("HH:mm"), Convert.ToDateTime("00:00").ToString("HH:mm"), select_dPicker.Text,commentString);
                    reloadListView();
                    catogory_cBox.Text = "";
                    issue_cbox.Text = "";
                    comment.Text = "";
                    //sendApprovedcBox.IsChecked = false;
                    Mouse.OverrideCursor = null;
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Sorry, permission has expired.", "BQuTMSWithJira Incorrect Date Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please select the field(s).", "BQuTMSWithJira Empty Field(s)", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //item database insertion code
        void InsertTable(string projectId, string projname, string catId, string categoryname,string issue_id, string issue, string pstime, string petime, string date, string comment)
        {
            try
            {
                string quary = "";
                if (Connection.MyConnection.State != ConnectionState.Open)
                {
                    Connection.MyConnection.Open();
                }
                using (OdbcCommand MyCommand = new OdbcCommand(quary, Connection.MyConnection))
                {
                    int time = 1;

                    if (timeTracker_visible_grid.Visibility == Visibility.Visible)
                    {
                        time = Convert.ToInt32(DateTime.Parse(petime).Subtract(DateTime.Parse(pstime)).TotalMinutes);
                    }
                    else
                    {
                        time = Convert.ToDateTime(start_TPicker.Value).Hour * 60 + Convert.ToDateTime(start_TPicker.Value).Minute;
                    }

                    string worklog_idPlusIssue_id = UpdateJiraWorkLogTime(time, issue_id, comment, Convert.ToDateTime(date));
                    int status = 0;//(sendApprovedcBox.IsChecked == true) ? 0 : 1;
                    quary = "INSERT INTO tms_timesheet (project_id,project_name, issue_id,category_name,employee_id,date_year,date_month,date_day,work_time,category_id,category_type_name,start_time,end_time,status,comment,worklog_id) VALUES ('" + projectId + "','" + projname + "','" + issue_id + "','" + issue + "','" + Login.userid + "','" + Convert.ToDateTime(date).Year + "','" + Convert.ToDateTime(date).Month + "','" + Convert.ToDateTime(date).Day + "','" + time + "','" + catId + "','" + categoryname + "','" + Convert.ToDateTime(pstime).ToString("HH:mm:ss") + "','" + Convert.ToDateTime(petime).ToString("HH:mm:ss") + "','" + status + "','" + comment + "','" + worklog_idPlusIssue_id + "')";
                    MyCommand.CommandText = quary;
                    MyCommand.ExecuteNonQuery();//insert values to hrm_timesheet table
                }
            }
            catch (Exception)
            {
                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server.", "BQuTMSWithJira Connection Fail", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private string UpdateJiraWorkLogTime(int time, string issue_id, string comment, DateTime dateofwork)
        {
            WorkLogDetails details = new WorkLogDetails();
            details.Comment = comment;
            details.timeSpent = time.ToString();
            string worklog_id = manager.updateIssueLog(issue_id, details);
            return issue_id + "." + worklog_id;
            #region old datalog
            //string user = UserConfig.configlist[0].ToString();
            //string password = UserConfig.configlist[1].ToString();
            //string token = soapSDK.Login(user, password);
            //bool b = soapSDK.updateIssue(token, issue_id, time, comment, dateofwork);
            //soapSDK.Logout(token);
            #endregion
        }

        private void select_dPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                reloadListView();
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. Please refresh (Help -> Refresh) and try again.", " BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //reload listview items when change date
        void reloadListView()
        {
            timesheetlist.Clear();
            try
            {
                //select tms_timesheet.timesheet_id ,tms_timesheet.work_time,tms_projects.project_name, tms_category.category_name, tms_timesheet.category_name, CASE tms_timesheet.status WHEN -1 THEN 'Rejected' WHEN 0 THEN 'Pending' WHEN 1 THEN 'User Approved' WHEN 2 THEN 'Admin Approved' END AS status from tms_timesheet inner join tms_projects on tms_timesheet.project_id =tms_projects.project_id inner join tms_category on tms_timesheet.category_id =tms_category.category_id where tms_timesheet.date_year='" + select_dPicker.SelectedDate.Value.Year + "' and tms_timesheet.date_month ='" + select_dPicker.SelectedDate.Value.Month + "' and tms_timesheet.date_day='" + select_dPicker.SelectedDate.Value.Day + "' and employee_id='" + Login.userid + "'"

                using (OdbcCommand MyCommand = new OdbcCommand("select tms_timesheet.timesheet_id ,tms_timesheet.work_time,tms_timesheet.project_name, tms_timesheet.category_type_name, tms_timesheet.category_name, CASE tms_timesheet.status WHEN -1 THEN 'Rejected' WHEN 0 THEN 'Pending' WHEN 1 THEN 'User Approved' WHEN 2 THEN 'Admin Approved' END AS status from tms_timesheet where tms_timesheet.date_year='" + select_dPicker.SelectedDate.Value.Year + "' and tms_timesheet.date_month ='" + select_dPicker.SelectedDate.Value.Month + "' and tms_timesheet.date_day='" + select_dPicker.SelectedDate.Value.Day + "' and employee_id='" + Login.userid + "'", Connection.MyConnection))
                {
                    //select catogory for relevent project
                    MyDataReader = MyCommand.ExecuteReader();
                    string temphour = "";
                    string tempmin = "";
                    int totaltime = 0;
                    while (MyDataReader.Read())
                    {
                        totaltime = totaltime + MyDataReader.GetInt32(1);
                        temphour = Convert.ToString(MyDataReader.GetInt32(1) / 60);
                        if (temphour.Length < 2)
                        {
                            temphour = "0" + temphour;
                        }
                        tempmin = Convert.ToString(MyDataReader.GetInt32(1) % 60);
                        if (tempmin.Length < 2)
                        {
                            tempmin = "0" + tempmin;
                        }
                        timesheetlist.Add(new AddTimesheet(MyDataReader.GetInt32(0), temphour + " : " + tempmin, MyDataReader.GetString(2), MyDataReader.GetString(3), MyDataReader.GetString(4), MyDataReader.GetString(5)));
                    }
                    MyDataReader.Close();
                    tottime_lab.Content = TimeDisplay(totaltime);
                    add_lBox.ItemsSource = timesheetlist;
                    add_lBox.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " ");
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

        //items delete code
        void DeleteSelected()
        {
            try
            {
                if (Connection.MyConnection.State == ConnectionState.Open)
                {
                    var selectedStockObject = add_lBox.SelectedItems[0] as AddTimesheet;//select id
                    string issue_key_dot_worklog_id = "";
                   
                    if (selectedStockObject == null)
                    {
                        return;
                    }
                    using (OdbcCommand MyCommand2 = new OdbcCommand("Select worklog_id from tms_timesheet where timesheet_id='" + selectedStockObject.ID + "' ", Connection.MyConnection))
                    {
                        issue_key_dot_worklog_id = MyCommand2.ExecuteScalar().ToString() ;
                        if (issue_key_dot_worklog_id != "")
                        {
                            string[] parts = issue_key_dot_worklog_id.Split('.');
                            manager.deleteWorkLog(parts[0].ToString(), parts[1].ToString());
                        }
                    }
                    using (OdbcCommand MyCommand = new OdbcCommand("DELETE FROM tms_timesheet WHERE timesheet_id='" + selectedStockObject.ID + "'", Connection.MyConnection))
                    {
                        MyCommand.ExecuteNonQuery();
                        string[] labletime = (tottime_lab.Content).ToString().Split('.');
                        string[] deletetime = (selectedStockObject.WTime).ToString().Split(':');
                        int tempworktime = ((Convert.ToInt32(labletime[0])) * 60 + Convert.ToInt32(labletime[1])) - ((Convert.ToInt32(deletetime[0])) * 60 + Convert.ToInt32(deletetime[1]));
                        tottime_lab.Content = TimeDisplay(tempworktime);
                        timesheetlist.RemoveAt(add_lBox.SelectedIndex);
                        add_lBox.ItemsSource = timesheetlist;
                        add_lBox.Items.Refresh();
                    }
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. Please refresh (Help -> Refresh) and try again.", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Duhh");
            }
        }

        private void add_lBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Delete) && (Microsoft.Windows.Controls.MessageBox.Show("Do you really want to delete this item?", "BQuTMSWithJira Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
            {
                Mouse.OverrideCursor = Cursors.Wait;
                DeleteSelected();
                Mouse.OverrideCursor = null;
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                start_TPicker.Value = Convert.ToDateTime("02:00");
                if (checkflag)
                {
                    select_dPicker.SelectedDate = Login.tsUpdateLastDay;
                    checkflag = false;
                }
            }
        }

        public void getDetailsFromJira()
        {
            this.user = UserConfig.configlist[0].ToString();
            this.password = UserConfig.configlist[1].ToString();
            this.manager = new JiraManager(user, password);
            List<Issue> issueList = manager.GetEmployeeOpenIssues(user);

            foreach(Issue issue in issueList)
            {
                detailTable.Rows.Add(issue.Key, issue.Fields.Summary, issue.Fields.issuetype.Id, issue.Fields.issuetype.Name, issue.Fields.project.Id, issue.Fields.project.Key, issue.Fields.project.Name);
            }


            #region old method
            //string token = soapSDK.Login(user, password);
            //RemoteIssue[] issueList = soapSDK.getUserIssues(token, user);
            //RemoteIssueType[] issueTypes = soapSDK.getUserIssueTypes(token);
            //RemoteProject[] projects = soapSDK.getAllProjects(token);
            //for (int i = 0; i < issueList.Length; i++)
            //{
            //    string typename = "Sub task";
            //    string projectName = string.Empty;
            //    if (issueList[i].status.ToString() == "5" || issueList[i].status.ToString() == "6")
            //    {
            //        //do nothing
            //        //neglecting closed and resolved issues
            //    }
            //    else
            //    {
            //        foreach (RemoteIssueType t in issueTypes)
            //        {
            //            if (t.id == issueList[i].type.ToString())
            //            {
            //                typename = t.name;
            //            }
            //        }

            //        foreach (RemoteProject p in projects)
            //        {
            //            if (p.key == issueList[i].project.ToString())
            //            {
            //                projectName = p.name;
            //            }
            //        }

            //        detailTable.Rows.Add(issueList[i].key.ToString(), issueList[i].summary.ToString(), issueList[i].type.ToString(), typename, issueList[i].project.ToString(), projectName);
            //    }
            //}

            //soapSDK.Logout(token);
            #endregion

            populateProjectcombobox(detailTable);
        }

        void populateProjectcombobox(DataTable Table)
        {
            DataTable result = new DataTable();
            result.Columns.Add("pid");
            result.Columns.Add("p_name");
            var results = (from myRow in Table.AsEnumerable() select myRow["ProjectId"]).Distinct();
            ////var collection = (from details in Table.AsEnumerable()  orderby details["ProjectId"] select new { projectid = details["ProjectId"], project = details["ProjectName"] }).Distinct();
            foreach (var item in results)
            {
                DataRow[] row = Table.Select("ProjectId ='" + item.ToString() + "'");
                string projectName = row[0]["ProjectName"].ToString();

                result.Rows.Add(item.ToString(), projectName);
            }

            result.DefaultView.Sort = "p_name";
            prjName_cbox.ItemsSource = result.DefaultView;
            prjName_cbox.SelectedValuePath = result.Columns["pid"].ToString();
            prjName_cbox.DisplayMemberPath = result.Columns["p_name"].ToString();
        }

        private void stop_but_Click(object sender, RoutedEventArgs e)
        {
            if (Connection.MyConnection.State != ConnectionState.Open)
            {
                Connection.MyConnection.Open();
            }
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                try
                {
                    XmlTextReader reader = new XmlTextReader(Util.GetUserDataPath() + "\\tmsdata.xml");
                    XmlNodeType type;
                    string date = string.Empty;
                    string project = string.Empty;
                    string project_id = string.Empty;
                    string category = string.Empty;
                    string category_id = string.Empty;
                    string note = string.Empty;
                    string issue_id = string.Empty;
                    string stime = string.Empty;
                    while (reader.Read())
                    {
                        type = reader.NodeType;
                        if (type == XmlNodeType.Element)
                        {
                            if (reader.Name == "date")
                            {
                                reader.Read();
                                date = reader.Value;
                            }
                            if (reader.Name == "project")
                            {
                                reader.Read();
                                project = reader.Value;
                            }
                            if (reader.Name == "project_id")
                            {
                                reader.Read();
                                project_id = reader.Value;
                            }
                            if (reader.Name == "category")
                            {
                                reader.Read();
                                category = reader.Value;
                            }
                            if (reader.Name == "category_id")
                            {
                                reader.Read();
                                category_id = reader.Value;
                            }
                            if (reader.Name == "note")
                            {
                                reader.Read();
                                note = reader.Value;
                            }
                            if (reader.Name == "issue_id")
                            {
                                reader.Read();
                                issue_id = reader.Value;
                            }
                            if (reader.Name == "stime")
                            {
                                reader.Read();
                                stime = reader.Value;
                            }
                        }
                    }
                    reader.Close();
                    if (Microsoft.Windows.Controls.MessageBox.Show("You are working on " + project + " project from " + stime + " to " + DateTime.Now.ToShortTimeString() + " .Do you want to stop?", "BQuTMSWithJira Stop Timer Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        timer_pBar.Visibility = Visibility.Hidden;
                        textBlock7.Visibility = Visibility.Hidden;
                        stimelab.Visibility = Visibility.Hidden;
                        InsertTable(project_id, project, category_id, category, issue_id,(note).Replace("'", "\\'"), stime, DateTime.Now.ToShortTimeString(), date,comment.Text);
                        start_but.Visibility = Visibility.Visible;
                        stop_but.Visibility = Visibility.Hidden;
                        timer_pBar.IsIndeterminate = false;
                        xrw.UpdateSingleValue(string.Empty, "TimeSheet", "date");
                        prjName_cbox.IsEnabled = true;
                        catogory_cBox.IsEnabled = true;
                        start_TPicker.IsEnabled = true;
                        //note_tBox.IsEnabled = true;
                        reloadListView();
                        //note_tBox.Text = "";
                        issue_cbox.Text = "";
                        catogory_cBox.Text = "";
                        Mouse.OverrideCursor = null;
                    }
                }
                catch (Exception ex)
                {
                    Microsoft.Windows.Controls.MessageBox.Show(ex + " ");
                }
            }
        }

        private void start_but_Click(object sender, RoutedEventArgs e)
        {
            if ((prjName_cbox.Text != "") && (catogory_cBox.Text != ""))
            {
                if (Microsoft.Windows.Controls.MessageBox.Show("Do you want to start the Time Tracker?", "BQuTMSWithJira Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    //note_tBox.Text,
                    xrw.TimeSheetStart(select_dPicker.SelectedDate.Value.ToShortDateString(), prjName_cbox.Text, catogory_cBox.Text, issue_cbox.Text, DateTime.Now.ToShortTimeString(), prjName_cbox.SelectedValue.ToString(), issue_cbox.SelectedValue.ToString(), catogory_cBox.SelectedValue.ToString());//write to the file in time sheet started details
                    start_but.Visibility = Visibility.Hidden;
                    stop_but.Visibility = Visibility.Visible;
                    timer_pBar.Visibility = Visibility.Visible;
                    timer_pBar.IsIndeterminate = true;
                    textBlock7.Visibility = Visibility.Visible;
                    stimelab.Visibility = Visibility.Visible;
                    stimelab.Content = DateTime.Now.ToShortTimeString();
                    prjName_cbox.IsEnabled = false;
                    catogory_cBox.IsEnabled = false;
                    start_TPicker.IsEnabled = false;
                    //note_tBox.IsEnabled = false;
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please select the field(s)", "BQuTMSWithJira Empty Field(s)", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void catogory_cBox_DropDownClosed(object sender, EventArgs e)
        {
            if (catogory_cBox.SelectedIndex.ToString() != "-1")
            {
                ComboBox cmb = (ComboBox)sender;
                int selectedIndex = cmb.SelectedIndex;
                string selectedValue = cmb.SelectedValue.ToString();
                string selectedprojectid = prjName_cbox.SelectedValue.ToString();
                DataTable result = new DataTable();
                result.Columns.Add("iid");
                result.Columns.Add("i_name");
                var collection = (from details in detailTable.AsEnumerable() where details["ProjectId"].ToString() == selectedprojectid && details["IssueTypeId"].ToString() == selectedValue select new { id = details["IssueKey"], issue = details["Issue"] }).Distinct();
                foreach (var item in collection)
                {
                    result.Rows.Add(item.id, item.id + "\t" + item.issue);
                }
                issue_cbox.ItemsSource = result.DefaultView;
                issue_cbox.SelectedValuePath = result.Columns[0].ToString();
                issue_cbox.DisplayMemberPath = result.Columns[1].ToString();
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://bqujira.atlassian.net/issues/?filter=-1");
        }

    }
}
