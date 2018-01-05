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
        public List<BearingItemsGroup> GrindingOrders = new List<BearingItemsGroup>();

        private void CheckArrangement(List<BearingItemsGroup> paramPreviousItemGroups, int paramLevel, int paramMaxLevel, BearingType paramBearingType, List<List<BearingItemsGroup>> paramItemsGroupsList, List<BearingGroup> paramPossibleBearingGroups)
        {
            if (paramLevel <= paramMaxLevel)
            {
                foreach (BearingItemsGroup curItemGroupList in paramItemsGroupsList[paramLevel])
                {
                    List<BearingItemsGroup> curItemsGroups = new List<BearingItemsGroup>(paramPreviousItemGroups)
                    {
                        curItemGroupList
                    };
                    CheckArrangement(curItemsGroups, paramLevel + 1, paramMaxLevel, paramBearingType, paramItemsGroupsList, paramPossibleBearingGroups);
                }
            }
            else
            {
                BearingGroup curBearingGroup = new BearingGroup()
                {
                    Type = paramBearingType
                };
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

            //будем комплектовать по массовости шара, которая была получена до запуска комплектовки
            var IEnumItemsGroups = from qItemsGroup in paramItemsGroups
                                   where qItemsGroup.ItemType.Type == "04"
                                   orderby qItemsGroup.SortOrder descending
                                   select qItemsGroup;
            paramItemsGroups = IEnumItemsGroups.ToList();

            //paramItemsGroups.Sort((x, y) => y.ArrangementCount.CompareTo(x.ArrangementCount));//сортировка от наименее к наиболее востребованным деталям

            for (ItemNumber = 0; (ItemNumber < paramItemsGroups.Count) && (ArrOrderCount > 0); ItemNumber++)
            {
                var CurItemGroup = paramItemsGroups[ItemNumber];
                if ((CurItemGroup.ItemCount > 0) && (CurItemGroup.ArrangementCount > 0))
                {
                    matches = FindBearingGroupsOfItemGroupInList(CurItemGroup, paramPossibleBearingGroups).OrderBy(RD => RD.Rad1Devation()).ThenByDescending(BC => BC.Count).ToList();
                    //matches.OrderBy(RD => RD.Rad1Devation()).ThenBy(BC => BC.Count);

                    //matches.Sort((x, y) => x.Rad1Devation().CompareTo(y.Rad1Devation()));

                    if (matches.Count > 0)
                    {
                        //перебираем все группы для данной детали, пока она не закончится
                        foreach (var curPossibleBearingGroup in matches)
                        {
                            BearingGroup curSolutionGroup = new BearingGroup(curPossibleBearingGroup);
                            curSolutionGroup.SetCount(Math.Min(curSolutionGroup.GetCount(), ArrOrderCount));
                            int iMinArrangeCount = Math.Min(ArrOrderCount, paramArrOrder.BearingType.MinArrangeCount);
                            if (curSolutionGroup.Count >= iMinArrangeCount)
                            {
                                ArrOrderCount -= curSolutionGroup.Count;
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
                                if (ArrOrderCount == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if (ArrOrderCount == 0)
                {
                    break;
                }

            }
        }

        private void AddOrdersToSolution(BearingsArrangementOrder paramArrOrder, List<BearingItemsGroup> paramUsedItems, ref int paramNeededBearingCount, List<NotCompleteBearingGroup> paramSolution)
        {
            var curNeededItems = paramArrOrder.BearingType.CreateItemGroupsToCompleteOrder(paramUsedItems);
            if (curNeededItems.Count > 0)
            {
                var curNotCompletedGroup = new NotCompleteBearingGroup()
                {
                    Type = paramArrOrder.BearingType
                };
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
                    if (curNotCompletedGroup.CheckMinItemsOrderCount())
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

        }

        private void MakeOrders(BearingsArrangementOrder paramArrOrder, List<BearingItemsGroup> paramItemsGroups, List<NotCompleteBearingGroup> paramSolution)
        //формирование заказов на поступление колец на сборку
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
            if ((cur04Items.Count > 0))
            {
                cur04Items.Sort((x, y) => y.SortOrder.CompareTo(x.SortOrder));
                curOtherItems.Sort((x, y) => y.ItemCount.CompareTo(x.ItemCount));
                bool bOrdersComplete = false;



                for (int i04ItemNumber = 0; (i04ItemNumber < cur04Items.Count) && (!bOrdersComplete); i04ItemNumber++)
                {
                    var cur04Group = cur04Items[i04ItemNumber];
                    //поищем соседние размеры шара в пределах +/- допуск на диаметр/2
                    var cur04ItemsGoodSize = (from qItem04 in cur04Items
                                              where (qItem04.Size1 >= (cur04Group.Size1 - cur04Group.ItemType.Size1MinDifference / 2)) && (qItem04.Size1 <= (cur04Group.Size1 + cur04Group.ItemType.Size1MinDifference / 2)) && (qItem04.ItemCount > 0)
                                              orderby qItem04.SortOrder descending
                                              select qItem04).ToList();
                    //cur04ItemsGoodSize.Sort((x, y) => y.SortOrder.CompareTo(x.SortOrder));
                    for (int i04ItemNumber1 = 0; (i04ItemNumber1 < cur04ItemsGoodSize.Count) && (!bOrdersComplete); i04ItemNumber1++)
                    {
                        var cur04Group1 = cur04ItemsGoodSize[i04ItemNumber1];
                        for (int iOtherItemNumber = 0; (iOtherItemNumber < curOtherItems.Count) && (!bOrdersComplete); iOtherItemNumber++)
                        {
                            var curOtherItemGroup = curOtherItems[iOtherItemNumber];
                            if (curOtherItemGroup.ItemCount > 0)
                            {
                                AddOrdersToSolution(paramArrOrder, new List<BearingItemsGroup> { cur04Group1, curOtherItemGroup }, ref iNeededBearingCount, paramSolution);

                                if (iNeededBearingCount == 0)
                                {
                                    bOrdersComplete = true;
                                }
                            }
                        }
                    }
                }

                if (iNeededBearingCount > 0)//перекрестных заказов недостаточно, делаем заказы от шара
                {
                    for (int i04ItemNumber = 0; (i04ItemNumber < cur04Items.Count) && (!bOrdersComplete); i04ItemNumber++)
                    {
                        var cur04Group = cur04Items[i04ItemNumber];
                        //поищем соседние размеры шара в пределах +/- допуск на диаметр/2
                        var cur04ItemsGoodSize = (from qItem04 in cur04Items
                                                  where (qItem04.Size1 >= cur04Group.Size1 - cur04Group.ItemType.Size1MinDifference / 2) && (qItem04.Size1 <= cur04Group.Size1 + cur04Group.ItemType.Size1MinDifference / 2)
                                                         && (qItem04.ItemCount > 0)
                                                  orderby qItem04.SortOrder descending
                                                  select qItem04).ToList();
                        //cur04ItemsGoodSize.Sort((x, y) => y.ItemCount.CompareTo(x.ItemCount));
                        for (int i04ItemNumber1 = 0; (i04ItemNumber1 < cur04ItemsGoodSize.Count) && (!bOrdersComplete); i04ItemNumber1++)
                        {
                            var cur04Group1 = cur04ItemsGoodSize[i04ItemNumber1];
                            AddOrdersToSolution(paramArrOrder, new List<BearingItemsGroup> { cur04Group1 }, ref iNeededBearingCount, paramSolution);

                            if (iNeededBearingCount == 0) { bOrdersComplete = true; }
                        }
                    }
                }
            }
        }

        public void DoArrangement()
        {
            var CurrentSolution = new List<BearingGroup> { };
            List<BearingItemsGroup> curGrindingOrders = new List<BearingItemsGroup>();

            //отсортировать шарики по количеству в кучах
            var cur04Items = from qGroups in ItemsGroups
                             where qGroups.ItemType.Type == "04"
                             orderby qGroups.ItemCount
                             select qGroups;
            int iItemGroupNumber = 0;
            foreach (var curItemGroup in cur04Items)
            {
                curItemGroup.SortOrder = iItemGroupNumber;
                iItemGroupNumber++;
            }

            //сначала комплектуем все заказы
            foreach (BearingsArrangementOrder curArrOrder in BearingArrOrders)
            {

                //List<BearingGroup> PossibleBearingGroups = new List<BearingGroup> { };
                List<List<BearingItemsGroup>> curItemsGroupsLists = new List<List<BearingItemsGroup>> { };
                List<BearingGroup> curPossibleBearingGroups = new List<BearingGroup> { };
                List<BearingItemsGroup> curItemsGroups = new List<BearingItemsGroup> { };
                List<BearingItemsGroup> curBearingItems = new List<BearingItemsGroup> { };

                foreach (var curItemType in curArrOrder.BearingType.ValidBearingItemTypes)
                {
                    IEnumerable<BearingItemsGroup> curItemGroups = from qGroups in ItemsGroups
                                                                   where (qGroups.ItemType.Description == curItemType.Value.Description) && (qGroups.ItemType.CharachteristicID == curItemType.Value.CharachteristicID) && (qGroups.ItemCount > 0)
                                                                   && ((curArrOrder.BearingType.ValidBearingItemsSize1Max[curItemType.Key] >= qGroups.Size1) && (curArrOrder.BearingType.ValidBearingItemsSize1Min[curItemType.Key] <= qGroups.Size1))
                                                                   select qGroups;
                    curItemsGroupsLists.Add(curItemGroups.ToList());
                    curItemsGroups.AddRange(curItemGroups.ToList());
                }

                int iValidItemTypesCount = curArrOrder.BearingType.ValidBearingItemTypes.Count();
                CheckArrangement(curBearingItems, 0, iValidItemTypesCount - 1, curArrOrder.BearingType, curItemsGroupsLists, curPossibleBearingGroups);

                FindSolution(curArrOrder, curPossibleBearingGroups, curItemsGroups, curArrOrder.ArrangedBearings);
                curArrOrder.ArrangedBearings.Sort((x, y) => x.BearingItemsGroups["04"].Size1.CompareTo(y.BearingItemsGroups["04"].Size1));
            }

            //потом делаем заказы на докомплектовку
            foreach (BearingsArrangementOrder curArrOrder in BearingArrOrders)
            {
                List<BearingItemsGroup> curItemsGroups = new List<BearingItemsGroup> { };

                foreach (var curItemType in curArrOrder.BearingType.ValidBearingItemTypes)
                {
                    IEnumerable<BearingItemsGroup> curItemGroups = from qGroups in ItemsGroups
                                                                   where (qGroups.ItemType.Description == curItemType.Value.Description) && (qGroups.ItemType.CharachteristicID == curItemType.Value.CharachteristicID) && (qGroups.ItemCount > 0)
                                                                   && ((curArrOrder.BearingType.ValidBearingItemsSize1Max[curItemType.Key] >= qGroups.Size1) && (curArrOrder.BearingType.ValidBearingItemsSize1Min[curItemType.Key] <= qGroups.Size1))
                                                                   select qGroups;
                    curItemsGroups.AddRange(curItemGroups.ToList());
                }

                if (curArrOrder.ArrangedBearingsCount() < curArrOrder.OrderCount)
                {
                    //скомплектовалось недостаточно, надо дозаказывать
                    MakeOrders(curArrOrder, curItemsGroups, curArrOrder.NotCompletedBearings);

                    //соберем в кучу одинаковые заказы на детали
                    foreach (var curNotCompletedBearing in curArrOrder.NotCompletedBearings)
                    {
                        foreach (var NeededGroup in curNotCompletedBearing.NeededBearingItemsGroups)
                        {
                            var curItemGroup = from qGrOrder in curGrindingOrders
                                               where (qGrOrder.ItemType == NeededGroup.Value.ItemType) && (qGrOrder.Size1 == NeededGroup.Value.Size1)
                                               select qGrOrder;
                            if (curItemGroup.Count() > 0)
                            {
                                curItemGroup.First().ItemCount += curNotCompletedBearing.Type.BearingItemsCount[NeededGroup.Value.ItemType.Type] * curNotCompletedBearing.Count;
                            }
                            else
                            {
                                var curGroup = new BearingItemsGroup()
                                {
                                    ItemType = NeededGroup.Value.ItemType,
                                    Size1 = NeededGroup.Value.Size1,
                                    ItemCount = curNotCompletedBearing.Type.BearingItemsCount[NeededGroup.Value.ItemType.Type] * curNotCompletedBearing.Count
                                };
                                curGrindingOrders.Add(curGroup);
                            }
                        }
                    }
                }
            }

            GrindingOrders = curGrindingOrders.OrderBy(ItemType => ItemType.ItemType.Type).ThenBy(Size1 => Size1.Size1).ToList();

        }
    }
}
