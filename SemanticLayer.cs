using System;
using System.Text.RegularExpressions;

namespace ShannonHuffmanBenchmark.Services
{
    public static class SemanticLayer
    {
        // LỚP LỌC 1 (Mô phỏng Low-Attention): Các cụm từ mào đầu, thủ tục, khách sáo
        // Hệ thống sẽ nhận diện và bóc toàn bộ cụm này ra mà không làm hỏng câu
        private static readonly string[] NoisePhrases =
        {
            "nhân dịp kỷ niệm", "nhân dịp", "kính gửi các bậc", "kính gửi các", "kính gửi",
            "thông báo tới toàn thể", "thông báo tới", "thông báo về việc", "thông báo về", "thông báo",
            "vì lý do", "do lý do", "để đảm bảo", "với mục đích", "xin trân trọng",
            "trân trọng kính mời", "đề nghị mọi người", "yêu cầu tất cả", "đề nghị tất cả",
            "đề nghị", "yêu cầu", "xin vui lòng", "vui lòng", "đã thực hiện", "đang thực hiện", "sẽ thực hiện"
        };

        // LỚP LỌC 2 (Mô phỏng Zero-Weight): Hư từ, từ nối, mạo từ đơn lẻ
        private static readonly string[] NoiseWords =
        {
            "và", "của", "với", "cho", "tại", "vào", "ra", "lên", "xuống", "qua", "về", "đến", "tới",
            "từ", "bởi", "vì", "nên", "do", "nhưng", "mà", "thì", "hoặc", "hay", "như", "bằng",
            "trong", "ngoài", "trên", "dưới", "trước", "sau", "giữa", "cùng",
            "là", "được", "bị", "các", "những", "một", "này", "đó", "kia", "nọ", "ấy", "vậy", "thế",
            "tôi", "bạn", "anh", "chị", "em", "ông", "bà", "chúng", "họ", "nó",
            "đã", "đang", "sẽ", "vừa", "mới", "cũng", "vẫn", "còn", "không", "chưa", "chẳng",
            "cái", "con", "chiếc", "sự", "việc"
        };

        /// <summary>
        /// Trích xuất Trục Ngữ nghĩa (Semantic Core) bao quát cho mọi văn bản.
        /// Áp dụng biểu thức chính quy (Regex) để không làm vỡ cấu trúc từ ghép Tiếng Việt.
        /// </summary>
        public static string ExtractSemanticCore(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            string processed = text;

            // BƯỚC 1: Bóc tách các cụm từ nhiễu dài (Loại bỏ mào đầu an toàn)
            foreach (var phrase in NoisePhrases)
            {
                // (?i) giúp Regex không phân biệt hoa/thường
                // \b đảm bảo chỉ xóa đúng cụm từ độc lập
                processed = Regex.Replace(processed, $@"(?i)\b{phrase}\b", " ");
            }

            // BƯỚC 2: Loại bỏ dấu câu thừa (chỉ giữ lại chữ và số để tạo luồng bit sạch)
            processed = Regex.Replace(processed, @"[^\p{L}\p{N}\s]", " ");

            // BƯỚC 3: Quét và bóc các hư từ đơn lẻ
            foreach (var word in NoiseWords)
            {
                // Regex \b (Word Boundary) cực kỳ quan trọng: 
                // Nó đảm bảo xóa chữ "có", nhưng KHÔNG xóa chữ "có" nằm bên trong chữ "cóc" hay "cáo".
                processed = Regex.Replace(processed, $@"(?i)\b{word}\b", " ");
            }

            // BƯỚC 4: Chuẩn hóa khoảng trắng (Xóa các khoảng trắng thừa do việc bóc từ để lại)
            processed = Regex.Replace(processed, @"\s+", " ").Trim();

            return processed;
        }
    }
}