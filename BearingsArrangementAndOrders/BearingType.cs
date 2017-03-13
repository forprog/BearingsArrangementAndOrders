using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingsArrangementAndOrders
{
    class BearingType
    {
        public string Description;
        public Dictionary<string, BearingItemType> ValidBearingItemTypes = new Dictionary<string, BearingItemType> { };
        public Dictionary<string, int> BearingItemsCount = new Dictionary<string, int> { };
        public double? Rad1Nominal;
        public double? Rad1Min;
        public double? Rad1Max;
        public double MinArrangeCount
        {
            get
            {
                return pMinArrangeCount;
            }
            set
            {
                pMinArrangeCount = Math.Max(1, value);
            }
        }
        private double pMinArrangeCount = 1;


        public List<BearingItemsGroup> CreateItemGroupsToCompleteOrder(List<BearingItemsGroup> paramItemsGroups)
        {
            Dictionary<string, BearingItemsGroup> dItemsGroups = new Dictionary<string, BearingItemsGroup> { { "01", new BearingItemsGroup() }, { "02", new BearingItemsGroup() }, { "52", new BearingItemsGroup() }, { "92", new BearingItemsGroup() }, { "04", new BearingItemsGroup() } };
            List<BearingItemsGroup> result = new List<BearingItemsGroup>();

            foreach (var curItemsGroup in paramItemsGroups)
            {
                dItemsGroups[curItemsGroup.ItemType.Type] = curItemsGroup;
            }
            if (dItemsGroups["04"].ItemCount > 0)// есть информация о шарах
            {
                if (dItemsGroups["01"].ItemCount > 0)//есть 01
                {
                    var curGroup02 = new BearingItemsGroup();
                    curGroup02.ItemType = ValidBearingItemTypes["02"];
                    curGroup02.Size1Max = Rad1Nominal + dItemsGroups["01"].Size1 - 2 * dItemsGroups["04"].Size1 - Rad1Min;
                    curGroup02.Size1Min = Rad1Nominal + dItemsGroups["01"].Size1 - 2 * dItemsGroups["04"].Size1 - Rad1Max;
                    if (curGroup02.DoValid())
                    {
                        result.Add(curGroup02);
                    }
                }
                else if (dItemsGroups["02"].ItemCount > 0)//есть 02
                {
                    var curGroup01 = new BearingItemsGroup();
                    curGroup01.ItemType = ValidBearingItemTypes["01"];
                    curGroup01.Size1Max = Rad1Max - Rad1Nominal + dItemsGroups["02"].Size1 + 2 * dItemsGroups["04"].Size1;
                    curGroup01.Size1Min = Rad1Min - Rad1Nominal + dItemsGroups["02"].Size1 + 2 * dItemsGroups["04"].Size1;
                    if (curGroup01.DoValid())
                    {
                        result.Add(curGroup01);
                    }
                }
                else//нет ни 01, ни 02
                {
                    var curGroup01 = new BearingItemsGroup();
                    curGroup01.ItemType = ValidBearingItemTypes["01"];
                    curGroup01.Size1Max = dItemsGroups["04"].Size1 + (Rad1Max - Rad1Min) / 4;
                    curGroup01.Size1Min = dItemsGroups["04"].Size1 - (Rad1Max - Rad1Min) / 4;

                    if (curGroup01.DoValid())
                    {
                        result.Add(curGroup01);
                    }

                    var curGroup02 = new BearingItemsGroup();
                    curGroup02.ItemType = ValidBearingItemTypes["02"];
                    curGroup02.Size1Max = -dItemsGroups["04"].Size1 + (Rad1Max - Rad1Min) / 4;
                    curGroup02.Size1Min = -dItemsGroups["04"].Size1 - (Rad1Max - Rad1Min) / 4;

                    if (curGroup02.DoValid())
                    {
                        result.Add(curGroup02);
                    }
                }
            }

            return result;

        }
    }
}
