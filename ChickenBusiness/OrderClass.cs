using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenBusiness
{
    // get and set properties of an order
    class OrderClass
    {
        public Int32 senderId { get; set; }
        public Int32 cardNo { get; set; }
        public Int32 amount { get; set; }
        public DateTime timeOfOrder { get; set; }
    }
}
