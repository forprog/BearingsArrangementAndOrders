using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace BearingsArrangementAndOrders
{
    class ExcelInterface : IDisposable
    {

        const string sBearingArrangementOrderListName = "Задание на комплектовку";
        const int iBearingArrangementOrderFirstRow = 2;
        const int iBearingArrangementOrderFirstCol = 1;

        const string sBearingTypesListName = "Данные ПШ";
        const int iBearingTypesFirstRow = 2;
        const int iBearingTypesFirstCol = 1;

        const string sBearingItemsTypesListName = "Данные деталей";
        const int iBearingItemsTypesFirstRow = 2;
        const int iBearingItemsTypesFirstCol = 1;

        const int iItemsFirstRow = 3;
        const int iItemsFirstCol = 1;

        bool disposed = false;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                object misValue = System.Reflection.Missing.Value;

                //закрытие Excel правильное
                //Marshal.ReleaseComObject(pObjWorkBook);
                //try
                //{
                //    while (Marshal.ReleaseComObject(pObjWorkBook) > 0) ;
                //}
                //catch { }
                //finally
                //{
                //    pObjWorkBook = null;
                //};

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                pObjWorkBook.Close(false, misValue, misValue);
                Marshal.FinalReleaseComObject(pObjWorkBook);
                pObjWorkBook = null;

                pObjExcel.Quit();
                Marshal.FinalReleaseComObject(pObjExcel);
                pObjExcel = null;

                //try
                //{
                //    while (Marshal.ReleaseComObject(pObjExcel) > 0) ;
                //}
                //catch { }
                //finally
                //{
                //}
            }

            disposed = true;
        }


        public ExcelInterface(string paramFileName)
        {

            pObjExcel = new Microsoft.Office.Interop.Excel.Application();
            var WorkBooks = pObjExcel.Workbooks;
            pObjWorkBook = WorkBooks.Open(paramFileName, 0, false, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);

            Marshal.FinalReleaseComObject(WorkBooks);
            WorkBooks = null;

        }

        static string NullToString(object Value)
        {

            return Value == null ? "" : Value.ToString();

        }

        static double NullToDouble(object Value)
        {

            return Value == null ? 0 : Convert.ToDouble(NullToString(Value.ToString()));

        }
        private void AddItemTypeToBearingType(BearingType paramBearingType, string sParamItemDescr, string sParamCharID, string sParamItemCount, string sParamSize1Min, string sParamSize1Max, List<BearingItemType> paramItemTypes)
        {
            if ((sParamItemDescr != "") && (sParamItemCount != ""))
            {
                var curFoundItemTypes = from qItemType in paramItemTypes
                                        where (qItemType.Description == sParamItemDescr) && (qItemType.CharachteristicID == sParamCharID)
                                        select qItemType;
                BearingItemType curItemType = curFoundItemTypes.First();

                //todo А если не нашли тип детали???

                paramBearingType.ValidBearingItemTypes.Add(curItemType.Type, curItemType);
                paramBearingType.BearingItemsCount.Add(curItemType.Type, Convert.ToInt32(sParamItemCount));
                paramBearingType.ValidBearingItemsSize1Min.Add(curItemType.Type, Convert.ToInt32(sParamSize1Min));
                paramBearingType.ValidBearingItemsSize1Max.Add(curItemType.Type, Convert.ToInt32(sParamSize1Max));
            }

        }

        public void LoadItemTypesFromExcel(List<BearingItemType> paramItemTypes)
        {
            Microsoft.Office.Interop.Excel.Worksheet Worksheet;
            var Worksheets = pObjWorkBook.Sheets;
            Worksheet = Worksheets[sBearingItemsTypesListName];

            int iExcelRowNumber = iBearingItemsTypesFirstRow;
            string sDescription;

            do
            {
                sDescription = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol].Value);
                if (sDescription != "")
                {
                    BearingItemType CurItemType = new BearingItemType()
                    {
                        Description = sDescription,
                        Type = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 2].Value),
                        ID = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol].Value),
                        CharachteristicDescription = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 1].Value),
                        CharachteristicID = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 1].Value),
                        //CurItemType.Size1Min = NullToDouble(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 2].Value);
                        //CurItemType.Size1Max = NullToDouble(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 3].Value);
                        MinOrderCount = NullToDouble(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 5].Value),
                        Size1MinDifference = NullToDouble(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 6].Value)
                    };
                    paramItemTypes.Add(CurItemType);
                }
                iExcelRowNumber++;
            }
            while (sDescription != "");

            Marshal.FinalReleaseComObject(Worksheet);
            Worksheet = null;

            Marshal.FinalReleaseComObject(Worksheets);
            Worksheets = null;

        }


        public void LoadBearingTypesFromExcel(List<BearingType> paramBearingTypes, List<BearingItemType> paramItemTypes)
        {

            Microsoft.Office.Interop.Excel.Worksheet Worksheet;
            var Worksheets = pObjWorkBook.Sheets;
            Worksheet = Worksheets[sBearingTypesListName];

            int iExcelRowNumber = iBearingTypesFirstRow;
            string sDescription, sItem01Descr, sItem01Count, sItem01Size1Max, sItem01Size1Min, sItem01Characht,
                sItem02Descr, sItem02Count, sItem02Size1Max, sItem02Size1Min, sItem02Characht,
                sItem92Descr, sItem92Count, sItem92Size1Max, sItem92Size1Min, sItem92Characht,
                sItem52Descr, sItem52Count, sItem52Size1Max, sItem52Size1Min, sItem52Characht,
                sItem04Descr, sItem04Count, sItem04Size1Max, sItem04Size1Min, sItem04Characht,
                sR1Nom, sR1Min, sR1Max, sMinArrangeCount;

            do
            {
                sDescription = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol].Value);
                if (sDescription != "")
                {
                    BearingType CurBearingType = new BearingType()
                    {
                        Description = sDescription,
                        ID = sDescription
                    };

                    //01
                    sItem01Descr =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 1].Value);
                    sItem01Characht =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 2].Value);
                    sItem01Count =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 3].Value);
                    sItem01Size1Min =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 4].Value);
                    sItem01Size1Max =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 5].Value);
                    AddItemTypeToBearingType(CurBearingType, sItem01Descr, sItem01Characht, sItem01Count, sItem01Size1Min, sItem01Size1Max, paramItemTypes);

                    //02
                    sItem02Descr =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 6].Value);
                    sItem02Characht =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 7].Value);
                    sItem02Count =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 8].Value);
                    sItem02Size1Min =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 9].Value);
                    sItem02Size1Max =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 10].Value);
                    
                    AddItemTypeToBearingType(CurBearingType, sItem02Descr, sItem02Characht, sItem02Count, sItem02Size1Min, sItem02Size1Max, paramItemTypes);

                    //92
                    sItem92Descr =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 11].Value);
                    sItem92Characht =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 12].Value);
                    sItem92Count =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 13].Value);
                    sItem92Size1Min =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 14].Value);
                    sItem92Size1Max =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 15].Value);
                    
                    AddItemTypeToBearingType(CurBearingType, sItem92Descr, sItem92Characht, sItem92Count, sItem92Size1Min, sItem92Size1Max, paramItemTypes);

                    //52
                    sItem52Descr =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 16].Value);
                    sItem52Characht =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 17].Value);
                    sItem52Count =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 18].Value);
                    sItem52Size1Min =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 19].Value);
                    sItem52Size1Max =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 20].Value);
                    
                    AddItemTypeToBearingType(CurBearingType, sItem52Descr, sItem52Characht, sItem52Count, sItem52Size1Min, sItem52Size1Max, paramItemTypes);

                    //04
                    sItem04Descr =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 21].Value);
                    sItem04Characht =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 22].Value);
                    sItem04Count =      NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 23].Value);
                    sItem04Size1Min =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 24].Value);
                    sItem04Size1Max =   NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 25].Value);
                    AddItemTypeToBearingType(CurBearingType, sItem04Descr, sItem04Characht, sItem04Count, sItem04Size1Min, sItem04Size1Max, paramItemTypes);

                    sR1Nom = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 26].Value);
                    CurBearingType.Rad1Nominal = Convert.ToDouble(sR1Nom);

                    sR1Min = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 27].Value);
                    CurBearingType.Rad1Min = Convert.ToDouble(sR1Min);

                    sR1Max = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 28].Value);
                    CurBearingType.Rad1Max = Convert.ToDouble(sR1Max);

                    sMinArrangeCount = NullToString(Worksheet.Cells[iExcelRowNumber, iBearingTypesFirstCol + 29].Value);
                    CurBearingType.MinArrangeCount = Convert.ToInt32(sMinArrangeCount);

                    paramBearingTypes.Add(CurBearingType);
                }
                iExcelRowNumber++;
            }
            while (sDescription != "");

            Marshal.FinalReleaseComObject(Worksheet);
            Worksheet = null;
            Marshal.FinalReleaseComObject(Worksheets);
            Worksheets = null;

        }

        public void LoadBearingArrangementOrderFromExcel(List<BearingsArrangementOrder> paramArrOrders, List<BearingType> paramBearingTypes)
        {

            Microsoft.Office.Interop.Excel.Worksheet Worksheet;
            var Worksheets = pObjWorkBook.Sheets;
            Worksheet = Worksheets[sBearingArrangementOrderListName];

            int iExcelRowNumber = iBearingArrangementOrderFirstRow;
            string sDescription;
            int iCount;

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
                        iCount = Convert.ToInt32(NullToString(Worksheet.Cells[iExcelRowNumber, iBearingArrangementOrderFirstCol + 1].Value));
                        if (iCount > 0)
                        {
                            BearingsArrangementOrder curArrOrder = new BearingsArrangementOrder()
                            {
                                BearingType = BearingType.First()
                            };
                            curArrOrder.OrderCount = iCount;

                            paramArrOrders.Add(curArrOrder);

                        }
                    }
                    else
                    {
                        //todo не найден тип подшипника
                    }
                }
                iExcelRowNumber++;
            }
            while (sDescription != "");

            Marshal.FinalReleaseComObject(Worksheet);
            Worksheet = null;
            Marshal.FinalReleaseComObject(Worksheets);
            Worksheets = null;

        }

        public void LoadBearingItemsGroupsFromExcel(List<BearingItemsGroup> paramItemsGroups, List<BearingItemType> paramBearingItemTypes)
        {

            int iExcelRowNumber = iItemsFirstRow;
            double dSize;
            string sDescription, sCount, sSize, sType, sCharact;
            BearingItemType curType;
            BearingItemsGroup curGroup;

            var Worksheets = pObjWorkBook.Sheets;

            foreach (Worksheet Worksheet in Worksheets)
            {
                if (Worksheet.Name.Length > 6)
                {

                    if (Worksheet.Name.Substring(0, 6) == "Деталь")
                    {
                        sDescription = NullToString(Worksheet.Cells[1, 1].Value);
                        if (sDescription != "")
                        {
                            sType = NullToString(Worksheet.Cells[1, 2].Value);
                            sCharact = NullToString(Worksheet.Cells[1, 3].Value);
                            var ItemType = from CurItemType in paramBearingItemTypes
                                           where (CurItemType.Description == sDescription)&& (CurItemType.CharachteristicID == sCharact)
                                           select CurItemType;

                            if (ItemType.Count() > 0)
                            {
                                curType = ItemType.First();
                            }
                            else
                            {
                                curType = new BearingItemType { Description = sDescription, Type = sType, CharachteristicID = sCharact, CharachteristicDescription = sCharact, ID = sDescription };
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
                                        curGroup = new BearingItemsGroup()
                                        {
                                            ItemType = curType,
                                            Size1 = dSize
                                        };
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
                Marshal.FinalReleaseComObject(Worksheet);
            }

            Marshal.FinalReleaseComObject(Worksheets);
            Worksheets = null;

        }

        private void UsedItemsOutput(List<BearingItemsGroup> paramBearingItemsGroups)
        {
            string sValue;
            const int iLastColNumber = 5;

            Worksheet UsedItemsTemplateWorksheet = pObjWorkBook.Sheets["ШаблонВыводаИспользуемыеДетали"];
            Worksheet UsedItemsWorksheet = pObjWorkBook.Sheets["ИспользованныеДетали"];

            UsedItemsWorksheet.Cells.Delete();

            UsedItemsTemplateWorksheet.Range[UsedItemsTemplateWorksheet.Cells[1, 1], UsedItemsTemplateWorksheet.Cells[2, iLastColNumber]].Copy(UsedItemsWorksheet.Cells[1, 1]);
            foreach (Range Cell in UsedItemsWorksheet.Range[UsedItemsWorksheet.Cells[1, 1], UsedItemsWorksheet.Cells[2, iLastColNumber]])
            {
                sValue = NullToString(Cell.Value);
                switch (sValue)
                {
                    case "ДатаВремяКомплектовки":
                        Cell.Value = "Время комплектовки: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ");
                        break;
                }
            }

            UsedItemsWorksheet.PageSetup.PrintTitleRows = "$1:$2";

            int iExcelRowNumber = 3;

            foreach (var curItemsGroup in paramBearingItemsGroups)
            {

                UsedItemsTemplateWorksheet.Range[UsedItemsTemplateWorksheet.Cells[3, 1], UsedItemsTemplateWorksheet.Cells[7, iLastColNumber]].Copy(UsedItemsWorksheet.Cells[iExcelRowNumber, 1]);

                foreach (Range Cell in UsedItemsWorksheet.Range[UsedItemsWorksheet.Cells[iExcelRowNumber, 1], UsedItemsWorksheet.Cells[iExcelRowNumber, iLastColNumber]])
                {
                    sValue = NullToString(Cell.Value);
                    Cell.Value = "";
                    switch (sValue)
                    {
                        //Деталь	Размер1	КоличествоОстаток	КоличествоВыдать	КоличествоЗарезервировать

                        case "Деталь":
                            Cell.Value = curItemsGroup.ItemType.Description;
                            break;
                        case "Размер1":
                            Cell.Value = curItemsGroup.Size1;
                            break;
                        case "КоличествоОстаток":
                            Cell.Value = curItemsGroup.ItemCount;
                            break;
                        case "КоличествоВыдать":
                            Cell.Value = curItemsGroup.GiveOutItemCount;
                            break;
                        case "КоличествоЗарезервировать":
                            Cell.Value = curItemsGroup.ReservedItemCount;
                            break;
                    }
                }
                iExcelRowNumber++;
            }

            Marshal.ReleaseComObject(UsedItemsTemplateWorksheet);
            UsedItemsTemplateWorksheet = null;
            Marshal.ReleaseComObject(UsedItemsWorksheet);
            UsedItemsWorksheet = null;
        }

        private void GrindingOrdersOutput(List<BearingItemsGroup> paramGrindingOrders)
        {
            string sValue;
            const int iLastColNumber = 3;

            Worksheet GrindingOrdersTemplateWorksheet = pObjWorkBook.Sheets["ШаблонВыводаДеталиДокомплект"];
            Worksheet GrindingOrdersWorksheet = pObjWorkBook.Sheets["ДеталиДокомплект"];

            GrindingOrdersWorksheet.Cells.Delete();

            GrindingOrdersTemplateWorksheet.Range[GrindingOrdersTemplateWorksheet.Cells[1, 1], GrindingOrdersTemplateWorksheet.Cells[2, iLastColNumber]].Copy(GrindingOrdersWorksheet.Cells[1, 1]);
            foreach (Range Cell in GrindingOrdersWorksheet.Range[GrindingOrdersWorksheet.Cells[1, 1], GrindingOrdersWorksheet.Cells[2, iLastColNumber]])
            {
                sValue = NullToString(Cell.Value);
                switch (sValue)
                {
                    case "ДатаВремяКомплектовки":
                        Cell.Value = "Время комплектовки: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ");
                        break;
                }
            }

            GrindingOrdersWorksheet.PageSetup.PrintTitleRows = "$1:$2";

            int iExcelRowNumber = 3;

            foreach (var curItemsGroup in paramGrindingOrders)
            {

                GrindingOrdersTemplateWorksheet.Range[GrindingOrdersTemplateWorksheet.Cells[3, 1], GrindingOrdersTemplateWorksheet.Cells[3, iLastColNumber]].Copy(GrindingOrdersWorksheet.Cells[iExcelRowNumber, 1]);

                foreach (Range Cell in GrindingOrdersWorksheet.Range[GrindingOrdersWorksheet.Cells[iExcelRowNumber, 1], GrindingOrdersWorksheet.Cells[iExcelRowNumber, iLastColNumber]])
                {
                    sValue = NullToString(Cell.Value);
                    Cell.Value = "";
                    switch (sValue)
                    {
                        //Деталь	Размер1	Количество

                        case "Деталь":
                            Cell.Value = curItemsGroup.ItemType.Description;
                            break;
                        case "Размер1":
                            Cell.Value = curItemsGroup.Size1;
                            break;
                        case "Количество":
                            Cell.Value = curItemsGroup.ItemCount;
                            break;
                    }
                }
                iExcelRowNumber++;
            }

            Marshal.ReleaseComObject(GrindingOrdersTemplateWorksheet);
            GrindingOrdersTemplateWorksheet = null;
            Marshal.ReleaseComObject(GrindingOrdersWorksheet);
            GrindingOrdersWorksheet = null;
        }


        public void ArrOrdersResultOutput(List<BearingsArrangementOrder> paramBearingArrOrders, List<BearingItemsGroup> paramBearingItemsGroups, List<BearingItemsGroup> paramGrindingOrders)
        {

            SolutionOutput(paramBearingArrOrders);
            NotCompletedBearingsOutput(paramBearingArrOrders);
            UsedItemsOutput(paramBearingItemsGroups);
            GrindingOrdersOutput(paramGrindingOrders);

            pObjWorkBook.Save();

        }

        private void NotCompletedBearingsOutput(List<BearingsArrangementOrder> paramBearingArrOrders)
        {

            string sValue;
            const int iLastColNumber = 8;

            Worksheet SolutionTemplateWorksheet = pObjWorkBook.Sheets["ШаблонВыводаКомплектовка"];
            Worksheet SolutionOutputWorksheet = pObjWorkBook.Sheets["Докомплектовать"];

            SolutionOutputWorksheet.Cells.Delete();

            SolutionTemplateWorksheet.Range[SolutionTemplateWorksheet.Cells[1, 1], SolutionTemplateWorksheet.Cells[2, iLastColNumber]].Copy(SolutionOutputWorksheet.Cells[1, 1]);
            foreach (Range Cell in SolutionOutputWorksheet.Range[SolutionOutputWorksheet.Cells[1, 1], SolutionOutputWorksheet.Cells[2, iLastColNumber]])
            {
                sValue = NullToString(Cell.Value);
                switch (sValue)
                {
                    case "ДатаВремяКомплектовки":
                        Cell.Value = "Время комплектовки: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ");
                        break;
                }
            }

            SolutionOutputWorksheet.PageSetup.PrintTitleRows = "$1:$2";

            int iExcelRowNumber = 3;
            int iBearingSectionHeight = 5;
            int iNumberOfSectionsOnPage = 8;
            int iSectionNumber = 0;

            foreach (var curSolution in paramBearingArrOrders)
            {
                foreach (NotCompleteBearingGroup curBearing in curSolution.NotCompletedBearings)
                {
                    SolutionTemplateWorksheet.Range[SolutionTemplateWorksheet.Cells[3, 1], SolutionTemplateWorksheet.Cells[7, iLastColNumber]].Copy(SolutionOutputWorksheet.Cells[iExcelRowNumber, 1]);

                    foreach (Range Cell in SolutionOutputWorksheet.Range[SolutionOutputWorksheet.Cells[iExcelRowNumber, 1], SolutionOutputWorksheet.Cells[iExcelRowNumber + iBearingSectionHeight, iLastColNumber]])
                    {
                        sValue = NullToString(Cell.Value);
                        Cell.Value = "";
                        switch (sValue)
                        {
                            case "Подшипник":
                                Cell.Value = curBearing.Type.Description;
                                break;
                            case "ПодшипникКоличество":
                                Cell.Value = curBearing.Count;
                                break;
                        }
                        if (sValue != "")
                        {
                            var sFirstSymbol = sValue.Substring(0, 1);
                            if (sFirstSymbol == "+")
                            {
                                var sItemType = sValue.Substring(1, 2);
                                sValue = sValue.Substring(3, sValue.Length - 3);
                                switch (sValue)
                                {
                                    case "Деталь":
                                        if (curBearing.UsedBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.UsedBearingItemsGroups[sItemType].ItemType.Description;
                                        }
                                        else if (curBearing.NeededBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.NeededBearingItemsGroups[sItemType].ItemType.Description;
                                            Cell.Font.Bold = true;
                                        }
                                        break;
                                    case "Размер":
                                        if (curBearing.UsedBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.UsedBearingItemsGroups[sItemType].Size1;
                                        }
                                        else if (curBearing.NeededBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.NeededBearingItemsGroups[sItemType].Size1;
                                        }
                                        break;
                                    case "Размер1":
                                        if (curBearing.UsedBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.UsedBearingItemsGroups[sItemType].Size1Min;
                                        }
                                        else if (curBearing.NeededBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.NeededBearingItemsGroups[sItemType].Size1Min;
                                        }
                                        break;
                                    case "Размер2":
                                        if (curBearing.UsedBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.UsedBearingItemsGroups[sItemType].Size1Max;
                                        }
                                        else if (curBearing.NeededBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.NeededBearingItemsGroups[sItemType].Size1Max;
                                        }
                                        break;
                                    case "Количество":
                                        if (curBearing.UsedBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.Type.BearingItemsCount[sItemType] * curBearing.Count;
                                        }
                                        else if (curBearing.NeededBearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.Type.BearingItemsCount[sItemType] * curBearing.Count;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    iExcelRowNumber = iExcelRowNumber + iBearingSectionHeight;

                    iSectionNumber++;
                    if (iSectionNumber == iNumberOfSectionsOnPage)
                    {
                        SolutionOutputWorksheet.HPageBreaks.Add(SolutionOutputWorksheet.Cells[iExcelRowNumber, 1]);
                        iSectionNumber = 0;
                    }
                }
            }


            Marshal.ReleaseComObject(SolutionOutputWorksheet);
            SolutionOutputWorksheet = null;
            Marshal.ReleaseComObject(SolutionTemplateWorksheet);
            SolutionTemplateWorksheet = null;

        }

        private void SolutionOutput(List<BearingsArrangementOrder> paramBearingArrOrders)
        {
            string sValue;
            const int iLastColNumber = 8;
            int iExcelRowNumber = 3;
            const int iBearingSectionHeight = 5;
            const int iNumberOfSectionsOnPage = 8;
            int iSectionNumber = 0;

            Worksheet SolutionTemplateWorksheet = pObjWorkBook.Sheets["ШаблонВыводаКомплектовка"];
            Worksheet SolutionOutputWorksheet = pObjWorkBook.Sheets["Комплектовка"];

            //var iLastRow = SolutionOutputWorksheet.Range(SolutionOutputWorksheet.Rows.Count, 1).End(xlUp).Row;
            var iFullRow = SolutionOutputWorksheet.Rows.Count;
            var iLastRow = SolutionOutputWorksheet.Cells[iFullRow, 1].End(XlDirection.xlUp).Row;
            SolutionOutputWorksheet.Range[SolutionOutputWorksheet.Cells[1, 1], SolutionOutputWorksheet.Cells[iLastRow + iBearingSectionHeight, iLastColNumber]].Delete();

            SolutionTemplateWorksheet.Range[SolutionTemplateWorksheet.Cells[1, 1], SolutionTemplateWorksheet.Cells[2, iLastColNumber]].Copy(SolutionOutputWorksheet.Cells[1, 1]);
            foreach (Range Cell in SolutionOutputWorksheet.Range[SolutionOutputWorksheet.Cells[1, 1], SolutionOutputWorksheet.Cells[2, iLastColNumber]])
            {
                sValue = NullToString(Cell.Value);
                switch (sValue)
                {
                    case "ДатаВремяКомплектовки":
                        Cell.Value = "Время комплектовки: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ");
                        break;
                }
            }

            SolutionOutputWorksheet.PageSetup.PrintTitleRows = "$1:$2";

            foreach (var curSolution in paramBearingArrOrders)
            {
                foreach (BearingGroup curBearing in curSolution.ArrangedBearings)
                {
                    SolutionTemplateWorksheet.Range[SolutionTemplateWorksheet.Cells[3, 1], SolutionTemplateWorksheet.Cells[7, iLastColNumber]].Copy(SolutionOutputWorksheet.Cells[iExcelRowNumber, 1]);

                    foreach (Range Cell in SolutionOutputWorksheet.Range[SolutionOutputWorksheet.Cells[iExcelRowNumber, 1], SolutionOutputWorksheet.Cells[iExcelRowNumber + iBearingSectionHeight, iLastColNumber]])
                    {
                        sValue = NullToString(Cell.Value);
                        Cell.Value = "";
                        switch (sValue)
                        {
                            case "Подшипник":
                                Cell.Value = curBearing.Type.Description;
                                break;
                            case "ПодшипникКоличество":
                                Cell.Value = curBearing.Count;
                                break;
                            case "РЗ":
                                Cell.Value = curBearing.Rad1();
                                break;
                        }
                        if (sValue != "")
                        {
                            var sFirstSymbol = sValue.Substring(0, 1);
                            if (sFirstSymbol == "+")
                            {
                                var sItemType = sValue.Substring(1, 2);
                                sValue = sValue.Substring(3, sValue.Length - 3);
                                switch (sValue)
                                {
                                    case "Деталь":
                                        if (curBearing.BearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.BearingItemsGroups[sItemType].ItemType.Description;
                                        }
                                        break;
                                    case "Размер":
                                        if (curBearing.BearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.BearingItemsGroups[sItemType].Size1;
                                        }
                                        break;
                                    case "Размер1":
                                        if (curBearing.BearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.BearingItemsGroups[sItemType].Size1Min;
                                        }
                                        break;
                                    case "Размер2":
                                        if (curBearing.BearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.BearingItemsGroups[sItemType].Size1Max;
                                        }
                                        break;
                                    case "Количество":
                                        if (curBearing.BearingItemsGroups.ContainsKey(sItemType))
                                        {
                                            Cell.Value = curBearing.Type.BearingItemsCount[sItemType] * curBearing.Count;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    iExcelRowNumber = iExcelRowNumber + iBearingSectionHeight;

                    iSectionNumber++;
                    if (iSectionNumber == iNumberOfSectionsOnPage)
                    {
                        SolutionOutputWorksheet.HPageBreaks.Add(SolutionOutputWorksheet.Cells[iExcelRowNumber, 1]);
                        iSectionNumber = 0;
                    }
                }
            }

            Marshal.ReleaseComObject(SolutionOutputWorksheet);
            SolutionOutputWorksheet = null;
            Marshal.ReleaseComObject(SolutionTemplateWorksheet);
            SolutionTemplateWorksheet = null;
        }

    }


}
