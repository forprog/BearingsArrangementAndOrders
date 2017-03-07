using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingsArrangementAndOrders
{
    class BearingItemsGroup
    //класс содержит информацию о количестве деталей, одинаковых по всем размерам, обозначению и виду детали
    //todo можно развести count на 2 части - доступный и зарезервированный. Тогда после поиска решений, по каждой группе деталей будет сразу понятно, сколько надо резевировать деталей
    {
        public double Size1;
        public double? Size1Max;
        public double? Size1Min;

        public double? Size2;
        public double? Size2Max;
        public double? Size2Min;

        public double? Size3;
        public double? Size3Max;
        public double? Size3Min;

        public BearingItemType ItemType;

        public long ArrangementCount;

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

        private int pReservedItemCount = 0;
        public int ReservedItemCount
        {
            get
            {
                return pReservedItemCount;
            }
            set
            {
                pReservedItemCount = value;
            }
        }

        private int pGiveOutItemCount = 0;
        public int GiveOutItemCount
        {
            get
            {
                return pGiveOutItemCount;
            }
            set
            {
                pGiveOutItemCount = value;
            }
        }

        public bool DoValid()
        {
            bool bReturn = false;
            if ((Size1Max.HasValue)&&(Size1Min.HasValue))
            {
                double iSize1Max = Size1Max ?? 0;
                double iSize1Min = Size1Min ?? 0;
                double iTypeSize1Max = ItemType.Size1Max ?? 0;
                double iTypeSize1Min = ItemType.Size1Min ?? 0;
                if ((iSize1Max > iTypeSize1Min) && (iSize1Min < iTypeSize1Max))
                {
                    Size1Max = Math.Min(iSize1Max , iTypeSize1Max);
                    Size1Min = Math.Max(iSize1Min , iTypeSize1Min);
                    bReturn = true;
                }
            }
            return bReturn;
        }

    }
}
