using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;

namespace GW2TPBrowser
{
    class GW2TPItemDatabase
    {
        public List<GW2TPItem> items = [];
        public List<GW2TPItemStats> stats = [];
        public List<GW2TPItem> searchedItems = [];
        private string directoryPath = Directory.GetCurrentDirectory();
        private string jsonPath = @"ItemData.json";
        private string fullPath = "";
        private MainWindow mainWindow;

        public GW2TPItemDatabase()
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
            fullPath = Path.Combine(directoryPath, jsonPath);
            Initialize();
        }

        public async void Initialize()
        {
            if (!File.Exists(fullPath))
            {
                await using FileStream createStream = File.Create(fullPath);
            }
        }

        public async Task LoadItemsFromFileAsync()
        {
            items.Clear();
            string jsonResult = GetStringFromFile(fullPath);
            if (jsonResult.Length == 0)
            {
                mainWindow.ShowError("Refresh database!");
                return;
            }
            try
            {
                items = JsonConvert.DeserializeObject<List<GW2TPItem>>(jsonResult);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e} -- Refresh Database");
                mainWindow.ShowError("Refresh database!");
            }
        }

        private string GetStringFromFile(string path)
        {
            StreamReader sr = new(path);
            string result = sr.ReadToEnd();
            return result;
        }

        private void WriteItemsToFile(string path)
        {
            StreamWriter sw = new(path);
            Debug.WriteLine($"Saved to {((FileStream)(sw.BaseStream)).Name}");
            string jsonString = JsonConvert.SerializeObject(items, Formatting.Indented);
            sw.Write(jsonString);
            sw.Flush();
            sw.Close();
        }

        public async Task GetItemsFromAPIAsync()
        {
            mainWindow.ShowError("Refreshing database. This will take a few minutes. Please wait...");
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            items.Clear();
            using (HttpClient hc = new())
            {
                string response = "";
                try
                {
                    response = await hc.GetStringAsync("https://api.guildwars2.com/v2/commerce/prices");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    mainWindow.ShowError("API unavailable. Try again later.");
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                }
                
                if (response != null)
                {
                    await GetItemNamesThrottle(response);
                }
            }
            WriteItemsToFile(fullPath);
            await LoadItemsFromFileAsync();
            mainWindow.ShowError("");
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private async Task GetItemNamesThrottle(string jsonResponse)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(jsonResponse);
            for (int i = 1; i <= (int)Math.Ceiling((decimal)((ids.Length + 200) / 200)); i++ )
            {
                int range = i * 200;
                string requestString = CreateItemIDString(ids, range);
                
                await GetItems(requestString);
            }
        }

        private async Task GetItems(string requestString)
        {
            using (HttpClient hc = new())
            {
                string response = await hc.GetStringAsync($"https://api.guildwars2.com/v2/items?ids={requestString}");
                GW2TPItemStats[] gW2TPItemStats = JsonConvert.DeserializeObject<GW2TPItemStats[]>(response);
                stats.AddRange(gW2TPItemStats);
                foreach (GW2TPItemStats stats in gW2TPItemStats)
                {
                    GW2TPItem item = new();
                    item.ItemID = stats.id;
                    item.ItemName = stats.name;
                    items.Add(item);
                }
            }
        }

        private string CreateItemIDString(int[] idList, int range)
        {
            string itemIDString = $"{idList[range - 200]}";
            for (int i = range-199; i < range; i++)
            {
                if(i < idList.Length)
                {
                    itemIDString += $",{idList[i]}";
                }
            }
            return itemIDString;
        }

        public void SearchItems(string searchText)
        {
            searchedItems.Clear();
            for (int i = 0; i < items.Count ; i++)
            {
                if (items[i].ItemName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    searchedItems.Add(items[i]);
                }
            }
        }
    }
}
