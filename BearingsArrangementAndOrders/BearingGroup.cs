using System.Collections.Generic;

namespace BearingsArrangementAndOrders
{
    class BearingGroup
    {
        public BearingType Type;
        public Dictionary<string, BearingItemsGroup> BearingItemsGroups;
        public Dictionary<string, int> BearingItemsCount;
        public double? Rad1()
        {
            return 1;
        }
        public int Count;
        public bool IsArrangement()
        {
            if ((Rad1() >= Type.Rad1Min) && (Rad1() <= Type.Rad1Max))
            { return true; }
            else
            { return false; }
        }
    }
}

