using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GameRoyak.Enums;
using GameRoyak.Logic;
using GameRoyak.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FilePath = System.IO.Path;

namespace GameRoyak.Pages
{
    public partial class ChestRoom : Page
    {
        private Random _random;
        private readonly string _directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private Player Player { get; set; }
        
        private RelayCommand _commandFetch;
        public RelayCommand CommandFetch
        {
            get
            {
                return _commandFetch ?? (_commandFetch = new RelayCommand(obj =>
                    {
                        Player.Items.Add((Item) obj);
                        Player.HP += ((Item) obj).Hp;
                        Player.CurrentHP += ((Item) obj).Hp;
                        Player.Damage += ((Item) obj).Damage;
                        Player.Armor += ((Item) obj).Armor;
                        StatePage.State = StatesWindow.LevelField;
                    }
                ));

            }
        }
        
        public ChestRoom(Player player)
        {
            InitializeComponent();
            Exit.Click += ExitOnClick;
            Player = player;
            _random = new Random();
            ItemButton.Command = CommandFetch;
            var allJson = JObject.Parse(File.ReadAllText(FilePath.Combine(_directory, "Items.json")));
            var numberItems = int.Parse(allJson["numberItems"].ToString());
            var items = JsonConvert.DeserializeObject<List<Item>>(allJson["items"].ToString());
            var numItem = _random.Next(0, numberItems);
            ItemButton.CommandParameter = items[numItem];
            SetCardItem(items[numItem]);
        }
        
        private void ExitOnClick(object sender, RoutedEventArgs e)
        {
            StatePage.State = StatesWindow.LevelField;
        }

        private void SetCardItem(Item item)
        {
            var gridItem = Item;
            var icon = new Image
                {Source = new BitmapImage(new Uri(FilePath.Combine(_directory, item.Icon)))};
            var name = new TextBlock {Text = item.Name};
            var price = new TextBlock {Text = item.Price.ToString()};
            var damage = new TextBlock {Text = item.Damage.ToString()};
            var armor = new TextBlock {Text = item.Armor.ToString()};
            var hp = new TextBlock {Text = item.Hp.ToString()};
            
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

        }
    }
}