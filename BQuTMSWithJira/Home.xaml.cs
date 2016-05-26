using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Odbc;
using System.Windows.Media;

namespace BQuTMSWithJira
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
       
        public Home()
        {
            InitializeComponent();
        }
        int calenderCount;
        List<Events> event_list = new List<Events>();
        List<Holidays> holiday_list = new List<Holidays>();

        bool check = true;
        OdbcDataReader MyDataReader;

        #region calender creation methods

        void configCalender(int currentYear, int currentMonth, int currentDay)
        {
            resetAllDatesLab();
            DateTime currentDate = DateTime.Parse(Convert.ToDateTime(currentYear + "/" + currentMonth + "/" + currentDay).ToString("yyyy-MM-dd"));
            DateTime actualCurrentDate = DateTime.Now;
            DateTime month_first_of_Date = DateTime.Parse(Convert.ToDateTime(currentYear + "/" + currentMonth + "/" + 1).ToString("yyyy-MM-dd"));
            int currentMonthTotalDays = Convert.ToInt32((month_first_of_Date.AddMonths(1) - month_first_of_Date).TotalDays);
            int priviousMonthTotalDays = Convert.ToInt32((month_first_of_Date - month_first_of_Date.AddMonths(-1)).TotalDays);

            int int_currentmonth_dayofweek = Convert.ToInt32(month_first_of_Date.DayOfWeek);//first date of current month form int
            if (int_currentmonth_dayofweek == 0)
            {
                int_currentmonth_dayofweek = 7;
            }
            switch (int_currentmonth_dayofweek)
            {
                case 1:
                    markWithOne(currentMonthTotalDays);
                    break;
                case 2:
                    markWithTwo(currentMonthTotalDays, priviousMonthTotalDays);
                    break;
                case 3:
                    markWithThree(currentMonthTotalDays, priviousMonthTotalDays);
                    break;
                case 4:
                    markWithFour(currentMonthTotalDays, priviousMonthTotalDays);
                    break;
                case 5:
                    markWithFive(currentMonthTotalDays, priviousMonthTotalDays);
                    break;
                case 6:
                    markWithSix(currentMonthTotalDays, priviousMonthTotalDays);
                    break;
                case 7:
                    markWithSeven(currentMonthTotalDays, priviousMonthTotalDays);
                    break;
                default:
                    break;
            }
            if (currentYear == actualCurrentDate.Year && currentMonth == actualCurrentDate.Month)
            {
                markResetCurrentDate(currentDay, int_currentmonth_dayofweek, CalenderColorCode.font_color_today, CalenderColorCode.back_color_today);
            }
            ////call this method to not holiday equal to current date
            //configHoliday(currentDay, int_currentmonth_dayofweek, CalenderColorCode.back_color_holiday, DateTime.Now.AddDays(5));
            //configHoliday(currentDay, int_currentmonth_dayofweek, CalenderColorCode.back_color_holiday, DateTime.Now.AddDays(6));
            foreach (Holidays holidays in holiday_list)
            {
                if (holidays.MONTH == currentMonth && currentYear == DateTime.Now.Year)
                {
                    string holiday_description;
                    if (holidays.DESCRIPTION == "")
                    {
                        holiday_description = null;
                    }
                    else
                    {
                        holiday_description = holidays.DESCRIPTION;
                    }

                    configHolidayAndBlockDays(currentDay, int_currentmonth_dayofweek, CalenderColorCode.back_color_holiday, DateTime.Parse(Convert.ToDateTime(DateTime.Now.Year + "/" + holidays.MONTH + "/" + holidays.DAY).ToString("yyyy-MM-dd")), holiday_description);
                }
            }

            //foreach (BlockDay blockdays in blockday_list)
            //{
            //    if (blockdays.MONTH == currentMonth && currentYear == DateTime.Now.Year)
            //    {
            //        string blockday_description;
            //        if (blockdays.DESCRIPTION == "")
            //        {
            //            blockday_description = null;
            //        }
            //        else
            //        {
            //            blockday_description = blockdays.DESCRIPTION;
            //        }

            //        configHolidayAndBlockDays(currentDay, int_currentmonth_dayofweek, CalenderColorCode.back_color_blockday, DateTime.Parse(Convert.ToDateTime(DateTime.Now.Year + "/" + blockdays.MONTH + "/" + blockdays.DAY).ToString("yyyy-MM-dd")), blockday_description);
            //    }
            //}

            configMonthLable();
        }

        void configHolidayAndBlockDays(int currentDay, int int_currentmonth_dayofweek, SolidColorBrush back_color, DateTime holiday, string description)
        {
            int position = holiday.Day + int_currentmonth_dayofweek - 1;


            switch (position)
            {
                case 1:
                    cd_1_lab.Background = back_color;
                    cd_1_lab.ToolTip = description;
                    break;
                case 2:
                    cd_2_lab.Background = back_color;
                    cd_2_lab.ToolTip = description;
                    break;
                case 3:
                    cd_3_lab.Background = back_color;
                    cd_3_lab.ToolTip = description;
                    break;
                case 4:
                    cd_4_lab.Background = back_color;
                    cd_4_lab.ToolTip = description;
                    break;
                case 5:
                    cd_5_lab.Background = back_color;
                    cd_5_lab.ToolTip = description;
                    break;
                case 6:
                    cd_6_lab.Background = back_color;
                    cd_6_lab.ToolTip = description;
                    break;
                case 7:
                    cd_7_lab.Background = back_color;
                    cd_7_lab.ToolTip = description;
                    break;
                case 8:
                    cd_8_lab.Background = back_color;
                    cd_8_lab.ToolTip = description;
                    break;
                case 9:
                    cd_9_lab.Background = back_color;
                    cd_9_lab.ToolTip = description;
                    break;
                case 10:
                    cd_10_lab.Background = back_color;
                    cd_10_lab.ToolTip = description;
                    break;
                case 11:
                    cd_11_lab.Background = back_color;
                    cd_11_lab.ToolTip = description;
                    break;
                case 12:
                    cd_12_lab.Background = back_color;
                    cd_12_lab.ToolTip = description;
                    break;
                case 13:
                    cd_13_lab.Background = back_color;
                    cd_13_lab.ToolTip = description;
                    break;
                case 14:
                    cd_14_lab.Background = back_color;
                    cd_14_lab.ToolTip = description;
                    break;
                case 15:
                    cd_15_lab.Background = back_color;
                    cd_15_lab.ToolTip = description;
                    break;
                case 16:
                    cd_16_lab.Background = back_color;
                    cd_16_lab.ToolTip = description;
                    break;
                case 17:
                    cd_17_lab.Background = back_color;
                    cd_17_lab.ToolTip = description;
                    break;
                case 18:
                    cd_18_lab.Background = back_color;
                    cd_18_lab.ToolTip = description;
                    break;
                case 19:
                    cd_19_lab.Background = back_color;
                    cd_19_lab.ToolTip = description;
                    break;
                case 20:
                    cd_20_lab.Background = back_color;
                    cd_20_lab.ToolTip = description;
                    break;
                case 21:
                    cd_21_lab.Background = back_color;
                    cd_21_lab.ToolTip = description;
                    break;
                case 22:
                    cd_22_lab.Background = back_color;
                    cd_22_lab.ToolTip = description;
                    break;
                case 23:
                    cd_23_lab.Background = back_color;
                    cd_23_lab.ToolTip = description;
                    break;
                case 24:
                    cd_24_lab.Background = back_color;
                    cd_1_lab.ToolTip = description;
                    break;
                case 25:
                    cd_25_lab.Background = back_color;
                    cd_25_lab.ToolTip = description;
                    break;
                case 26:
                    cd_26_lab.Background = back_color;
                    cd_26_lab.ToolTip = description;
                    break;
                case 27:
                    cd_27_lab.Background = back_color;
                    cd_27_lab.ToolTip = description;
                    break;
                case 28:
                    cd_28_lab.Background = back_color;
                    cd_28_lab.ToolTip = description;
                    break;
                case 29:
                    cd_29_lab.Background = back_color;
                    cd_29_lab.ToolTip = description;
                    break;
                case 30:
                    cd_30_lab.Background = back_color;
                    cd_30_lab.ToolTip = description;
                    break;
                case 31:
                    cd_31_lab.Background = back_color;
                    cd_31_lab.ToolTip = description;
                    break;
                case 32:
                    cd_32_lab.Background = back_color;
                    cd_32_lab.ToolTip = description;
                    break;

                case 33:
                    cd_33_lab.Background = back_color;
                    cd_33_lab.ToolTip = description;
                    break;
                case 34:
                    cd_34_lab.Background = back_color;
                    cd_34_lab.ToolTip = description;
                    break;
                case 35:
                    cd_35_lab.Background = back_color;
                    cd_35_lab.ToolTip = description;
                    break;
                case 36:
                    cd_36_lab.Background = back_color;
                    cd_36_lab.ToolTip = description;
                    break;
                case 37:
                    cd_37_lab.Background = back_color;
                    cd_37_lab.ToolTip = description;
                    break;
                case 38:
                    cd_38_lab.Background = back_color;
                    cd_38_lab.ToolTip = description;
                    break;
                case 39:
                    cd_39_lab.Background = back_color;
                    cd_39_lab.ToolTip = description;
                    break;
                case 40:
                    cd_40_lab.Background = back_color;
                    cd_40_lab.ToolTip = description;
                    break;
                case 41:
                    cd_41_lab.Background = back_color;
                    cd_41_lab.ToolTip = description;
                    break;
                case 42:
                    cd_42_lab.Background = back_color;
                    cd_42_lab.ToolTip = description;
                    break;

                default:
                    break;
            }
        }


        void resetAllDatesLab()
        {
            cd_1_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_1_lab.Background = CalenderColorCode.back_color_reset;
            cd_1_lab.ToolTip = null;

            cd_2_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_2_lab.Background = CalenderColorCode.back_color_reset;
            cd_2_lab.ToolTip = null;


            cd_3_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_3_lab.Background = CalenderColorCode.back_color_reset;
            cd_3_lab.ToolTip = null;

            cd_4_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_4_lab.Background = CalenderColorCode.back_color_reset;
            cd_4_lab.ToolTip = null;

            cd_5_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_5_lab.Background = CalenderColorCode.back_color_reset;
            cd_5_lab.ToolTip = null;

            cd_6_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_6_lab.Background = CalenderColorCode.back_color_reset;
            cd_6_lab.ToolTip = null;

            cd_7_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_7_lab.Background = CalenderColorCode.back_color_reset;
            cd_7_lab.ToolTip = null;

            cd_8_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_8_lab.Background = CalenderColorCode.back_color_reset;
            cd_8_lab.ToolTip = null;

            cd_9_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_9_lab.Background = CalenderColorCode.back_color_reset;
            cd_9_lab.ToolTip = null;

            cd_10_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_10_lab.Background = CalenderColorCode.back_color_reset;
            cd_1_lab.ToolTip = null;

            cd_11_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_11_lab.Background = CalenderColorCode.back_color_reset;
            cd_11_lab.ToolTip = null;

            cd_12_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_12_lab.Background = CalenderColorCode.back_color_reset;
            cd_12_lab.ToolTip = null;

            cd_13_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_13_lab.Background = CalenderColorCode.back_color_reset;
            cd_13_lab.ToolTip = null;

            cd_14_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_14_lab.Background = CalenderColorCode.back_color_reset;
            cd_14_lab.ToolTip = null;

            cd_15_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_15_lab.Background = CalenderColorCode.back_color_reset;
            cd_15_lab.ToolTip = null;

            cd_16_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_16_lab.Background = CalenderColorCode.back_color_reset;
            cd_16_lab.ToolTip = null;

            cd_17_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_17_lab.Background = CalenderColorCode.back_color_reset;
            cd_17_lab.ToolTip = null;

            cd_18_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_18_lab.Background = CalenderColorCode.back_color_reset;
            cd_18_lab.ToolTip = null;

            cd_19_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_19_lab.Background = CalenderColorCode.back_color_reset;
            cd_19_lab.ToolTip = null;

            cd_20_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_20_lab.Background = CalenderColorCode.back_color_reset;
            cd_20_lab.ToolTip = null;

            cd_21_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_21_lab.Background = CalenderColorCode.back_color_reset;
            cd_21_lab.ToolTip = null;

            cd_22_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_22_lab.Background = CalenderColorCode.back_color_reset;
            cd_22_lab.ToolTip = null;

            cd_23_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_23_lab.Background = CalenderColorCode.back_color_reset;
            cd_23_lab.ToolTip = null;

            cd_24_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_24_lab.Background = CalenderColorCode.back_color_reset;
            cd_24_lab.ToolTip = null;

            cd_25_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_25_lab.Background = CalenderColorCode.back_color_reset;
            cd_25_lab.ToolTip = null;

            cd_26_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_26_lab.Background = CalenderColorCode.back_color_reset;
            cd_26_lab.ToolTip = null;

            cd_27_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_27_lab.Background = CalenderColorCode.back_color_reset;
            cd_27_lab.ToolTip = null;

            cd_28_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_28_lab.Background = CalenderColorCode.back_color_reset;
            cd_28_lab.ToolTip = null;

            cd_29_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_29_lab.Background = CalenderColorCode.back_color_reset;
            cd_29_lab.ToolTip = null;

            cd_30_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_30_lab.Background = CalenderColorCode.back_color_reset;
            cd_30_lab.ToolTip = null;

            cd_31_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_31_lab.Background = CalenderColorCode.back_color_reset;
            cd_31_lab.ToolTip = null;

            cd_32_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_32_lab.Background = CalenderColorCode.back_color_reset;
            cd_32_lab.ToolTip = null;

            cd_33_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_33_lab.Background = CalenderColorCode.back_color_reset;
            cd_33_lab.ToolTip = null;

            cd_34_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_34_lab.Background = CalenderColorCode.back_color_reset;
            cd_34_lab.ToolTip = null;

            cd_35_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_35_lab.Background = CalenderColorCode.back_color_reset;
            cd_35_lab.ToolTip = null;

            cd_36_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_36_lab.Background = CalenderColorCode.back_color_reset;
            cd_36_lab.ToolTip = null;

            cd_37_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_37_lab.Background = CalenderColorCode.back_color_reset;
            cd_37_lab.ToolTip = null;

            cd_38_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_38_lab.Background = CalenderColorCode.back_color_reset;
            cd_38_lab.ToolTip = null;

            cd_39_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_39_lab.Background = CalenderColorCode.back_color_reset;
            cd_39_lab.ToolTip = null;

            cd_40_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_40_lab.Background = CalenderColorCode.back_color_reset;
            cd_40_lab.ToolTip = null;

            cd_41_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_41_lab.Background = CalenderColorCode.back_color_reset;
            cd_41_lab.ToolTip = null;

            cd_42_lab.Foreground = CalenderColorCode.font_color_reset;
            cd_42_lab.Background = CalenderColorCode.back_color_reset;
            cd_42_lab.ToolTip = null;
        }

        void configMonthLable()
        {
            switch (DateTime.Now.AddMonths(calenderCount).Month)
            {
                case 1:
                    month_lab.Content = "January " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 2:
                    month_lab.Content = "February " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 3:
                    month_lab.Content = "March " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 4:
                    month_lab.Content = "April " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 5:
                    month_lab.Content = "May " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 6:
                    month_lab.Content = "June " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 7:
                    month_lab.Content = "July " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 8:
                    month_lab.Content = "August " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 9:
                    month_lab.Content = "September " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 10:
                    month_lab.Content = "October " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 11:
                    month_lab.Content = "November " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
                case 12:
                    month_lab.Content = "December " + DateTime.Now.AddMonths(calenderCount).Year;
                    break;
            }
        }

        void markResetCurrentDate(int currentDay, int int_currentmonth_dayofweek, SolidColorBrush font_color, SolidColorBrush back_color)
        {
            int position = currentDay + int_currentmonth_dayofweek - 1;
            //console.writeline("markResetCurrentDate " + position);
            switch (position)
            {
                case 1:
                    cd_1_lab.Background = back_color;
                    cd_1_lab.Foreground = font_color;
                    break;
                case 2:
                    cd_2_lab.Background = back_color;
                    cd_2_lab.Foreground = font_color;
                    break;
                case 3:
                    cd_3_lab.Background = back_color;
                    cd_3_lab.Foreground = font_color;
                    break;
                case 4:
                    cd_4_lab.Background = back_color;
                    cd_4_lab.Foreground = font_color;
                    break;
                case 5:
                    cd_5_lab.Background = back_color;
                    cd_5_lab.Foreground = font_color;
                    break;
                case 6:
                    cd_6_lab.Background = back_color;
                    cd_6_lab.Foreground = font_color;
                    break;
                case 7:
                    cd_7_lab.Background = back_color;
                    cd_7_lab.Foreground = font_color;
                    break;
                case 8:
                    cd_8_lab.Background = back_color;
                    cd_8_lab.Foreground = font_color;
                    break;
                case 9:
                    cd_9_lab.Background = back_color;
                    cd_9_lab.Foreground = font_color;
                    break;
                case 10:
                    cd_10_lab.Background = back_color;
                    cd_10_lab.Foreground = font_color;
                    break;
                case 11:
                    cd_11_lab.Background = back_color;
                    cd_11_lab.Foreground = font_color;
                    break;
                case 12:
                    cd_12_lab.Background = back_color;
                    cd_12_lab.Foreground = font_color;
                    break;
                case 13:
                    cd_13_lab.Background = back_color;
                    cd_13_lab.Foreground = font_color;
                    break;
                case 14:
                    cd_14_lab.Background = back_color;
                    cd_14_lab.Foreground = font_color;
                    break;
                case 15:
                    cd_15_lab.Background = back_color;
                    cd_15_lab.Foreground = font_color;
                    break;
                case 16:
                    cd_16_lab.Background = back_color;
                    cd_16_lab.Foreground = font_color;
                    break;
                case 17:
                    cd_17_lab.Background = back_color;
                    cd_17_lab.Foreground = font_color;
                    break;
                case 18:
                    cd_18_lab.Background = back_color;
                    cd_18_lab.Foreground = font_color;
                    break;
                case 19:
                    cd_19_lab.Background = back_color;
                    cd_19_lab.Foreground = font_color;
                    break;
                case 20:
                    cd_20_lab.Background = back_color;
                    cd_20_lab.Foreground = font_color;
                    break;
                case 21:
                    cd_21_lab.Background = back_color;
                    cd_21_lab.Foreground = font_color;
                    break;
                case 22:
                    cd_22_lab.Background = back_color;
                    cd_22_lab.Foreground = font_color;
                    break;
                case 23:
                    cd_23_lab.Background = back_color;
                    cd_23_lab.Foreground = font_color;
                    break;
                case 24:
                    cd_24_lab.Background = back_color;
                    cd_24_lab.Foreground = font_color;
                    break;
                case 25:
                    cd_25_lab.Background = back_color;
                    cd_25_lab.Foreground = font_color;
                    break;
                case 26:
                    cd_26_lab.Background = back_color;
                    cd_26_lab.Foreground = font_color;
                    break;
                case 27:
                    cd_27_lab.Background = back_color;
                    cd_27_lab.Foreground = font_color;
                    break;
                case 28:
                    cd_28_lab.Background = back_color;
                    cd_28_lab.Foreground = font_color;
                    break;
                case 29:
                    cd_29_lab.Background = back_color;
                    cd_29_lab.Foreground = font_color;
                    break;
                case 30:
                    cd_30_lab.Background = back_color;
                    cd_30_lab.Foreground = font_color;
                    break;
                case 31:
                    cd_31_lab.Background = back_color;
                    cd_31_lab.Foreground = font_color;
                    break;
                case 32:
                    cd_32_lab.Background = back_color;
                    cd_32_lab.Foreground = font_color;
                    break;
                case 33:
                    cd_33_lab.Background = back_color;
                    cd_33_lab.Foreground = font_color;
                    break;
                case 34:
                    cd_34_lab.Background = back_color;
                    cd_34_lab.Foreground = font_color;
                    break;
                case 35:
                    cd_35_lab.Background = back_color;
                    cd_35_lab.Foreground = font_color;
                    break;
                case 36:
                    cd_36_lab.Background = back_color;
                    cd_36_lab.Foreground = font_color;
                    break;
                case 37:
                    cd_37_lab.Background = back_color;
                    cd_37_lab.Foreground = font_color;
                    break;
                case 38:
                    cd_38_lab.Background = back_color;
                    cd_38_lab.Foreground = font_color;
                    break;
                default:
                    break;
            }
        }

        void markWithSeven(int currentMonthTotalDays, int priviousMonthTotalDays)
        {
            cd_1_lab.Foreground = Brushes.Gray;
            cd_1_lab.Content = priviousMonthTotalDays - 5;
            cd_2_lab.Foreground = Brushes.Gray;
            cd_2_lab.Content = priviousMonthTotalDays - 4;
            cd_3_lab.Foreground = Brushes.Gray;
            cd_3_lab.Content = priviousMonthTotalDays - 3;
            cd_4_lab.Foreground = Brushes.Gray;
            cd_4_lab.Content = priviousMonthTotalDays - 2;
            cd_5_lab.Foreground = Brushes.Gray;
            cd_5_lab.Content = priviousMonthTotalDays - 1;
            cd_6_lab.Foreground = Brushes.Gray;
            cd_6_lab.Content = priviousMonthTotalDays;
            int i = 1;
            cd_7_lab.Content = i++;
            cd_8_lab.Content = i++;
            cd_9_lab.Content = i++;
            cd_10_lab.Content = i++;
            cd_11_lab.Content = i++;
            cd_12_lab.Content = i++;
            cd_13_lab.Content = i++;
            cd_14_lab.Content = i++;
            cd_15_lab.Content = i++;
            cd_16_lab.Content = i++;
            cd_17_lab.Content = i++;
            cd_18_lab.Content = i++;
            cd_19_lab.Content = i++;
            cd_20_lab.Content = i++;
            cd_21_lab.Content = i++;
            cd_22_lab.Content = i++;
            cd_23_lab.Content = i++;
            cd_24_lab.Content = i++;
            cd_25_lab.Content = i++;
            cd_26_lab.Content = i++;
            cd_27_lab.Content = i++;
            cd_28_lab.Content = i++;
            cd_29_lab.Content = i++;
            cd_30_lab.Content = i++;
            cd_31_lab.Content = i++;
            cd_32_lab.Content = i++;
            cd_33_lab.Content = i++;
            cd_34_lab.Content = i++;
            if (i == (currentMonthTotalDays + 1))
            {
                int ii = 1;
                cd_35_lab.Foreground = Brushes.Gray;
                cd_35_lab.Content = ii++;
                cd_36_lab.Foreground = Brushes.Gray;
                cd_36_lab.Content = ii++;
                cd_37_lab.Foreground = Brushes.Gray;
                cd_37_lab.Content = ii++;
                cd_38_lab.Foreground = Brushes.Gray;
                cd_38_lab.Content = ii++;
                cd_39_lab.Foreground = Brushes.Gray;
                cd_39_lab.Content = ii++;
                cd_40_lab.Foreground = Brushes.Gray;
                cd_40_lab.Content = ii++;
                cd_41_lab.Foreground = Brushes.Gray;
                cd_41_lab.Content = ii++;
                cd_42_lab.Foreground = Brushes.Gray;
                cd_42_lab.Content = ii++;
            }
            else
            {
                cd_35_lab.Content = i++;
                if (i == (currentMonthTotalDays + 1))
                {
                    int ii = 1;
                    cd_36_lab.Content = ii++;
                    cd_36_lab.Foreground = Brushes.Gray;
                    cd_37_lab.Content = ii++;
                    cd_37_lab.Foreground = Brushes.Gray;
                    cd_38_lab.Content = ii++;
                    cd_38_lab.Foreground = Brushes.Gray;
                    cd_39_lab.Content = ii++;
                    cd_39_lab.Foreground = Brushes.Gray;
                    cd_40_lab.Content = ii++;
                    cd_40_lab.Foreground = Brushes.Gray;
                    cd_41_lab.Content = ii++;
                    cd_41_lab.Foreground = Brushes.Gray;
                    cd_42_lab.Content = ii++;
                    cd_42_lab.Foreground = Brushes.Gray;
                }
                else
                {
                    cd_36_lab.Content = i++;
                    if (i == (currentMonthTotalDays + 1))
                    {
                        int ii = 1;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                    else
                    {
                        cd_37_lab.Content = i;
                        int ii = 1;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                }
            }
        }

        void markWithSix(int currentMonthTotalDays, int priviousMonthTotalDays)
        {
            cd_1_lab.Foreground = Brushes.Gray;
            cd_1_lab.Content = priviousMonthTotalDays - 4;
            cd_2_lab.Foreground = Brushes.Gray;
            cd_2_lab.Content = priviousMonthTotalDays - 3;
            cd_3_lab.Foreground = Brushes.Gray;
            cd_3_lab.Content = priviousMonthTotalDays - 2;
            cd_4_lab.Foreground = Brushes.Gray;
            cd_4_lab.Content = priviousMonthTotalDays - 1;
            cd_5_lab.Foreground = Brushes.Gray;
            cd_5_lab.Content = priviousMonthTotalDays;
            int i = 1;
            cd_6_lab.Content = i++;
            cd_7_lab.Content = i++;
            cd_8_lab.Content = i++;
            cd_9_lab.Content = i++;
            cd_10_lab.Content = i++;
            cd_11_lab.Content = i++;
            cd_12_lab.Content = i++;
            cd_13_lab.Content = i++;
            cd_14_lab.Content = i++;
            cd_15_lab.Content = i++;
            cd_16_lab.Content = i++;
            cd_17_lab.Content = i++;
            cd_18_lab.Content = i++;
            cd_19_lab.Content = i++;
            cd_20_lab.Content = i++;
            cd_21_lab.Content = i++;
            cd_22_lab.Content = i++;
            cd_23_lab.Content = i++;
            cd_24_lab.Content = i++;
            cd_25_lab.Content = i++;
            cd_26_lab.Content = i++;
            cd_27_lab.Content = i++;
            cd_28_lab.Content = i++;
            cd_29_lab.Content = i++;
            cd_30_lab.Content = i++;
            cd_31_lab.Content = i++;
            cd_32_lab.Content = i++;
            cd_33_lab.Content = i++;
            if (i == (currentMonthTotalDays + 1))
            {
                int ii = 1;
                cd_34_lab.Foreground = Brushes.Gray;
                cd_34_lab.Content = ii++;
                cd_35_lab.Foreground = Brushes.Gray;
                cd_35_lab.Content = ii++;
                cd_36_lab.Foreground = Brushes.Gray;
                cd_36_lab.Content = ii++;
                cd_37_lab.Foreground = Brushes.Gray;
                cd_37_lab.Content = ii++;
                cd_38_lab.Foreground = Brushes.Gray;
                cd_38_lab.Content = ii++;
                cd_39_lab.Foreground = Brushes.Gray;
                cd_39_lab.Content = ii++;
                cd_40_lab.Foreground = Brushes.Gray;
                cd_40_lab.Content = ii++;
                cd_41_lab.Foreground = Brushes.Gray;
                cd_41_lab.Content = ii++;
                cd_42_lab.Foreground = Brushes.Gray;
                cd_42_lab.Content = ii++;
            }
            else
            {
                cd_34_lab.Content = i++;
                if (i == (currentMonthTotalDays + 1))
                {
                    int ii = 1;
                    cd_35_lab.Content = ii++;
                    cd_35_lab.Foreground = Brushes.Gray;
                    cd_36_lab.Content = ii++;
                    cd_36_lab.Foreground = Brushes.Gray;
                    cd_37_lab.Content = ii++;
                    cd_37_lab.Foreground = Brushes.Gray;
                    cd_38_lab.Content = ii++;
                    cd_38_lab.Foreground = Brushes.Gray;
                    cd_39_lab.Content = ii++;
                    cd_39_lab.Foreground = Brushes.Gray;
                    cd_40_lab.Content = ii++;
                    cd_40_lab.Foreground = Brushes.Gray;
                    cd_41_lab.Content = ii++;
                    cd_41_lab.Foreground = Brushes.Gray;
                    cd_42_lab.Content = ii++;
                    cd_42_lab.Foreground = Brushes.Gray;
                }
                else
                {
                    cd_35_lab.Content = i++;
                    if (i == (currentMonthTotalDays + 1))
                    {
                        int ii = 1;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                    else
                    {
                        cd_36_lab.Content = i;
                        int ii = 1;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                }
            }
        }

        void markWithFive(int currentMonthTotalDays, int priviousMonthTotalDays)
        {
            cd_1_lab.Foreground = Brushes.Gray;
            cd_1_lab.Content = priviousMonthTotalDays - 3;
            cd_2_lab.Foreground = Brushes.Gray;
            cd_2_lab.Content = priviousMonthTotalDays - 2;
            cd_3_lab.Foreground = Brushes.Gray;
            cd_3_lab.Content = priviousMonthTotalDays - 1;
            cd_4_lab.Foreground = Brushes.Gray;
            cd_4_lab.Content = priviousMonthTotalDays;
            int i = 1;
            cd_5_lab.Content = i++;
            cd_6_lab.Content = i++;
            cd_7_lab.Content = i++;
            cd_8_lab.Content = i++;
            cd_9_lab.Content = i++;
            cd_10_lab.Content = i++;
            cd_11_lab.Content = i++;
            cd_12_lab.Content = i++;
            cd_13_lab.Content = i++;
            cd_14_lab.Content = i++;
            cd_15_lab.Content = i++;
            cd_16_lab.Content = i++;
            cd_17_lab.Content = i++;
            cd_18_lab.Content = i++;
            cd_19_lab.Content = i++;
            cd_20_lab.Content = i++;
            cd_21_lab.Content = i++;
            cd_22_lab.Content = i++;
            cd_23_lab.Content = i++;
            cd_24_lab.Content = i++;
            cd_25_lab.Content = i++;
            cd_26_lab.Content = i++;
            cd_27_lab.Content = i++;
            cd_28_lab.Content = i++;
            cd_29_lab.Content = i++;
            cd_30_lab.Content = i++;
            cd_31_lab.Content = i++;
            cd_32_lab.Content = i++;
            if (i == (currentMonthTotalDays + 1))
            {
                int ii = 1;
                cd_33_lab.Foreground = Brushes.Gray;
                cd_33_lab.Content = ii++;
                cd_34_lab.Foreground = Brushes.Gray;
                cd_34_lab.Content = ii++;
                cd_35_lab.Foreground = Brushes.Gray;
                cd_35_lab.Content = ii++;
                cd_36_lab.Foreground = Brushes.Gray;
                cd_36_lab.Content = ii++;
                cd_37_lab.Foreground = Brushes.Gray;
                cd_37_lab.Content = ii++;
                cd_38_lab.Foreground = Brushes.Gray;
                cd_38_lab.Content = ii++;
                cd_39_lab.Foreground = Brushes.Gray;
                cd_39_lab.Content = ii++;
                cd_40_lab.Foreground = Brushes.Gray;
                cd_40_lab.Content = ii++;
                cd_41_lab.Foreground = Brushes.Gray;
                cd_41_lab.Content = ii++;
                cd_42_lab.Foreground = Brushes.Gray;
                cd_42_lab.Content = ii++;
            }
            else
            {
                cd_33_lab.Content = i++;
                if (i == (currentMonthTotalDays + 1))
                {
                    int ii = 1;
                    cd_34_lab.Content = ii++;
                    cd_34_lab.Foreground = Brushes.Gray;
                    cd_35_lab.Content = ii++;
                    cd_35_lab.Foreground = Brushes.Gray;
                    cd_36_lab.Content = ii++;
                    cd_36_lab.Foreground = Brushes.Gray;
                    cd_37_lab.Content = ii++;
                    cd_37_lab.Foreground = Brushes.Gray;
                    cd_38_lab.Content = ii++;
                    cd_38_lab.Foreground = Brushes.Gray;
                    cd_39_lab.Content = ii++;
                    cd_39_lab.Foreground = Brushes.Gray;
                    cd_40_lab.Content = ii++;
                    cd_40_lab.Foreground = Brushes.Gray;
                    cd_41_lab.Content = ii++;
                    cd_41_lab.Foreground = Brushes.Gray;
                    cd_42_lab.Content = ii++;
                    cd_42_lab.Foreground = Brushes.Gray;
                }
                else
                {
                    cd_34_lab.Content = i++;
                    if (i == (currentMonthTotalDays + 1))
                    {
                        int ii = 1;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                    else
                    {
                        cd_35_lab.Content = i;
                        int ii = 1;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                }
            }
        }

        void markWithFour(int currentMonthTotalDays, int priviousMonthTotalDays)
        {
            cd_1_lab.Foreground = Brushes.Gray;
            cd_1_lab.Content = priviousMonthTotalDays - 2;
            cd_2_lab.Foreground = Brushes.Gray;
            cd_2_lab.Content = priviousMonthTotalDays - 1;
            cd_3_lab.Foreground = Brushes.Gray;
            cd_3_lab.Content = priviousMonthTotalDays;
            int i = 1;
            cd_4_lab.Content = i++;
            cd_5_lab.Content = i++;
            cd_6_lab.Content = i++;
            cd_7_lab.Content = i++;
            cd_8_lab.Content = i++;
            cd_9_lab.Content = i++;
            cd_10_lab.Content = i++;
            cd_11_lab.Content = i++;
            cd_12_lab.Content = i++;
            cd_13_lab.Content = i++;
            cd_14_lab.Content = i++;
            cd_15_lab.Content = i++;
            cd_16_lab.Content = i++;
            cd_17_lab.Content = i++;
            cd_18_lab.Content = i++;
            cd_19_lab.Content = i++;
            cd_20_lab.Content = i++;
            cd_21_lab.Content = i++;
            cd_22_lab.Content = i++;
            cd_23_lab.Content = i++;
            cd_24_lab.Content = i++;
            cd_25_lab.Content = i++;
            cd_26_lab.Content = i++;
            cd_27_lab.Content = i++;
            cd_28_lab.Content = i++;
            cd_29_lab.Content = i++;
            cd_30_lab.Content = i++;
            cd_31_lab.Content = i++;
            if (i == (currentMonthTotalDays + 1))
            {
                int ii = 1;
                cd_32_lab.Foreground = Brushes.Gray;
                cd_32_lab.Content = ii++;
                cd_33_lab.Foreground = Brushes.Gray;
                cd_33_lab.Content = ii++;
                cd_34_lab.Foreground = Brushes.Gray;
                cd_34_lab.Content = ii++;
                cd_35_lab.Foreground = Brushes.Gray;
                cd_35_lab.Content = ii++;
                cd_36_lab.Foreground = Brushes.Gray;
                cd_36_lab.Content = ii++;
                cd_37_lab.Foreground = Brushes.Gray;
                cd_37_lab.Content = ii++;
                cd_38_lab.Foreground = Brushes.Gray;
                cd_38_lab.Content = ii++;
                cd_39_lab.Foreground = Brushes.Gray;
                cd_39_lab.Content = ii++;
                cd_40_lab.Foreground = Brushes.Gray;
                cd_40_lab.Content = ii++;
                cd_41_lab.Foreground = Brushes.Gray;
                cd_41_lab.Content = ii++;
                cd_42_lab.Foreground = Brushes.Gray;
                cd_42_lab.Content = ii++;
            }
            else
            {
                cd_32_lab.Content = i++;
                if (i == (currentMonthTotalDays + 1))
                {
                    int ii = 1;
                    cd_33_lab.Content = ii++;
                    cd_33_lab.Foreground = Brushes.Gray;
                    cd_34_lab.Content = ii++;
                    cd_34_lab.Foreground = Brushes.Gray;
                    cd_35_lab.Content = ii++;
                    cd_35_lab.Foreground = Brushes.Gray;
                    cd_36_lab.Content = ii++;
                    cd_36_lab.Foreground = Brushes.Gray;
                    cd_37_lab.Content = ii++;
                    cd_37_lab.Foreground = Brushes.Gray;
                    cd_38_lab.Content = ii++;
                    cd_38_lab.Foreground = Brushes.Gray;
                    cd_39_lab.Content = ii++;
                    cd_39_lab.Foreground = Brushes.Gray;
                    cd_40_lab.Content = ii++;
                    cd_40_lab.Foreground = Brushes.Gray;
                    cd_41_lab.Content = ii++;
                    cd_41_lab.Foreground = Brushes.Gray;
                    cd_42_lab.Content = ii++;
                    cd_42_lab.Foreground = Brushes.Gray;
                }
                else
                {
                    cd_33_lab.Content = i++;
                    if (i == (currentMonthTotalDays + 1))
                    {
                        int ii = 1;
                        cd_34_lab.Content = ii++;
                        cd_34_lab.Foreground = Brushes.Gray;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                    else
                    {
                        cd_34_lab.Content = i;
                        int ii = 1;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                }
            }
        }

        void markWithThree(int currentMonthTotalDays, int priviousMonthTotalDays)
        {
            cd_1_lab.Foreground = Brushes.Gray;
            cd_1_lab.Content = priviousMonthTotalDays - 1;
            cd_2_lab.Foreground = Brushes.Gray;
            cd_2_lab.Content = priviousMonthTotalDays;
            int i = 1;
            cd_3_lab.Content = i++;
            cd_4_lab.Content = i++;
            cd_5_lab.Content = i++;
            cd_6_lab.Content = i++;
            cd_7_lab.Content = i++;
            cd_8_lab.Content = i++;
            cd_9_lab.Content = i++;
            cd_10_lab.Content = i++;
            cd_11_lab.Content = i++;
            cd_12_lab.Content = i++;
            cd_13_lab.Content = i++;
            cd_14_lab.Content = i++;
            cd_15_lab.Content = i++;
            cd_16_lab.Content = i++;
            cd_17_lab.Content = i++;
            cd_18_lab.Content = i++;
            cd_19_lab.Content = i++;
            cd_20_lab.Content = i++;
            cd_21_lab.Content = i++;
            cd_22_lab.Content = i++;
            cd_23_lab.Content = i++;
            cd_24_lab.Content = i++;
            cd_25_lab.Content = i++;
            cd_26_lab.Content = i++;
            cd_27_lab.Content = i++;
            cd_28_lab.Content = i++;
            cd_29_lab.Content = i++;
            cd_30_lab.Content = i++;
            if (i == (currentMonthTotalDays + 1))
            {
                int ii = 1;
                cd_31_lab.Foreground = Brushes.Gray;
                cd_31_lab.Content = ii++;
                cd_32_lab.Foreground = Brushes.Gray;
                cd_32_lab.Content = ii++;
                cd_33_lab.Foreground = Brushes.Gray;
                cd_33_lab.Content = ii++;
                cd_34_lab.Foreground = Brushes.Gray;
                cd_34_lab.Content = ii++;
                cd_35_lab.Foreground = Brushes.Gray;
                cd_35_lab.Content = ii++;
                cd_36_lab.Foreground = Brushes.Gray;
                cd_36_lab.Content = ii++;
                cd_37_lab.Foreground = Brushes.Gray;
                cd_37_lab.Content = ii++;
                cd_38_lab.Foreground = Brushes.Gray;
                cd_38_lab.Content = ii++;
                cd_39_lab.Foreground = Brushes.Gray;
                cd_39_lab.Content = ii++;
                cd_40_lab.Foreground = Brushes.Gray;
                cd_40_lab.Content = ii++;
                cd_41_lab.Foreground = Brushes.Gray;
                cd_41_lab.Content = ii++;
                cd_42_lab.Foreground = Brushes.Gray;
                cd_42_lab.Content = ii++;
            }
            else
            {
                cd_31_lab.Content = i++;
                if (i == (currentMonthTotalDays + 1))
                {
                    int ii = 1;
                    cd_32_lab.Content = ii++;
                    cd_32_lab.Foreground = Brushes.Gray;
                    cd_33_lab.Content = ii++;
                    cd_33_lab.Foreground = Brushes.Gray;
                    cd_34_lab.Content = ii++;
                    cd_34_lab.Foreground = Brushes.Gray;
                    cd_35_lab.Content = ii++;
                    cd_35_lab.Foreground = Brushes.Gray;
                    cd_36_lab.Content = ii++;
                    cd_36_lab.Foreground = Brushes.Gray;
                    cd_37_lab.Content = ii++;
                    cd_37_lab.Foreground = Brushes.Gray;
                    cd_38_lab.Content = ii++;
                    cd_38_lab.Foreground = Brushes.Gray;
                    cd_39_lab.Content = ii++;
                    cd_39_lab.Foreground = Brushes.Gray;
                    cd_40_lab.Content = ii++;
                    cd_40_lab.Foreground = Brushes.Gray;
                    cd_41_lab.Content = ii++;
                    cd_41_lab.Foreground = Brushes.Gray;
                    cd_42_lab.Content = ii++;
                    cd_42_lab.Foreground = Brushes.Gray;
                }
                else
                {
                    cd_32_lab.Content = i++;
                    if (i == (currentMonthTotalDays + 1))
                    {
                        int ii = 1;
                        cd_33_lab.Content = ii++;
                        cd_33_lab.Foreground = Brushes.Gray;
                        cd_34_lab.Content = ii++;
                        cd_34_lab.Foreground = Brushes.Gray;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                    else
                    {
                        cd_33_lab.Content = i;
                        int ii = 1;
                        cd_34_lab.Content = ii++;
                        cd_34_lab.Foreground = Brushes.Gray;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                }
            }
        }

        void markWithTwo(int currentMonthTotalDays, int priviousMonthTotalDays)
        {
            cd_1_lab.Foreground = Brushes.Gray;
            cd_1_lab.Content = priviousMonthTotalDays;
            int i = 1;
            cd_2_lab.Content = i++;
            cd_3_lab.Content = i++;
            cd_4_lab.Content = i++;
            cd_5_lab.Content = i++;
            cd_6_lab.Content = i++;
            cd_7_lab.Content = i++;
            cd_8_lab.Content = i++;
            cd_9_lab.Content = i++;
            cd_10_lab.Content = i++;
            cd_11_lab.Content = i++;
            cd_12_lab.Content = i++;
            cd_13_lab.Content = i++;
            cd_14_lab.Content = i++;
            cd_15_lab.Content = i++;
            cd_16_lab.Content = i++;
            cd_17_lab.Content = i++;
            cd_18_lab.Content = i++;
            cd_19_lab.Content = i++;
            cd_20_lab.Content = i++;
            cd_21_lab.Content = i++;
            cd_22_lab.Content = i++;
            cd_23_lab.Content = i++;
            cd_24_lab.Content = i++;
            cd_25_lab.Content = i++;
            cd_26_lab.Content = i++;
            cd_27_lab.Content = i++;
            cd_28_lab.Content = i++;
            cd_29_lab.Content = i++;
            if (i == (currentMonthTotalDays + 1))
            {
                int ii = 1;
                cd_30_lab.Foreground = Brushes.Gray;
                cd_30_lab.Content = ii++;
                cd_31_lab.Foreground = Brushes.Gray;
                cd_31_lab.Content = ii++;
                cd_32_lab.Foreground = Brushes.Gray;
                cd_32_lab.Content = ii++;
                cd_33_lab.Foreground = Brushes.Gray;
                cd_33_lab.Content = ii++;
                cd_34_lab.Foreground = Brushes.Gray;
                cd_34_lab.Content = ii++;
                cd_35_lab.Foreground = Brushes.Gray;
                cd_35_lab.Content = ii++;
                cd_36_lab.Foreground = Brushes.Gray;
                cd_36_lab.Content = ii++;
                cd_37_lab.Foreground = Brushes.Gray;
                cd_37_lab.Content = ii++;
                cd_38_lab.Foreground = Brushes.Gray;
                cd_38_lab.Content = ii++;
                cd_39_lab.Foreground = Brushes.Gray;
                cd_39_lab.Content = ii++;
                cd_40_lab.Foreground = Brushes.Gray;
                cd_40_lab.Content = ii++;
                cd_41_lab.Foreground = Brushes.Gray;
                cd_41_lab.Content = ii++;
                cd_42_lab.Foreground = Brushes.Gray;
                cd_42_lab.Content = ii++;
            }
            else
            {
                cd_30_lab.Content = i++;
                if (i == (currentMonthTotalDays + 1))
                {
                    int ii = 1;
                    cd_31_lab.Content = ii++;
                    cd_31_lab.Foreground = Brushes.Gray;
                    cd_32_lab.Content = ii++;
                    cd_32_lab.Foreground = Brushes.Gray;
                    cd_33_lab.Content = ii++;
                    cd_33_lab.Foreground = Brushes.Gray;
                    cd_34_lab.Content = ii++;
                    cd_34_lab.Foreground = Brushes.Gray;
                    cd_35_lab.Content = ii++;
                    cd_35_lab.Foreground = Brushes.Gray;
                    cd_36_lab.Content = ii++;
                    cd_36_lab.Foreground = Brushes.Gray;
                    cd_37_lab.Content = ii++;
                    cd_37_lab.Foreground = Brushes.Gray;
                    cd_38_lab.Content = ii++;
                    cd_38_lab.Foreground = Brushes.Gray;
                    cd_39_lab.Content = ii++;
                    cd_39_lab.Foreground = Brushes.Gray;
                    cd_40_lab.Content = ii++;
                    cd_40_lab.Foreground = Brushes.Gray;
                    cd_41_lab.Content = ii++;
                    cd_41_lab.Foreground = Brushes.Gray;
                    cd_42_lab.Content = ii++;
                    cd_42_lab.Foreground = Brushes.Gray;
                }
                else
                {
                    cd_31_lab.Content = i++;
                    if (i == (currentMonthTotalDays + 1))
                    {
                        int ii = 1;
                        cd_32_lab.Content = ii++;
                        cd_32_lab.Foreground = Brushes.Gray;
                        cd_33_lab.Content = ii++;
                        cd_33_lab.Foreground = Brushes.Gray;
                        cd_34_lab.Content = ii++;
                        cd_34_lab.Foreground = Brushes.Gray;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                    else
                    {
                        cd_32_lab.Content = i;
                        int ii = 1;
                        cd_33_lab.Content = ii++;
                        cd_33_lab.Foreground = Brushes.Gray;
                        cd_34_lab.Content = ii++;
                        cd_34_lab.Foreground = Brushes.Gray;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                }
            }
        }

        void markWithOne(int currentMonthTotalDays)
        {
            int i = 1;
            cd_1_lab.Content = i++;
            cd_2_lab.Content = i++;
            cd_3_lab.Content = i++;
            cd_4_lab.Content = i++;
            cd_5_lab.Content = i++;
            cd_6_lab.Content = i++;
            cd_7_lab.Content = i++;
            cd_8_lab.Content = i++;
            cd_9_lab.Content = i++;
            cd_10_lab.Content = i++;
            cd_11_lab.Content = i++;
            cd_12_lab.Content = i++;
            cd_13_lab.Content = i++;
            cd_14_lab.Content = i++;
            cd_15_lab.Content = i++;
            cd_16_lab.Content = i++;
            cd_17_lab.Content = i++;
            cd_18_lab.Content = i++;
            cd_19_lab.Content = i++;
            cd_20_lab.Content = i++;
            cd_21_lab.Content = i++;
            cd_22_lab.Content = i++;
            cd_23_lab.Content = i++;
            cd_24_lab.Content = i++;
            cd_25_lab.Content = i++;
            cd_26_lab.Content = i++;
            cd_27_lab.Content = i++;
            cd_28_lab.Content = i++;
            //console.writeline(i);
            if (i == (currentMonthTotalDays + 1))
            {
                int ii = 1;
                cd_29_lab.Foreground = Brushes.Gray;
                cd_29_lab.Content = ii++;
                cd_30_lab.Foreground = Brushes.Gray;
                cd_30_lab.Content = ii++;
                cd_31_lab.Foreground = Brushes.Gray;
                cd_31_lab.Content = ii++;
                cd_32_lab.Foreground = Brushes.Gray;
                cd_32_lab.Content = ii++;
                cd_33_lab.Foreground = Brushes.Gray;
                cd_33_lab.Content = ii++;
                cd_34_lab.Foreground = Brushes.Gray;
                cd_34_lab.Content = ii++;
                cd_35_lab.Foreground = Brushes.Gray;
                cd_35_lab.Content = ii++;
                cd_36_lab.Foreground = Brushes.Gray;
                cd_36_lab.Content = ii++;
                cd_37_lab.Foreground = Brushes.Gray;
                cd_37_lab.Content = ii++;
                cd_38_lab.Foreground = Brushes.Gray;
                cd_38_lab.Content = ii++;
                cd_39_lab.Foreground = Brushes.Gray;
                cd_39_lab.Content = ii++;
                cd_40_lab.Foreground = Brushes.Gray;
                cd_40_lab.Content = ii++;
                cd_41_lab.Foreground = Brushes.Gray;
                cd_41_lab.Content = ii++;
                cd_42_lab.Foreground = Brushes.Gray;
                cd_42_lab.Content = ii++;
            }
            else
            {
                cd_29_lab.Content = i++;
                if (i == (currentMonthTotalDays + 1))
                {
                    int ii = 1;
                    cd_30_lab.Content = ii++;
                    cd_30_lab.Foreground = Brushes.Gray;
                    cd_31_lab.Content = ii++;
                    cd_31_lab.Foreground = Brushes.Gray;
                    cd_32_lab.Content = ii++;
                    cd_32_lab.Foreground = Brushes.Gray;
                    cd_33_lab.Content = ii++;
                    cd_33_lab.Foreground = Brushes.Gray;
                    cd_34_lab.Content = ii++;
                    cd_34_lab.Foreground = Brushes.Gray;
                    cd_35_lab.Content = ii++;
                    cd_35_lab.Foreground = Brushes.Gray;
                    cd_36_lab.Content = ii++;
                    cd_36_lab.Foreground = Brushes.Gray;
                    cd_37_lab.Content = ii++;
                    cd_37_lab.Foreground = Brushes.Gray;
                    cd_38_lab.Content = ii++;
                    cd_38_lab.Foreground = Brushes.Gray;
                    cd_39_lab.Content = ii++;
                    cd_39_lab.Foreground = Brushes.Gray;
                    cd_40_lab.Content = ii++;
                    cd_40_lab.Foreground = Brushes.Gray;
                    cd_41_lab.Content = ii++;
                    cd_41_lab.Foreground = Brushes.Gray;
                    cd_42_lab.Content = ii++;
                    cd_42_lab.Foreground = Brushes.Gray;
                }
                else
                {
                    cd_30_lab.Content = i++;
                    if (i == (currentMonthTotalDays + 1))
                    {
                        int ii = 1;
                        cd_31_lab.Content = ii++;
                        cd_31_lab.Foreground = Brushes.Gray;
                        cd_32_lab.Content = ii++;
                        cd_32_lab.Foreground = Brushes.Gray;
                        cd_33_lab.Content = ii++;
                        cd_33_lab.Foreground = Brushes.Gray;
                        cd_34_lab.Content = ii++;
                        cd_34_lab.Foreground = Brushes.Gray;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                    else
                    {
                        cd_31_lab.Content = i;
                        int ii = 1;
                        cd_32_lab.Content = ii++;
                        cd_32_lab.Foreground = Brushes.Gray;
                        cd_33_lab.Content = ii++;
                        cd_33_lab.Foreground = Brushes.Gray;
                        cd_34_lab.Content = ii++;
                        cd_34_lab.Foreground = Brushes.Gray;
                        cd_35_lab.Content = ii++;
                        cd_35_lab.Foreground = Brushes.Gray;
                        cd_36_lab.Content = ii++;
                        cd_36_lab.Foreground = Brushes.Gray;
                        cd_37_lab.Content = ii++;
                        cd_37_lab.Foreground = Brushes.Gray;
                        cd_38_lab.Content = ii++;
                        cd_38_lab.Foreground = Brushes.Gray;
                        cd_39_lab.Content = ii++;
                        cd_39_lab.Foreground = Brushes.Gray;
                        cd_40_lab.Content = ii++;
                        cd_40_lab.Foreground = Brushes.Gray;
                        cd_41_lab.Content = ii++;
                        cd_41_lab.Foreground = Brushes.Gray;
                        cd_42_lab.Content = ii++;
                        cd_42_lab.Foreground = Brushes.Gray;
                    }
                }
            }
        }

        private void calender_left_but_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            calenderCount -= 1;
            //console.writeline("calenderCount " + calenderCount);
            configCalender(DateTime.Now.AddMonths(calenderCount).Year, DateTime.Now.AddMonths(calenderCount).Month, DateTime.Now.AddMonths(calenderCount).Day);
        }

        private void calender_right_but_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            calenderCount += 1;
            //console.writeline("calenderCount " + calenderCount);
            configCalender(DateTime.Now.AddMonths(calenderCount).Year, DateTime.Now.AddMonths(calenderCount).Month, DateTime.Now.AddMonths(calenderCount).Day);
        }

        #endregion

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (check)
            {
                try
                {
                    using (OdbcCommand MyCommand = new OdbcCommand("select title,description,day,month from jos_jcalpro2_events where cast(concat(`jos_jcalpro2_events`.`year`,'-',`jos_jcalpro2_events`.`month`,'-',`jos_jcalpro2_events`.`day`) as date)>=CURDATE() and approved=1  ORDER BY CAST(CONCAT(`jos_jcalpro2_events`.`year`,'-',`jos_jcalpro2_events`.`month`,'-',`jos_jcalpro2_events`.`day`) AS DATE) ASC LIMIT 2", Connection.MyConnection))
                    {
                        int x = 0;
                        MyDataReader = MyCommand.ExecuteReader();
                        while (MyDataReader.Read())
                        {
                            event_list.Add(new Events(MyDataReader.GetString(0), MyDataReader.GetString(1), MyDataReader.GetInt32(2),MyDataReader.GetInt32(3)));
                            x++;
                        }
                        MyDataReader.Close();
                        //console.writeline("x "+x);
                        if (x==0)
                        {
                            event_grid1.Visibility=Visibility.Hidden;
                            event_grid2.Visibility = Visibility.Hidden;
                        }
                        else if (x==1)
                        {
                             event_grid2.Visibility = Visibility.Hidden;
                             ConfigEventGrid1();
                        }
                        else if (x==2)
                        {
                            ConfigEventGrid1();
                            ConfigEventGrid2();
                        }
                       
                        ConfigHolidaysCalender();
                    }
                }
                catch (Exception)
                {
                    //configure calender
                    calenderCount = 0;
                    configCalender(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    event_grid1.Visibility = Visibility.Hidden;
                    event_grid2.Visibility = Visibility.Hidden;
                }
            }
        }

        private void ConfigEventGrid1()
        {
            event_title_tBlock1.Text = HtmlRemoval.StripTagsCharArray(event_list[0].EventTitle.ToString());
                event_description_tBlock1.Text = HtmlRemoval.StripTagsCharArray(event_list[0].EventDis.ToString());
                event_day_lab1.Content = HtmlRemoval.StripTagsCharArray(event_list[0].DAY.ToString());

                DateTime eventDate_with_month = DateTime.Parse(Convert.ToDateTime(DateTime.Now.Year + "/" + event_list[0].MONTH + "/" + event_list[0].DAY).ToString("yyyy-MM-dd"));
                event_month_lab1.Content = eventDate_with_month.ToString("MMM").ToUpper();
        }

        private void ConfigEventGrid2()
        {
                event_title_tBlock2.Text = HtmlRemoval.StripTagsCharArray(event_list[1].EventTitle.ToString());
                event_description_tBlock2.Text = HtmlRemoval.StripTagsCharArray(event_list[1].EventDis.ToString());
                event_day_lab2.Content = HtmlRemoval.StripTagsCharArray(event_list[1].DAY.ToString());
                DateTime eventDate_with_month = DateTime.Parse(Convert.ToDateTime(DateTime.Now.Year + "/" + event_list[1].MONTH + "/" + event_list[1].DAY).ToString("yyyy-MM-dd"));
                event_month_lab2.Content = eventDate_with_month.ToString("MMM").ToUpper();
         
        }

        ////add holidays to calander
        void ConfigHolidaysCalender()
        {
          //  DateTime currentDate = DateTime.Parse(Convert.ToDateTime(currentYear + "/" + currentMonth + "/" + currentDay).ToString("yyyy-MM-dd"));

            using (OdbcCommand MyCommand = new OdbcCommand("select h_description,h_date,h_month from tms_holidays where h_year='" + DateTime.Now.Year + "' ", Connection.MyConnection))
            {
                OdbcDataReader MyDataReader = MyCommand.ExecuteReader();
                while (MyDataReader.Read())
                {       
                           holiday_list.Add(new Holidays(MyDataReader.GetString(0), MyDataReader.GetInt32(1), MyDataReader.GetInt32(2)));
                    //configHoliday(MyDataReader.GetInt32(1), MyDataReader.GetInt32(0), CalenderColorCode.back_color_holiday, DateTime.Now.AddDays(5));
                }
                MyDataReader.Close();
            }
            //configure calender
            calenderCount = 0;
            configCalender(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

       
    }
}
