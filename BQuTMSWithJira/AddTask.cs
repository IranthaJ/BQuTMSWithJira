using System;

namespace BQuTMSWithJira
{
    class AddTask
    {
        private int _id = 0;
        private string _projectname = "";
        private string _categoryname = "";
        private string _name = "";
        private string _duedate = "";

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

        public String Name
        {
            set
            {
                _name = value;
            }
            get
            {
                return _name;
            }
        }

        public String DueDate
        {
            set
            {
                _duedate = value;
            }
            get
            {
                return _duedate;
            }
        }

        public AddTask(int id, string projectname, string categoryname, string name, string duedate)
        {
            _id = id;
            _projectname = projectname;
            _categoryname = categoryname;
            _name = name;
            _duedate = duedate;
        }
    }
}
