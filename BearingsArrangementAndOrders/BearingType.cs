using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingsArrangementAndOrders
{
    class BearingType
    {
        public string Description;
        public Dictionary<string, BearingItemType> ValidBearingItemTypes = new Dictionary<string, BearingItemType> { };
        public double? Rad1Nominal;
        public double? Rad1Min;
        public double? Rad1Max;
    }
}
