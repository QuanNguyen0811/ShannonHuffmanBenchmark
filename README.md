# 🚀 Semantic Communication vs Classical Source Coding Benchmark

![.NET](https://img.shields.io/badge/.NET-8.0%2F9.0-purple)
![Language](https://img.shields.io/badge/Language-C%23-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## 📌 Giới thiệu & Đặt vấn đề
Lý thuyết thông tin kinh điển (Shannon, 1948) tập trung vào Mã hóa nguồn không tổn hao ở **Mức Kỹ thuật (Level A)**. Các thuật toán như **Shannon-Fano** và **Huffman** nén dữ liệu dựa thuần túy trên tần suất ký hiệu mà không quan tâm đến ý nghĩa. 

Tiến tới mạng 6G và kỷ nguyên AI (Agent-to-Agent), truyền thông yêu cầu sự dịch chuyển sang **Truyền thông Ngữ nghĩa (Semantic Communication - SemCom)**. Dự án này xây dựng một công cụ Benchmark bằng C# để đối chứng hiệu năng giữa 2 hệ tư tưởng:
1. **Luồng Thống kê:** Shannon-Fano & Huffman thuần túy.
2. **Luồng Ngữ nghĩa:** Đánh giá Tầm quan trọng Ngữ nghĩa (Semantic Importance - SI), lọc nhiễu cú pháp và giữ lại thực từ cốt lõi, trước khi đưa qua lớp nén tầng dưới (Sub-layer Entropy Coding).

## 🛠️ Kiến trúc Mô phỏng

```text
                       ┌──► [Shannon-Fano Coder] ──► Bitstream (Lossless)
[Original Text (X)] ───┼──► [Huffman Coder] ───────► Bitstream (Optimal Lossless)
                       │
                       └──► [Semantic Layer] ──────► [Huffman Coder] ──► Bitstream (SemCom)
                            (Trích xuất Ngữ nghĩa)     (Mã hóa Lớp dưới)
```

## 📊 Kết quả Kỹ thuật Nổi bật
* **Tính toàn vẹn (Lossless 100%):** Thuật toán Huffman và Shannon-Fano mã hóa và giải mã chuẩn xác tuyệt đối dữ liệu.
* **Tối ưu Ký hiệu:** Huffman luôn đạt Độ dài trung bình từ mã ($\bar{L}$) và Độ dư thừa ($R$) tối ưu hơn hoặc bằng Shannon-Fano.
* **Sức mạnh SemCom:** Bằng cách áp dụng SI và giảm thiểu nhiễu cú pháp (cân bằng với hàm RDDR về quyền riêng tư), luồng nén Ngữ nghĩa giúp đẩy tỷ lệ nén ($CR$) lên mức **>80%**, giải quyết triệt để rào cản băng thông mà không làm mất đi ý định hành động ở bên nhận.

## 💻 Hướng dẫn Chạy (Interactive CLI)
```bash
git clone https://github.com/USERNAME/ShannonHuffmanBenchmark.git
cd ShannonHuffmanBenchmark
dotnet run
```
*(Chương trình hỗ trợ nhập văn bản trực tiếp từ bàn phím hoặc đọc file `.txt` để xuất bảng phân tích 5 thông số theo thời gian thực).*
