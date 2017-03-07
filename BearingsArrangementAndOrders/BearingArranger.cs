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
                ArrOrderCount = paramArrOrder.OrderCount;
            List<BearingGroup> matches;

            //будем комплектовать по массовости шара
            var IEnumItemsGroups =  from qItemsGroup in paramItemsGroups
                                     where qItemsGroup.ItemType.Type == "04"
                                     select qItemsGroup;
            paramItemsGroups = IEnumItemsGroups.ToList();

            paramItemsGroups.Sort((x, y) => y.ArrangementCount.CompareTo(x.ArrangementCount));//сортировка от наименее к наиболее востребованным деталям

            for (ItemNumber = 0; (ItemNumber < paramItemsGroups.Count) && (ArrOrderCount > 0); ItemNumber++)
            {
                var CurItemGroup = paramItemsGroups[ItemNumber];
                if ((CurItemGroup.ItemCount > 0) && (CurItemGroup.ArrangementCount > 0))
                {
                    matches = FindBearingGroupsOfItemGroupInList(CurItemGroup, paramPossibleBearingGroups);
                    matches.Sort((x, y) => x.Rad1Devation().CompareTo(y.Rad1Devation()));

                    if (matches.Count > 0)
                    {
                        //перебираем все группы для данной детали, пока она не закончится
                        for (int i = 0; i < matches.Count; i++)
                        {
                            var curPossibleBearingGroup = matches[i];
                            BearingGroup curSolutionGroup = new BearingGroup(curPossibleBearingGroup);
                            curSolutionGroup.SetCount(Math.Min(curSolutionGroup.GetCount(), ArrOrderCount));
                            //todo добавить проверку минимального комплектуемого количества
                            ArrOrderCount -= curSolutionGroup.Count;

                            if (curSolutionGroup.Count > 0)
                            {
                                foreach (var item in curSolutionGroup.BearingItemsGroups)
                                {
                                    var curItemsGroup = item.Value;

                                    //уменьшить количество деталей в группах деталей и убрать возможные группы подшипников с группой деталей, а если надо будет набирать количество - то надо будет уменьшать количество возможных подшипников везде, где используются группы деталей, взятые в решение
                                    var iGiveOutItemCount = curSolutionGroup.Type.BearingItemsCount[curItemsGroup.ItemType.Type] * curSolutionGroup.Count;
                                    curItemsGroup.GiveOutItemCount += iGiveOutItemCount;
                                    curItemsGroup.ItemCount -= iGiveOutItemCount;

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
        }

        private void AddOrdersToSolution(BearingsArrangementOrder paramArrOrder, List<BearingItemsGroup> paramUsedItems, ref int paramNeededBearingCount, List<NotCompleteBearingGroup> paramSolution)
        {
            var curNeededItems = paramArrOrder.BearingType.CreateItemGroupsToCompleteOrder(paramUsedItems);
            if (curNeededItems.Count > 0)
            {
                var curNotCompletedGroup = new NotCompleteBearingGroup();
                curNotCompletedGroup.Type = paramArrOrder.BearingType;
                foreach (var curItemGroup in paramUsedItems)
                {
                    curNotCompletedGroup.UsedBearingItemsGroups.Add(curItemGroup.ItemType.Type, curItemGroup);
                }

                foreach (var NeedItem in curNeededItems)
                {
                    curNotCompletedGroup.NeededBearingItemsGroups.Add(NeedItem.ItemType.Type, NeedItem);
                }

                curNotCompletedGroup.SetCount(Math.Min(curNotCompletedGroup.GetCount(), paramNeededBearingCount));

                if (curNotCompletedGroup.Count > 0)
                {
                    paramSolution.Add(curNotCompletedGroup);
                    paramNeededBearingCount -= curNotCompletedGroup.Count;
                    foreach (var ItemGroup in paramUsedItems)
                    {
                        var iReservedItemCount = curNotCompletedGroup.Count * paramArrOrder.BearingType.BearingItemsCount[ItemGroup.ItemType.Type];
                        ItemGroup.ReservedItemCount += iReservedItemCount;
                        ItemGroup.ItemCount -= iReservedItemCount;
                    }
                }

            }

        }



        private void MakeOrders(BearingsArrangementOrder paramArrOrder, List<BearingItemsGroup> paramItemsGroups, List<NotCompleteBearingGroup> paramSolution)
        {
            //если сюда попали - значит по-любому подшипник в нужном количестве из существующих деталей не комплектуется
            int iNeededBearingCount = paramArrOrder.OrderCount - paramArrOrder.ArrangedBearingsCount();
            List<BearingItemsGroup> cur04Items = new List<BearingItemsGroup>(),
                                    curOtherItems = new List<BearingItemsGroup>();

            //набираем списки деталей для кокретного подшипника
            foreach (var KVP in paramArrOrder.BearingType.ValidBearingItemTypes)
            {
                var curItemType = KVP.Value;
                var curItemsEnumerable = from curItemGroup in paramItemsGroups
                                         where (curItemGroup.ItemType.Type == curItemType.Type) && (curItemGroup.ItemCount > 0)
                                         select curItemGroup;
                var curItemsList = curItemsEnumerable.ToList();
                if (curItemType.Type == "04")
                {
                    cur04Items = new List<BearingItemsGroup>(curItemsList);
                }
                else
                {
                    curOtherItems.AddRange(curItemsList);
                }
            }

            //ищем перекрестные заказы для шара и одной из деталей
            if ((cur04Items.Count > 0) && (curOtherItems.Count > 0))
            {
                cur04Items.Sort((x, y) => x.ItemCount.CompareTo(y.ItemCount));
                curOtherItems.Sort((x, y) => x.ItemCount.CompareTo(y.ItemCount));
                bool bOrdersComplete = false;

                for (int i04ItemNumber = 0; (i04ItemNumber < cur04Items.Count) && (!bOrdersComplete); i04ItemNumber++)
                {
                    var cur04Group = cur04Items[i04ItemNumber];
                    for (int iOtherItemNumber = 0; (iOtherItemNumber < curOtherItems.Count) && (!bOrdersComplete); iOtherItemNumber++)
                    {
                        var curOtherItemGroup = curOtherItems[iOtherItemNumber];
                        if (curOtherItemGroup.ItemCount > 0)
                        {
                            AddOrdersToSolution(paramArrOrder, new List<BearingItemsGroup> { cur04Group, curOtherItemGroup }, ref iNeededBearingCount, paramSolution);

                            if (iNeededBearingCount == 0) { bOrdersComplete = true; }

                        }
                    }
                }


                if (iNeededBearingCount > 0)//перекрестных заказов недостаточно, делаем заказы от шара
                {
                    for (int i04ItemNumber = 0; (i04ItemNumber < cur04Items.Count) && (!bOrdersComplete); i04ItemNumber++)
                    {
                        var cur04Group = cur04Items[i04ItemNumber];
                        if (cur04Group.ItemCount > 0)
                        {
                            AddOrdersToSolution(paramArrOrder, new List<BearingItemsGroup> { cur04Group }, ref iNeededBearingCount, paramSolution);

                            if (iNeededBearingCount == 0) { bOrdersComplete = true; }
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

                FindSolution(curArrOrder, curPossibleBearingGroups, curItemsGroups, curArrOrder.ArrangedBearings);
                curArrOrder.ArrangedBearings.Sort((x, y) => x.BearingItemsGroups["04"].Size1.CompareTo(y.BearingItemsGroups["04"].Size1));

                if (curArrOrder.ArrangedBearingsCount() < curArrOrder.OrderCount)
                {
                    //скомплектовалось недостаточно, надо дозаказывать
                    MakeOrders(curArrOrder, curItemsGroups, curArrOrder.NotCompletedBearings);
                }
            }
        }
    }
}
