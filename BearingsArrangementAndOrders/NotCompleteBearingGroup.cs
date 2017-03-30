using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingsArrangementAndOrders
{
    class NotCompleteBearingGroup
    {
        public BearingType Type;
        public Dictionary<string, BearingItemsGroup> UsedBearingItemsGroups = new Dictionary<string, BearingItemsGroup>();
        public Dictionary<string, BearingItemsGroup> NeededBearingItemsGroups = new Dictionary<string, BearingItemsGroup>();

        private int pCount;
        public int Count
        {
            get { return pCount; }
        }

        public int GetCount()
        {
            int iBearingCount = int.MaxValue;
            foreach (var curKVPare in UsedBearingItemsGroups)
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

        //проверка - чтобы заказы комплектовались не меньше минимально допустимого количества заказываемых деталей
        public bool CheckMinItemsOrderCount()
        {
            bool bResult = true;
            
            foreach (var curNeededItemGroup in NeededBearingItemsGroups)
            {
                int ItemCount = Count * Type.BearingItemsCount[curNeededItemGroup.Value.ItemType.Type];
                if (ItemCount < curNeededItemGroup.Value.ItemType.MinOrderCount)
                {
                    bResult = false;
                    break;
                }
            }

            return bResult;
        }


    }
}
