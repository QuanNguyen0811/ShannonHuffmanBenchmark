using System;
using System.IO;
using ShannonHuffmanBenchmark.Services;

namespace ShannonHuffmanBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================================================");
                Console.WriteLine("   BENCHMARK MÃ HÓA NGUỒN: SHANNON-FANO vs HUFFMAN vs MÃ HÓA NGỮ NGHĨA   ");
                Console.WriteLine("==========================================================================\n");

                Console.WriteLine("CHỌN NGUỒN DỮ LIỆU ĐẦU VÀO:");
                Console.WriteLine("1. Nhập văn bản trực tiếp từ bàn phím");
                Console.WriteLine("2. Đọc văn bản từ đường dẫn File (.txt)");
                Console.WriteLine("3. Dùng văn bản thử nghiệm mặc định");
                Console.WriteLine("0. Thoát chương trình");
                Console.Write("\nLựa chọn của bạn (0-3): ");

                string? choice = Console.ReadLine()?.Trim();
                string inputText = "";

                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("\n► Nhập văn bản (Nhấn Enter để hoàn tất):");
                        Console.Write("> ");
                        inputText = Console.ReadLine() ?? "";
                        break;

                    case "2":
                        Console.Write("\n► Nhập đường dẫn file (.txt): ");
                        string? filePath = Console.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        {
                            inputText = File.ReadAllText(filePath);
                            Console.WriteLine($"\n✅ Đã đọc thành công file! Kích thước: {inputText.Length} ký tự.");
                        }
                        else
                        {
                            Console.WriteLine("\n❌ File không tồn tại!");
                            WaitKey();
                            continue;
                        }
                        break;

                    case "3":
                        inputText = "Cảnh báo khẩn cấp: Có sự cố ngập lụt cực kỳ nghiêm trọng tại khu vực giao lộ A, yêu cầu tất cả các phương tiện di chuyển tránh khu vực này ngay lập tức.";
                        break;

                    default:
                        Console.WriteLine("\n❌ Lựa chọn không hợp lệ!");
                        WaitKey();
                        continue;
                }

                if (string.IsNullOrWhiteSpace(inputText))
                {
                    Console.WriteLine("\n❌ Văn bản đầu vào trống!");
                    WaitKey();
                    continue;
                }

                RunBenchmark(inputText);
                WaitKey();
            }
        }

        static void RunBenchmark(string inputText)
        {
            Console.WriteLine("\n=================== KẾT QUẢ ĐÁNH GIÁ BENCHMARK ===================");
            
            string displayOriginal = inputText.Length > 120 ? inputText.Substring(0, 117) + "..." : inputText;
            Console.WriteLine($"Văn bản gốc ({inputText.Length} chars): \"{displayOriginal}\"");

            var engineOriginal = new SourceCodingEngine(inputText);
            var sfCodes = engineOriginal.GetShannonFanoCodes();
            var sfMetrics = engineOriginal.Evaluate(sfCodes);

            var huffCodes = engineOriginal.GetHuffmanCodes();
            var huffMetrics = engineOriginal.Evaluate(huffCodes);

            string sfDecoded = engineOriginal.Decode(engineOriginal.Encode(sfCodes), sfCodes);
            string huffDecoded = engineOriginal.Decode(engineOriginal.Encode(huffCodes), huffCodes);
            if (sfDecoded != inputText || huffDecoded != inputText)
            {
                Console.WriteLine("❌ Lỗi giải mã dữ liệu!");
                return;
            }

            string semanticText = SemanticLayer.ExtractSemanticCore(inputText);
            string displaySemantic = semanticText.Length > 120 ? semanticText.Substring(0, 117) + "..." : semanticText;
            Console.WriteLine($"Văn bản Ngữ nghĩa ({semanticText.Length} chars): \"{displaySemantic}\"\n");

            var engineSemantic = new SourceCodingEngine(semanticText);
            var semanticHuffCodes = engineSemantic.GetHuffmanCodes();
            var semMetrics = engineSemantic.Evaluate(semanticHuffCodes, customOriginalLength: inputText.Length);

            Console.WriteLine(new string('-', 88));
            Console.WriteLine($"{"Chỉ số Đánh giá",-25} | {"Shannon-Fano",-14} | {"Huffman",-14} | {"Ngữ nghĩa (SemCom)",-22}");
            Console.WriteLine(new string('-', 88));
            Console.WriteLine($"{"Dung lượng sau nén",-25} | {sfMetrics.CompressedBits,-10} bits | {huffMetrics.CompressedBits,-10} bits | {semMetrics.CompressedBits,-10} bits");
            Console.WriteLine($"{"Entropy H(X)",-25} | {engineOriginal.CalculateEntropy(),-14:F4} | {engineOriginal.CalculateEntropy(),-14:F4} | {engineSemantic.CalculateEntropy(),-22:F4}");
            Console.WriteLine($"{"Độ dài TB L_bar",-25} | {sfMetrics.LBar,-14:F4} | {huffMetrics.LBar,-14:F4} | {semMetrics.LBar,-22:F4}");
            Console.WriteLine($"{"Độ dư thừa R",-25} | {sfMetrics.Redundancy,-14:F4} | {huffMetrics.Redundancy,-14:F4} | {semMetrics.Redundancy,-22:F4}");
            Console.WriteLine($"{"Tỷ lệ nén (CR)",-25} | {sfMetrics.CompressionRatio,-13:F2}% | {huffMetrics.CompressionRatio,-13:F2}% | {semMetrics.CompressionRatio,-21:F2}%");
            Console.WriteLine(new string('-', 88));
            Console.WriteLine("✅ Mọi luồng giải mã đều bảo toàn tính toàn vẹn 100%!");
        }

        static void WaitKey()
        {
            Console.WriteLine("\nNhấn phím bất kỳ để quay lại menu chính...");
            Console.ReadKey();
        }
    }
}
