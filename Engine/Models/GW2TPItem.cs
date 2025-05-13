using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GW2TPItem : BaseNotificationClass
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private int _id;
        private string _name;
        private GW2TPItemStats? _itemStats;
        private GW2TPItemPrice? _itemPrice;
        private GW2Price? _vendorPrice;
        private GW2Price? _buyPrice;
        private GW2Price? _sellPrice;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public GW2TPItemStats? ItemStats
        {
            get { return _itemStats; }
            set
            {
                _itemStats = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public GW2TPItemPrice? ItemPrice
        {
            get { return _itemPrice; }
            set
            {
                _itemPrice = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public GW2Price? VendorPrice
        {
            get { return _vendorPrice; }
            set
            {
                _vendorPrice = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public GW2Price? BuyPrice
        {
            get { return _buyPrice; }
            set
            {
                _buyPrice = value;
                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public GW2Price? SellPrice
        {
            get { return _sellPrice; }
            set
            {
                _sellPrice = value;
                OnPropertyChanged();
            }
        }

        public GW2TPItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public async Task GetItemStats()
        {
            string response = await _httpClient.GetStringAsync($"https://api.guildwars2.com/v2/items/{Id}");
            ItemStats = JsonSerializer.Deserialize<GW2TPItemStats>(response);
            if (ItemStats != null)
            {
                VendorPrice = new GW2Price(ItemStats.vendor_value);
            }
        }

        public async Task GetItemPrice()
        {
            string response = await _httpClient.GetStringAsync($"https://api.guildwars2.com/v2/commerce/prices/{Id}");
            ItemPrice = JsonSerializer.Deserialize<GW2TPItemPrice>(response);
            if (ItemPrice != null)
            {
                BuyPrice = new GW2Price(ItemPrice.buys.unit_price);
                SellPrice = new GW2Price(ItemPrice.sells.unit_price);
            }
        }
    }
}
