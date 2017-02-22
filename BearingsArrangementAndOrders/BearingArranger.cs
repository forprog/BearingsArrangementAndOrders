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

        private void CheckArrangement(List<BearingItemsGroup> paramPreviousItemGroups, int paramLevel, int paramMaxLevel, BearingType paramBearingType, List<List<BearingItemsGroup>> paramItemsGroupsList, List<BearingGroup> paramPossibleBearingGroups)
        {
            if (paramLevel <= paramMaxLevel)
            {
                foreach (BearingItemsGroup curItemGroupList in paramItemsGroupsList[paramLevel])
                {
                    List<BearingItemsGroup> curItemsGroups = new List<BearingItemsGroup>(paramPreviousItemGroups);
                    curItemsGroups.Add(curItemGroupList);
                    CheckArrangement(curItemsGroups, paramLevel + 1, paramMaxLevel, paramBearingType, paramItemsGroupsList, paramPossibleBearingGroups);
                }
            }
            else
            {
                BearingGroup curBearingGroup = new BearingGroup();
                curBearingGroup.Type = paramBearingType;
                int iArrangementCountIncrease = 1;
                foreach (BearingItemsGroup curItemGroup in paramPreviousItemGroups)
                {
                    curBearingGroup.BearingItemsGroups.Add(curItemGroup.ItemType.Type, curItemGroup);
                    iArrangementCountIncrease = iArrangementCountIncrease * curItemGroup.ItemCount;
                }
                curBearingGroup.SetCount();

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


        private List<BearingGroup> FindBearingGroupsOfItemGroupInList(BearingItemsGroup paramItemGroup, List<BearingGroup> paramBearingGroupList)
        {

            return paramBearingGroupList.Where(kvp => kvp.BearingItemsGroups.ContainsValue(paramItemGroup)).ToList();

        }

        private void RemoveBearingGroupsOfItemGroupFromList(BearingItemsGroup paramItemGroup, List<BearingGroup> paramBearingGroupList)
        {

            paramBearingGroupList.RemoveAll(kvp => kvp.BearingItemsGroups.ContainsValue(paramItemGroup));

        }

        private void FindSolution(BearingsArrangementOrder paramArrOrder, List<BearingGroup> paramPossibleBearingGroups, List<BearingItemsGroup> paramItemsGroups, List<BearingGroup> paramSolution)
        {
            //сортировка деталей, если нужна
            //IEnumerable<BearingGroup> curPossibleBearings = from qGroups in paramPossibleBearingGroups
            //                                                where qGroups.Type == paramArrOrder.BearingType
            //                                                select qGroups;
            int ItemNumber,
                ArrOrderCount = paramArrOrder.Count;
            List<BearingGroup> matches;

            paramItemsGroups.Sort((x, y) => x.ArrangementCount.CompareTo(y.ArrangementCount));//сортировка от наименее к наиболее востребованным деталям

            for (ItemNumber = 0; (ItemNumber < paramItemsGroups.Count) && (ArrOrderCount > 0); ItemNumber++)
            {
                var CurItemGroup = paramItemsGroups[ItemNumber];
                if ((CurItemGroup.ItemCount > 0) && (CurItemGroup.ArrangementCount > 0))
                {
                    matches = FindBearingGroupsOfItemGroupInList(CurItemGroup, paramPossibleBearingGroups);
                    matches.Sort((x, y) => x.Count.CompareTo(y.Count));

                    if (matches.Count > 0)
                    {
                        var curPossibleBearingGroup = matches[0];
                        BearingGroup curSolutionGroup = new BearingGroup(curPossibleBearingGroup);
                        curSolutionGroup.SetCount(Math.Min(curSolutionGroup.GetCount(), ArrOrderCount));
                        ArrOrderCount -= curSolutionGroup.Count;

                        if (curSolutionGroup.Count > 0)
                        {
                            foreach (var item in curSolutionGroup.BearingItemsGroups)
                            {
                                var curItemsGroup = item.Value;

                                //уменьшить количество деталей в группах деталей и убрать возможные группы подшипников с группой деталей, а если надо будет набирать количество - то надо будет уменьшать количество возможных подшипников везде, где используются группы деталей, взятые в решение
                                curItemsGroup.ItemCount -= curSolutionGroup.Type.BearingItemsCount[curItemsGroup.ItemType.Type] * curSolutionGroup.Count;
                                if (curItemsGroup.ItemCount > 0)
                                {
                                    var BearingGroups = FindBearingGroupsOfItemGroupInList(curItemsGroup, paramPossibleBearingGroups);
                                    foreach (var curBearingGroup in BearingGroups)
                                    {
                                        if (curBearingGroup != curPossibleBearingGroup)
                                        {
                                            curBearingGroup.SetCount();
                                        }
                                    }
                                }
                                else
                                {
                                    RemoveBearingGroupsOfItemGroupFromList(curItemsGroup, paramPossibleBearingGroups);
                                }

                            }
                            curPossibleBearingGroup.SetCount();
                            paramSolution.Add(curSolutionGroup);
                        }
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
                List<List<BearingItemsGroup>> curItemsGroupsLists = new List<List<BearingItemsGroup>> { };
                List<BearingGroup> curPossibleBearingGroups = new List<BearingGroup> { };
                List<BearingItemsGroup> curItemsGroups = new List<BearingItemsGroup> { };
                List<BearingItemsGroup> curBearingItems = new List<BearingItemsGroup> { };
                List<BearingGroup> curSolution = new List<BearingGroup> { };

                foreach (var curItemType in curArrOrder.BearingType.ValidBearingItemTypes)
                {
                    IEnumerable<BearingItemsGroup> curItemGroups = from qGroups in ItemsGroups
                                                                   where (qGroups.ItemType.Description == curItemType.Value.Description) && (qGroups.ItemCount > 0)
                                                                   select qGroups;
                    curItemsGroupsLists.Add(curItemGroups.ToList());
                    curItemsGroups.AddRange(curItemGroups.ToList());
                }

                int iValidItemTypesCount = curArrOrder.BearingType.ValidBearingItemTypes.Count();
                CheckArrangement(curBearingItems, 0, iValidItemTypesCount - 1, curArrOrder.BearingType, curItemsGroupsLists, curPossibleBearingGroups);

                FindSolution(curArrOrder, curPossibleBearingGroups, curItemsGroups, curSolution);

                //todo если есть нежокомплектованные заказы, от шара получить частично комплектующиеся группы, для них найти перекрестные заказы. для остальных дать стандартный заказ на кольца под шар

            }

        }

    }
}
