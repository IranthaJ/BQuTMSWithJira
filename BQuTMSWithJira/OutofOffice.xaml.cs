using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Odbc;
using System.Data;
using System.Threading;
using System.Globalization;

namespace BQuTMSWithJira
{
    public partial class OutofOffice : UserControl
    {
        OdbcCommand MyCommand;
        public static bool checkflago = true;

        public OutofOffice()
        {
            InitializeComponent();
        }

        private void apply_but_Click(object sender, RoutedEventArgs e)
        {
            if ((Connection.MyConnection.State == ConnectionState.Open))
            {
                if ((category_cBox.Text != "") && (date_dPicker.Text != ""))
                {
                    //if (Convert.ToDateTime(date_dPicker.Text) <= DateTime.Now)
                    //{
                        //DateMonthSelection dms = new DateMonthSelection();
                        //if (dms.checkItem(Convert.ToDateTime(oHH_cBox.Value), Convert.ToDateTime(iHH_cBox.Value), "select out_time,in_time from tms_out_of_office where employee_id='" + Login.userid + "' and o_year='" + date_dPicker.SelectedDate.Value.Year + "' and o_month='" + date_dPicker.SelectedDate.Value.Month + "' and o_day='" + date_dPicker.SelectedDate.Value.Day + "'"))
                        //{
                        MyCommand.CommandText = "select cat_id from tms_out_of_office_categories where cat_name='" + category_cBox.Text + "'";
                        int category_id = Convert.ToInt32(MyCommand.ExecuteScalar());
                        MyCommand.CommandText = "insert into tms_out_of_office(cat_id,employee_id,o_year,o_month,o_day,out_time,in_time,comment) values('" + category_id + "','" + Login.userid + "','" + date_dPicker.SelectedDate.Value.Year + "','" + date_dPicker.SelectedDate.Value.Month + "','" + date_dPicker.SelectedDate.Value.Day + "','" +
 Convert.ToDateTime(oHH_cBox.Text).ToString("HH:mm:ss") + "','" + Convert.ToDateTime(iHH_cBox.Text).ToString("HH:mm:ss") + "','" + (comment_tBox.Text).Replace("'", "\\'") + "')";
                        MyCommand.ExecuteNonQuery();
                        comment_tBox.Watermark = "No comment";
                        Microsoft.Windows.Controls.MessageBox.Show("Submitted successfully.", "BQuTMSWithJira Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    //else
                    //{
                    //    Microsoft.Windows.Controls.MessageBox.Show("Date and time already allocated or time selection is incorrect.", "BQuTMSWithJira Invalid Time Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //}
                    //}
                    //else
                    //{
                    //    Microsoft.Windows.Controls.MessageBox.Show("Date selection is incorrect", "BQuTMSWithJira Invalid Date Selection", MessageBoxButton.OK, MessageBoxImage.Error);
                    //}
                }
                else
                {
                    Microsoft.Windows.Controls.MessageBox.Show("Please fill the field(s).", "BQuTMSWithJira Empty Field(s)", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                Microsoft.Windows.Controls.MessageBox.Show("Unable to connect to the server. Please refresh (Help -> Refresh) and try again.", "BQuTMSWithJira Connection Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Grid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                oHH_cBox.Value = DateTime.Now;
                iHH_cBox.Value = DateTime.Now;
                comment_tBox.Text = "";
                comment_tBox.Watermark = "No comment";
                if (checkflago)
                {
                    //console.writeline("callcall");
                    //change date format
                    Thread.CurrentThread.CurrentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone(); Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "ddd, MMM dd, yyyy";

                    MyCommand = new OdbcCommand("select cat_name from tms_out_of_office_categories", Connection.MyConnection);
                    OdbcDataReader MyDataReader = MyCommand.ExecuteReader();
                    while (MyDataReader.Read())
                    {
                        category_cBox.Items.Add(MyDataReader.GetString(0));
                    }
                    MyDataReader.Close();
                    checkflago = false;
                }
                date_dPicker.SelectedDate = DateTime.Now;
            }
        }

        private void date_dPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            comment_tBox.Watermark = "No comment";
        }

        private void category_cBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comment_tBox.Watermark = "No comment";
        }
    }
}
