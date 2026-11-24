# 🎮 Cyber Escape: The Runner (Game Né Chướng Ngại Vật 3D)

Dự án game 3D thể loại **Endless Runner / Né chướng ngại vật** hoàn chỉnh được xây dựng trên nền tảng **Unity Engine** (ngôn ngữ C#).

Dự án đã được cấu hình sẵn sàng theo kiến trúc module hóa chuẩn công nghiệp, tự động khởi tạo môi trường (Procedural Generation), có sẵn hệ thống âm thanh, đồ họa Cyberpunk và UI Canvas đầy đủ mà không cần phụ thuộc vào asset ngoài.

---

## 🌟 1. Tính năng nổi bật của trò chơi

1. **Cơ chế di chuyển 3 làn (3-Lane Movement System):**
   - Chuyển làn mượt mà giữa 3 làn đường (Trái, Giữa, Phải).
   - Thao tác nhảy cao vượt vật cản thấp và trượt người chui qua chướng ngại vật tầm cao.
2. **Hệ thống chướng ngại vật đa dạng:**
   - **Rào cản thấp (Low Barrier):** Bắt buộc phải **Nhảy**.
   - **Thanh laser cao (High Barrier):** Bắt buộc phải **Trượt**.
   - **Khối hộp chặn đường (Full Block):** Bắt buộc phải **Đổi làn**.
   - **Chướng ngại vật di động (Moving Block):** Dao động qua lại giữa các làn.
   - **Quy tắc an toàn (Solvability):** Thuật toán sinh tự động luôn đảm bảo có ít nhất 1 lối thoát khả thi cho người chơi.
3. **Vật phẩm hỗ trợ (Power-ups & Collectibles):**
   - 🪙 **Lõi năng lượng / Coins:** Tăng điểm số (+100 điểm/lõi).
   - 🛡️ **Khiên bảo vệ (Shield):** Chống đỡ 1 lần va chạm mà không bị Game Over, có hiệu ứng hào quang bảo vệ quanh nhân vật.
   - 🧲 **Nam châm (Magnet):** Tự động hút toàn bộ Coin xung quanh về phía người chơi trong 8 giây.
   - ⏱️ **Làm chậm (Slow-Motion):** Giảm tốc độ chạy trong 5 giây giúp xử lý tình huống khó.
4. **Hệ thống điểm số & Độ khó tăng dần:**
   - Tốc độ chạy tăng dần theo thời gian sinh tồn.
   - Lưu điểm kỷ lục (High Score) tự động qua `PlayerPrefs`.
5. **Giao diện người dùng (UI Canvas):**
   - **HUD:** Hiển thị điểm số, điểm kỷ lục, số Coin, tốc độ hiện tại, trạng thái Khiên và thời gian Nam châm.
   - **Game Over Panel:** Hiển thị tổng điểm, kỷ lục, nút Chơi lại (Restart).
   - **Pause Menu:** Phím ESC / P để tạm dừng hoặc tiếp tục.
6. **Âm thanh tổng hợp (Procedural Audio Synthesizer):**
   - Tích hợp sẵn bộ tạo âm thanh trực tiếp bằng code C# cho: Nhảy, Trượt, Đổi làn, Ăn xu, Kích hoạt Power-up, Vỡ khiên và Thua cuộc. Không lo lỗi thiếu file audio!

---

## 🕹️ 2. Hướng dẫn điều khiển (Controls)

| Phím bấm | Chức năng |
| :--- | :--- |
| **A** hoặc **Mũi tên Trái** (⬅️) | Đổi sang làn bên trái |
| **D** hoặc **Mũi tên Phải** (➡️) | Đổi sang làn bên phải |
| **W** / **Space** / **Mũi tên Lên** (⬆️) | Nhảy lên |
| **S** hoặc **Mũi tên Xuống** (⬇️) | Trượt người xuống |
| **ESC** hoặc **P** | Tạm dừng (Pause) / Tiếp tục (Resume) |

---

## 🚀 3. Hướng dẫn mở và chạy trên Unity

1. Mở **Unity Hub**.
2. Chọn **Add** (Thêm project) -> Chọn thư mục chứa dự án này (`c:\Users\acer\Downloads\long`).
3. Chọn phiên bản Unity (tương thích mọi phiên bản Unity từ **2020.x, 2021.x, 2022.x đến Unity 6.x**).
4. Mở Scene chính tại: `Assets/Scenes/MainScene.unity`.
5. Nhấn nút **Play** (▶️) trên thanh công cụ Unity để trải nghiệm game ngay lập tức!
   * *Mẹo:* Bạn cũng có thể vào menu trên thanh công cụ: `Tools > Cyber Escape > Setup Complete Scene` để tạo hoặc làm mới toàn bộ scene bất kỳ lúc nào.

---

## 📂 4. Cấu trúc thư mục dự án

```text
├── Assets/
│   ├── Editor/
│   │   └── SceneSetupWindow.cs       # Tool menu hỗ trợ setup scene 1-click trong Unity Editor
│   ├── Scenes/
│   │   └── MainScene.unity           # Scene trò chơi chính
│   └── Scripts/
│       ├── Audio/
│       │   └── SoundManager.cs       # Quản lý và tổng hợp âm thanh bằng code C#
│       ├── Core/
│       │   ├── GameManager.cs        # Vòng đời game, điểm số, độ khó, trạng thái
│       │   └── GameInitializer.cs    # Tự động dựng môi trường, lighting, player khi bấm Play
│       ├── Environment/
│       │   ├── GroundLooper.cs       # Vòng lặp đường băng vô tận
│       │   └── GroundTile.cs         # Xử lý sinh vật cản & coin trên từng đoạn đường
│       ├── Obstacles/
│       │   ├── Obstacle.cs           # Định nghĩa và xử lý loại chướng ngại vật
│       │   └── MovingObstacle.cs     # Chướng ngại vật chuyển động qua lại
│       ├── Pickups/
│       │   ├── PickupItem.cs         # Xử lý hiệu ứng và nhặt vật phẩm / nam châm
│       │   └── PickupType.cs         # Enum các loại vật phẩm (Coin, Shield, Magnet, SlowMo)
│       ├── Player/
│       │   ├── PlayerController.cs   # Điều khiển nhân vật 3 làn, nhảy, trượt, va chạm
│       │   └── CameraFollow.cs       # Camera góc nhìn thứ ba + hiệu ứng rung màn hình
│       └── UI/
│           └── UIManager.cs          # Quản lý giao diện HUD, Pause và Game Over
├── Packages/
│   └── manifest.json                 # Cấu hình các package cơ bản của Unity
├── ProjectSettings/                  # Cài đặt thẻ Tag (Obstacle, Pickup...), Input, Layer
├── .gitignore                        # File bỏ qua file rác (Library, Temp, obj) chuẩn Unity
├── Phan_Tich_Thiet_Ke_Game_Unity.docx# File báo cáo phân tích thiết kế game
└── README.md                         # Hướng dẫn chi tiết dự án
```

---

## 📤 5. Hướng dẫn tự đẩy dự án lên GitHub

Dự án đã có sẵn file `.gitignore` chuẩn cho Unity (giúp bỏ qua các file tạm hàng Gigabyte như `Library/`, `Temp/`, `Logs/`).

Để đưa lên GitHub cá nhân của bạn, mở **Terminal / PowerShell** tại thư mục này và chạy các lệnh sau:

```bash
# 1. Khởi tạo Git repository (nếu chưa có)
git init

# 2. Thêm tất cả các file mã nguồn và cấu hình
git add .

# 3. Tạo commit đầu tiên
git commit -m "feat: complete 3D obstacle dodger game in Unity"

# 4. Đặt tên nhánh chính là main
git branch -M main

# 5. Liên kết tới repository trên GitHub của bạn (thay URL bằng repo của bạn)
git remote add origin https://github.com/TÊN_GITHUB_CỦA_BẠN/TÊN_REPO.git

# 6. Đẩy mã nguồn lên GitHub
git push -u origin main
```

Chúc bạn hoàn thành xuất sắc đồ án và môn học! 🎉
