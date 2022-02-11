using Newtonsoft.Json;

namespace GameRoyak.Models
{
    public class ChanceCell
    {
        [JsonProperty(PropertyName = "cell1")] public int ChanceCell1 { get; set; }
        [JsonProperty(PropertyName = "cell2")] public int ChanceCell2 { get; set; }
        [JsonProperty(PropertyName = "cell3")] public int ChanceCell3 { get; set; }
        [JsonProperty(PropertyName = "cell4")] public int ChanceCell4 { get; set; }
        [JsonProperty(PropertyName = "cell5")] public int ChanceCell5 { get; set; }
        [JsonProperty(PropertyName = "cell6")] public int ChanceCell6 { get; set; }
    }
}