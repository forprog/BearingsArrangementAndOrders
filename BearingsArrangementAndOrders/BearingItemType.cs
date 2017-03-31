using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingsArrangementAndOrders
{
    class BearingItemType
    {
        public string Description; //уникальное наименование из 1С
        public string Type;//вид детали
        public double? Size1Max;
        public double? Size1Min;
        public double? Size1MinDifference;
        public double MinOrderCount
        {
            get {
                return Math.Max(1, pMinOrderCount);
            }
            set {
                pMinOrderCount = value;
            }
        }

        private double pMinOrderCount=1;
    }
}
