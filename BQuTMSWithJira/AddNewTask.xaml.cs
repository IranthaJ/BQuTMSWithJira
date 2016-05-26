using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.Odbc;

namespace BQuTMSWithJira
{
    /// <summary>
    /// Interaction logic for AddNewTask.xaml
    /// </summary>
    public partial class AddNewTask : UserControl
    {
        public AddNewTask()
        {
            InitializeComponent();
        }
        public static bool checkflaglr = true;

        OdbcDataReader MyDataReader;
        int[] taskUPidarray = new int[3];
        int[] taskOvridarray = new int[2];
        private bool checkflagtaskadd = true;
        private bool checkflagtaskhistory = true;
        private bool checkflagviewtask = true;
        ReminderTSAdd RTA = new ReminderTSAdd();
        List<AddTask> tasklist = new List<AddTask>();//add timesheet table items to this list

        private void task_lab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            hideControllers();
            hideUP1();
            hideUP2();
            hideUP3();
            hideOvr1();
            hideOvr2();

            ConfigureTask();
            taskhistory_click_tBlock.Visibility = Visibility.Visible;
            tasks_grid.Visibility = Visibility.Visible;
        }

        private void taskhistory_lab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            hideControllers();
            showTaskHistory();
            task_click_tBlock.Visibility = Visibility.Visible;
            taskhistory_grid.Visibility = Visibility.Visible;
        }

        private void showTaskHistory()
        {
            if (checkflagtaskhistory)
            {
                using (OdbcCommand MyCommand = new OdbcCommand("select tms_projects.project_name from  tms_projects inner join tms_userinproject on tms_projects.project_id=tms_userinproject.project_id where tms_userinproject.employee_id='" + Login.userid + "' and tms_projects.status=1", Connection.MyConnection))
                {
                    if (prjName_cbox.Items.IsEmpty)
                    {
                        MyDataReader = MyCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            prjName_cbox.Items.Add(MyDataReader.GetString(0));
                        }
                        MyDataReader.Close();
                    }
                }
                checkflagtaskhistory = false;
            }
            reloadListView();
        }

        private void addtask_btn_img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            hideControllers();
            showTaskAdd();
            addtask_grid.Visibility = Visibility.Visible;
            taskhistory_click_tBlock.Visibility = Visibility.Visible;
            task_click_tBlock.Visibility = Visibility.Visible;
        }

        void hideControllers() 
        {
            taskhistory_click_tBlock.Visibility = Visibility.Hidden;
            task_click_tBlock.Visibility = Visibility.Hidden;
            addtask_grid.Visibility = Visibility.Hidden;
            taskhistory_grid.Visibility = Visibility.Hidden;
            tasks_grid.Visibility = Visibility.Hidden;
        }

        private void MenuItem_delete(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteSelected();
            }
            catch (Exception)
            {
                Microsoft.Windows.Controls.MessageBox.Show("Incorrect selection", "BQuTMSWithJira Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible && checkflagviewtask)
            {
                ConfigureTask();
                checkflagviewtask = false;
            }
        }

        private void ConfigureTask()
        {
            using (OdbcCommand MyCommand = new OdbcCommand("SELECT tms_reminders.rid , tms_reminders.reminder, tms_reminders.duedate ,tms_reminders.minute, tms_reminders.priority,  tms_reminders.note FROM tms_reminders WHERE tms_reminders.assignedto='" + Login.userid + "' AND tms_reminders.duedate> NOW() ORDER BY tms_reminders.duedate LIMIT 3 ", Connection.MyConnection))
            {
                MyDataReader = MyCommand.ExecuteReader();
                for (int i = 1; i < 4; i++)
                {
                    if (MyDataReader.Read())
                    {
                        if (i == 1)
                        {
                            taskUPidarray[0] = MyDataReader.GetInt32(0);
                            tasknameUp_tBlock1.Text = MyDataReader.GetString(1);

                            dateUp_tBlock1.Text = MyDataReader.GetDateTime(2).ToString("yyyy-MM-dd hh:mmtt");
                            tothourUp_tBlock1.Text = TimeDisplay(MyDataReader.GetInt32(3));
                            colorUp_lal1.Background = ColorCode(MyDataReader.GetString(4));
                            //add tooltip
                            ToolTip tooltip1 = new ToolTip { Content = MyDataReader.GetString(5) };
                            taskbackUp_grid1.ToolTip = tooltip1;
                          //  dateUp_tBlock1.ToolTip = tooltip1;
                            if (MyDataReader.GetString(5) != String.Empty)
                            {
                                tooltip1.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                tooltip1.Visibility = Visibility.Hidden;
                            }
                            showUP1();
                        }
                        else if (i == 2)
                        {
                            taskUPidarray[1] = MyDataReader.GetInt32(0);
                            tasknameUp_tBlock2.Text = MyDataReader.GetString(1);

                            dateUp_tBlock2.Text = MyDataReader.GetDateTime(2).ToString("yyyy-MM-dd hh:mmtt");
                            tothourUp_tBlock2.Text = TimeDisplay(MyDataReader.GetInt32(3));
                            colorUp_lal2.Background = ColorCode(MyDataReader.GetString(4));

                            ToolTip tooltip2 = new ToolTip { Content = MyDataReader.GetString(5) };
                            taskbackUp_grid2.ToolTip = tooltip2;
                            if (MyDataReader.GetString(5) != String.Empty)
                            {
                                tooltip2.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                tooltip2.Visibility = Visibility.Hidden;
                            }
                            showUP2();
                        }
                        else if (i == 3)
                        {
                            taskUPidarray[2] = MyDataReader.GetInt32(0);
                            tasknameUp_tBlock3.Text = MyDataReader.GetString(1);

                            dateUp_tBlock3.Text = MyDataReader.GetDateTime(2).ToString("yyyy-MM-dd hh:mmtt");
                            tothourUp_tBlock3.Text = TimeDisplay(MyDataReader.GetInt32(3));
                            colorUp_lal3.Background = ColorCode(MyDataReader.GetString(4));
                            ToolTip tooltip3 = new ToolTip { Content = MyDataReader.GetString(5) };
                            taskbackUp_grid3.ToolTip = tooltip3;

                            if (MyDataReader.GetString(5) != String.Empty)
                            {
                                tooltip3.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                tooltip3.Visibility = Visibility.Hidden;
                            }
                            showUP3();
                        }
                    }
                }

                MyDataReader.Close();
            }

            using (OdbcCommand MyCommand = new OdbcCommand("SELECT tms_reminders.rid , tms_reminders.reminder, tms_reminders.duedate ,tms_reminders.minute, tms_reminders.priority,tms_reminders.note FROM tms_reminders WHERE tms_reminders.assignedto='" + Login.userid + "' AND tms_reminders.duedate< NOW() ORDER BY tms_reminders.duedate DESC LIMIT 2 ", Connection.MyConnection))
            {

                MyDataReader = MyCommand.ExecuteReader();

                for (int i = 1; i < 3; i++)
                {
                    if (MyDataReader.Read())
                    {
                        if (i == 1)
                        {
                            taskOvridarray[0] = MyDataReader.GetInt32(0);
                            tasknameOvr_tBlock1.Text = MyDataReader.GetString(1);

                            dateOvr_tBlock1.Text = MyDataReader.GetDateTime(2).ToString("yyyy-MM-dd hh:mmtt");
                            tothourOvr_tBlock1.Text = TimeDisplay(MyDataReader.GetInt32(3));
                            colorOvr_lal1.Background = ColorCode(MyDataReader.GetString(4));

                            ToolTip toolOvrtip1 = new ToolTip { Content = MyDataReader.GetString(5) };
                            taskbackOvr_grid1.ToolTip = toolOvrtip1;
                            if (MyDataReader.GetString(5) != String.Empty)
                            {

                                toolOvrtip1.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                toolOvrtip1.Visibility = Visibility.Hidden;
                            }
                            showOvr1();

                        }
                        else if (i == 2)
                        {
                            taskOvridarray[1] = MyDataReader.GetInt32(0);
                            tasknameOvr_tBlock2.Text = MyDataReader.GetString(1);

                            dateOvr_tBlock2.Text = MyDataReader.GetDateTime(2).ToString("yyyy-MM-dd hh:mmtt");
                            tothourOvr_tBlock2.Text = TimeDisplay(MyDataReader.GetInt32(3));
                            colorOvr_lal2.Background = ColorCode(MyDataReader.GetString(4));

                            ToolTip toolOvrtip2 = new ToolTip { Content = MyDataReader.GetString(5) };
                            taskbackOvr_grid2.ToolTip = toolOvrtip2;
                            if (MyDataReader.GetString(5) != String.Empty)
                            {
                                toolOvrtip2.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                toolOvrtip2.Visibility = Visibility.Hidden;
                            }
                            showOvr2();
                        }

                    }
                }

                MyDataReader.Close();
            }
        }


        private SolidColorBrush ColorCode(string priority)
        {

            if (priority == "High")
            {
                return Brushes.Red;
            }
            else if (priority == "Medium")
            {
                return Brushes.Green;
            }
            else
            {
                return Brushes.Yellow;
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

        private void submitUp_cBox1_Checked(object sender, RoutedEventArgs e)
        {
            if (submitUp_cBox1.IsChecked == true)
            {
                RTA.ID = taskUPidarray[0];
                RTA.NOTE = tasknameUp_tBlock1.Text;
                RTA.HOUR = tothourUp_tBlock1.Text;
                RTA.Load();
                RTA.ShowDialog();
                if (ReminderTSAdd.checkflag == false)
                {
                    submitUp_cBox1.IsChecked = false;
                }
                else
                {
                    submitUp_cBox1.IsEnabled = false;
                    //console.writeline("comecome");
                }
            }
        }

        private void submitUp_cBox2_Checked(object sender, RoutedEventArgs e)
        {
            if (submitUp_cBox2.IsChecked == true)
            {
                RTA.ID = taskUPidarray[1];
                RTA.NOTE = tasknameUp_tBlock2.Text;
                RTA.HOUR = tothourUp_tBlock2.Text;
                RTA.Load();
                RTA.ShowDialog();
                if (ReminderTSAdd.checkflag == false)
                {
                    submitUp_cBox2.IsChecked = false;
                }
            }
            else
            {
                submitUp_cBox2.IsEnabled = false;
            }
        }

        private void submitUp_cBox3_Checked(object sender, RoutedEventArgs e)
        {
            if (submitUp_cBox3.IsChecked == true)
            {
                RTA.ID = taskUPidarray[2];
                RTA.NOTE = tasknameUp_tBlock3.Text;
                RTA.HOUR = tothourUp_tBlock3.Text;
                RTA.Load();
                RTA.ShowDialog();
                if (ReminderTSAdd.checkflag == false)
                {
                    submitUp_cBox3.IsChecked = false;
                }
                else
                {
                    submitUp_cBox3.IsEnabled = false;
                }
            }
        }


        private void submitOvr_cBox1_Checked(object sender, RoutedEventArgs e)
        {
            if (submitOvr_cBox1.IsChecked == true)
            {
                RTA.ID = taskOvridarray[0];
                RTA.NOTE = tasknameOvr_tBlock1.Text;
                RTA.HOUR = tothourOvr_tBlock1.Text;
                RTA.Load();
                RTA.ShowDialog();
                if (ReminderTSAdd.checkflag == false)
                {
                    submitOvr_cBox1.IsChecked = false;
                }
                else
                {
                    submitOvr_cBox1.IsEnabled = false;
                }
            }
        }

        private void submitOvr_cBox2_Checked(object sender, RoutedEventArgs e)
        {
            if (submitOvr_cBox2.IsChecked == true)
            {
                RTA.ID = taskOvridarray[1];
                RTA.NOTE = tasknameOvr_tBlock2.Text;
                RTA.HOUR = tothourOvr_tBlock2.Text;
                RTA.Load();
                RTA.ShowDialog();
                //console.writeline("111");
                if (ReminderTSAdd.checkflag == false)
                {
                    //console.writeline("222");
                    submitOvr_cBox2.IsChecked = false;
                }
                else
                {
                    //console.writeline("333");
                    submitOvr_cBox2.IsEnabled = false;
                }
            }
        }


        private void showUP1()
        {

            taskUp_border1.Visibility = Visibility.Visible;
            colorUp_lal1.Visibility = Visibility.Visible;
            submitUp_cBox1.Visibility = Visibility.Visible;
            //tasknameUp_lab1.Visibility = Visibility.Visible;
            //tothourUp_lab1.Visibility = Visibility.Visible;
            //thourUptBlock1.Visibility = Visibility.Visible;

        }

        private void showUP2()
        {
            taskUp_border2.Visibility = Visibility.Visible;
            colorUp_lal2.Visibility = Visibility.Visible;
            submitUp_cBox2.Visibility = Visibility.Visible;
            //tasknameUp_lab2.Visibility = Visibility.Visible;
            //tothourUp_lab2.Visibility = Visibility.Visible;
            //thourUptBlock2.Visibility = Visibility.Visible;
        }

        private void showUP3()
        {
            taskUp_border3.Visibility = Visibility.Visible;
            colorUp_lal3.Visibility = Visibility.Visible;
            submitUp_cBox3.Visibility = Visibility.Visible;
            //colorUp_lal3.Visibility = Visibility.Visible;
            //tasknameUp_lab3.Visibility = Visibility.Visible;
            //tothourUp_lab3.Visibility = Visibility.Visible;
            //thourUptBlock3.Visibility = Visibility.Visible;
        }

        private void showOvr1()
        {
            taskOvr_border1.Visibility = Visibility.Visible;
            colorOvr_lal1.Visibility = Visibility.Visible;
            submitOvr_cBox1.Visibility = Visibility.Visible;
            //colorOvr_lal1.Visibility = Visibility.Visible;
            //tasknameOvr_lab1.Visibility = Visibility.Visible;
            //tothourOvr_lab1.Visibility = Visibility.Visible;
            //thourOvrtBlock1.Visibility = Visibility.Visible;
        }

        private void showOvr2()
        {
            taskOvr_border2.Visibility = Visibility.Visible;
            colorOvr_lal2.Visibility = Visibility.Visible;
            submitOvr_cBox2.Visibility = Visibility.Visible;
            //colorOvr_lal2.Visibility = Visibility.Visible;
            //tasknameOvr_lab2.Visibility = Visibility.Visible;
            //tothourOvr_lab2.Visibility = Visibility.Visible;
            //thourOvrtBlock2.Visibility = Visibility.Visible;
        }

        ///////////////////////////////////////////////
        private void hideUP1()
        {
            taskUp_border1.Visibility = Visibility.Hidden;
            colorUp_lal1.Visibility = Visibility.Hidden;
            submitUp_cBox1.Visibility = Visibility.Hidden;
            submitUp_cBox1.IsChecked = false;
            //tasknameUp_lab1.Visibility = Visibility.Hidden;
            //tothourUp_lab1.Visibility = Visibility.Hidden;
            //thourUptBlock1.Visibility = Visibility.Hidden;
        }

        private void hideUP2()
        {
            taskUp_border2.Visibility = Visibility.Hidden;
            colorUp_lal2.Visibility = Visibility.Hidden;
            submitUp_cBox2.Visibility = Visibility.Hidden;
            submitUp_cBox2.IsChecked = false;
            //tasknameUp_lab2.Visibility = Visibility.Hidden;
            //tothourUp_lab2.Visibility = Visibility.Hidden;
            //thourUptBlock2.Visibility = Visibility.Hidden;
        }

        private void hideUP3()
        {
            taskUp_border3.Visibility = Visibility.Hidden;
            colorUp_lal3.Visibility = Visibility.Hidden;
            submitUp_cBox3.Visibility = Visibility.Hidden;
            submitUp_cBox3.IsChecked = false;
            //tasknameUp_lab3.Visibility = Visibility.Hidden;
            //tothourUp_lab3.Visibility = Visibility.Hidden;
            //thourUptBlock3.Visibility = Visibility.Hidden;
        }

        private void hideOvr1()
        {
            taskOvr_border1.Visibility = Visibility.Hidden;
            colorOvr_lal1.Visibility = Visibility.Hidden;
            submitOvr_cBox1.Visibility = Visibility.Hidden;
            submitOvr_cBox1.IsChecked = false;
            //tasknameOvr_lab1.Visibility = Visibility.Hidden;
            //tothourOvr_lab1.Visibility = Visibility.Hidden;
            //thourOvrtBlock1.Visibility = Visibility.Hidden;
        }

        private void hideOvr2()
        {
            taskOvr_border2.Visibility = Visibility.Hidden;
            colorOvr_lal2.Visibility = Visibility.Hidden;
            submitOvr_cBox2.Visibility = Visibility.Hidden;
            submitOvr_cBox2.IsChecked = false;
            //tasknameOvr_lab2.Visibility = Visibility.Hidden;
            //tothourOvr_lab2.Visibility = Visibility.Hidden;
            //thourOvrtBlock2.Visibility = Visibility.Hidden;
        }

        private void add_lBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelected();
            }
        }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //items delete code
        void DeleteSelected()
        {
            try
            {
                if (Connection.MyConnection.State == ConnectionState.Open)
                {
                    var selectedStockObject = add_lBox.SelectedItems[0] as AddTask;//select id
                    if (selectedStockObject == null)
                    {
                        return;
                    }
                    if (Microsoft.Windows.Controls.MessageBox.Show("Do you really want to delete this item?", "BQuTMSWithJira Delete confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        using (OdbcCommand MyCommand = new OdbcCommand("DELETE FROM tms_reminders WHERE rid='" + selectedStockObject.ID + "'", Connection.MyConnection))
                        {
                            MyCommand.ExecuteNonQuery();
                            tasklist.RemoveAt(add_lBox.SelectedIndex);
                            add_lBox.ItemsSource = tasklist;
                            add_lBox.Items.Refresh();
                            if (TaskReminderCount.rid != 0)
                            {
                                TaskReminderCount.TaskCount();
                            }
                        }
                    }
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. Please refresh (Help -> Refresh) and try again.", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                //  MessageBox.Show(ex+"");
            }
        }

        private void add_but_Click(object sender, RoutedEventArgs e)
        {
            //update_tblock.Visibility = Visibility.Collapsed;
            if (prjName_cbox.Text != "" && catogory_cBox.Text != "" && name_wTextBox.Text != "" && duedate_dPicker.Value != null && noofhour_TPicker.Text != "" && pro_cBox.Text != "" && assgn_cBox.Text != "")
            {
                if (Convert.ToDateTime(duedate_dPicker.Value).Date > DateTime.Now.Date)
                {
                    InsertTable(prjName_cbox.Text, catogory_cBox.Text, (name_wTextBox.Text).Replace("'", "\\'"), Convert.ToDateTime(duedate_dPicker.Value), Convert.ToDateTime(noofhour_TPicker.Value).Hour * 60 + Convert.ToDateTime(noofhour_TPicker.Value).Minute, pro_cBox.Text, (note_tBox.Text).Replace("'", "\\'"), assgn_cBox.Text);

                    ///reset input box
                    catogory_cBox.Text = "";
                    prjName_cbox.Text = "";
                    note_tBox.Text = "";
                    name_wTextBox.Text = "";
                    pro_cBox.SelectedIndex = 1;
                    assgn_cBox.Text = "";
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Due date should be greater than today", "BQuTMSWithJira Invalid Date Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please select the field(s).", "BQuTMSWithJira Empty Field(s)", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //reload listview items when change date
        private void reloadListView()
        {
            tasklist.Clear();
            try
            {
                using (OdbcCommand MyCommand = new OdbcCommand("SELECT tms_reminders.rid ,tms_projects.project_name, tms_category.category_name, tms_reminders.reminder, tms_reminders.duedate FROM tms_reminders INNER JOIN tms_projects ON tms_reminders.project= tms_projects.project_id INNER JOIN tms_category ON tms_reminders.category =tms_category.category_id WHERE (tms_reminders.duedate LIKE '" + DateTime.Now.Year + "%" + "' OR tms_reminders.duedate LIKE '" + DateTime.Now.AddYears(1).Year + "%" + "' ) AND tms_reminders.assignedby= '" + Login.userid + "' order by rid Desc", Connection.MyConnection))
                {
                    //select catogory for relevent project
                    MyDataReader = MyCommand.ExecuteReader();
                    while (MyDataReader.Read())
                    {
                        tasklist.Add(new AddTask(MyDataReader.GetInt32(0), MyDataReader.GetString(1), MyDataReader.GetString(2), MyDataReader.GetString(3), MyDataReader.GetDateTime(4).ToString("yyyy-MM-dd hh:mm tt")));
                    }
                    MyDataReader.Close();
                    add_lBox.ItemsSource = tasklist;
                    add_lBox.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                Microsoft.Windows.Controls.MessageBox.Show(ex + " ");
            }
        }

        private void prjName_cbox_DropDownClosed(object sender, EventArgs e)
        {
            using (OdbcCommand MyCommand = new OdbcCommand("select tms_category.category_name from tms_projects inner join tms_category on tms_projects.project_id=tms_category.project_id where tms_projects.project_name= '" + prjName_cbox.Text + "' order by tms_category.category_name asc", Connection.MyConnection))
            {
                catogory_cBox.Items.Clear();
                MyDataReader = MyCommand.ExecuteReader();
                while (MyDataReader.Read())
                {
                    catogory_cBox.Items.Add(MyDataReader.GetString(0));
                }
                MyDataReader.Close();
            }
            using (OdbcCommand MyCommand = new OdbcCommand("select jos_users.username from jos_users inner join tms_userinproject on jos_users.id=tms_userinproject.employee_id inner join tms_projects on tms_userinproject.project_id=tms_projects.project_id where tms_projects.project_name= '" + prjName_cbox.Text + "'", Connection.MyConnection))
            {
                assgn_cBox.Items.Clear();
                MyDataReader = MyCommand.ExecuteReader();
                while (MyDataReader.Read())
                {
                    assgn_cBox.Items.Add(MyDataReader.GetString(0));
                }
                MyDataReader.Close();
            }
        }

        void InsertTable(string projname, string categoryname, string name, DateTime duedate, int noofmin, string prority, string note, string assginto)
        {
            using (OdbcCommand MyCommand = new OdbcCommand("insert into tms_reminders(duedate,priority,minute,assignedby,reminder,note,completed,project,category,assignedto) values('" + duedate.ToString("yyyy-MM-dd HH:mm:ss ") + "','" + prority + "','" + noofmin + "','" + Login.userid + "','" + name + "','" + note + "',0,(select tms_projects.project_id from tms_projects where tms_projects.project_name='" + projname + "') ,(select tms_category.category_id from tms_category inner join tms_projects on tms_projects.project_id=tms_category.project_id where tms_projects.project_name='" + projname + "' and tms_category.category_name='" + categoryname + "'), (select jos_users.id from jos_users where jos_users.username='" + assginto + "'))", Connection.MyConnection))
            {
                if (MyCommand.ExecuteNonQuery() == 1)
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Submitted successfully.", "BQuTMSWithJira Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void showTaskAdd()
        {
            noofhour_TPicker.Value = Convert.ToDateTime("05:00");
            if (checkflagtaskadd)
            {
                duedate_dPicker.Value = DateTime.Now;
                using (OdbcCommand MyCommand = new OdbcCommand("select tms_projects.project_name from  tms_projects inner join tms_userinproject on tms_projects.project_id=tms_userinproject.project_id where tms_userinproject.employee_id='" + Login.userid + "' and tms_projects.status=1 order by tms_projects.project_name asc", Connection.MyConnection))
                {
                    if (prjName_cbox.Items.IsEmpty)
                    {
                        MyDataReader = MyCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            prjName_cbox.Items.Add(MyDataReader.GetString(0));
                        }
                        MyDataReader.Close();
                    }
                }
                checkflagtaskadd = false;
            }
        }













      
    }
}
