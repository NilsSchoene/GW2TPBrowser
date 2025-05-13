using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Engine.EventArgs;
using Engine.Models;

namespace Engine.ViewModels
{
    public class AppSession : BaseNotificationClass
    {
        public event EventHandler<MessageEventArgs> OnMessageRaised;

        #region Properties

        private readonly string _directoryPath = Directory.GetCurrentDirectory();
        private readonly string _filePath = @"ItemData.json";
        private string _combinedPath = string.Empty;

        private static readonly HttpClient _httpClient = new HttpClient();

        private ObservableCollection<GW2TPItem> _searchResults;

        public List<GW2TPItem> ItemList { get; set; } = new List<GW2TPItem>();
        public List<int> ItemIds { get; set; } = new List<int>();
        public ObservableCollection<GW2TPItem> SearchResults
        {
            get { return _searchResults; }
            set
            {
                _searchResults = value;
                OnPropertyChanged();
            }
        }
        public string SearchText { get; set; } = "";
        public GW2TPItem CurrentItem { get; set; }
        public bool IsRefreshing { get; set; } = false;

        #endregion

        public AppSession()
        {
            _combinedPath = Path.Combine(_directoryPath, _filePath);
            SearchResults = new ObservableCollection<GW2TPItem>();
            InitializeApp();
        }

        private async void InitializeApp()
        {
            await Task.Delay(1000); // Simulate some initialization delay
            if (File.Exists(_combinedPath))
            {
                RaiseMessage($"File found at {_combinedPath}.");
                Debug.WriteLine($"File found at {_combinedPath}.");
                await GetItemsFromFile();
            }
            else
            {
                RaiseMessage($"File not found at {_combinedPath}. Creating new file.");
                File.Create(_combinedPath).Close();
            }
        }

        public async Task GetItemsFromFile()
        {
            // Clear the list before loading new items
            ItemList.Clear();

            // Check if the file exists
            if (File.Exists(_combinedPath))
            {
                string json = await File.ReadAllTextAsync(_combinedPath);
                if (string.IsNullOrEmpty(json))
                {
                    RaiseMessage("File is empty. Refreash database.");
                    return;
                }
                ItemList = JsonSerializer.Deserialize<List<GW2TPItem>>(json) ?? new List<GW2TPItem>();
                CurrentItem = ItemList.FirstOrDefault();
                RaiseMessage($"Loaded {ItemList.Count} items from file. Current Item: {CurrentItem.Name}");
                SearchItem("");
            }
            else
            {
                RaiseMessage($"File not found at {_combinedPath}.");
                //File.Create(_combinedPath).Close();
            }
        }

        public async void RefreshDatabase()
        {
            await GetItemsFromAPIAsync();
            await SaveItemsToFileAsync();
            await GetItemsFromFile();
        }

        private async Task GetItemsFromAPIAsync()
        {
            IsRefreshing = true;
            ItemList.Clear();
            RaiseMessage("Refreshing database...");
            // Simulate a delay for the API call
            await Task.Delay(2000);
            // Here you would normally call your API and get the data
            // For now, we will just simulate it with a new list of items
            await GetItemIdsAsync();
            await GetItemNamesAsync();
            IsRefreshing = false;
            RaiseMessage("Database refreshed.");
        }

        private async Task GetItemIdsAsync()
        {
            string response = await _httpClient.GetStringAsync("https://api.guildwars2.com/v2/commerce/prices");
            // Simulate a delay for the API call
            await Task.Delay(2000);
            // Here you would normally call your API and get the data
            // For now, we will just simulate it with a new list of item IDs
            ItemIds = JsonSerializer.Deserialize<List<int>>(response) ?? new List<int>();
            RaiseMessage($"Loaded {ItemIds.Count} item IDs from API.");
        }

        private async Task GetItemNamesAsync()
        {
            for (int i = 0; i < ItemIds.Count + 200; i += 200)
            {
                await CallItemsInBulk(i);
            }
        }

        private async Task CallItemsInBulk(int id)
        {
            if (id >= ItemIds.Count)
            {
                return;
            }
            string requestString = $"{ItemIds[id]}";
            for (int i = id + 1; i < id + 199; i++)
            {
                if (i >= ItemIds.Count)
                {
                    break;
                }
                requestString += $",{ItemIds[i]}";
            }

            string response = await _httpClient.GetStringAsync($"https://api.guildwars2.com/v2/items?ids={requestString}");
            Debug.WriteLine($"{response}");

            List<GW2TPItemStats> items = JsonSerializer.Deserialize<List<GW2TPItemStats>>(response) ?? new List<GW2TPItemStats>();
            foreach (GW2TPItemStats itemStats in items)
            {
                if (!ItemList.Any(i => i.Id == itemStats.id))
                {
                    GW2TPItem item = new GW2TPItem(itemStats.id, itemStats.name);
                    ItemList.Add(item);
                }
            }
        }

        private async Task SaveItemsToFileAsync()
        {
            string json = JsonSerializer.Serialize(ItemList, new JsonSerializerOptions() { WriteIndented = true });
            await File.WriteAllTextAsync(_combinedPath, json);
            RaiseMessage("Items saved to file.");
        }

        public void SearchItem(string searchText)
        {
            SearchResults.Clear();
            foreach (GW2TPItem item in ItemList)
            {
                if (item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    SearchResults.Add(item);
                    RaiseMessage($"Found {SearchResults.Count} items");
                }
                else if (SearchResults.Count == 0)
                {
                    RaiseMessage($"No items found");
                }

            }
        }

        private void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new MessageEventArgs(message));
        }
    }
}
