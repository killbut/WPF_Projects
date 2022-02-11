using System.IO;
using System.Reflection;
using GameRoyak.Pages;
using Newtonsoft.Json.Linq;
using FilePath = System.IO.Path;

namespace GameRoyak.Models
{
    public static class Settings
    {
        public static int SpeedAnimation { get; set; }
        public static double Volume { get; set; }
        public static int PlayerWidth { get; set; }
        public static int PlayerHeight { get; set; }
        public static int TorchWidth { get; set; }
        public static int TorchHeight { get; set; }
        public static double TorchOpacity { get; set; }
        public static int TorchEffectRadius { get; set; }
        public static double OpacityDarkRoom { get; set; }
        public static int NumRoomShop { get; set; }
        public static int NumRoomChest { get; set; }
        public static int NumRoomBoss { get; set; }
        public static int NumRoomHeal { get; set; }
        public static int NumRoomFight { get; set; }

        public static readonly string Directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void InitializeSettings()
        {
            var settings = JObject.Parse(File.ReadAllText(FilePath.Combine(Directory, "Settings.json")));
            SpeedAnimation = int.Parse(settings["SpeedAnimation"].ToString());
            Volume = double.Parse(settings["Volume"].ToString());
            PlayerWidth = int.Parse(settings["PlayerWidth"].ToString());
            PlayerHeight = int.Parse(settings["PlayerHeight"].ToString());
            TorchWidth = int.Parse(settings["TorchWidth"].ToString());
            TorchHeight = int.Parse(settings["TorchHeight"].ToString());
            TorchOpacity = double.Parse(settings["TorchOpacity"].ToString());
            TorchEffectRadius = int.Parse(settings["TorchEffectRadius"].ToString());
            OpacityDarkRoom = double.Parse(settings["OpacityDarkRoom"].ToString());
            NumRoomShop = int.Parse(settings["NumRoomShop"].ToString());
            NumRoomChest = int.Parse(settings["NumRoomChest"].ToString());
            NumRoomBoss = int.Parse(settings["NumRoomBoss"].ToString());
            NumRoomHeal = int.Parse(settings["NumRoomHeal"].ToString());
            NumRoomFight = int.Parse(settings["NumRoomFight"].ToString());
            LevelField.SoundBackground.Volume = Volume = double.Parse(settings["Volume"].ToString());
        }
    }
}