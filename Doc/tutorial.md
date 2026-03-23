# Hướng Dẫn Chạy Project Astro-Alchemist (MVP)

Chào mừng bạn đến với dự án **Astro-Alchemist**! Đây là một bản nguyên mẫu (MVP) thuộc thể loại Thám hiểm Phiêu lưu kết hợp Giả kim thuật góc nhìn thứ 3 (TPS).

Tài liệu này sẽ hướng dẫn bạn cách clone code từ GitHub và chạy dự án này trên Unity Editor một cách suôn sẻ nhất.

## Yêu cầu Hệ thống
1. **Engine:** Unity Editor phiên bản `Unity 6.3 LTS` (hoặc tương đương dòng 6000.x trở lên).
2. **HĐH:** Windows, macOS, hoặc Linux (khuyến nghị có hỗ trợ OpenGL 4.5+).
3. **Phần mềm quản lý:** Unity Hub (để tải và quản lý phiên bản Unity).
4. **Git:** Đã cài đặt Git để clone repository.

## Các Bước Cài Đặt Khởi Đầu

### Bước 1: Clone Repository
Mở Command Prompt, Terminal hoặc Git Bash tại thư mục bạn muốn cài đặt game và chạy lệnh sau:

```bash
git clone <đường_link_github_của_dự_án>
cd Astro-Alchemist
```
*(Lưu ý: Do dự án đã có sẵn file `.gitignore` chuẩn Unity, kích thước tải về ban đầu sẽ rất nhẹ vì các thư mục tạm thời đã được loại bỏ).*

### Bước 2: Mở Project Bằng Unity Hub
1. Khởi động **Unity Hub**.
2. Nhấn vào nút **Add** (hoặc mũi tên xổ xuống chọn **Add project from disk**).
3. Duyệt tới thư mục `Astro-Alchemist` mà bạn vừa clone về và chọn nó.
4. Lần đầu tiên mở project, Unity sẽ mất **vài phút để biên dịch lại toàn bộ mã nguồn** và tạo lại thư mục `Library` (thư mục này đã bị bỏ qua khi clone để tiết kiệm dung lượng). Xin vui lòng kiên nhẫn chờ thanh loading.

## Hướng Dẫn Setup Scene Đầu Tiên (Exploration)

Mặc định khi clone về, nếu không tự động load Scene, bạn hãy làm theo các bước sau để thấy trò chơi hoạt động:

1. Di chuyển vào thư mục theo đường dẫn: `Assets/Scenes/`.
2. Mở (Double-click) vào Scene có sẵn tên là **`SampleScene`** (Hoặc tạo mới nếu chưa có).
3. Nhấn nút **Play (▶)** ở giữa đỉnh màn hình Unity Editor. 

### Cách Thức Điều Khiển (Góc Nhìn Thứ 3 - TPS)
*   **W A S D** hoặc Phím Mũi Tên: Di chuyển nhân vật.
*   **Chuột:** Lắc chuột để xoay camera quan sát xung quanh (Crosshair Tâm ruồi luôn hiện giữa màn hình).
*   **Giữ Left-Shift:** Chạy nước rút (Sẽ tiêu hao thanh Thể Lực - Stamina).
*   **Phím E:** Tương tác/Nhặt vật phẩm. Hãy đưa hồng tâm nhỏ xíu (Crosshair) chĩa thẳng vào các Khối Nấm (Item) cho tới khi Nấm phát sáng **Xanh Lá Cây**, sau đó bấm "E" để nhặt bỏ vào Túi Đồ.
*   **Phím ESC:** Thoát con trỏ chuột ra khỏi chế độ khóa màn hình game để thao tác với Unity Editor.

---
## Lỗi Thường Gặp & Cách Khắc Phục (Troubleshooting)

### Lỗi 1: Nhân vật rớt lọt thỏm xuống hố đen vũ trụ vô cực!
*   **Nguyên nhân:** Scene hiện tại chưa có mặt đất (Ground).
*   **Cách khắc phục:** Trên khoanh Hierarchy, Bấm *Chuột phải -> 3D Object -> Plane*. (Nhớ Reset Transform về tọa độ 0, 0, 0 để nằm dưới chân Player nhé).

### Lỗi 2: Lỗi màu hường chói lóa (Pink Textures)
*   **Nguyên nhân:** Lỗi Material của Universal Render Pipeline (URP).
*   **Cách khắc phục:** Trên menu ngang góc trái trên cùng, chọn *Window -> Rendering -> Render Pipeline Converter*. Đánh dấu tick vào *Material Upgrade* và bấm *Convert Assets*.

### Lỗi 3: Tia Raycast Tương Tác Xuyên Qua UI Không Nhặt Được Đồ
*   **Nguyên nhân:** Khi tạo UI, bạn quên chưa tắt chặn tia Laser.
*   **Cách sửa:** Nếu bạn muốn tự tạo UI Crosshair, hãy nhớ Click vào Script `Image` của nó, và **BỎ ĐÁNH DẤU (Untick)** vào dòng `Raycast Target` đi để tia nhìn có thể xuyên qua giao diện chạm tới vật phẩm 3D phía sau.

### Lỗi 4: Máy Quay (Camera) Nằm Kẹt Dưới Đất / Sai Góc
*   **Cấu trúc chuẩn TPS:** 
    *   GameObject con `CameraRoot` phải đặt ngang đầu hoặc vai của Player (Y = 0.6).
    *   `Main Camera` phải là Child (Con) của `CameraRoot`, với Position lùi nhẹ ra sau `(X = 0, Y = 0, Z = -3)`.
    *   Trong `PlayerController` kéo `CameraRoot` vào ô cắm gán camera điều hướng.

---
