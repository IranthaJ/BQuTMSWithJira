using System;

namespace BQuTMSWithJira
{
    class AllLeaves
    {
        //help from this code to insert data to leaverequest all leave listview table
        private string _leavename = "";
        private int _no_of_allleaves = 0;
        private double _got_leaves = 0;
        private double _remine_leaves = 0;

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

        public int No_of_Allleaves
        {
            set
            {
                _no_of_allleaves = value;
            }
            get
            {
                return _no_of_allleaves;
            }
        }

        public double Got_Leaves
        {
            set
            {
                _got_leaves = value;
            }
            get
            {
                return _got_leaves;
            }
        }

        public double RemineLeaves
        {
            set
            {
                _remine_leaves = value;
            }
            get
            {
                return _remine_leaves;
            }
        }

        public AllLeaves(string leavename, int no_of_allleaves, double got_leaves, double remine_leaves)
        {
            _leavename = leavename;
            _no_of_allleaves = no_of_allleaves;
            _got_leaves = got_leaves;
            _remine_leaves = remine_leaves;
        }
    }
}
