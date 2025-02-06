using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GW2TPBrowser
{
    class GW2TPItem
    {
        public int ItemID { get; set; }
        public string? ItemName { get; set; }

        [JsonIgnore]
        public GW2TPItemStats? itemStats { get; set; }
        [JsonIgnore]
        public GW2TPPrices? itemPrices { get; set; }

        public async Task GetItemPrices()
        {
            using (HttpClient hc = new())
            {
                string response = await hc.GetStringAsync($"https://api.guildwars2.com/v2/commerce/prices/{ItemID}");
                itemPrices = JsonConvert.DeserializeObject<GW2TPPrices>(response);
            }
        }

        public async Task GetItemStats()
        {
            using (HttpClient hc = new())
            {
                try
                {
                    string response = await hc.GetStringAsync($"https://api.guildwars2.com/v2/items/{ItemID}");
                    itemStats = JsonConvert.DeserializeObject<GW2TPItemStats>(response);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Error with ItemID {ItemID} : {e}");
                    itemStats = new();
                }
            }
        }
    }
}
