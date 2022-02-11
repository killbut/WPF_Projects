using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Navigation;
using GameRoyak.Enums;
using GameRoyak.Logic;
using GameRoyak.Models;
using GameRoyak.Pages;
using Newtonsoft.Json;

namespace GameRoyak
{
    public partial class MainWindow
    {
        private Player Player { get; set; }
        private Page LevelField { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            Player = new Player();
            LevelField = new LevelField(Player);
            StatePage.OnChangedNumber += NumberPageOnOnChangedNumber;
            StatePage.State = StatesWindow.Main;
            var menuFrame = new Frame
                {Content = new MainMenu(Player), NavigationUIVisibility = NavigationUIVisibility.Hidden};
            Grid.SetRow(menuFrame, 0);
            Grid.SetColumn(menuFrame,0);
            Grid.SetRowSpan(menuFrame,3);
            Grid.SetColumnSpan(menuFrame,3);
            MainGrid.Children.Add(menuFrame);
        }
        private void NumberPageOnOnChangedNumber(object sender, EventArgs e)
        {
            MainGrid.Children[0].Opacity = 0.5;
            MainGrid.Children[0].IsEnabled = false;
            switch (StatePage.State)
            {
                case StatesWindow.Main:
                    StatePage.IsStartGame = true;
                    var startFrame = new Frame
                        {Content = new MainMenu(Player), NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(startFrame, 0);
                    Grid.SetColumn(startFrame,0);
                    Grid.SetRowSpan(startFrame,3);
                    Grid.SetColumnSpan(startFrame,3);
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(startFrame);
                    break;
                case StatesWindow.Settings:
                    var settingsFrame = new Frame
                        {Content = new SettingsPage(), NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(settingsFrame, 1);
                    Grid.SetColumn(settingsFrame,1);
                    MainGrid.Children.Add(settingsFrame);
                    break;
                case StatesWindow.Menu:
                    var menuFrame = new Frame
                        {Content = new MainMenu(Player), NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(menuFrame, 1);
                    Grid.SetColumn(menuFrame,1);
                    MainGrid.Children.Add(menuFrame);
                    break;
                case StatesWindow.LevelField:
                    if (StatePage.IsStartGame)
                        LevelField = new LevelField(Player);
                    ((LevelField) LevelField).Player = Player;
                    var mainFrame = new Frame
                        {Content = LevelField, NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(mainFrame, 0);
                    Grid.SetColumn(mainFrame,0);
                    Grid.SetRowSpan(mainFrame,3);
                    Grid.SetColumnSpan(mainFrame,3);
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(mainFrame);
                    break;
                case StatesWindow.Shop:
                    var shopFrame = new Frame
                        {Content = new Shop(Player), NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(shopFrame, 1);
                    Grid.SetColumn(shopFrame,1);
                    MainGrid.Children.Add(shopFrame);
                    break;
                case StatesWindow.Heal:
                    var healFrame = new Frame
                        {Content = new HealRoom(Player), NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(healFrame, 1);
                    Grid.SetColumn(healFrame,1);
                    MainGrid.Children.Add(healFrame);
                    break;
                case StatesWindow.Chest:
                    var chestFrame = new Frame
                        {Content = new ChestRoom(Player), NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(chestFrame, 1);
                    Grid.SetColumn(chestFrame,1);
                    MainGrid.Children.Add(chestFrame);
                    break;
                case StatesWindow.Fight:
                    var fightFrame = new Frame
                        {Content = new Fight(Player), NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(fightFrame, 0);
                    Grid.SetColumn(fightFrame,0);
                    Grid.SetRowSpan(fightFrame,3);
                    Grid.SetColumnSpan(fightFrame,3);
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(fightFrame);
                    break;
                case StatesWindow.Death:
                    var deathFrame = new Frame
                        {Content = new DeathScreen(), NavigationUIVisibility = NavigationUIVisibility.Hidden};
                    Grid.SetRow(deathFrame, 0);
                    Grid.SetColumn(deathFrame,0);
                    Grid.SetRowSpan(deathFrame,3);
                    Grid.SetColumnSpan(deathFrame,3);
                    MainGrid.Children.Clear();
                    MainGrid.Children.Add(deathFrame);
                    break;
                case StatesWindow.Close:
                    Close();
                    break;
            }
        }
    }
}