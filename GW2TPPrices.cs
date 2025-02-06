using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2TPBrowser
{

    public class GW2TPPrices
    {
        public int id { get; set; }
        public bool whitelisted { get; set; }
        public Buys buys { get; set; }
        public Sells sells { get; set; }
    }

    public class Buys
    {
        public int quantity { get; set; }
        public int unit_price { get; set; }
    }

    public class Sells
    {
        public int quantity { get; set; }
        public int unit_price { get; set; }
    }
}
