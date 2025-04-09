using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GW2TPItemStats
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public string Rarity { get; set; }
        public int Vendor_value { get; set; }
        public string[] Game_types { get; set; }
        public string[] Flags { get; set; }
        public object[] Restrictions { get; set; }
        public int Id { get; set; }
        public string Chat_link { get; set; }
        public string Icon { get; set; }
        public Details Details { get; set; }
    }

    public class Details
    {
        public string Type { get; set; }
    }
}
