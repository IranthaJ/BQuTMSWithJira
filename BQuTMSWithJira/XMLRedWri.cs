using System;
using System.Text;
using System.Xml;

namespace BQuTMSWithJira
{
    class XMLRedWri
    {
        //write  user/Application Data/BQuTMSWithJira/tmsdata.xml file to time sheet details when start timer
        public void TimeSheetStart(string sedate, string seproject, string scategory, string note, string setime, string project_id, string issue_id, string category_id)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(Util.GetUserDataPath() + "\\tmsdata.xml", Encoding.UTF8);
                writer.WriteStartDocument();
                writer.WriteStartElement("TimeSheet");

                writer.WriteStartElement("date");
                writer.WriteString(sedate);
                writer.WriteEndElement();

                writer.WriteStartElement("project");
                writer.WriteString(seproject);
                writer.WriteEndElement();

                writer.WriteStartElement("category");
                writer.WriteString(scategory);
                writer.WriteEndElement();

                writer.WriteStartElement("note");
                writer.WriteString(note);
                writer.WriteEndElement();

                writer.WriteStartElement("stime");
                writer.WriteString(setime);
                writer.WriteEndElement();

                writer.WriteStartElement("project_id");
                writer.WriteString(project_id);
                writer.WriteEndElement();

                writer.WriteStartElement("issue_id");
                writer.WriteString(issue_id);
                writer.WriteEndElement();

                writer.WriteStartElement("category_id");
                writer.WriteString(category_id);
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Source);
            }
        }

        //read tmsdata.xml file
        public String FileRead(string name)
        {
            string txt = string.Empty;
            try
            {
                XmlTextReader reader = new XmlTextReader(Util.GetUserDataPath() + "\\tmsdata.xml");
                XmlNodeType type;
                while (reader.Read())
                {
                    type = reader.NodeType;
                    if (type == XmlNodeType.Element)
                    {
                        if (reader.Name == name)
                        {
                            reader.Read();
                            txt = reader.Value;
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex + "");
            }
            return txt;
        }

        //update single value in tmsdata.xml file
        public void UpdateSingleValue(string txt, string mainnode, string node)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(Util.GetUserDataPath() + "\\tmsdata.xml", Encoding.UTF8);
                writer.WriteStartDocument();
                writer.WriteStartElement(mainnode);

                writer.WriteStartElement(node);
                writer.WriteString(txt);
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Source);
            }
        }
    }
}
