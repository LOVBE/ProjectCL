# Astro-Alchemist: Tài liệu Thiết kế Dự án (GDD)

## 1. Core Gameplay (Lối chơi Cốt lõi)
Trò chơi hoạt động theo ba giai đoạn nối tiếp nhau trong một chu kỳ ngày và đêm, được quản lý chặt chẽ bởi hệ thống trạng thái của game:

- **Giai đoạn Thám hiểm (Sáng):** Game bắt đầu bằng việc tải bản đồ môi trường và sinh ra các loài sinh vật. Người chơi sẽ đi lại khám phá, dùng máy ảnh lén lút tiếp cận để chụp hình các sinh vật, đồng thời nhặt các nguyên liệu thực vật, khoáng sản rơi vãi để cất vào túi đồ.
- **Giai đoạn Chế tạo (Tối):** Người chơi quay về phòng thí nghiệm. Lúc này game chuyển sang góc nhìn tương tác đồ vật. Người chơi lấy nguyên liệu từ túi, bỏ vào cối giã hoặc vạc nấu. Bằng các thao tác vật lý khéo léo, người chơi chế tạo ra các lọ thuốc dùng để hỗ trợ thám hiểm cho ngày hôm sau hoặc để bán. Nếu làm sai, vật dụng có thể phát nổ.
- **Giai đoạn Quản lý (Giao dịch):** Trò chơi tự động chấm điểm các bức ảnh đã chụp trong ngày. Người chơi gửi ảnh cho tòa soạn và bán thuốc cho thương nhân để thu về tiền tệ. Tiền này dùng để đóng thuế, mua sắm hoặc nâng cấp dụng cụ, ống kính máy ảnh. Hoàn tất việc này, game chuyển sang ngày mới.

## 2. Cơ chế

Đội ngũ Lập trình cần tập trung vào hai hệ thống cốt lõi sau:

### A. Hệ thống Nhiếp ảnh
Hệ thống này sử dụng cơ chế phóng tia dò tìm trong không gian 3D để xác định chất lượng bức ảnh.
- **Cơ chế dò tìm:** Khi người chơi bấm nút chụp, game sẽ phóng các đường tia ảo từ ống kính camera về phía trước để dò xem có trúng vật thể (sinh vật) nào không.
- **Hệ thống chấm điểm:** Điểm của một bức ảnh được tính toán dựa trên bốn tham số:
  - **Khoảng cách:** Hệ thống đo xem chủ thể chiếm bao nhiêu phần trăm diện tích khung hình. Đẹp nhất là tỷ lệ vừa vặn, tối ưu khoảng 30% đến 50%.
  - **Góc độ:** Tính toán chênh lệch giữa hướng nhìn của camera và hướng mặt của sinh vật. Chụp thẳng mặt sẽ được điểm cao nhất, chụp từ sau lưng bị điểm thấp nhất.
  - **Hành vi:** Hệ thống đọc trạng thái hiện tại của trí tuệ nhân tạo sinh vật. Ví dụ: đang ngủ được 50 điểm, đang đi săn được 100 điểm.
  - **Vật cản:** Nếu các tia dò đụng phải thân cây hay tảng đá trước khi đụng vào sinh vật, hệ thống tính toán tỷ lệ bị che khuất. Bị che càng nhiều, điểm càng thấp.

### B. Hệ thống Giả kim Vật lý
Đòi hỏi xây dựng các module vật lý thực tế cho các vật thể trong phòng thí nghiệm.
- **Tương tác Vật lý (Nghiền nát):** Khi người chơi điều khiển chày đập vào nguyên liệu, game tính toán lực tác động. Lực đập đủ mạnh và đủ thời gian sẽ biến hình ảnh vật liệu nguyên bản thành dạng bột.
- **Hệ thống Nhiệt độ:** Cái vạc nấu có một biến số ẩn lưu trữ mức nhiệt độ. Khi bật lửa, nhiệt độ tăng dần theo thời gian. Mỗi nguyên liệu có một ngưỡng chịu nhiệt tối đa.
- **Logic Pha trộn:** Game không dùng công thức ghép đồ cứng nhắc. Hệ thống sẽ trộn màu sắc và cộng dồn các thuộc tính hóa học của nguyên liệu lại với nhau một cách từ từ. Màu sắc nước trong vạc thay đổi giúp người chơi nhận biết tiến độ.

