using Newtonsoft.Json;

namespace GameRoyak.Models
{
    public class Item
    {
        [JsonProperty(PropertyName = "name")] public string Name { get; set; }
        
        [JsonProperty(PropertyName = "price")] public int Price { get; set; }
        
        [JsonProperty(PropertyName = "icon")] public string Icon { get; set; }
        
        [JsonProperty(PropertyName = "damage")] public int Damage { get; set; }
        
        [JsonProperty(PropertyName = "armor")] public int Armor { get; set; }
        
        [JsonProperty(PropertyName = "hp")] public int Hp { get; set; }
    }
}