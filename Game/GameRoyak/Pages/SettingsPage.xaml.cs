using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using GameRoyak.Enums;
using GameRoyak.Logic;
using GameRoyak.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FilePath = System.IO.Path;

namespace GameRoyak.Pages
{
    public partial class SettingsPage : Page
    {
        public int NumVolume { get; set; }

        public SettingsPage()
        {
            InitializeComponent();
            ExitButton.Click += ExitButtonOnClick;
            SaveButton.Click += SaveButtonOnClick;
        }
        private static readonly string Directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private void SaveButtonOnClick(object sender, RoutedEventArgs e)
        {
            var settings = JObject.Parse(File.ReadAllText(FilePath.Combine(Directory, "Settings.json")));
            settings["Volume"] = double.Parse((SliderVolume.Value / 10).ToString("0.0"));
            settings["SpeedAnimation"] = int.Parse((10 - Math.Floor(SliderSpeed.Value - 1)).ToString()) * 100;
            var output = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(FilePath.Combine(Directory, "Settings.json"), output);
            Settings.InitializeSettings();
            MessageBox.Show(
                $"Настройки сохранены\n" +
                $"Громкость = {Math.Floor(SliderVolume.Value)}\n" +
                $"Скорость игрока = {Math.Floor(SliderSpeed.Value)}");
        }

        private void ExitButtonOnClick(object sender, RoutedEventArgs e)
        {
            StatePage.State = StatePage.IsStartGame ? StatesWindow.Main : StatesWindow.Menu;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd=e.NewValue;
        }
    }
}