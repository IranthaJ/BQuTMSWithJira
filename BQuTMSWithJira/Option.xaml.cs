using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.Odbc;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Input;

namespace BQuTMSWithJira
{
    public partial class Option : UserControl
    {
        public Option()
        {
            InitializeComponent();
        }

        private bool checkflag = true;
        //    private string current_companyname;
        string companyDBName;
        string companyDBsusername;
        string companyDBspassword;
        string companyDBsip;//server IP
        string companyDBsport;

        private void check_but_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(user_tBox.Text) && !string.IsNullOrEmpty(upword_tBox.Password) && !string.IsNullOrEmpty(comname_tBox.Text))
            {

                try
                {
                    if (Connection.Connect(ConfigurationManager.AppSettings["sip"], ConfigurationManager.AppSettings["susername"], Util.DecryptDES(ConfigurationManager.AppSettings["spassword"]), ConfigurationManager.AppSettings["sport"], ConfigurationManager.AppSettings["dbname"]) == true)//connect with server with new update feild
                    {
                        //connect main database and get details which include company information
                        using (OdbcCommand MyCommand = new OdbcCommand("SELECT db_name,db_username,db_password,db_host,db_port from tms_connections where company_name='" + comname_tBox.Text + "'", Connection.MyConnection))
                        {
                            OdbcDataReader MyDataReader = MyCommand.ExecuteReader();
                            MyDataReader.Read();
                            companyDBName = MyDataReader.GetString(0);
                            companyDBsusername = MyDataReader.GetString(1);
                            companyDBspassword = MyDataReader.GetString(2);
                            companyDBsip = MyDataReader.GetString(3);
                            companyDBsport = MyDataReader.GetString(4);

                        }
                    }
                    Connection.MyConnection.Close();
                    //connect company database
                    if ((companyDBName != null) && (companyDBsusername != null) && (companyDBsip != null) && (companyDBsport != null))
                    {
                        if (Connection.MyConnection.State == ConnectionState.Open)
                        {
                            Connection.MyConnection.Close();
                        }


                        if (Connection.Connect(companyDBsip, companyDBsusername, companyDBspassword, companyDBsport, companyDBName) == true)//connect with server with new update feild
                        {
                            //console.writeline("companyDBsip " + companyDBsip);
                            //console.writeline("companyDBsusername " + companyDBsusername);
                            //console.writeline("companyDBspassword " + companyDBspassword);
                            //console.writeline("companyDBsport " + companyDBsport);
                            //console.writeline("companyDBName " + companyDBName);
                            Login lg = new Login();
                            if (lg.checkuser(user_tBox.Text, upword_tBox.Password) == true)//check username and password weather correct or incorrect
                            {
                                if (Microsoft.Windows.Controls.MessageBox.Show("Do you want to update application settings?", "BQuTMSWithJira Update Settings", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    Mouse.OverrideCursor = Cursors.Wait;
                                    UpdateConfigFile();
                                    downloadUserImage();
                                    Assembly assem = Assembly.GetEntryAssembly();
                                    AssemblyName assemName = assem.GetName();
                                    lg.UpdateVersion(assemName.Version.ToString(), user_tBox.Text);
                                    Connection.MyConnection.Close();
                                    var info = new ProcessStartInfo();
                                    info.FileName = "ProcessReStarter";
                                    info.WindowStyle = ProcessWindowStyle.Hidden;
                                    Process.Start(info);
                                    Environment.Exit(0);
                                }
                            }
                        }
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        Microsoft.Windows.Controls.MessageBox.Show("Please check company name again.", "BQuTMSWithJira Company Name Incorrect", MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                }
                catch (Exception)
                {
                    Mouse.OverrideCursor = null;
                    Microsoft.Windows.Controls.MessageBox.Show("Please check company name again.", "BQuTMSWithJira Company Name Incorrect", MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please fill the field(s).", "BQuTMSWithJira Empty Field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void downloadUserImage()
        {
            string localFilename = Util.GetUserDataPath() + "\\image\\" + user_tBox.Text.ToString() + ".png";
            string ProfilePic = "https://bqujira.atlassian.net/secure/useravatar?size=xxlarge&username=" +user_tBox.Text.ToString();
            FileInfo f1 = new FileInfo(localFilename);
             try
              {
                  if (f1.Exists)
                  {
                      f1.Delete();
                  }
                int BytesToRead = 100;

                WebRequest request = WebRequest.Create(new Uri(ProfilePic, UriKind.Absolute));
                string base64Credentials = GetEncodedCredentials();
                request.Headers.Add("Authorization", "Basic " + base64Credentials);
                request.Timeout = -1;
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                BinaryReader reader = new BinaryReader(responseStream);
                MemoryStream memoryStream = new MemoryStream();

                byte[] bytebuffer = new byte[BytesToRead];
                int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                while (bytesRead > 0)
                {
                    memoryStream.Write(bytebuffer, 0, bytesRead);
                    bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                }

                FileStream file = new FileStream(localFilename, FileMode.Create, FileAccess.Write);
                memoryStream.WriteTo(file);
                file.Close();
                memoryStream.Close();
            }
            catch (WebException)
            {
                string commonuser = Util.GetUserDataPath() + "\\image\\user.png";
                File.Copy(commonuser, localFilename);
            }
        }

        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", user_tBox.Text.ToString(), upword_tBox.Password.ToString());
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        //update xml file with new user details
        void UpdateConfigFile()
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(Util.GetUserDataPath() + "\\userdetails.xml", Encoding.UTF8);
                writer.WriteStartDocument();
                writer.WriteStartElement("SAVED");

                writer.WriteStartElement("user");
                writer.WriteString(user_tBox.Text);
                writer.WriteEndElement();

                writer.WriteStartElement("password");
                writer.WriteString(upword_tBox.Password);
                writer.WriteEndElement();

                writer.WriteStartElement("companyname");
                writer.WriteString(comname_tBox.Text);
                writer.WriteEndElement();

                writer.WriteStartElement("sip");
                writer.WriteString(companyDBsip);
                writer.WriteEndElement();

                writer.WriteStartElement("sport");
                writer.WriteString(companyDBsport);
                writer.WriteEndElement();

                writer.WriteStartElement("susername");
                writer.WriteString(companyDBsusername);
                writer.WriteEndElement();

                writer.WriteStartElement("spassword");
                writer.WriteString(Util.EncryptDES(companyDBspassword));
                writer.WriteEndElement();

                writer.WriteStartElement("dbname");
                writer.WriteString(companyDBName);
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception ex)
            {
                Microsoft.Windows.Controls.MessageBox.Show(ex.Source);
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (checkflag)
            {
                try
                {
                    //load details from xml file
                    user_tBox.Text = UserConfig.configlist[0];
                    upword_tBox.Password = UserConfig.configlist[1];
                    //  current_companyname = UserConfig.configlist[2];
                    comname_tBox.Text = UserConfig.configlist[2];
                }
                catch (Exception)
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Please update settings", "BQuTMSWithJira Empty Settings", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                checkflag = false;
            }
        }

        private void loginDetails_lab_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //change lable and controllers
            loginDetailsGrid.Visibility = Visibility.Visible;
            changePasswordGrid.Visibility = Visibility.Hidden;
            changePassword_tBlock_tab.Visibility = Visibility.Visible;
            loginDetails_tBlock_tab.Visibility = Visibility.Hidden;
        }

        private void changePassword_lab_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (UserConfig.configlist[1]!=string.Empty)
            {
            //change lable and controllers
            changePasswordGrid.Visibility = Visibility.Visible;
            loginDetailsGrid.Visibility = Visibility.Hidden;
            changePassword_tBlock_tab.Visibility = Visibility.Hidden;
            loginDetails_tBlock_tab.Visibility = Visibility.Visible;
            }
            else
	{
        Microsoft.Windows.Controls.MessageBox.Show("Before change password please log in to app using current user details", "", MessageBoxButton.OK, MessageBoxImage.Warning);
	}
        }

        private void submit_but_Click(object sender, RoutedEventArgs e)
        {
            if (currentPassword_pBox.Password != string.Empty && newPassword_pBox.Password != string.Empty && verifyPassword_pBox.Password != string.Empty)
            {
                //UserConfig.configlist[1]
                if (currentPassword_pBox.Password == UserConfig.configlist[1])
                {
                    if (newPassword_pBox.Password.Length >= 4)
                    {
                        if (newPassword_pBox.Password == verifyPassword_pBox.Password)
                        {
                            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                            var random = new Random();
                            var result = new string(
                                Enumerable.Repeat(chars, 32)//character length
                                          .Select(s => s[random.Next(s.Length)])
                                          .ToArray());

                            Login lr = new Login();
                            string newPassword = lr.Encripting(newPassword_pBox.Password + result) + ":" + result;
                            if (Connection.MyConnection.State == ConnectionState.Closed)
                            {
                                Connection.MyConnection.Open();
                            }
                            if (Connection.MyConnection.State == ConnectionState.Open)
                            {
                                using (OdbcCommand MyCommand = new OdbcCommand("update jos_users set password='" + newPassword + "' where username='" + UserConfig.configlist[0] + "' ", Connection.MyConnection))
                                {
                                    MyCommand.ExecuteNonQuery();

                                    //update userdetails xml file
                                    try
                                    {
                                        XmlDocument doc = new XmlDocument();
                                        doc.Load(Util.GetUserDataPath() + "\\userdetails.xml");
                                        XmlElement node1 = doc.SelectSingleNode("/SAVED/password") as XmlElement;
                                        if (node1 != null)
                                        {
                                            node1.InnerText = newPassword_pBox.Password; // if you want a text
                                        }
                                        doc.Save(Util.GetUserDataPath() + "\\userdetails.xml");
                                        upword_tBox.Password = newPassword_pBox.Password;

                                        currentPassword_pBox.Password = string.Empty;
                                        newPassword_pBox.Password = string.Empty;
                                        verifyPassword_pBox.Password = string.Empty;
                                        Microsoft.Windows.Controls.MessageBox.Show("New password saved successfully", "Saved successfully", MessageBoxButton.OK, MessageBoxImage.Information);

                                    }
                                    catch (Exception ex)
                                    {
                                        Microsoft.Windows.Controls.MessageBox.Show(ex.ToString());
                                    }
                                }
                            }
                            else
                            {
                                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. You need to restart the application", "BQuTMSWithJira Signout Reminder", MessageBoxButton.OK, MessageBoxImage.Stop);
                                Environment.Exit(0);
                            }
                        }
                        else
                        {
                            Microsoft.Windows.Controls.MessageBox.Show("New passwords do not match", "BQuTMSWithJira Wrong Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        Microsoft.Windows.Controls.MessageBox.Show("Please choose a password minimum 4 characters", "BQuTMSWithJira Wrong Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Please check current password again", "BQuTMSWithJira Wrong Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else 
            {
                Microsoft.Windows.Controls.MessageBox.Show("Please fill the field(s).", "BQuTMSWithJira Empty Field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
