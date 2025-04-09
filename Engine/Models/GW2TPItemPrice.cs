using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GW2TPItemPrice
    {
        public int Id { get; set; }
        public bool Whitelisted { get; set; }
        public Buys Buys { get; set; }
        public Sells Sells { get; set; }
    }

    public class Buys
    {
        public int Quantity { get; set; }
        public int Unit_price { get; set; }
    }

    public class Sells
    {
        public int Quantity { get; set; }
        public int Unit_price { get; set; }
    }
}
