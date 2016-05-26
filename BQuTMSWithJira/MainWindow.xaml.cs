using System;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Input;
using System.Xml;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls.Primitives;

namespace BQuTMSWithJira
{
    public partial class MainWindow : Window
    {
        Login lg = new Login();
        CheckServerConnection csc = new CheckServerConnection();
        string tempCheck;//for privious date timesheet add check
        public MainWindow()
        {
            InitializeComponent();
            // Has to be done in the Loaded event because the window
            // handle is not valid until the window has loaded.
            this.Loaded += OnLoaded;
            Notification.MessageReceived += messagetext =>
            {
                showNotification(messagetext);
            };
        }

        private void showNotification(string messagetext)
        {
            Dispatcher.Invoke(new Action(() =>
            {
            TaskbarIcon MyNotifyIcon;
            BaloonNotification balloon;
            balloon = new BaloonNotification();
            balloon.BalloonText = "A leave requested !";
            balloon.InfoText = "Please check your TMS web portal,\nYou have a pending leave approval. ";

            MyNotifyIcon = new TaskbarIcon();
            MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, null);
            }));
        }


        #region main window hight change methos
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Get the handle for this window.
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            // Get the Win32 window that hosts the WPF content.
            HwndSource window = HwndSource.FromHwnd(windowHandle);

            // Get the visual manager and set its background to transparent.
            window.CompositionTarget.BackgroundColor = Colors.Transparent;

            // Set the desired margins.
            MARGINS margins = new MARGINS();
            //margins.cxTopHeight = 30;
            margins.cxTopHeight = 81;
            // WPF is DPI independent.  Simply passing 30 into the
            // Windows API does not take into account differences in the
            // user's DPI settings.  We must adjust the margins to
            // reflect different settings.
            margins = AdjustForDPISettings(margins, windowHandle);

