using System;
using System.Collections.Generic;

namespace BearingsArrangementAndOrders
{
    class BearingGroup
    {
        public BearingType Type;
        public Dictionary<string, BearingItemsGroup> BearingItemsGroups = new Dictionary<string, BearingItemsGroup>();
        public double Rad1()
        {
            return Type.Rad1Nominal.GetValueOrDefault() + BearingItemsGroups["01"].Size1 - BearingItemsGroups["02"].Size1 - 2 * BearingItemsGroups["04"].Size1;
        }

        public double Rad1Devation()
        {
            return Math.Abs((Type.Rad1Max.GetValueOrDefault()-Type.Rad1Min.GetValueOrDefault())-Rad1());
        } 

        private int pCount;
        public int Count
        {
            get { return pCount; }
        }

        public  int GetCount()
        {
            int iBearingCount = int.MaxValue;
            foreach (var curKVPare in BearingItemsGroups)
            {
                var curItemGroup = curKVPare.Value;
                int iItemCount = Convert.ToInt32(Math.Floor(curItemGroup.ItemCount / Convert.ToDouble(Type.BearingItemsCount[curItemGroup.ItemType.Type])));
                iBearingCount = Math.Min(iBearingCount, iItemCount);
            }
            return iBearingCount;
        }

        public void SetCount(int paramCount)
        {
            pCount = paramCount;
        }

        public void SetCount()
        {
            pCount = GetCount();

        }

        public bool IsArrangement()
        {
            if ((Rad1() >= Type.Rad1Min) && (Rad1() <= Type.Rad1Max))
            { return true; }
            else
            { return false; }
        }

        public BearingGroup(BearingGroup paramBearingGroup)
        {
            Type = paramBearingGroup.Type;
            foreach (var item in paramBearingGroup.BearingItemsGroups)
            {
                BearingItemsGroups.Add(item.Key, item.Value);
            }
        }
        public BearingGroup()
        {
        }
    }
}

