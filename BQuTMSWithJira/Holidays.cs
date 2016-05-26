using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BQuTMSWithJira
{
        //set event to listview table
        class Holidays
        {
            private string description = "";
            private int day = 0;
            private int month = 0;

            public String DESCRIPTION
            {
                set
                {
                    description = value;
                }
                get
                {
                    return description;
                }
            }

            public int DAY
            {
                set
                {
                    day = value;
                }
                get
                {
                    return day;
                }
            }

            public int MONTH
            {
                set
                {
                    month = value;
                }
                get
                {
                    return month;
                }
            }

            public Holidays(string _description, int _day, int _month)
            {
                description = _description;
                day = _day;
                month = _month;
            }
        }
    }

