using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace GW2TPBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GW2TPItemDatabase ItemDatabase { get; set; }

        private GW2Price sellPrice = new();
        private GW2Price buyPrice = new();
        private GW2Price vendorPrice = new();

        public MainWindow()
        {
            InitializeComponent();
            ItemDatabase = new();
            ItemDatabase.LoadItemsFromFileAsync();
            UpdateList(ItemDatabase.items);
        }

        private void OnStartTypingSearchBar(object sender, TextChangedEventArgs e)
        {
            ItemDatabase.SearchItems(txtSearch.Text);
            UpdateList(ItemDatabase.searchedItems);
            lstItems.SelectedIndex = 0;
        }

        private async void OnRefreshDatabaseClick(object sender, RoutedEventArgs e)
        {
            await ItemDatabase.GetItemsFromAPIAsync();
            UpdateList(ItemDatabase.items);
        }

        private async void OnSelectedItemChange(object sender, SelectionChangedEventArgs e)
        {
            if(lstItems.SelectedItem == null) return;
            GW2TPItem item = lstItems.SelectedItem as GW2TPItem;
            await item.GetItemStats();
            await item.GetItemPrices();
            SetImage(item);
            txtItemName.Text = $"{item.ItemName}";
            txtItemType.Text = item.itemStats.type;
            txtItemLevel.Text = $"Level: {item.itemStats.level}";
            sellPrice = GoldConverted(item.itemPrices.sells.unit_price);
            buyPrice = GoldConverted(item.itemPrices.buys.unit_price);
            vendorPrice = GoldConverted(item.itemStats.vendor_value);
            txtSellG.Text = sellPrice.gold.ToString();
            txtSellS.Text = sellPrice.silver.ToString();
            txtSellC.Text = sellPrice.copper.ToString();
            txtSupply.Text = $"{item.itemPrices.sells.quantity}";
            txtBuyG.Text = buyPrice.gold.ToString();
            txtBuyS.Text = buyPrice.silver.ToString();
            txtBuyC.Text = buyPrice.copper.ToString();
            txtDemand.Text = $"{item.itemPrices.buys.quantity}";
            txtVendorG.Text = vendorPrice.gold.ToString();
            txtVendorS.Text = vendorPrice.silver.ToString();
            txtVendorC.Text = vendorPrice.copper.ToString();
            txtIngameLink.Text = $"{item.itemStats.chat_link}";
        }

        private void UpdateList(List<GW2TPItem> items)
        {
            lstItems.UnselectAll();
            lstItems.ItemsSource = null;
            lstItems.ItemsSource = items;
            lstItems.DisplayMemberPath = "ItemName";
            lstItems.SelectedIndex = 0;
        }

        private void SetImage(GW2TPItem item)
        {
            Image image = new();
            string fileURL = item.itemStats.icon;

            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileURL);
            bitmap.EndInit();
            imgItemImage.Source = bitmap;
        }

        private GW2Price GoldConverted(int price)
        {
            double priceDezimal = price;
            int priceGold = Convert.ToInt32(Math.Floor(priceDezimal / 10000));
            int priceSilver = Convert.ToInt32(Math.Floor((priceDezimal - (priceGold * 10000)) / 100));
            int priceCopper = Convert.ToInt32(Math.Floor((priceDezimal - (priceGold * 10000) - (priceSilver * 100))));
            GW2Price convertedPrice = new GW2Price();
            convertedPrice.gold = priceGold;
            convertedPrice.silver = priceSilver;
            convertedPrice.copper = priceCopper;
            
            return convertedPrice;
        }

        public void ShowError(string message)
        {
            txtError.Text = message;
        }
    }
}