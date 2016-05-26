using System;
using Microsoft.Data.Odbc;

namespace BQuTMSWithJira
{
    class Department
    {
        //return deparment id
        public int GetDepartment(int empid)
        {
            using (OdbcCommand MyCommand = new OdbcCommand("select cb_departmentid from jos_comprofiler where user_id='" + empid + "'", Connection.MyConnection))
            {
                return Convert.ToInt32(MyCommand.ExecuteScalar());
            }
        }

        //return deparmant head id
        public int GetDepartmentHead(int depid)
        {
            using (OdbcCommand MyCommand = new OdbcCommand("select department_head from tms_department where department_id='" + depid + "'", Connection.MyConnection))
            {
                return Convert.ToInt32(MyCommand.ExecuteScalar());
            }
        }
    }
}
