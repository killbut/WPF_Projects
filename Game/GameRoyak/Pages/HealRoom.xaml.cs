using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameRoyak.Enums;
using GameRoyak.Logic;
using GameRoyak.Models;
using FilePath = System.IO.Path;

namespace GameRoyak.Pages
{
    public partial class HealRoom : Page
    {
        private readonly Player _player;

        public HealRoom(Player player)
        {
            _player = player;
            InitializeComponent();
            var bonfire = new Image
            {
                Source = new BitmapImage(new Uri(FilePath.Combine(Settings.Directory, "Images/Heal.png"))),
                Stretch = Stretch.Uniform
            };
            Grid.SetRow(bonfire, 1);
            Grid.SetColumn(bonfire, 1);
            MenuGrid.Children.Add(bonfire);
            YesButton.Click += YesButtonOnClick;
            NoButton.Click += NoButtonOnClick;
        }

        private void NoButtonOnClick(object sender, RoutedEventArgs e)
        {
            FieldProvider.Field[_player.X][_player.Y].IsVisited = false;
            FieldProvider.Field[_player.X][_player.Y].IsWorking = true;
            StatePage.State = StatesWindow.LevelField;
        }

        private void YesButtonOnClick(object sender, RoutedEventArgs e)
        {
            FieldProvider.Field[_player.X][_player.Y].IsWorking = false;
            if (_player.CurrentHP < _player.HP)
            {
                _player.CurrentHP = _player.HP;
                _player.Armor = _player.MaxArmor;
            }

            StatePage.State = StatesWindow.LevelField;
        }
    }
}