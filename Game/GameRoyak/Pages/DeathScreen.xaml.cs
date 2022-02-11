using System;
using System.Reflection;
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
    public partial class DeathScreen : Page
    {
        private readonly string _directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private Player Player { get; set; }
        public DeathScreen()
        {
            InitializeComponent();
            MenuGrid.Background = new ImageBrush(new BitmapImage(new Uri(FilePath.Combine(_directory, $"Images/BackgroundDead.png"))));
            RestartButton.Click += RestartButtonOnClick;
        }

        private void RestartButtonOnClick(object sender, RoutedEventArgs e)
        {
            StatePage.State = StatesWindow.Main;
        }
    }
}