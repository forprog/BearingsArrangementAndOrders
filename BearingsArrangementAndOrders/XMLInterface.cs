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

        public void ArrOrdersResultOutput(List<BearingsArrangementOrder> paramBearingArrOrders, List<BearingItemsGroup> paramBearingItemsGroups, List<BearingItemsGroup> paramGrindingOrders)
        {

            SolutionOutput(paramBearingArrOrders);
            //NotCompletedBearingsOutput(paramBearingArrOrders);
            UsedItemsOutput(paramBearingItemsGroups);
            GrindingOrdersOutput(paramGrindingOrders);
        }

        private void GrindingOrdersOutput(List<BearingItemsGroup> paramGrindingOrders)
        {
            XmlSerializer serialiser = new XmlSerializer(typeof(List<BearingItemsGroup>));
            TextWriter FileStream = new StreamWriter(@"D:\GrindingOrders.xml");

            serialiser.Serialize(FileStream, paramGrindingOrders);

            FileStream.Close();
        }

        private void UsedItemsOutput(List<BearingItemsGroup> paramBearingItemsGroups)
        {

            XmlSerializer serialiser = new XmlSerializer(typeof(List<BearingItemsGroup>));
            TextWriter FileStream = new StreamWriter(@"D:\UsedItems.xml");

            serialiser.Serialize(FileStream, paramBearingItemsGroups);

            FileStream.Close();

        }

        private void SolutionOutput(List<BearingsArrangementOrder> paramBearingArrOrders)

        {
            ////create the serialiser to create the xml
            XmlSerializer serialiser = new XmlSerializer(typeof(List<BearingsArrangementOrder>));

            //// Create the TextWriter for the serialiser to use
            TextWriter FileStream = new StreamWriter(@"D:\Solution.xml");

            ////write to the file
            serialiser.Serialize(FileStream, paramBearingArrOrders);            

            // Close the file
            FileStream.Close();
            
        }

    }
}
