using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace BearingsArrangementAndOrders
{
    class ExcelLoader
    {

        const string sBearingArrangementOrderListName = "Задание на комплектовку";
        const int iBearingArrangementOrderFirstRow = 2;
        const int iBearingArrangementOrderFirstCol = 1;

        const string sBearingTypesListName = "Данные ПШ";
        const int iBearingTypesFirstRow = 2;
        const int iBearingTypesFirstCol = 1;

        const int iItemsFirstRow = 3;
        const int iItemsFirstCol = 1;

        private Microsoft.Office.Interop.Excel.Application pObjExcel;
        public Microsoft.Office.Interop.Excel.Application ObjExcel
        {
            get { return pObjExcel; }
        }

        private Microsoft.Office.Interop.Excel.Workbook pObjWorkBook;
        public Microsoft.Office.Interop.Excel.Workbook ObjWorkBook
        {
            get { return pObjWorkBook; }
        }

        ~ExcelLoader()
        {
            object misValue = System.Reflection.Missing.Value;

            pObjWorkBook.Close(false);

            pObjExcel.Quit();
        }

        public ExcelLoader(string paramFileName)
        {
            pObjExcel = new Microsoft.Office.Interop.Excel.Application();
            pObjWorkBook = pObjExcel.Workbooks.Open(paramFileName, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
        }

        static string NullToString(object Value)
        {

            return Value == null ? "" : Value.ToString();

        }

        private void AddItemTypeToBearingType(BearingType paramBearingType, string sParamItemDescr, string sParamItemCount, string sParamItemType)
        {
            if ((sParamItemDescr != "") && (sParamItemCount != ""))
            {
                BearingItemType curItemType = new BearingItemType();
                curItemType.Description = sParamItemDescr;
                curItemType.Type = sParamItemType;
                paramBearingType.ValidBearingItemTypes.Add(sParamItemType, curItemType);
                paramBearingType.BearingItemsCount.Add(sParamItemType, Convert.ToInt32(sParamItemCount));
            }

        }

        public void LoadBearingTypesFromExcel(List<BearingType> paramBearingTypes)
        {

            Microsoft.Office.Interop.Excel.Worksheet Worksheet;
            Worksheet = pObjWorkBook.Sheets[sBearingTypesListName];

            int iExcelRowNumber = iBearingTypesFirstRow;
            string sDescription, sItem01Descr, sItem01Count,
                sItem02Descr, sItem02Count,
                sItem92Descr, sItem92Count,
                sItem52Descr, sItem52Count,
                sItem04Descr, sItem04Count,
                sR1Nom, sR1Min, sR1Max;

            do
            {
                sDescription = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol].Value);
                if (sDescription != "")
                {
                    BearingType CurBearingType = new BearingType();

                    CurBearingType.Description = sDescription;

                    //01
                    sItem01Descr = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 1].Value);
                    sItem01Count = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 2].Value);
                    AddItemTypeToBearingType(CurBearingType, sItem01Descr, sItem01Count, "01");

                    //02
                    sItem02Descr = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 3].Value);
                    sItem02Count = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 4].Value);
                    AddItemTypeToBearingType(CurBearingType, sItem02Descr, sItem02Count, "02");

                    //92
                    sItem92Descr = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 5].Value);
                    sItem92Count = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 6].Value);
                    AddItemTypeToBearingType(CurBearingType, sItem92Descr, sItem92Count, "92");

                    //52
                    sItem52Descr = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 7].Value);
                    sItem52Count = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 8].Value);
                    AddItemTypeToBearingType(CurBearingType, sItem52Descr, sItem52Count, "52");

                    //04
                    sItem04Descr = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 9].Value);
                    sItem04Count = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 10].Value);
                    AddItemTypeToBearingType(CurBearingType, sItem04Descr, sItem04Count, "04");

                    sR1Nom = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 11].Value);
                    CurBearingType.Rad1Nominal = Convert.ToDouble(sR1Nom);

                    sR1Min = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 12].Value);
                    CurBearingType.Rad1Min = Convert.ToDouble(sR1Min);

                    sR1Max = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 13].Value);
                    CurBearingType.Rad1Max = Convert.ToDouble(sR1Max);

                    paramBearingTypes.Add(CurBearingType);
                }
                iExcelRowNumber++;
            }
            while (sDescription != "");
        }

        public void LoadBearingArrangementOrderFromExcel(List<BearingsArrangementOrder> paramArrOrders, List<BearingType> paramBearingTypes)
        {

            Microsoft.Office.Interop.Excel.Worksheet Worksheet;
            Worksheet = pObjWorkBook.Sheets[sBearingArrangementOrderListName];

            int iExcelRowNumber = iBearingArrangementOrderFirstRow;
            string sDescription, sCount;

            do
            {
                sDescription = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingArrangementOrderFirstCol].Value);
                if (sDescription != "")
                {

                    var BearingType = from CurBearingType in paramBearingTypes
                                      where CurBearingType.Description == sDescription
                                      select CurBearingType;

                    if (BearingType.Count() > 0)
                    {
                        BearingsArrangementOrder curArrOrder = new BearingsArrangementOrder();
                        curArrOrder.BearingType = BearingType.First();

                        sCount = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingArrangementOrderFirstCol + 1].Value);
                        curArrOrder.Count = Convert.ToInt32(sCount);

                        paramArrOrders.Add(curArrOrder);
                    }
                    else
                    {
                        //todo не найден тип подшипника
                    }
                }
                iExcelRowNumber++;
            }
            while (sDescription != "");

        }

        public void LoadBearingItemsGroupsFromExcel(List<BearingItemsGroup> paramItemsGroups, List<BearingItemType> paramBearingItemTypes)
        {

            int iExcelRowNumber = iItemsFirstRow;
            double dSize;
            string sDescription, sCount, sSize, sType;
            BearingItemType curType;
            BearingItemsGroup curGroup;

            foreach (Worksheet Worksheet in pObjWorkBook.Sheets)
            {
                if (Worksheet.Name.Length > 6)
                {

                    if (Worksheet.Name.Substring(0, 6) == "Деталь")
                    {
                        sDescription = NullToString(Worksheet.Cells[1, 1].Value);
                        if (sDescription != "")
                        {
                            sType = NullToString(Worksheet.Cells[1, 2].Value);

                            var ItemType = from CurItemType in paramBearingItemTypes
                                           where CurItemType.Description == sDescription
                                           select CurItemType;

                            if (ItemType.Count() > 0)
                            {
                                curType = ItemType.First();
                            }
                            else
                            {
                                curType = new BearingItemType { Description = sDescription, Type = sType };
                                paramBearingItemTypes.Add(curType);
                            }

                            iExcelRowNumber = iItemsFirstRow;

                            do
                            {
                                sSize = NullToString(Worksheet.Cells[iExcelRowNumber, iItemsFirstCol].Value);
                                if (sSize != "")
                                {
                                    dSize = Convert.ToDouble(sSize);
                                    var ItemGroups = from curItemGroup in paramItemsGroups
                                                     where (curItemGroup.ItemType == curType) && (curItemGroup.Size1 == dSize)
                                                     select curItemGroup;
                                    if (ItemGroups.Count() > 0)
                                    {
                                        curGroup = ItemGroups.First();
                                    }
                                    else
                                    {
                                        curGroup = new BearingItemsGroup();
                                        curGroup.ItemType = curType;
                                        curGroup.Size1 = dSize;
                                        paramItemsGroups.Add(curGroup);
                                    }

                                    sCount = NullToString(Worksheet.Cells[iExcelRowNumber, iItemsFirstCol + 1].Value);
                                    curGroup.ItemCount += Convert.ToInt32(sCount);
                                }
                                iExcelRowNumber++;
                            }
                            while (sSize != "");
                        }

                    }
                }


            }

        }
    }
}
