# BÁO CÁO PHÂN TÍCH CÁC VI PHẠM NGUYÊN LÝ SOLID TRONG CLUBMANAGER
## HỌC PHẦN: PHÁT TRIỂN PHẦN MỀM HƯỚNG ĐỐI TƯỢNG (IN4211)
- **Sinh viên thực hiện**: Lê Tuấn Kiệt
- **Mã số sinh viên**: 0024417193
- **Lớp**: Đại học Sư phạm Tin học / Công nghệ thông tin — Trường Đại học Đồng Tháp

---

### TỔNG QUAN VỀ "GOD CLASS" CLUBMANAGER (MÃ NGUỒN BAN ĐẦU)
Trong mã nguồn ban đầu của bài tập thực hành Chương 2, lớp `ClubManager` bị thiết kế theo mẫu phản thiết kế (anti-pattern) kinh điển mang tên **God Class** (Lớp Thần Thánh). Lớp này ôm đồm toàn bộ trách nhiệm từ quản lý thực thể, lưu trữ dữ liệu trong bộ nhớ, kiểm tra tính hợp lệ dữ liệu, ghi nhật ký ra tập tin, gửi thông báo qua thư điện tử/mạng xã hội đến việc kết xuất báo cáo định dạng chuỗi.

Dưới đây là bảng phân tích chi tiết các vi phạm nguyên lý thiết kế SOLID được phát hiện:

---

### 1. VI PHẠM NGUYÊN LÝ ĐƠN TRÁCH NHIỆM (SINGLE RESPONSIBILITY PRINCIPLE - SRP)

#### Vị trí vi phạm trong mã nguồn cũ:
- **Phương thức `CreateClub` (dòng 35 - 64)**: Cùng lúc gánh vác 3 trách nhiệm hoàn toàn khác nhau:
  1. Kiểm tra tính hợp lệ nghiệp vụ của tên câu lạc bộ (`string.IsNullOrWhiteSpace`, `name.Length > 100`).
  2. Lưu trữ đối tượng câu lạc bộ vào danh sách bộ nhớ `_clubs`.
  3. Ghi trực tiếp nhật ký vào tệp tin đĩa cứng (`File.AppendAllText`).
  4. Gửi thông báo đến chủ nhiệm câu lạc bộ qua chuỗi điều kiện phân nhánh thư điện tử.
- **Phương thức `AddMemberToClub` (dòng 67 - 84)**: Vừa kiểm tra sĩ số tối đa, vừa tạo và lưu thành viên, vừa ghi nhật ký tệp, vừa gửi thông báo chào mừng qua Email và Slack.
- **Phương thức `PrintClubReport` (dòng 103 - 113)**: Trộn lẫn logic thu thập dữ liệu thống kê, định dạng hiển thị ra màn hình Console và ghi nhật ký tệp.

#### Hậu quả:
- **Nhiều lý do để thay đổi (Coupling cao)**: Bất kỳ thay đổi nào liên quan đến giao diện người dùng, công nghệ lưu trữ, hệ thống ghi log hay nhà cung cấp dịch vụ thông báo đều buộc phải mở lớp `ClubManager` ra sửa đổi.
- Rất khó viết kiểm thử đơn vị độc lập (Unit Test), dễ phát sinh lỗi dây chuyền (regression bugs).

#### Giải pháp tái cấu trúc:
- Tách quy tắc kiểm tra tính toàn vẹn dữ liệu về đúng thực thể nghiệp vụ: `Club.Create` và `Club.HasRoom`.
- Tách tầng kho lưu trữ: `IClubRepository`, `IMemberRepository`, `IEventRepository`.
- Tách tầng dịch vụ nghiệp vụ điều phối: `ClubService`, `EventService`, `ReportService`.
- Tách tầng hạ tầng: `IAppLogger`, `INotificationService`.

---

### 2. VI PHẠM NGUYÊN LÝ ĐÓNG / MỞ (OPEN/CLOSED PRINCIPLE - OCP)

#### Vị trí vi phạm trong mã nguồn cũ:
- **Khối điều kiện gửi email trong `CreateClub` (dòng 52 - 63)**:
  ```csharp
  if (presidentEmail.Contains("@gmail.com")) { ... }
  else if (presidentEmail.Contains("@yahoo.com")) { ... }
  else { ... }
  ```
- **Phương thức `AddMemberToClub` (dòng 82 - 83)**: Gửi thông báo trực tiếp qua `Console.WriteLine` gắn chặt với Email và Slack.
- **Phương thức `PrintClubReport` (dòng 103 - 113)**: Cố định một cách in báo cáo ra màn hình dòng lệnh duy nhất. Muốn xuất ra tập tin CSV hoặc HTML, lập trình viên buộc phải can thiệp trực tiếp và sửa mã nguồn của lớp này.

