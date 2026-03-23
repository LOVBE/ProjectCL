# Lịch Sử Nhật Ký Cập Nhật (Dành cho Dev Team ghi chú)
*(Các commit thay đổi code tiếp theo sẽ được append vào bên dưới mục này)*

* **[23/03/2026] - [Core MVP, Bug Fixes & Localization] - Thêm mới / Chỉnh sửa**
  - **Mô tả:** Hoàn thiện MVP vòng lặp gameplay, sửa các bug nghiêm trọng và triển khai hệ thống đa ngôn ngữ EN/VI.

  - **1. Cấu trúc dự án:**
    - Di chuyển `dev-doc.md`, `todo.md`, `tutorial.md` vào thư mục `Doc/`. Khởi tạo `README.md`.

  - **2. Core Gameplay UI & Crafting Loop (MVP):**
    - HUD runtime: thanh `Stamina`, `Oxygen`, số `Gold`, ngày hiện tại, đếm ngược thuế, `Crosshair`.
    - Crafting UI runtime: hiển thị túi đồ, nhiệt độ vạc, trạng thái lửa, nguyên liệu trong vạc, nút điều khiển.
    - `Cauldron` triển khai `IInteractable`, bấm `E` mở Crafting UI.
    - Mở rộng `GameManager` (GameOver, tổng kết ngày, phạt ngất), `EconomyManager` (thuế, vàng).
    - Đồng bộ `PlayerController` (sinh tồn, hiệu ứng thuốc), `PlayerInventory`, `PickupItem`, `ShyCreature` (Silent Step AI).
    - Bootstrap runtime trong `AlchemyManager`: tự tạo vạc + spawn nguyên liệu mẫu nếu scene thiếu.

  - **3. Bug Fixes:**
    - **Font crash (Unity 6):** Thay `Arial.ttf` → `LegacyRuntime.ttf` trong `UIManager.cs`.
    - **Rơi tự do xuyên đất:** Gộp 2 lần gọi `controller.Move()` thành 1 lần duy nhất/frame, reset gravity đầu frame trong `PlayerController.cs`.
    - **Raycast TPS tự ngắm lưng:** Đổi sang `Physics.RaycastAll` + bỏ qua Collider thuộc `transform.root`. Tăng tầm tia nháp Camera lên 100m, tia tương tác lên 15m.
    - Refactor `PlayerInteraction.cs`: thêm `verboseRaycastLogging`, `ResolveReferences()`, `ClearHoveredItem()`.

  - **4. Localization EN/VI:**
    - Tạo `Loc.cs` [NEW]: static singleton, 2 Dictionary hard-code (~30 key), `string.Format`, event `OnLanguageChanged`, hàm `Toggle()`.
    - `UIManager.cs`: thay toàn bộ chuỗi hard-code bằng `Loc.Get()`. Thêm nút "English / Tiếng Việt" trên HUD. Khi chuyển ngôn ngữ, hủy + dựng lại toàn bộ Canvas.

  - **File thay đổi:** `Assets/Scripts/Core/Loc.cs` [NEW], `Assets/Scripts/Core/GameManager.cs`, `Assets/Scripts/Core/EconomyManager.cs`, `Assets/Scripts/Core/UIManager.cs`, `Assets/Scripts/Alchemy/AlchemyManager.cs`, `Assets/Scripts/Alchemy/Cauldron.cs`, `Assets/Scripts/Player/PlayerController.cs`, `Assets/Scripts/Player/PlayerInteraction.cs`, `Assets/Scripts/Player/PlayerInventory.cs`, `Assets/Scripts/Player/PickupItem.cs`, `Assets/Scripts/AI/ShyCreature.cs`
  - **Lưu ý:** Test tại `Assets/active_handling.unity`. Các biến tuning đều đã expose trong Inspector. Debug.Log giữ nguyên tiếng Việt (chỉ dev đọc). Yêu cầu Unity 6+ (font `LegacyRuntime.ttf`).

