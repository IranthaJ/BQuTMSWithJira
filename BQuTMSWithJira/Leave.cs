using System;

namespace BQuTMSWithJira
{
    class Leave
    {
        //help to display leave history table in leave request section
            private int _id = 0;
            private string _leavename = "";
            private string _sday = "";
            private string _eday = "";
            private string _status = "";
            private string _halffull = "";
            private string _comment = "";

            public int ID
            {
                set
                {
                    _id = value;
                }
                get
                {
                    return _id;
                }
            }

            public String LeaveName
            {
                set
                {
                    _leavename = value;
                }
                get
                {
                    return _leavename;
                }
            }

            public String StartDay
            {
                set
                {
                    _sday = value;
                }
                get
                {
                    return _sday;
                }
            }

            public String EndDay
            {
                set
                {
                    _eday = value;
                }
                get
                {
                    return _eday;
                }
            }

            public String Status
            {
                set
                {
                    _status = value;
                }
                get
                {
                    return _status;
                }
            }

            public String HalfFull
            {
                set
                {
                    _halffull = value;
                }
                get
                {
                    if (_halffull == "1")
                    {
                        _halffull = "full day";
                    }
                    else if (_halffull == "0")
                    {
                        _halffull = "half day";
                    }
                    return _halffull;
                }
            }

            public String Comment
            {
                set
                {
                    _comment = value;
                }
                get
                {
                    return _comment;
                }
            }

            public Leave(int id, string leavename, string sday, string eday, string status, string halffull, string comment)
            {
                _id = id;
                _leavename = leavename;
                _sday = sday;
                _eday = eday;
                _status = status;
                _halffull = halffull;
                _comment = comment;
            }
        }
}