#### Hậu quả:
- Vi phạm nguyên tắc: *"Phần mềm phải mở cho việc mở rộng, nhưng đóng cho việc sửa đổi"*.
- Mỗi lần nhà trường yêu cầu bổ sung kênh thông báo mới (ví dụ SMS, Zalo, Telegram) hoặc một mẫu báo cáo mới (CSV, PDF), ta đều phải mạo hiểm sửa đổi mã nguồn cốt lõi đang chạy ổn định.

#### Giải pháp tái cấu trúc:
- Áp dụng **Mẫu Phương thức Nhà máy (Factory Method Pattern)**: Tạo `INotificationServiceFactory` và `INotificationService` với các lớp cụ thể `EmailNotificationService`, `SmsNotificationService`, `SlackNotificationService`. Thêm kênh mới chỉ cần tạo thêm class kế thừa mà không ảnh hưởng code cũ.
- Áp dụng **Mẫu Chiến lược (Strategy Pattern)**: Tách thuật toán kết xuất báo cáo thành `IReportStrategy` với `ConsoleReportStrategy` và `CsvReportStrategy`.

---

### 3. VI PHẠM NGUYÊN LÝ PHÂN TÁCH GIAO DIỆN (INTERFACE SEGREGATION PRINCIPLE - ISP)

#### Vị trí vi phạm trong mã nguồn cũ:
- Toàn bộ các thao tác không hề có hợp đồng giao diện (`interface`).
- Nếu một client bên ngoài chỉ cần xem danh sách câu lạc bộ, nó vẫn phải phụ thuộc vào toàn bộ lớp `ClubManager` khổng lồ chứa cả logic ghi log tệp và gửi thư điện tử.

#### Hậu quả:
- Lớp gọi bị phụ thuộc vào những chức năng mà bản thân nó không bao giờ sử dụng.
- Không thể tráo đổi các cài đặt khác nhau (như mock dữ liệu để test).

#### Giải pháp tái cấu trúc:
- Phân rã thành các giao diện nhỏ, tập trung và chuyên biệt:
  - `IClubRepository`: chỉ đảm nhận tác vụ dữ liệu của câu lạc bộ.
  - `IMemberRepository`: chỉ đảm nhận tác vụ dữ liệu của thành viên.
  - `IEventRepository`: chỉ đảm nhận tác vụ dữ liệu của sự kiện.
  - `IClubService`, `IEventService`, `IReportService`: các giao diện dịch vụ chuyên biệt cho từng nghiệp vụ.

---

### 4. VI PHẠM NGUYÊN LÝ ĐẢO NGƯỢC PHỤ THUỘC (DEPENDENCY INVERSION PRINCIPLE - DIP)

#### Vị trí vi phạm trong mã nguồn cũ:
- **Ghi nhật ký trực tiếp xuống đĩa (dòng 48, 79, 94, 111)**:
  `File.AppendAllText(_logFilePath, ...)` được gọi cứng (hard-coded) trực tiếp bên trong logic nghiệp vụ.
- **Quản trị dữ liệu bằng biến nội bộ**:
  Biến `_clubs` và `_members` được khai báo và khởi tạo cụ thể bằng `new List<Club>()` ngay trong `ClubManager`.

#### Hậu quả:
- Mô đun cấp cao (nghiệp vụ trường học) bị phụ thuộc trực tiếp vào mô đun cấp thấp (hệ điều hành ghi tệp tin `System.IO.File`, cơ chế lưu trữ RAM).
- Khi chạy Unit Test, code sẽ liên tục tạo tệp rác trên ổ cứng máy tính và không thể chạy kiểm thử độc lập mà không có quyền ghi ổ đĩa.

#### Giải pháp tái cấu trúc:
- Đảo ngược phụ thuộc thông qua các trừu tượng (Abstractions):
  - Thay `File.AppendAllText` bằng giao diện `IAppLogger`, triển khai cụ thể qua `ConsoleLogger` và `FileLogger`.
  - Thay `List<T>` nội bộ bằng `IClubRepository`, `IMemberRepository`, `IEventRepository`.
- Toàn bộ phụ thuộc đều được tiêm qua hàm khởi tạo (**Constructor Injection**).
- Điểm hợp thành (**Composition Root**) duy nhất đặt tại `Program.cs`.

---

### 5. ÁP DỤNG CÁC MẪU THIẾT KẾ BỔ TRỢ (DESIGN PATTERNS)
1. **Factory Method Pattern**: `INotificationServiceFactory` đóng gói quyết định khởi tạo kênh gửi tin tương ứng với enum `NotificationChannel`.
2. **Decorator Pattern**: `LoggingClubRepository` bọc bên ngoài `IClubRepository` để ghi nhật ký truy cập mà không can thiệp sửa đổi lớp `InMemoryClubRepository`.
3. **Strategy Pattern**: `IReportStrategy` cho phép lựa chọn linh hoạt chiến lược kết xuất dữ liệu (`ConsoleReportStrategy`, `CsvReportStrategy`).
4. **Observer Pattern**: `IEventObserver` với các quan sát viên `EmailNotifier`, `LogNotifier` tự động nhận thông báo khi `EventService` tạo sự kiện mới theo mô hình xuất bản - đăng ký (Publish - Subscribe).
