using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
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

        private List<GW2TPItem> _itemList;
        private List<GW2TPItem> _searchResults;

        public List<GW2TPItem> ItemList { get; set; } = new List<GW2TPItem>();
        public List<GW2TPItem> SearchResults { get; set; } = new List<GW2TPItem>();
        public string SearchText { get; set; } = string.Empty;
        public GW2TPItem CurrentItem { get; set; }
        public bool IsRefreshing { get; set; } = false;

        #endregion

        public AppSession()
        {
            _combinedPath = Path.Combine(_directoryPath, _filePath);
            InitializeApp();
            
        }

        private async void InitializeApp()
        {
            await Task.Delay(1000); // Simulate some initialization delay
            if (File.Exists(_combinedPath))
            {
                RaiseMessage($"File found at {_combinedPath}.");
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
            GetItemsFromFile();
        }

        private async Task GetItemsFromAPIAsync()
        {
            IsRefreshing = true;
            RaiseMessage("Refreshing database...");
            // Simulate a delay for the API call
            await Task.Delay(2000);
            // Here you would normally call your API and get the data
            // For now, we will just simulate it with a new list of items
            ItemList = new List<GW2TPItem>
            {
                new GW2TPItem(1, "Item1"),
                new GW2TPItem(2, "Item2"),
                new GW2TPItem(3, "Item3")
            };
            IsRefreshing = false;
            RaiseMessage("Database refreshed.");
        }

        private async Task SaveItemsToFileAsync()
        {
            string json = JsonSerializer.Serialize(ItemList);
            await File.WriteAllTextAsync(_combinedPath, json);
            RaiseMessage("Items saved to file.");
        }

        private void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new MessageEventArgs(message));
        }
    }
}
