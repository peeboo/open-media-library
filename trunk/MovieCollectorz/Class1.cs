using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OMLSDK;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;

namespace MovieCollectorz
{
    public class Class1 : IDataImporter
    {
        OMLDataSet ods;
        TextReader tr = null;

        public Class1()
        {
            ods = new OMLDataSet();
        }
        public bool Load(string filename)
        {
            try
            {
                tr = new StreamReader(filename);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filename);

            XmlNodeList nodeList = xDoc.SelectNodes("//movie");
            DataRow row = ods.NewRow();
            ArrayList columnNames = ods.GetColumnNames();
            foreach (XmlNode node in nodeList)
            {
                foreach (string colName in columnNames)
                {
                    XmlNode dataNode = node.SelectSingleNode(colName);
                    if (dataNode != null)
                        row[colName] = dataNode.InnerText;
                }
                if (OMLPlugin.ValidateRow(ods, row))
                    ods.AddRow(row);
            }

            return true;
        }

        public OMLDataSet GetOmlDataSet()
        {
            return ods;
        }
        public string GetName()
        {
            return "MovieCollectorzPlugin";
        }
    }
}
