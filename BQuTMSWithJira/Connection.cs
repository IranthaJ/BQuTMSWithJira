using Microsoft.Data.Odbc;
using System.Windows;

namespace BQuTMSWithJira
{
    class Connection
    {
        public static OdbcConnection MyConnection;
        public static OdbcConnection MyConnection2;
        public static OdbcConnection tempConnection;
        //create connection
        public static bool Connect(string _server, string _username, string _password, string _port, string _dbname)
        {
            if (!string.IsNullOrEmpty(_server))
            {
                try
                {
                    string MyConString = "DRIVER={MySQL ODBC 5.1 Driver};SERVER=" + _server + ";PORT=" + _port + ";DATABASE=" + _dbname + ";UID=" + _username + ";PASSWORD=" + _password + ";OPTION=3";
                    MyConnection = new OdbcConnection(MyConString);
                    tempConnection = new OdbcConnection(MyConString);
                    MyConnection2 = new OdbcConnection(MyConString);
                    MyConnection.Open();
                    return true;
                }
                catch (OdbcException MyOdbcException)
                {
                    if (MyOdbcException.Errors[0].SQLState == "HY000")
                    {
                        Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server.", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        Microsoft.Windows.Controls.MessageBox.Show(MyOdbcException.Errors[0].Message + "\n" + " Please contact your system administrator.", "BQuTMSWithJira Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
