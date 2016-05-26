using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace BQuTMSWithJira
{
     //get user configuration details from user/Application Data/BQuTMSWithJira/userdetails.xml file
    class UserConfig
    {
        public static List<string> configlist = new List<string>();

        public static void ConfigureConfig()
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(Util.GetUserDataPath() + "\\userdetails.xml");
                XmlNodeType type;
                while (reader.Read())
                {
                    type = reader.NodeType;
                    if (type == XmlNodeType.Element)
                    {
                        if (reader.Name == "user")
                        {
                            reader.Read();
                            configlist.Add(reader.Value);//configlist[0]
                        }
                        if (reader.Name == "password")
                        {
                            reader.Read();
                            configlist.Add(reader.Value);//configlist[1]
                        }
                        if (reader.Name == "companyname")
                        {
                            reader.Read();
                            configlist.Add(reader.Value);//configlist[2]
                        }
                        if (reader.Name == "sip")
                        {
                            reader.Read();
                            configlist.Add(reader.Value);//configlist[3]
                        }
                        if (reader.Name == "sport")
                        {
                            reader.Read();
                            configlist.Add(reader.Value);//configlist[4]
                        }
                        if (reader.Name == "susername")
                        {
                            reader.Read();
                            configlist.Add(reader.Value);//configlist[5]
                        }
                        if (reader.Name == "spassword")
                        {
                            reader.Read();
                            configlist.Add(reader.Value);//configlist[6]
                        }
                        if (reader.Name == "dbname")
                        {
                            reader.Read();
                            configlist.Add(reader.Value);//configlist[7]
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Please update settings", "BQuTMSWithJira Empty Settings", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
