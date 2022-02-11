using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameRoyak.Enums;
using GameRoyak.Logic;
using GameRoyak.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FilePath = System.IO.Path;

namespace GameRoyak.Pages
{
    public partial class Shop : Page
    {
        private Random _random;
        private readonly string _directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private List<Item> _addedItems = new List<Item>();
        private Player Player { get; set; }
        
        private RelayCommand _commandFetch;
        public RelayCommand CommandFetch
        {
            get
            {
                return _commandFetch ?? (_commandFetch = new RelayCommand(obj =>
                    {
                        Player.Items.Add(_addedItems[int.Parse(obj.ToString())]);
                        Player.HP += _addedItems[int.Parse(obj.ToString())].Hp;
                        Player.CurrentHP += _addedItems[int.Parse(obj.ToString())].Hp;
                        Player.Damage += _addedItems[int.Parse(obj.ToString())].Damage;
                        Player.Armor += _addedItems[int.Parse(obj.ToString())].Armor;
                        Player.Coins -= _addedItems[int.Parse(obj.ToString())].Price;
                        StatePage.State = StatesWindow.LevelField;
                    }
                ));

            }
        }
        public Shop(Player player)
        {
            InitializeComponent();
            Exit.Click += ExitOnClick;
            Player = player;
            _random = new Random();
            Item1Button.Command = CommandFetch;
            Item2Button.Command = CommandFetch;
            Item3Button.Command = CommandFetch;
            Item1Button.CommandParameter = 0;
            Item2Button.CommandParameter = 1;
            Item3Button.CommandParameter = 2;
            var allJson = JObject.Parse(File.ReadAllText(FilePath.Combine(_directory, "Items.json")));
            var numberItems = int.Parse(allJson["numberItems"].ToString());
            var items = JsonConvert.DeserializeObject<List<Item>>(allJson["items"].ToString());
            for (var i = 0; i < 3; i++)
            {
                var numItem = _random.Next(0, numberItems);
                SetCardItem(items[numItem], i);
                _addedItems.Add(items[numItem]);
                items.RemoveAt(numItem);
                numberItems--;
            }
        }

        private void ExitOnClick(object sender, RoutedEventArgs e)
        {
            StatePage.State = StatesWindow.LevelField;
        }

        private void SetCardItem(Item item, int numCard)
        {
            var gridItem = GetNameGrid(numCard);
            var gridButton = GetNameButton(numCard);
            var icon = new Image
            { Source = new BitmapImage(new Uri(FilePath.Combine(_directory, item.Icon))) };
            var name = new TextBlock { Text = item.Name };
            var damage = new TextBlock { Text = item.Damage.ToString() };
            var armor = new TextBlock { Text = item.Armor.ToString() };
            var hp = new TextBlock { Text = item.Hp.ToString() };
            var price = new TextBlock { Text = item.Price.ToString() };
            if (int.Parse(price.Text) > Player.Coins)
            {
                gridItem.Opacity = 0.5;
                gridButton.IsEnabled = false;
            }
            Grid.SetRow(icon, 0);
            Grid.SetColumn(icon, 1);
            gridItem.Children.Add(icon);

            Grid.SetRow(name, 1);
            name.VerticalAlignment = VerticalAlignment.Center;
            name.HorizontalAlignment = HorizontalAlignment.Center;
            name.FontSize = 14;
            gridItem.Children.Add(name);

            if (hp.Text != "0")
            {
                Grid.SetRow(hp, 2);
                hp.VerticalAlignment = VerticalAlignment.Center;
                hp.HorizontalAlignment = HorizontalAlignment.Center;
                hp.Text = "Здоровье: " + hp.Text;
                gridItem.Children.Add(hp);
            }

            if (damage.Text != "0")
            {
                Grid.SetRow(damage, 3);
                damage.VerticalAlignment = VerticalAlignment.Center;
                damage.HorizontalAlignment = HorizontalAlignment.Center;
                damage.Text = "Урон: " + damage.Text;
                gridItem.Children.Add(damage);
            }

            if (armor.Text != "0")
            {
                Grid.SetRow(armor, 4);
                armor.VerticalAlignment = VerticalAlignment.Center;
                armor.HorizontalAlignment = HorizontalAlignment.Center;
                armor.Text = "Защита: " + armor.Text;
                gridItem.Children.Add(armor);
            }
                
            Grid.SetRow(price, 5);
            Grid.SetColumn(price, 0);
            price.VerticalAlignment = VerticalAlignment.Center;
            price.HorizontalAlignment = HorizontalAlignment.Center;
            price.Text = "Цена: " + price.Text + " монет";
            gridItem.Children.Add(price);
        }

        private Grid GetNameGrid(int num)
        {
            switch (num)
            {
                case 0:
                    return Item1;
                case 1:
                    return Item2;
                case 2:
                    return Item3;
            }

            return null;
        }
        
        private Button GetNameButton(int num)
        {
            switch (num)
            {
                case 0:
                    return Item1Button;
                case 1:
                    return Item2Button;
                case 2:
                    return Item3Button;
            }

            return null;
        }
    }
}