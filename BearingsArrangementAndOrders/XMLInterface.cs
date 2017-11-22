using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BearingsArrangementAndOrders
{
    class XMLInterface
    {

        private void SolutionOutput(List<BearingsArrangementOrder> paramBearingArrOrders)

        {
            //create the serialiser to create the xml
            XmlSerializer serialiser = new XmlSerializer(typeof(List<T>));

            // Create the TextWriter for the serialiser to use
            TextWriter FileStream = new StreamWriter(@"C:\output.xml");

            //write to the file
            serialiser.Serialize(FileStream, paramBearingArrOrders);

            // Close the file
            FileStream.Close();
            
        }

    }
}