            // Call into the windows API to extend the frame.
            // Only supported on OS versions with Aero (Vista, 7).
            // Throws an exception on non-supported operating systems.
            int result = DwmExtendFrameIntoClientArea(windowHandle, ref margins);
        }

        /// <summary>
        /// Adjusts the margins based on the users DPI settings.
        /// </summary>
        /// <param name="input">The unadjusted margin.</param>
        /// <param name="hWnd">The window handle.</param>
        /// <returns>Adjusted margins.</returns>
        private MARGINS AdjustForDPISettings(MARGINS input, IntPtr hWnd)
        {
            MARGINS adjusted = new MARGINS();

            // Gets the graphic object from the window handle
            // so we can get the current DPI settings.
            var graphics = System.Drawing.Graphics.FromHwnd(hWnd);

            // The default DPI is 96.  This creates a ratio that
            // will be applied to the incoming values to adjust them
            // based on whatever the current DPI setting is.
            float dpiRatioX = graphics.DpiX / 96;
            float dpiRatioY = graphics.DpiY / 96;

            // Adjust settings.
            adjusted.cxLeftWidth = (int)(input.cxLeftWidth * dpiRatioX);
            adjusted.cxRightWidth = (int)(input.cxRightWidth * dpiRatioX);
            adjusted.cxTopHeight = (int)(input.cxTopHeight * dpiRatioY);
            adjusted.cxBottomHeight = (int)(input.cxBottomHeight * dpiRatioY);

            return adjusted;
        }

        /// <summary>
        /// Structure to hold the new window frame.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cxTopHeight;
            public int cxBottomHeight;
        }
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
          IntPtr hWnd, ref MARGINS pMarInset);

        #endregion

        #region system tray icon

        private WindowState lastWindowState;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.lastWindowState = WindowState;
            //this.Hide();
        }
        //private void OnBalloonClick(object sender, RoutedEventArgs e)
        //{

        //    this.notifyIcon.ShowBalloonTip(2000);

        //}
        private void OnNotifyIconDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.Show();
                this.WindowState = this.lastWindowState;
            }
        }
        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = this.lastWindowState;
        }


        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection.MyConnection.Close();
            }

            catch (Exception)
            {
            }

            Environment.Exit(0);
        }
        #endregion


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            bool flag = true;
            UserConfig.ConfigureConfig();
            int x = 0;
            for (int i = 0; i < 10; i++)
            {
                if (csc.CheckConnection())//check connection whether available
                {
                    if (flag == false)
                    {
                        this.Show();
                        flag = true;
                    }
                    break;
                }
                else
                {
                    if (flag)
                    {
                        flag = false;
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
            if (flag)
            {
                lg.getServerDetails();

                if (Connection.MyConnection.State == ConnectionState.Open)
                {
                    lg.userLogin();
                    lg.loginReminder(2);
                    //x = lg.checklog();
                    //signin_tBlock.Content = "Log out";
                    //signout_but.Visibility = Visibility.Hidden;
                    //signin_but.Visibility = Visibility.Hidden;
                    //GetIdleTime git = new GetIdleTime();
                    
                    name_lab.Content = Login.userfname;
                    leaverequest.Visibility = Visibility.Visible;
                    timesheet.Visibility = Visibility.Visible;
                    outofoffice.Visibility = Visibility.Visible;
                    addnewtask.Visibility = Visibility.Visible;

                    leaverequest.Visibility = Visibility.Hidden;
                    timesheet.Visibility = Visibility.Hidden;
                    outofoffice.Visibility = Visibility.Hidden;
                    addnewtask.Visibility = Visibility.Hidden;

                    loadAvatar();
                    //lg.loginReminder(2);


                    timesheet.getDetailsFromJira();
                    //if (x == 1)//if x=1 he is alredy signin
                    //{
                    //    signin_tBlock.Content = "Log out";
                    //    signout_but.Visibility = Visibility.Visible;
                    //    signin_but.Visibility = Visibility.Hidden;

                    //    name_lab.Content = Login.userfname;
                    //    leaverequest.Visibility = Visibility.Visible;
                    //    timesheet.Visibility = Visibility.Visible;
                    //    outofoffice.Visibility = Visibility.Visible;
                    //    addnewtask.Visibility = Visibility.Visible;

                    //    leaverequest.Visibility = Visibility.Hidden;
                    //    timesheet.Visibility = Visibility.Hidden;
                    //    outofoffice.Visibility = Visibility.Hidden;
                    //    addnewtask.Visibility = Visibility.Hidden;

                    //    loadAvatar();
                    //    lg.loginReminder(2);
                    //    timesheet.getDetailsFromJira();
                    //    tempCheck = lg.checkTimesheet();
                    //    if (tempCheck != "true")
                    //    {
                    //        Thread ThreadTimeSheetReminder;
                    //        ThreadTimeSheetReminder = new Thread(ShowTimehseetReminder);
                    //        ThreadTimeSheetReminder.Start((Object)tempCheck);
                    //    }
                    //}
                    //else if (x == 2)//if x=2 he is alredy signout
                    //{
                    //    signin_tBlock.Content = "Exit";
                    //    signout_but.IsEnabled = true;
                    //    signin_but.IsEnabled = true;
                    //    name_lab.Content = Login.userfname;
                    //}
                    //else if (x == 0)//if x=0 not signed in for today
                    //{
                    //    MessageBoxResult result = System.Windows.MessageBox.Show("Hi " + UserConfig.configlist[0] + ", our system tracked that you haven't log in for today.  Do you want log in?", "BQuTMS Sign In Alert", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    //    if (result == MessageBoxResult.Yes)
                    //    {
                    //        SplashScreen splashScreen = new SplashScreen("images/SplashScreen.png");
                    //        splashScreen.Show(true);
                    //        sigiin();
                    //        loadAvatar();
                    //        timesheet.getDetailsFromJira();
                    //        x = 1;
                    //    }
                    //}

                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Check your BQuTMSWithJira application settings or internet connection", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            this.Show();

            Thread ThreadTimeSheetReminder;
            ThreadTimeSheetReminder = new Thread(ShowTimehseetReminder);
            ThreadTimeSheetReminder.Start();

       //     this.ResizeMode = ResizeMode.NoResize;
            //if (x == 1)
            //{
            //    Thread ThreadUpdateCheck = new Thread(() =>
            //    {
            //        //System.Threading.Thread.Sleep(5000);
            //        string newVersion = String.Empty;
            //        string newUpdates = String.Empty;
            //        if (updateFunction(out newVersion, out newUpdates))
            //        {
            //            if (System.Windows.MessageBox.Show("BQuTMSWithJira " + newVersion + " version here.Do you want to get this version? " + newUpdates, "BQuTMSWithJira Update", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly) == MessageBoxResult.Yes)
            //            {
            //                if (Login.userid != 0)
            //                {
            //                    lg.UpdateVersion(newVersion, UserConfig.configlist[0]);
            //                }
            //                System.Diagnostics.Process.Start("BQuTMSJiraUpdate.exe");
            //                Environment.Exit(0);
            //            }
            //        }
            //    });
            //    ThreadUpdateCheck.Start();
                
            //}

            //Dispatcher.Invoke(new Action(() =>
            //{
            //    Notification not = new Notification();
            //    not.startNotificationDisplayer();
            //}));

            //Notification not = new Notification();irantha.jayasekara
            //not.startNotificationDisplayer();
            //if (UserConfig.configlist[0].ToString() == "whitney.fraser")
            //{
            //    Thread NotificationDisplayer = new Thread(() =>
            //        {
            //            Notification not = new Notification();
            //            not.startNotificationDisplayer();
            //        });
            //    NotificationDisplayer.SetApartmentState(ApartmentState.STA);
            //    //NotificationDisplayer.IsBackground = true;
            //    NotificationDisplayer.Start();
            //}
        }

        private void loadAvatar()
        {
            string localFilename = Util.GetUserDataPath() + "\\image\\" + UserConfig.configlist[0].ToString() + ".png";
            BitmapImage image = new BitmapImage();
            using (FileStream stream = File.OpenRead(localFilename))
            {
              image.BeginInit();
              image.CacheOption = BitmapCacheOption.OnLoad;
              image.StreamSource = stream;
              image.EndInit();
            }
            profile_pic_img.Source = image;          
        }

        void ShowTimehseetReminder() 
        {

            Dispatcher.Invoke(new Action(() =>
            {
                tempCheck = lg.checkTimesheet();
                if (tempCheck != "true")
                {
                    if (Microsoft.Windows.Controls.MessageBox.Show("Please update your timesheet for '" + tempCheck + "'", "BQuTMSWithJira Timesheet Reminder", MessageBoxButton.OKCancel,
                        MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        timesheetClick();
                    }
                }
            }));              
          
        }

        private void refresh_click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            if (csc.CheckConnection())//check connection weather avalability
            {
                if (Connection.MyConnection.State != ConnectionState.Open)
                {
                    if (csc.CheckConnection())//check connection weather avalability
                    {
                        Connection.MyConnection.Open();
                    }
                }
                else
                {
                    timesheet.getDetailsFromJira();
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please check your internet connection.", "Internet connection Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Mouse.OverrideCursor = null;
        }

        private void restart_click(object sender, RoutedEventArgs e)
        {
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                Connection.MyConnection.Close();
            }
            var info = new ProcessStartInfo();
            info.FileName = "ProcessReStarter";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info);
            Environment.Exit(0);
        }

        private void help_click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.bqutms.com/");
        }

        private void update_click(object sender, RoutedEventArgs e)
        {
            string newVersion = String.Empty;
            string newUpdates = String.Empty;
            if (updateFunction(out newVersion, out newUpdates))
            {
                if (Microsoft.Windows.Controls.MessageBox.Show("BQuTMSWithJira " + newVersion + " version here. Do you want to get this version? " + newUpdates, "BQuTMSWithJira Update", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (Login.userid != 0)
                    {
                        lg.UpdateVersion(newVersion, UserConfig.configlist[0]);
                    }
                    System.Diagnostics.Process.Start("BQuTMSJiraUpdate.exe");
                    Environment.Exit(0);
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("No updates available", "No updates", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool updateFunction(out string newversion, out string newupdates)
        {
            newversion = string.Empty;
            newupdates = string.Empty;
            if (csc.CheckConnection())//check connection weather avalability
            {
                try
                {
                    XmlTextReader reader = new XmlTextReader("http://jira.bqutms.com/TMS_JIRA_Application/BQuTMSVersion.xml");
                    XmlNodeType type;
                    while (reader.Read())
                    {
                        type = reader.NodeType;
                        if (type == XmlNodeType.Element)
                        {
                            if (reader.Name == "x86")
                            {
                                reader.Read();
                                newversion = reader.Value;
                            }
                            else if (reader.Name == "Updates")
                            {
                                reader.Read();
                                newupdates = reader.Value.Replace("\\n", "\n");
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Cannot find new updates, error occurred: "+ex.Message, "Update checker error", MessageBoxButton.OK, MessageBoxImage.Warning); 
                    return false;
                }
                Assembly assem = Assembly.GetEntryAssembly();
                AssemblyName assemName = assem.GetName();
                Version mchnver = assemName.Version;
                if (mchnver.ToString() == newversion)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private void about_click(object sender, RoutedEventArgs e)
        {
            AboutTMS AT = new AboutTMS();
            AT.ShowDialog();
        }

        private void home_but_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            hidelab();
            home_click_btn_lab.Visibility = Visibility.Visible;
            home.Visibility = Visibility.Visible;
        }

        private void timesheet_but_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                timesheetClick();
            }
            catch (Exception ee)
            {

                Microsoft.Windows.Controls.MessageBox.Show(ee.Message);
                //Microsoft.Windows.Controls.MessageBox.Show("Please check your internet connetion.", "Internet connetion Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void outofoffice_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (Connection.MyConnection.State == ConnectionState.Open)
                {
                    hidelab();
                    ooo_click_btn_lab.Visibility = Visibility.Visible;
                    outofoffice.Visibility = Visibility.Visible;
                }
                else
                {
                    if (csc.CheckConnection())
                    {
                        Connection.MyConnection.Open();
                        if (Connection.MyConnection.State == ConnectionState.Open)
                        {
                            hidelab();
                            ooo_click_btn_lab.Visibility = Visibility.Visible;
                            outofoffice.Visibility = Visibility.Visible;
                        }
                    }
                }
                Mouse.OverrideCursor = null;
            }
            catch (Exception)
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please check your internet connetion.", "Internet connetion Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void leverequest_but_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (Connection.MyConnection.State == ConnectionState.Open)
                {
                    hidelab();
                    lr_click_btn_lab.Visibility = Visibility.Visible;
                    leaverequest.Visibility = Visibility.Visible;
                }
                else
                {
                    if (csc.CheckConnection())
                    {
                        Connection.MyConnection.Open();
                        if (Connection.MyConnection.State == ConnectionState.Open)
                        {
                            hidelab();
                            lr_click_btn_lab.Visibility = Visibility.Visible;
                            leaverequest.Visibility = Visibility.Visible;
                        }
                    }
                }
                Mouse.OverrideCursor = null;
            }
            catch (Exception)
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please check your internet connetion.", "Internet connetion Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void taskremind_but_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (Connection.MyConnection.State == ConnectionState.Open)
                {
                    hidelab();
                    taskreminder_click_btn_lab.Visibility = Visibility.Visible;
                    addnewtask.Visibility = Visibility.Visible;
                }
                else
                {
                    if (csc.CheckConnection())
                    {
                        Connection.MyConnection.Open();
                        if (Connection.MyConnection.State == ConnectionState.Open)
                        {
                            hidelab();
                            taskreminder_click_btn_lab.Visibility = Visibility.Visible;
                            addnewtask.Visibility = Visibility.Visible;
                        }
                    }
                }
                Mouse.OverrideCursor = null;
            }
            catch (Exception)
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please check your internet connetion.", "Internet connetion Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void settings_but_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            hidelab();
            settings_click_btn_lab.Visibility = Visibility.Visible;
            option.Visibility = Visibility.Visible;
            Mouse.OverrideCursor = null;
        }

        void hidelab()
        {
            leaverequest.Visibility = Visibility.Hidden;
            timesheet.Visibility = Visibility.Hidden;
            home.Visibility = Visibility.Hidden;
            outofoffice.Visibility = Visibility.Hidden;
            option.Visibility = Visibility.Hidden;
            addnewtask.Visibility = Visibility.Hidden;

            home_click_btn_lab.Visibility = Visibility.Hidden;
            timesheet_click_btn_lab.Visibility = Visibility.Hidden;
            ooo_click_btn_lab.Visibility = Visibility.Hidden;
            lr_click_btn_lab.Visibility = Visibility.Hidden;
            taskreminder_click_btn_lab.Visibility = Visibility.Hidden;
            settings_click_btn_lab.Visibility = Visibility.Hidden;
            //backoption_tBlock.Visibility = Visibility.Hidden;
        }

//        private void login_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            //console.writeline("comecome1");
//            if (UserConfig.configlist[0] != String.Empty && UserConfig.configlist[1] != String.Empty)
//            {
//                //console.writeline("comecome2");
//                try
//                {
//                    if (Connection.MyConnection.State != ConnectionState.Open)
//                    {
//                        Connection.MyConnection.Open();
                        
//                    }
//                    if (Connection.MyConnection.State == ConnectionState.Open)//check connection status
//                    {
//                        //console.writeline("comecome3");
//                        sigiin();
//                    }
//                    else
//                    {
//                        Microsoft.Windows.Controls.MessageBox.Show("Check your BQuTMSWithJira application settings or internet connection", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
//                    }
//                }
//                catch (Exception)
//                {
//                    if (csc.CheckConnection())
//                    {
//                        lg.getServerDetails();

//                        if (Connection.MyConnection.State == ConnectionState.Open)
//                        {
//                            int x = lg.checklog();
//                            if (x == 1)
//                            {
//                                signin_tBlock.Content = "Sign Out";
//                                signin_tBlock.Foreground = Brushes.White;
//                                signout_but.Visibility = Visibility.Visible;
//                                signin_but.Visibility = Visibility.Hidden;
//                                name_lab.Content = Login.userfname;
//                            }
//                            else if (x == 2)
//                            {
//                                signin_tBlock.Content = "    Exit";
//                                signin_tBlock.Visibility = Visibility.Hidden;
//                                signin_tBlock.Foreground = Brushes.White;
//                                signout_but.IsEnabled = false;
//                                signin_but.Visibility = Visibility.Hidden;
//                                name_lab.Content = Login.userfname;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
//                    }
//                }
//            }
//            else
//            {
//                Microsoft.Windows.Controls.MessageBox.Show("Please go settings and enter your username, password and company name", "BQuTMSWithJira Userdetails", MessageBoxButton.OK, MessageBoxImage.Warning);
//            }
//        }

//        private void logout_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            ////Log out button click method
//            if (Microsoft.Windows.Controls.MessageBox.Show("Do you want to Log out?", "BQuTMSWithJira Confirmation", MessageBoxButton.YesNo,
//MessageBoxImage.Question) == MessageBoxResult.Yes)
//            {
//                try
//                {
//                    if (Connection.MyConnection.State == ConnectionState.Open)
//                    {
//                        sigiin();//call Log out  method
//                    }
//                    else
//                    {
//                        if (csc.CheckConnection())
//                        {
//                            Connection.MyConnection.Open();
//                            if (Connection.MyConnection.State == ConnectionState.Open)
//                            {
//                                sigiin();
//                            }
//                        }
//                    }
//                }
//                catch (Exception)
//                {
//                    Microsoft.Windows.Controls.MessageBox.Show("Please check your internet connection.", "Internet connetion Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
//                }
//            }
//        }

//        //Log in or Log out method
//        void sigiin()
//        {
//            if (signin_tBlock.Content.ToString() == "Log in")//when Log in
//            {
//                lg.userLogin();
//                signin_tBlock.Content = "Log out";
//                signout_but.Visibility = Visibility.Visible;
//                signin_but.Visibility = Visibility.Hidden;
//                name_lab.Content = Login.userfname;
//                string tempCheck = lg.checkTimesheet();
//                if (tempCheck != "true")
//                {
//                    Thread ThreadTimeSheetReminder;
//                    ThreadTimeSheetReminder = new Thread(ShowTimehseetReminder);

//                    ThreadTimeSheetReminder.Start((Object)tempCheck);
//                }
//            }
//            else if (signin_tBlock.Content.ToString() == "Sign in again")////still not working
//            {
//                lg.userLoginAgain();
//                signin_tBlock.Content = "Log out";
//                signout_but.Visibility = Visibility.Visible;
//                signin_but.Visibility = Visibility.Hidden;
//                name_lab.Content = Login.userfname;
//                string tempCheck = lg.checkTimesheet();
//                if (tempCheck != "true")
//                {
//                    Thread ThreadTimeSheetReminder;
//                    ThreadTimeSheetReminder = new Thread(ShowTimehseetReminder);

//                    ThreadTimeSheetReminder.Start((Object)tempCheck);
//                }
//            }
//            else if (signin_tBlock.Content.ToString() == "Log out")////when signout
//            {
//                lg.Logout();
//                Environment.Exit(0);
//            }
//            else if (signin_tBlock.Content.ToString() == "Exit")
//            {
//                Environment.Exit(0);
//            }
//        }
        

        private void timesheetClick()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            if (Connection.MyConnection.State == ConnectionState.Open)
            {
                hidelab();
                timesheet_click_btn_lab.Visibility = Visibility.Visible;
                timesheet.Visibility = Visibility.Visible;
            }
            else
            {
                if (csc.CheckConnection())
                {
                    Connection.MyConnection.Open();
                    if (Connection.MyConnection.State == ConnectionState.Open)
                    {
                        hidelab();
                        timesheet_click_btn_lab.Visibility = Visibility.Visible;
                        timesheet.Visibility = Visibility.Visible;
                    }
                }
            }
            Mouse.OverrideCursor = null;
        }

        //drag window on click and move any were
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                base.OnMouseLeftButtonDown(e);
                DragMove();
            }
            catch (Exception)
            {
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel=true;
            this.WindowState = WindowState.Minimized;
        }

    }
}
