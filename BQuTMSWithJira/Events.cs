using System;

namespace BQuTMSWithJira
{
    //set event to listview table
    class Events
    {
        private string event_title = "";
        private string event_dis = "";
        private int day=0;
        private int month=0;

        public String EventDis
        {
            set
            {
                event_dis = value;
            }
            get
            {
                return event_dis;
            }
        }

        public String EventTitle
        {
            set
            {
                event_title = value;
            }
            get
            {
                return event_title;
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

        public Events(string _eventtitle, string _eventdis, int _day,int _month)
        {
            event_title = _eventtitle;
            event_dis = _eventdis;
            day = _day;
            month = _month;
        }
    }
}
