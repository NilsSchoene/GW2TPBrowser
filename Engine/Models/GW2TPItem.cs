using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GW2TPItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public GW2TPItemStats? ItemStats { get; set; }
        [JsonIgnore]
        public GW2TPItemPrice? ItemPrice { get; set; }
        
        public GW2TPItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public async Task GetItemStats()
        {
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync($"https://api.guildwars2.com/v2/items/{Id}");
                ItemStats = JsonSerializer.Deserialize<GW2TPItemStats>(response);
            }
        }

        public async Task GetItemPrice()
        {
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync($"https://api.guildwars2.com/v2/commerce/prices/{Id}");
                ItemPrice = JsonSerializer.Deserialize<GW2TPItemPrice>(response);
            }
        }
    }
}