## 3. Kiến trúc NPC
Sinh vật tự hành động dựa trên logic nhánh. Trò chơi chia sinh vật thành ba nhóm tính cách: **Loài Nhút nhát**, **Loài Lãnh thổ**, và **Loài Tinh nghịch**.

- **Lang thang (Mặc định):** Khi an toàn, cả ba nhóm đi dạo ngẫu nhiên, tìm thức ăn hoặc ngủ theo chu kỳ.
- **Tò mò (Bị thu hút):** Kích hoạt khi có mồi nhử hoặc mùi hương do người chơi tạo ra. Sinh vật tiến lại gần ngửi hoặc ăn.
- **Báo động (Nghi ngờ):** Kích hoạt khi nghe tiếng động hoặc ngửi thấy mùi lạ. Sinh vật dừng lại, hiện cảnh báo trên đầu và chuẩn bị phản ứng.
- **Phản ứng khi bị phát hiện:**
  - **Loài Nhút nhát:** Hoảng sợ và chạy trốn vào bụi rậm. Người chơi mất cơ hội chụp ảnh.
  - **Loài Lãnh thổ:** Rống lên và lao thẳng vào húc văng người chơi ra khỏi khu vực.
  - **Loài Tinh nghịch:** Giả vờ bỏ chạy nhưng lén lút vòng lại từ sau lưng, nhảy lên ăn trộm nguyên liệu trong túi đồ người chơi rồi mới bỏ trốn.

## 4. Hệ thống Thất bại và Hình phạt
Trò chơi áp dụng các hình phạt về tài nguyên và thời gian thay vì bắt người chơi chơi lại từ đầu ngay lập tức.

### A. Thất bại Kinh tế (Phá sản)
- **Cơ chế:** Trò chơi theo dõi thời gian và yêu cầu đóng phí bảo trì phòng thí nghiệm định kỳ mỗi tuần.
- **Hậu quả:** Không đủ tiền đóng phí sẽ bị tịch thu bớt trang thiết bị pha chế hoặc bị hạ cấp bậc chứng chỉ (giảm giá bán ảnh). Nợ quá ba lần liên tiếp, trò chơi kết thúc (Game Over).

### B. Thất bại Giả kim thuật (Tai nạn phòng thí nghiệm)
- **Cơ chế:** Trong lúc nấu thuốc, hệ thống liên tục kiểm tra nhiệt độ lò. Lỗi xảy ra khi nhiệt độ vượt quá giới hạn hoặc pha trộn các chất kỵ nhau.
- **Hậu quả:** Vạc pha chế phát nổ. Mất trắng nguyên liệu của mẻ thuốc đó. Phòng ngập khói độc, khóa mọi chức năng. Người chơi buộc phải đi ngủ, lãng phí thời gian của ngày hôm đó.

### C. Thất bại Thám hiểm (Kiệt sức hoặc Gặp tai nạn)
- **Cơ chế:** Quản lý bằng thanh Thể lực và Dưỡng khí. Thất bại khi các thanh này cạn kiệt, hoặc khi chọc giận thú lớn, bị thú tinh nghịch trộm đồ.
- **Hậu quả:**
  - **Bị thú lãnh thổ húc văng:** Rớt nguyên liệu, xước ống kính máy ảnh (làm mờ ảnh sau đó).
  - **Bị thú tinh nghịch áp sát:** Mất nguyên liệu ngẫu nhiên trong túi.
  - **Cạn thể lực hoặc dưỡng khí:** Nhân vật ngất xỉu, chuyển cảnh về phòng thí nghiệm vào sáng hôm sau. Mất toàn bộ ảnh chụp trong ngày vì chưa lưu.

