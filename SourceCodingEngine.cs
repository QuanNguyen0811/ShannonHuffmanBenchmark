using System;
using System.Collections.Generic;
using System.Linq;
using ShannonHuffmanBenchmark.Models;

namespace ShannonHuffmanBenchmark.Services
{
    public class SourceCodingEngine
    {
        public string Text { get; }
        public int TotalChars => Text.Length;
        public Dictionary<char, int> FrequencyMap { get; }
        public Dictionary<char, double> Probabilities { get; }

        public SourceCodingEngine(string text)
        {
            Text = text ?? string.Empty;
            FrequencyMap = Text.GroupBy(c => c)
                               .ToDictionary(g => g.Key, g => g.Count());
            
            Probabilities = FrequencyMap.ToDictionary(
                kvp => kvp.Key, 
                kvp => (double)kvp.Value / TotalChars
            );
        }

        public double CalculateEntropy()
        {
            double entropy = 0.0;
            foreach (var p in Probabilities.Values)
            {
                if (p > 0) entropy -= p * Math.Log2(p);
            }
            return entropy;
        }

        public Dictionary<char, string> GetShannonFanoCodes()
        {
            var sortedList = Probabilities.OrderByDescending(x => x.Value).ToList();
            var codebook = new Dictionary<char, string>();
            BuildShannonFano(sortedList, "", codebook);
            return codebook;
        }

        private void BuildShannonFano(List<KeyValuePair<char, double>> sortedList, string prefix, Dictionary<char, string> codebook)
        {
            if (sortedList.Count == 1)
            {
                codebook[sortedList[0].Key] = string.IsNullOrEmpty(prefix) ? "0" : prefix;
                return;
            }

            double total = sortedList.Sum(x => x.Value);
            double acc = 0.0;
            int splitIdx = 0;
            double minDiff = double.MaxValue;

            for (int i = 0; i < sortedList.Count; i++)
            {
                acc += sortedList[i].Value;
                double diff = Math.Abs((total - acc) - acc);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    splitIdx = i + 1;
                }
            }

            BuildShannonFano(sortedList.Take(splitIdx).ToList(), prefix + "0", codebook);
            BuildShannonFano(sortedList.Skip(splitIdx).ToList(), prefix + "1", codebook);
        }

        public Dictionary<char, string> GetHuffmanCodes()
        {
            var pq = new PriorityQueue<HuffmanNode, double>();

            foreach (var kvp in Probabilities)
            {
                pq.Enqueue(new HuffmanNode(kvp.Key, kvp.Value), kvp.Value);
            }

            if (pq.Count == 1)
            {
                var singleNode = pq.Dequeue();
                return new Dictionary<char, string> { { singleNode.Symbol!.Value, "0" } };
            }

            while (pq.Count > 1)
            {
                var left = pq.Dequeue();
                var right = pq.Dequeue();
                var parent = new HuffmanNode(null, left.Probability + right.Probability)
                {
                    Left = left,
                    Right = right
                };
                pq.Enqueue(parent, parent.Probability);
            }

            var root = pq.Dequeue();
            var codebook = new Dictionary<char, string>();
            BuildHuffmanCodes(root, "", codebook);
            return codebook;
        }

        private void BuildHuffmanCodes(HuffmanNode? node, string currentCode, Dictionary<char, string> codebook)
        {
            if (node == null) return;
            if (node.Symbol.HasValue)
            {
                codebook[node.Symbol.Value] = currentCode;
                return;
            }
            BuildHuffmanCodes(node.Left, currentCode + "0", codebook);
            BuildHuffmanCodes(node.Right, currentCode + "1", codebook);
        }

        public string Encode(Dictionary<char, string> codebook)
        {
            return string.Concat(Text.Select(c => codebook[c]));
        }

        public string Decode(string encodedStream, Dictionary<char, string> codebook)
        {
            var reverseCodebook = codebook.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var decodedChars = new List<char>();
            string currentBit = "";

            foreach (char bit in encodedStream)
            {
                currentBit += bit;
                if (reverseCodebook.TryGetValue(currentBit, out char symbol))
                {
                    decodedChars.Add(symbol);
                    currentBit = "";
                }
            }
            return new string(decodedChars.ToArray());
        }

        public (double LBar, double Redundancy, double CompressionRatio, int CompressedBits) Evaluate(Dictionary<char, string> codebook, int? customOriginalLength = null)
        {
            double lBar = Probabilities.Sum(kvp => kvp.Value * codebook[kvp.Key].Length);
            double entropy = CalculateEntropy();
            double redundancy = lBar - entropy;

            int refLength = customOriginalLength ?? TotalChars;
            int originalBits = refLength * 8;
            int compressedBits = FrequencyMap.Sum(kvp => kvp.Value * codebook[kvp.Key].Length);
            double compressionRatio = originalBits > 0 ? (1.0 - ((double)compressedBits / originalBits)) * 100 : 0;

            return (lBar, redundancy, compressionRatio, compressedBits);
        }
    }
}
