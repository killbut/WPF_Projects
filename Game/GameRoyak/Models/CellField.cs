namespace GameRoyak.Models
{
    public class CellField
    {
        public bool WDirection { get; set; }
        public bool ADirection { get; set; }
        public bool SDirection { get; set; }
        public bool DDirection { get; set; }
        public bool IsVisited { get; set; }
        public bool IsWorking { get; set; }
        public bool IsGenerated { get; set; }
        public int CellNum { get; set; }
        public double OpacityCell { get; set; }
    }
}