## 5. Danh sách Vật phẩm Cơ sở
Hệ thống chia làm hai loại: Nguyên liệu tự nhiên và Thuốc thành phẩm.

### A. Nhóm Nguyên liệu sinh học
- **Nấm Phát Quang:** Nấm tỏa sáng xanh lục. Nhiệt độ sôi rất thấp, đun to lửa sẽ nổ. Chứa tinh chất phát sáng.
- **Rễ Cây Bốc Mùi:** Rễ xù xì bốc khói. Nhiệt độ sôi rất cao, cần đun lửa lớn và khuấy liên tục. Tạo mùi hương nồng nặc.
- **Lá Bạc Hà Băng Giá:** Lá trong suốt màu xanh lam. Chịu nhiệt trung bình, làm giảm nhiệt độ nồi nước khi thả vào. Chứa tinh chất làm lạnh và triệt tiêu âm thanh.
- **Quả Mọng Tinh Nghịch:** Quả tròn màu đỏ tía. Dễ bay hơi, đun lâu sinh nhiều khói. Chứa tinh chất tạo khói mù.

### B. Nhóm Thuốc thành phẩm
- **Thuốc Bước Chân Êm:** Giúp nhân vật không phát ra tiếng động khi di chuyển. Dùng để rón rén tiếp cận Loài Nhút nhát.
- **Bom Mùi Thịt Nướng:** Ném ra xa tạo vùng mùi hương hấp dẫn. Chuyển AI của sinh vật sang trạng thái Tò mò để dễ chụp ảnh.
- **Thuốc Kháng Nhiệt:** Phủ lớp sương lạnh lên người chơi, giúp thanh Thể lực không bị tụt ở bản đồ khắc nghiệt.
- **Bột Khói Mù tẩu thoát:** Ném xuống đất tạo đám khói đặc che khuất tầm nhìn sinh vật, giúp tẩu thoát khi chọc giận Loài Lãnh thổ.

## 6. Thiết kế Bản đồ và Môi trường
Bản đồ là một câu đố về không gian, âm thanh và ánh sáng.

### Bản đồ Khởi đầu: Khu Rừng Nấm Dạ Quang
- **Chủ đề:** Rừng rậm không có ánh mặt trời. Nguồn sáng từ nấm khổng lồ và đom đóm. Tông màu xanh lục lam, tím than, vàng huỳnh quang.
- **Cấu trúc Địa hình:**
  - **Rìa rừng:** Bằng phẳng, an toàn, dùng để nhặt nguyên liệu cơ bản và tập chụp ảnh.
  - **Bụi rậm đan chéo:** Hẹp, nhiều rễ cây và lá khô. Di chuyển trên lá khô sẽ phát ra tiếng động lớn.
  - **Đầm lầy Tĩnh lặng:** Nằm giữa rừng, nhiều sinh vật nhưng ít chỗ nấp. Cần tìm cách leo lên gờ đá cao để chụp ảnh.
- **Phân bổ AI Sinh vật:**
  - **Sóc Đuôi Đèn (Loài Nhút nhát):** Trốn trong bụi rậm, nhạy cảm tiếng động. Cần Thuốc Bước Chân Êm để tiếp cận.
  - **Khỉ Lục Lạc (Loài Tinh nghịch):** Sống trên tán cây. Lén trộm đồ nếu người chơi đứng yên quá lâu.
  - **Gấu Giáp Sinh Học (Loài Lãnh thổ):** Tuần tra quanh hồ nước. Tiến lại gần sẽ bị húc văng. Phải dùng Bom Mùi dụ đi chỗ khác.
- **Chướng ngại vật:**
  - **Bào tử phấn hoa gây ngủ:** Những đám mây màu tím ở hẻm núi hẹp. Đi xuyên qua mà không có Thuốc Kháng Độc sẽ tụt Dưỡng khí và ngất xỉu.
