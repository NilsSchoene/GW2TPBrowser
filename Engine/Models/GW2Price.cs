using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GW2Price
    {
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Copper { get; set; }

        public GW2Price(int totalCopper)
        {
            double totalCopperDouble = totalCopper;
            Gold = (int)(Math.Floor(totalCopperDouble / 10000));
            Silver = (int)(Math.Floor((totalCopperDouble - (Gold * 10000)) / 100));
            Copper = (int)(Math.Floor(totalCopperDouble - (Gold * 10000) - (Silver * 100)));
        }
    }
}
