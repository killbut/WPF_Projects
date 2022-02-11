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
using Newtonsoft.Json.Linq;
using WpfAnimatedGif;
using FilePath = System.IO.Path;

namespace GameRoyak.Pages
{
    public partial class MainMenu : Page
    {
        private readonly string _directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private Player Player { get; set; }
        public MainMenu(Player player)
        {
            InitializeComponent();
            Player = player;
            Settings.InitializeSettings();
            MenuGrid.Background = new ImageBrush(new BitmapImage(new Uri(FilePath.Combine(_directory, $"Images/BackgroundMenu.jpg"))));
            if (StatePage.IsStartGame)
            {
                StartButton.IsEnabled = false;
                StartButtonText.Text = "Старт";
                var classes = JObject.Parse(File.ReadAllText(FilePath.Combine(_directory, "PlayerClasses.json")));
                var classesList = new List<string>();
                foreach (var item in classes)
                    classesList.Add(item.Key);
                int classPlaceNum = 0;
                foreach (var item in classes)
                {
                    GridClasses.ColumnDefinitions.Add(new ColumnDefinition());
                    var buttonClass = new Button
                    {
                        Name = item.Key,
                        Content = item.Key,
                        FontSize = 24
                    };
                    buttonClass.HorizontalAlignment = HorizontalAlignment.Stretch;
                    buttonClass.Click += (o, e) => ButtonClass_Click(buttonClass, GridClasses);
                    Grid.SetColumn(buttonClass, classPlaceNum);
                    classPlaceNum++;
                    GridClasses.Children.Add(buttonClass);
                }
            }
            StartButton.Click += StartButtonOnClick;
            ExitButton.Click += ExitButtonOnClick;
            SettingsButton.Click += SettingsButtonOnClick;
            RestartButton.Click += RestartButtonOnClick;
            if (StatePage.IsStartGame)
                RestartButton.IsEnabled = false;
        }

        private void RestartButtonOnClick(object sender, RoutedEventArgs e)
        {
            StatePage.State = StatesWindow.Main;
        }

        private void SettingsButtonOnClick(object sender, RoutedEventArgs e)
        {
            StatePage.State = StatesWindow.Settings;
        }

        private void ButtonClass_Click(Button senderButton, Grid allButtons)
        {
            var classes = JObject.Parse(File.ReadAllText(FilePath.Combine(_directory, "PlayerClasses.json")));
            foreach (var item in allButtons.Children)
            {
                ((Button)item).IsEnabled = true;
                ((Button)item).Content = ((Button)item).Name;
                ((Button)item).FontSize = 24;
            }
            senderButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            Player.Class = senderButton.Content.ToString();
            senderButton.Content = string.Empty;
            senderButton.FontSize = 18;
            foreach (var item in classes)
            {
                if (Player.Class == item.Key)
                {
                    senderButton.Content += $"Здоровье: {item.Value["hp"]}\n";
                    senderButton.Content += $"Урон: {item.Value["damage"]}\n";
                    senderButton.Content += $"Защита: {item.Value["armor"]}\n";
                    senderButton.Content += $"Монеты: {item.Value["coins"]}";
                    Player.HP = int.Parse(item.Value["hp"].ToString());
                    Player.CurrentHP = Player.HP;
                    Player.Coins = int.Parse(item.Value["coins"].ToString());
                    Player.Items.Clear();
                    Player.X = 0;
                    Player.Y = 0;
                    Player.Damage = int.Parse(item.Value["damage"].ToString());
                    Player.Armor = int.Parse(item.Value["armor"].ToString());
                    Player.MaxArmor = int.Parse(item.Value["armor"].ToString());
                    Player.Image = new Image
                    {
                        Width = Settings.PlayerWidth,
                        Height = Settings.PlayerHeight,
                        Stretch = Stretch.Uniform
                    };

                    Player.Icon =
                        new BitmapImage(new Uri(FilePath.Combine(Settings.Directory, item.Value["icon"].ToString())));

                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(FilePath.Combine(Settings.Directory, item.Value["image"].ToString()));
                    image.EndInit();
                    Player.PathToImage = image.UriSource.ToString();
                    ImageBehavior.SetAnimatedSource(Player.Image, image);
                }
            }
        }

        private void ExitButtonOnClick(object sender, RoutedEventArgs e)
        {
            StatePage.State = StatesWindow.Close;
        }
        

        private void StartButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (Player.Class != null)
                StatePage.State = StatesWindow.LevelField;
        }
    }
}