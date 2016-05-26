using System;
using Microsoft.Data.Odbc;

namespace BQuTMSWithJira
{
    class DateMonthSelection
    {
        OdbcDataReader MyDataReader;
        //return privious month form this method
        public int PriviousMonth(int currentmonth)
        {
            if (currentmonth == 1)
            {
                return 12;
            }
            else
            {
                return (currentmonth - 1);
            }
        }
        //return next month form this methos
        public int NextMonth(int currentmonth)
        {
            if (currentmonth == 12)
            {
                return 1;

            }
            else
            {
                return (currentmonth + 1);
            }
        }
        //retun no for considered month
        public int Selectedmonth(string stringmonth)
        {
            int selmon = 0;
            switch (stringmonth)
            {
                case "January":
                    selmon = 1;
                    break;
                case "February":
                    selmon = 2;
                    break;
                case "March":
                    selmon = 3;
                    break;
                case "April":
                    selmon = 4;
                    break;
                case "May":
                    selmon = 5;
                    break;
                case "June":
                    selmon = 6;
                    break;
                case "July":
                    selmon = 7;
                    break;
                case "August":
                    selmon = 8;
                    break;
                case "September":
                    selmon = 9;
                    break;
                case "October":
                    selmon = 10;
                    break;
                case "November":
                    selmon = 11;
                    break;
                case "December":
                    selmon = 12;
                    break;
            }
            return selmon;
        }
        ////compere two date and determind lagest date(not use at this time)
        //public bool DateValidate(int fyear, int fmonth, int fday, int lyear, int lmonth, int lday)
        //{
        //    try
        //    {
        //        DateTime dt1; //= new DateTime(fyear, fmonth, fday);
        //        dt1 = DateTime.Parse(fyear + "," + fmonth + "," + fday);
        //        DateTime dt2; //= new DateTime(lyear, lmonth, lday);
        //        dt2 = DateTime.Parse(lyear + "," + lmonth + "," + lday);
        //        if (DateTime.Compare(dt1, dt2) <= 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (FormatException)
        //    {
        //        return false;
        //    }
        //}
        ////validate single date from year,month and day(not use at this time) 
        //public bool SingleDateValidate(int year, int month, int day)
        //{
        //    try
        //    {
        //        DateTime dt1 = DateTime.Parse(year + "," + month + "," + day);
        //        return true;
        //    }
        //    catch (FormatException)
        //    {
        //        return false;
        //    }
        //}
        ////time validate (not use this time)
        //public bool TimeValidate(string time)
        //{
        //    try
        //    {
        //        if (time.Length == 5)
        //        {
        //            string[] timeParts = time.Split('.');
        //            if (timeParts.Length == 2)
        //            {
        //                if (Convert.ToInt32(timeParts[0]) >= 0 && Convert.ToInt32(timeParts[0]) <= 25 && Convert.ToInt32(timeParts[1]) >= 0 && Convert.ToInt32(timeParts[1]) <= 59)
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (FormatException)
        //    {
        //        return false;
        //    }
        //}

        //check allocate same date or time allocate at same time
        public bool checkItem(DateTime sd1, DateTime sd2, string txtcommand)
        {
            bool check = true;
            DateTime dt1, dt2;
            if (DateTime.Compare(sd1, sd2) < 0)
            {
                using (OdbcCommand MyCommand = new OdbcCommand(txtcommand, Connection.MyConnection))
                {
                    MyDataReader = MyCommand.ExecuteReader();
                    while (MyDataReader.Read())
                    {
                        dt1 = MyDataReader.GetDateTime(0);
                        dt2 = MyDataReader.GetDateTime(1);
                        if (!(((DateTime.Compare(dt1, sd1) > 0) && (DateTime.Compare(dt1, sd2) > 0)) || ((DateTime.Compare(dt2, sd1) < 0) && (DateTime.Compare(dt2, sd2) < 0))))
                        {
                            check = false;
                        }
                    }
                    MyDataReader.Close();
                }
            }
            else
            {
                check = false;
            }
            return check;
        }
    }
}
