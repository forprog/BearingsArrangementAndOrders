using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingsArrangementAndOrders
{
    class BearingItemsGroup
    //класс содержит информацию о количестве деталей, одинаковых по всем размерам, обозначению и виду детали
    {
        public double? Size1;
        public double? Size1Max;
        public double? Size1Min;

        public double? Size2;
        public double? Size2Max;
        public double? Size2Min;

        public double? Size3;
        public double? Size3Max;
        public double? Size3Min;

        public BearingItemType ItemType;

        public int ArrangementCount;

        public void AddBearingItemsGroupToGroup(BearingItemsGroup paramBearingItemsGorup)
        {
            if ((paramBearingItemsGorup.Size1 == Size1) && (paramBearingItemsGorup.Size1Min == Size1Min) && (paramBearingItemsGorup.Size1Max == Size1Max)
                && (paramBearingItemsGorup.Size2 == Size2) && (paramBearingItemsGorup.Size2Min == Size2Min) && (paramBearingItemsGorup.Size2Max == Size2Max)
                && (paramBearingItemsGorup.Size3 == Size3) && (paramBearingItemsGorup.Size3Min == Size3Min) && (paramBearingItemsGorup.Size3Max == Size3Max)
                && (paramBearingItemsGorup.ItemType == ItemType))
            {
                pItemCount += paramBearingItemsGorup.ItemCount;
            }
        }
        public void RemoveBearingItemsGroupFromGroup(BearingItemsGroup paramBearingItemsGorup)
        {

            pItemCount -= paramBearingItemsGorup.ItemCount;

        }
        private int pItemCount = 0;
        public int ItemCount
        {
            get
            {
                return pItemCount;
            }
            set
            {
                pItemCount = value;
            }
        }
    }
}
