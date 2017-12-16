using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BearingsArrangementAndOrders
{
    [XmlRoot("BearingsArrangementOrder")]
    public class BearingsArrangementOrder
    {
        public BearingType BearingType;
        public int OrderCount;
        public List<BearingGroup> ArrangedBearings = new List<BearingGroup> { };
        public List<NotCompleteBearingGroup> NotCompletedBearings = new List<NotCompleteBearingGroup> { };

        public int ArrangedBearingsCount()
        {
            int iReturn = 0;
            foreach (var item in ArrangedBearings)
            {
                iReturn += item.Count;
            }
            return iReturn;
        }

        public int NotCompletedBearingsCount()
        {
            int iReturn = 0;
            foreach (var item in NotCompletedBearings)
            {
                iReturn += item.Count;
            }
            return iReturn;
        }

     }
}
