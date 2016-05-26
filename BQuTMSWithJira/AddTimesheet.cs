using System;

namespace BQuTMSWithJira
{
    class AddTimesheet
    {
        //help from this code to insert data to Timesheet listview table
        private int _id = 0;
        private string _worktime = "";
        private string _projectname = "";
        private string _categoryname = "";
        private string _note = "";
        private string _status = "";

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

        public String WTime
        {
            set
            {
                _worktime = value;
            }
            get
            {
                return _worktime;
            }
        }

        public String ProjectName
        {
            set
            {
                _projectname = value;
            }
            get
            {
                return _projectname;
            }
        }

        public String CategoryName
        {
            set
            {
                _categoryname = value;
            }
            get
            {
                return _categoryname;
            }
        }

        public String Note
        {
            set
            {
                _note = value;
            }
            get
            {
                return _note;
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

        public AddTimesheet(int id, string wtime, string projectname, string categoryname, string note, string status)
        {
            _id = id;
            _worktime = wtime;
            _projectname = projectname;
            _categoryname = categoryname;
            _note = note;
            _status = status;
        }
    }
}
