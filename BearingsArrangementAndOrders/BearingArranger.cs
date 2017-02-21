using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingsArrangementAndOrders
{
    class BearingArranger //хранит в себе данные о подшипниках и деталях и производит комплектовку
    {

        public List<BearingType> BearingTypes = new List<BearingType> { };
        public List<BearingsArrangementOrder> BearingArrOrders = new List<BearingsArrangementOrder> { };
        public List<BearingItemsGroup> ItemsGroups = new List<BearingItemsGroup> { };
        public List<BearingItemType> ItemTypes = new List<BearingItemType> { };

        public void CheckArrangement(List<BearingItemsGroup> paramPreviousItemGroups, int paramLevel, int paramMaxLevel, List<List<BearingItemsGroup>> paramItemGroupsLists, List<BearingGroup> paramPossibleBearingGroups, BearingType paramBearingType)
        {
            if (paramLevel <= paramMaxLevel)
            {
                foreach (BearingItemsGroup curItemGroupList in paramItemGroupsLists[paramLevel])
                {
                    List<BearingItemsGroup> curItemsGroups = new List<BearingItemsGroup>(paramPreviousItemGroups);
                    curItemsGroups.Add(curItemGroupList);
                    CheckArrangement(curItemsGroups, paramLevel + 1, paramMaxLevel, paramItemGroupsLists, paramPossibleBearingGroups, paramBearingType);
                }
            }
            else
            {
                BearingGroup curBearingGroup = new BearingGroup();
                curBearingGroup.Type = paramBearingType;
                int iArrangementCountIncrease = 1;
                foreach (BearingItemsGroup curItemGroup in paramPreviousItemGroups)
                {
                    curBearingGroup.BearingItemsGroups[curItemGroup.ItemType.Type] = curItemGroup;
                    iArrangementCountIncrease = iArrangementCountIncrease * curItemGroup.ItemCount;
                }
                if (curBearingGroup.IsArrangement())
                {
                    paramPossibleBearingGroups.Add(curBearingGroup);
                    foreach (var curItemGroup in paramPreviousItemGroups)
                    {
                        curItemGroup.ArrangementCount += iArrangementCountIncrease;
                    }

                }

            }
        }

        public void DoArrangement()
        {
            var CurrentSolution = new List<BearingGroup> { };

            foreach (BearingsArrangementOrder curArrOrder in BearingArrOrders)
            {

                List<BearingGroup> PossibleBearingGroups = new List<BearingGroup> { };
                List<List<BearingItemsGroup>> ItemsGroupsLists = new List<List<BearingItemsGroup>> { };

                foreach (var curItemType in curArrOrder.BearingType.ValidBearingItemTypes)
                {
                    IEnumerable<BearingItemsGroup> curItemGroups = from qGroups in ItemsGroups
                                                                   where (qGroups.ItemType.Description == curItemType.Value.Description) && (qGroups.ItemCount > 0)
                                                                   select qGroups;
                    ItemsGroupsLists.Add(curItemGroups.ToList());
                }

                int iValidItemTypesCount = curArrOrder.BearingType.ValidBearingItemTypes.Count();
                List<BearingItemsGroup> curBearingItems = new List<BearingItemsGroup> { };
                List<BearingGroup> curPossibleBearings = new List<BearingGroup> { };
                CheckArrangement(curBearingItems, 0, iValidItemTypesCount - 1, ItemsGroupsLists, curPossibleBearings, curArrOrder.BearingType);

            }

        }



    }
}
