using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace BearingsArrangementAndOrders
{
    class XMLInterface
    {

        //public void ArrOrdersResultOutput(List<BearingsArrangementOrder> paramBearingArrOrders, List<BearingItemsGroup> paramBearingItemsGroups, List<BearingItemsGroup> paramGrindingOrders)
        //{

        //    SolutionOutput(paramBearingArrOrders);
        //    //NotCompletedBearingsOutput(paramBearingArrOrders);
        //    UsedItemsOutput(paramBearingItemsGroups);
        //    GrindingOrdersOutput(paramGrindingOrders);
        //}

        public void GrindingOrdersOutput(List<BearingItemsGroup> paramGrindingOrders, string sParamFileName)
        {
            XmlSerializer serialiser = new XmlSerializer(typeof(List<BearingItemsGroup>));
            TextWriter FileStream = new StreamWriter(sParamFileName);

            serialiser.Serialize(FileStream, paramGrindingOrders);

            FileStream.Close();
        }

        public void UsedItemsOutput(List<BearingItemsGroup> paramBearingItemsGroups, string sParamFileName)
        {

            XmlSerializer serialiser = new XmlSerializer(typeof(List<BearingItemsGroup>));
            TextWriter FileStream = new StreamWriter(sParamFileName);

            serialiser.Serialize(FileStream, paramBearingItemsGroups);

            FileStream.Close();

        }

        public void SolutionOutput(List<BearingsArrangementOrder> paramBearingArrOrders, string sParamFileName)

        {
            ////create the serialiser to create the xml
            XmlSerializer serialiser = new XmlSerializer(typeof(List<BearingsArrangementOrder>));

            //// Create the TextWriter for the serialiser to use
            TextWriter FileStream = new StreamWriter(sParamFileName);

            ////write to the file
            serialiser.Serialize(FileStream, paramBearingArrOrders);

            // Close the file
            FileStream.Close();

        }



        private void AddItemTypeToBearingType(BearingType paramBearingType, string sParamItemDescr, string sParamItemID, string sParamItemType, string sParamChar, string sParamCharID, string sParamItemCount, string sParamSize1Min, string sParamSize1Max, int iParamMinOrderCount, int iParamSize1MinDifference, List<BearingItemType> paramItemTypes)
        {
            if ((sParamItemDescr != "") && (sParamItemCount != ""))
            {

                var curItemType = FindAddBearingItemType(paramItemTypes, sParamItemDescr, sParamItemType, sParamItemID, sParamChar, sParamCharID, iParamMinOrderCount, iParamSize1MinDifference);

                paramBearingType.ValidBearingItemTypes.Add(curItemType.Type, curItemType);

                int iItemCount = Convert.ToInt32(sParamItemCount == "" ? "0" : sParamItemCount);
                int iParamSize1Min = Convert.ToInt32(sParamSize1Min == "" ? "0" : sParamSize1Min);
                int iParamSize1Max = Convert.ToInt32(sParamSize1Max == "" ? "0" : sParamSize1Max);

                paramBearingType.BearingItemsCount.Add(curItemType.Type, iItemCount);
                paramBearingType.ValidBearingItemsSize1Min.Add(curItemType.Type, iParamSize1Min);
                paramBearingType.ValidBearingItemsSize1Max.Add(curItemType.Type, iParamSize1Max);
            }

        }

        internal void LoadBearingArrangementOrders(List<BearingsArrangementOrder> paramBearingArrOrders, List<BearingType> paramBearingTypes, string sParamFileName)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(sParamFileName);

            XmlNodeList elemList = doc.GetElementsByTagName("row");
            for (int i = 0; i < elemList.Count; i++)
            {
                string sBearingDescr = elemList[i].ChildNodes[0].InnerText;
                string sCount = elemList[i].ChildNodes[1].InnerText;
                string sBearingID = elemList[i].ChildNodes[2].InnerText;
                int iCount = Convert.ToInt32(sCount == "" ? "0" : sCount);

                if (iCount > 0)
                {
                    BearingType curBearingType = FindBearingType(paramBearingTypes, sBearingID);

                    BearingsArrangementOrder curArrOrder = new BearingsArrangementOrder()
                    {
                        BearingType = curBearingType,
                        OrderCount = iCount
                    };
                    paramBearingArrOrders.Add(curArrOrder);

                }
            }
        }

        internal void LoadItemStocks(List<BearingItemsGroup> paramItemsGroups, List<BearingItemType> paramBearingItemTypes, string sParamFileName)
        {
            BearingItemsGroup curGroup;

            XmlDocument doc = new XmlDocument();
            doc.Load(sParamFileName);

            XmlNodeList elemList = doc.GetElementsByTagName("row");
            for (int i = 0; i < elemList.Count; i++)
            {
                string sItemId = elemList[i].ChildNodes[1].InnerText;
                string sCharacteristicID = elemList[i].ChildNodes[3].InnerText;
                string sSize1 = elemList[i].ChildNodes[4].InnerText.Replace(".", ",");
                string sCount = elemList[i].ChildNodes[5].InnerText;
                int iCount = Convert.ToInt32(sCount == "" ? "0" : sCount);
                double dSize1 = Convert.ToDouble(sSize1 == "" ? "0" : sSize1);

                if (iCount > 0)
                {

                    var curType = FindBearingItemType(paramBearingItemTypes,sItemId,sCharacteristicID);

                    var ItemGroups = from curItemGroup in paramItemsGroups
                                     where (curItemGroup.ItemType == curType) && (curItemGroup.Size1 == dSize1)
                                     select curItemGroup;
                    if (ItemGroups.Count() > 0)
                    {
                        curGroup = ItemGroups.First();
                    }
                    else
                    {
                        curGroup = new BearingItemsGroup()
                        {
                            ItemType = curType,
                            Size1 = dSize1
                        };
                        paramItemsGroups.Add(curGroup);
                    }

                    curGroup.ItemCount += iCount;

                }
            }
        }

        private BearingItemType FindBearingItemType(List<BearingItemType> paramItemTypes, string paramID, string paramCharacteristicID)
        {
            BearingItemType resultBearingItemType = null;

            var curFoundItemTypes = from qItemType in paramItemTypes
                                    where (qItemType.ID == paramID) && (qItemType.CharachteristicID == paramCharacteristicID)
                                    select qItemType;

            if (curFoundItemTypes.Count() > 0)
            {
                resultBearingItemType = curFoundItemTypes.First();
            }
            return resultBearingItemType;

        }

        private BearingItemType FindAddBearingItemType(List<BearingItemType> paramItemTypes, string paramDescription, string paramType, string paramID, string paramCharacteristic, string paramCharacteristicID, int paramMinOrderCount, int paramSize1MinDifference)
        {
            BearingItemType resultBearingItemType = FindBearingItemType(paramItemTypes, paramID, paramCharacteristicID);

            if (resultBearingItemType == null)
            {
                resultBearingItemType = new BearingItemType()
                {
                    Description = paramDescription,
                    Type = paramType,
                    ID = paramID,
                    CharachteristicDescription = paramCharacteristic,
                    CharachteristicID = paramCharacteristicID,
                    MinOrderCount = paramMinOrderCount,
                    Size1MinDifference = paramSize1MinDifference
                };
                paramItemTypes.Add(resultBearingItemType);
            }

            return resultBearingItemType;

        }

        public void LoadTypes(List<BearingType> paramBearingTypes, List<BearingItemType> paramItemTypes, string sFileName)
        {
            //Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            doc.Load(sFileName);

            XmlNodeList elemList = doc.GetElementsByTagName("row");
            for (int i = 0; i < elemList.Count; i++)
            {
                string sBearingDescr = elemList[i].ChildNodes[0].InnerText;
                string sBearingID = elemList[i].ChildNodes[1].InnerText;
                string sItemDescr = elemList[i].ChildNodes[2].InnerText;
                string sItemID = elemList[i].ChildNodes[3].InnerText;
                string sCharDescr = elemList[i].ChildNodes[4].InnerText;
                string sCharID = elemList[i].ChildNodes[5].InnerText;
                string sItemCount = elemList[i].ChildNodes[6].InnerText;
                string sSize1Min = elemList[i].ChildNodes[7].InnerText.Replace(".", ",");
                string sSize1Max = elemList[i].ChildNodes[8].InnerText.Replace(".", ",");
                string sMinArrangeCount = elemList[i].ChildNodes[9].InnerText;
                string sRz1Max = elemList[i].ChildNodes[10].InnerText.Replace(".", ",");
                string sRz1Min = elemList[i].ChildNodes[11].InnerText.Replace(".", ",");
                string sRz1Nom = elemList[i].ChildNodes[12].InnerText.Replace(".", ",");
                string sItemType = elemList[i].ChildNodes[13].InnerText;
                string sMinOrderCount = elemList[i].ChildNodes[14].InnerText;
                string sMinR1Difference = elemList[i].ChildNodes[15].InnerText;

                int iMinOrderCount = Convert.ToInt32(sMinOrderCount == "" ? "0" : sMinOrderCount);
                int iMinR1Difference = Convert.ToInt32(sMinR1Difference == "" ? "0" : sMinR1Difference);

                BearingType curBearingType = FindAddBearingType(paramBearingTypes, sBearingDescr, sBearingID, sRz1Nom, sRz1Min, sRz1Max, sMinArrangeCount);
                AddItemTypeToBearingType(curBearingType, sItemDescr, sItemID, sItemType, sCharDescr, sCharID, sItemCount, sSize1Min, sSize1Max, iMinOrderCount, iMinR1Difference, paramItemTypes);

            }
        }

        private BearingType FindBearingType(List<BearingType> paramBearingTypes, string sParamID)
        {
            BearingType resultBearingType = null;

            var curFoundBearingTypes = from qBearingType in paramBearingTypes
                                       where (qBearingType.ID == sParamID)
                                       select qBearingType;

            if (curFoundBearingTypes.Count() > 0)
            {
                resultBearingType = curFoundBearingTypes.First();
            }

            return resultBearingType;

        }

        private BearingType FindAddBearingType(List<BearingType> paramBearingTypes, string sParamDescription, string sParamID, string sParamR1Nom, string sParamR1Min, string sParamR1Max, string sParamMinArrangeCount)
        {
            BearingType resultBearingType = FindBearingType(paramBearingTypes, sParamID);

            if (resultBearingType == null)
            {
                double dR1Nom = Convert.ToDouble(sParamR1Nom == "" ? "0" : sParamR1Nom);
                double dR1Min = Convert.ToDouble(sParamR1Min == "" ? "0" : sParamR1Min);
                double dR1Max = Convert.ToDouble(sParamR1Max == "" ? "0" : sParamR1Max);
                int iMinArrangeCount = Convert.ToInt32(sParamMinArrangeCount == "" ? "0" : sParamMinArrangeCount);

                resultBearingType = new BearingType()
                {
                    Description = sParamDescription,
                    ID = sParamID,
                    MinArrangeCount = iMinArrangeCount,
                    Rad1Max = dR1Max,
                    Rad1Min = dR1Min,
                    Rad1Nominal = dR1Nom
                };

                paramBearingTypes.Add(resultBearingType);
            }

            return resultBearingType;

        }
    }
}
