# Astro-Alchemist: Hướng dẫn Phát triển MVP (Minimum Viable Product)

## Tầm nhìn MVP
Mục tiêu của phiên bản MVP là nhanh chóng xây dựng và có thể chơi thử nghiệm (playable) **Vòng lặp Core Gameplay** (Thám hiểm -> Chế tạo -> Quản lý). Đội ngũ Development cần ưu tiên tốc độ, bỏ qua các chi tiết đồ họa hoặc vật lý quá phức tạp để tập trung vào logic hệ thống cốt lõi và cảm giác chơi.

---

## Giai đoạn 1: Nền tảng & Hệ thống Cơ bản (Ưu tiên Cao nhất)
**Mục tiêu:** Xây dựng framework quản lý luồng game và di chuyển cơ bản.

1. **Quản lý Trạng thái Game (Game Manager):**
   - Khởi tạo State Machine cơ bản quản lý chu kỳ ngày/đêm: `ExplorationState` (Thám hiểm) -> `CraftingState` (Chế tạo) -> `ManagementState` (Quản lý/Chuyển ngày).
2. **Controller Người chơi & UI cơ sở:**
   - Xây dựng hệ thống góc nhìn thứ nhất (FPS Controller).
   - Tạo UI Thanh trạng thái: **Thể lực** (Stamina) và **Dưỡng khí** (Oxygen). Cài đặt logic: Dưỡng khí tụt theo thời gian sống trong môi trường, Thể lực tụt khi chạy nhanh.
   - Xây dựng Inventory (Túi đồ) ẩn: Quản lý dưới dạng Data (ID Vật phẩm, Số lượng). Dùng UI đơn giản hóa để hiển thị đồ nhặt được.
3. **Môi trường Thử nghiệm (Greybox/Blockmesh):**
   - Không cần quan tâm tới model 3D đẹp. Xây dựng một khu vực địa hình bằng các khối hộp đại diện cho map "Khu Rừng Nấm Dạ Quang" (rìa rừng phẳng và 1 hẻm nhỏ hẹp).
   - Logic Tương tác (Interaction): Bắn tia Raycast từ giữa màn hình, dùng phím (vd: 'E') để nhặt khối hộp (đại diện nguyên liệu).

---

## Giai đoạn 2: Hệ thống Nhiếp ảnh MVP
**Mục tiêu:** Người chơi có thể giơ máy lên, chụp và hệ thống trả về điểm số.

1. **Cơ chế Camera:**
   - Cho phép người chơi bật chế độ Viewfinder (Khung ngắm máy ảnh).
   - Cơ chế dò tìm V1: Bắn một lưới Raycast (ví dụ: lưới 3x3 hoặc 5x5 tia) từ ống kính khi bấm chụp.
2. **Thuật toán Chấm điểm MVP:**
   - **Khoảng cách:** Tính bằng Distance của Raycast trúng vào Collider của quái vật. Đặt tham chiếu khoảng cách tối ưu để cho điểm cao.
   - **Góc độ:** Sử dụng phép toán `Vector3.Dot` giữa hướng nhìn (forward vector) của người chơi và hướng mặt của sinh vật. (Ra 1 -> Chụp mặt; -1 -> Chụp lưng).
   - **Hành vi:** Yêu cầu đọc trạng thái (State/Enum) của con AI lúc bấm chụp.
   - *Lưu ý MVP:* Bỏ qua cơ chế tính toán phần trăm bị che khuất rườm rà. MVP chỉ cần check xem tia Center Raycast có bị cản bởi Terrain hay không.

---

## Giai đoạn 3: Trí tuệ Nhân tạo (AI) MVP
**Mục tiêu:** Hiện thực hóa hành vi của 1 loại sinh vật đại diện.

1. **NavMesh & Bầu không khí:** Setup khu vực có thể di chuyển (NavMesh) cho sinh vật.
2. **Hệ thống AI Controller:**
   - Viết Behavior Tree hoặc State Machine cho **Loài Nhút nhát** (Vd: Sóc Đuôi Đèn).
   - **Các trạng thái:**
     - `Wander`: Đi tới các điểm (waypoints) ngẫu nhiên.
     - `Alert`: Dùng Trigger Collider/Sensor để nghe tiếng động (người chơi di chuyển). Sinh vật dừng lại và cảnh giác.
     - `Flee`: Bỏ chạy ra xa người chơi nếu người chơi tiếp cận quá gần.
   - Tích hợp liên kết với Hệ thống Nhiếp Ảnh (Cung cấp biến trạng thái hiện tại để hệ thống chụp ảnh có thể đọc).

---

## Giai đoạn 4: Hệ thống Giả kim MVP
**Mục tiêu:** Mô phỏng việc đun nguyên liệu và cho ra thành phẩm thông qua số liệu/logic thay vì vật lý không gian thực.

