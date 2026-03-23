# Kế Hoạch Phát Triển Astro-Alchemist (MVP) - TODO List

Dựa trên việc phân tích mã nguồn hiện tại của dự án (các hệ thống Camera, PlayerMovement, Alchemy, Economy, AI...), dưới đây là danh sách những tính năng/nghiệp vụ còn thiếu hoặc chỉ mới được "mock-up" (làm giả lập qua phím bấm). Bạn nên thực hiện theo thứ tự ưu tiên sau để hoàn thiện vòng lặp trò chơi (Core Gameplay Loop).

## 1. Hệ Thống Giao Diện Người Dùng (UI/UX)
Hiện tại dự án đang thiếu hệ thống UI chính thức để kết nối các State lại với nhau.

- [ ] **HUD Sinh Tồn (Exploration UI):**
  - Tạo thanh Thể Lực (Stamina) và Dưỡng Khí (Oxygen) liên kết với `currentStamina` và `currentOxygen` trong `PlayerController.cs`.
  - Hiển thị số Tiền (Gold) hiện có và Ngày hiện tại trên góc màn hình (`EconomyManager.cs` & `GameManager.cs`).
  - Thêm một Reticle/Crosshair nhỏ ở giữa màn hình cho `PlayerInteraction.cs` bắn tia raycast. Nhớ tắt `Raycast Target`.

- [ ] **Giao Diện Menu Giả Kim (Crafting UI):**
  - *Vấn đề hiện tại:* `AlchemyManager.cs` đang dùng phím cứng `Alpha1 -> Alpha4` để thả đồ vào Vạc.
  - *Việc cần làm:* Tạo một bảng Menu hiện ra khi chuyển sang chế độ `GameState.Crafting`. Hiển thị các vật phẩm đang có trong `PlayerInventory`.
  - Thêm nút hoặc cơ chế kéo-thả để chèn nguyên liệu (Ingredient) vào vạc.
  - Thêm UI thông báo Nhiệt Độ hiện tại (`currentTemperature`) của `Cauldron.cs`.
  - Thêm một nút trên UI thay cho phím `F` để Bật/Tắt lửa lò (`mainCauldron.ToggleFire()`).

- [ ] **Màn Hình Tổng Kết Ngày (Management UI):**
  - Hiển thị bảng tổng kết khi gọi hàm `GameManager.Instance.EndDay()`.
  - Báo cáo số ngày còn lại đến kỳ hạn Đóng Thuế (`daysToTax` từ `EconomyManager.cs`).
  - Thêm Nút **"Bắt Đầu Ngày Mới"** (`StartNextDay()`) để vòng lặp quay trở lại `Exploration` và reset các thông số nếu cần.

- [ ] **Màn Hình Game Over:**
  - Kích hoạt khi không đủ tiền đóng thuế (`EconomyManager: KHÔNG THỂ ĐÓNG THUẾ! GAME OVER`).

## 2. Hoàn Thiện Hệ Thống Sinh Tồn & Xử Phạt
- [ ] **Logic Phạt Khi Ngất Xỉu (Faint Penalty):**
  - Trong `GameManager.cs`, hàm `OnPlayerFainted` hiện tại chỉ đóng thẳng sang `Management` state. Bạn cần trừ đi một lượng Vàng (Tiền viện phí) hoặc rớt một số Item trong túi đồ để tăng tính sinh tồn.
- [ ] **Hiệu Ứng Thuốc (Potion Effects):**
  - Tích hợp logic sử dụng vật phẩm vào `PlayerController`.
  - Ví dụ: Uống `HeatResistPotion` (Thuốc kháng nhiệt) sẽ không bị mất bóng mờ nhòe nhiệt (nếu có).
  - Tích hợp cờ (flag) logic `"Thuốc Bước Chân Êm"` (`hasSilentStepMode`) vào Player, giúp `ShyCreature` không phát hiện được khoảng cách `detectionRadius` trong `ShyCreature.CheckSensors()`.

## 3. Hoàn Thiện Trí Tuệ Nhân Tạo (AI) Đầu Tiên
- [ ] **Cơ Chế Bắn/Ném Mồi Nhử (Decoy):**
  - `ShyCreature.cs` có đề cập đến state `CreatureState.Curious` nhưng chưa được viết logic.
  - Cần một script cho phép Player bấm phím (chuột phải hoặc G) để ném một Item ra xa. Khi vật phẩm phát ra tiếng động, quái vật sẽ chuyển sang state `Curious` và di chuyển đến vị trí mồi thay vì `Wander`.

## 4. Hoàn Thiện Tương Tác Vật Lý (Level Design)
- [ ] **Tạo Trạm Chế Tạo (Crafting Station Prefab):**
  - Đặt một Obect Vạc (Cauldron) vào Scene. Gắn Layer `Interactable` và script kế thừa từ `IInteractable` lên nó. 
  - Thay vì ấn item để nhặt, bấm `E` vào Vạc sẽ gọi `GameManager.Instance.ChangeState(GameState.Crafting)`.
- [ ] **Sắp Xếp Scene Thử Nghiệm:**
  - Tạo các khối đại diện cho `GlowingMushroom` (Nước), `SmellyRoot` (Rễ cây), `IceMint`.
  - Đảm bảo chúng nằm ở Layer `Interactable` và có Script `PickupItem` để tia Raycast của `PlayerInteraction` biến chúng thành màu xanh lá cây bằng hàm `Highlight()`.

## 5. Tích Hợp Cơ Chế Chụp Ảnh (Photography)
- [ ] **Mở Rộng Gameplay Loop:**
  - Dự án có 2 script `CameraSystem.cs` và `PhotoScoringSystem.cs` báo hiệu cơ chế chụp ảnh kiếm tiền.
  - Tích hợp một phím (như `Mouse1` hoặc `C`) để rút máy ảnh. Nếu chụp trúng `ShyCreature` từ xa (mà nó chưa chạy mất) thì gọi `EconomyManager.Instance.AddGold()` dựa theo điểm số tính được.

---
**Tóm Lại Bước Đi Gần Nhất:**
1. Hãy bắt tay vào làm Canvas UIs cho hệ thống **Alchemy/Crafting** và **HUD sinh tồn** trước tiên để thay thế các phím tắt Debug (Số 1-4, F).
2. Viết chức năng tương tác bấm `E` vào máy tạo tác/vạc để mở bảng UI đó.
