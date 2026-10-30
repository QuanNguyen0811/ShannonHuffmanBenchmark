namespace ShannonHuffmanBenchmark.Models
{
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public char? Symbol { get; set; }
        public double Probability { get; set; }
        public HuffmanNode? Left { get; set; }
        public HuffmanNode? Right { get; set; }

        public HuffmanNode(char? symbol, double probability)
        {
            Symbol = symbol;
            Probability = probability;
        }

        public int CompareTo(HuffmanNode? other)
        {
            if (other == null) return 1;
            return Probability.CompareTo(other.Probability);
        }
    }
}