1. **Chuyển cấp Scene/UI:**
   - Đổi góc nhìn Camera khóa cố định (Fixed Camera) vào khu vực bàn chế tạo phòng thí nghiệm.
   - Cung cấp UI đơn giản (Drag-Drop hoặc Click) để bỏ đồ từ Inventory vào Vạc nấu.
2. **Logic Vạc pha chế (Cauldron Logic):**
   - Biến số `Temperature`: Tăng dần biến này khi người chơi kích hoạt cơ chế đun lửa.
   - Component Vạc: Chứa một danh sách các nguyên liệu đang có bên trong.
   - **Check Công thức:** Kiểm tra danh sách nguyên liệu + Ngưỡng Nhiệt độ có duy trì phù hợp trong 3-5 giây không. Nếu đúng -> Trigger tạo ra Thuốc (vd: Bom Mùi, Thuốc Bước Chân Êm).
3. **Tai nạn V1:**
   - Xử lý mốc nhiệt độ: Nếu để nhiệt độ vượt quá ngưỡng giới hạn -> Gọi Function/Effect nổ -> Xóa/Reset nguyên liệu đang đun -> Trừ thêm thanh Thể Lực của người chơi ở ngày hôm sau.

---

## Giai đoạn 5: Kinh tế & Xử lý Thất bại (Hoàn thiện Vòng lặp)
**Mục tiêu:** Biến điểm số và vật phẩm thành áp lực Gameplay, kết nối chuỗi ngày chơi.

1. **Bảng Quản lý cuối ngày:**
   - UI hiển thị các bức hình đã chụp (Texture2D hoặc metadata) kèm điểm số hệ thống tự chấm.
   - Hàm quy đổi tự động: Convert `Total Score` thành Tiền tệ (Vàng).
2. **Tính năng Sinh tồn cốt lõi:**
   - Hàm `OnPlayerFainted()`: Trigger khi chết, hết Dưỡng khí hoặc Thể lực.
   - Reset: Ép LoadScene, xóa các vật phẩm chưa được lưu trú, cưỡng chế chuyển ngay sang `ManagementState` hoặc ngày tiếp theo với lời thông báo bị thương.
   - Theo dõi chu kỳ (WeeksFund): Đếm số ngày (ví dụ: chu kỳ 7 ngày). Đến Limit -> Trừ đi lượng Tiền Đóng Thuế/Phí. Nếu không đủ -> Kích hoạt màn hình Game Over.

---

## ⚠️ Nguyên tắc Thực thi Bắt buộc cho Đội Trưởng / Dev Lead
- **Fake it till you make it (Dùng thông số thay vật lý hạt):** Ở MVP, KHÔNG yêu cầu đổ shader chất lỏng cho vạc nước, không cần mô phỏng mesh tác động hạt của cái chày. Hãy sử dụng Đổi màu (Color transition) hoặc các Effect Particle hệ thống có sẵn để demo cảm giác.
- **Tunning Trực quan (Tools Support):** Dev phải public (Exposed / SerializeField) toàn bộ các tham số (Ví dụ: Các hệ số nhân khi chấm điểm nhiếp ảnh, các threshold Nhiệt độ đun sôi) ra màn hình Inspector ở Engine để Game Designer có thể tự điều chỉnh con số liên tục mà khôg cần phải nhờ Dev sửa hoặc Re-compile code liên tục.
- **Modular Code:** Các cụm tính năng về Tiền Tệ, AI logic, Controller và System Score phải độc lập (Decoupled tối đa). Sử dụng Observer Pattern / Event System để truyền-nhận tín hiệu, đảm bảo khi Giai đoạn sau update thêm quái vật hay vật phẩm sẽ không làm đổ vỡ logic base.

---

## 📝 Quy Định Cập Nhật (Commits & Changelogs)
**Yêu cầu bắt buộc đối với tất cả thành viên (Đội Dev & AI Assistant):**
Mọi thay đổi về mã nguồn, cấu trúc thư mục, hoặc thiết lập UI/UX sau khi thực hiện xong đều phải được ghi nhận rõ ràng vào `history_commit.md` để cả đội nắm được tiến độ và không bị "đụng" code (conflict).

**Format ghi commit/log:**
* **[DD/MM/YYYY] - [Tên Component/Hệ thống] - Thêm mới/Chỉnh sửa**
  - **Mô tả:** Đã code/sửa tính năng gì cụ thể.
  - **File thay đổi:** Liệt kê các script hoặc prefab bị ảnh hưởng (vd: `GameManager.cs`, `PlayerInventory.cs`).
  - **Lưu ý:** Ghi chú cách test thử hoặc các Inspector variable cần Game Designer chú ý tinh chỉnh.
