# EduHub Chương 2: Refactoring Kiến Trúc Hướng Đối Tượng với SOLID & 4 Design Patterns
## Học Phần: Phát Triển Phần Mềm Hướng Đối Tượng (IN4211) — Trường Đại học Đồng Tháp

- **Sinh viên thực hiện**: Lê Tuấn Kiệt
- **Mã số sinh viên**: 0024417193
- **Thư mục bài làm**: `OOP_0024417193/Ch2_0024417193`

---

## 1. Mục Tiêu Dự Án & Tóm Tắt Nhiệm Vụ
Tái cấu trúc (refactoring) mã nguồn hệ thống quản lý câu lạc bộ sinh viên **EduHub** từ mô hình "God Class" (`ClubManager.cs`) sang kiến trúc phần mềm chuẩn mực (**Clean Architecture**):
1. **Nhiệm vụ 1**: Phân tích chi tiết các vi phạm nguyên lý SOLID trong tài liệu [`docs/solid-analysis.md`](docs/solid-analysis.md).
2. **Nhiệm vụ 2**: Phân tách trách nhiệm thành các Service và Repository độc lập theo SRP, DIP, ISP.
3. **Nhiệm vụ 3**: Áp dụng 4 mẫu thiết kế kinh điển GoF:
   - **Factory Method Pattern**: Khởi tạo dịch vụ thông báo (`INotificationServiceFactory`).
   - **Decorator Pattern**: Bọc thêm chức năng ghi log cho kho lưu trữ câu lạc bộ (`LoggingClubRepository`).
   - **Strategy Pattern**: Thay đổi linh hoạt định dạng báo cáo Console và CSV (`IReportStrategy`).
   - **Observer Pattern**: Xuất bản sự kiện tự động cho các bên đăng ký (`IEventObserver`).
4. **Nhiệm vụ 4**: Chuyển các luật bất biến về đúng tầng Domain (`Club`, `Event`, `Member`) và xây dựng 21 bài kiểm thử tự động với xUnit theo mô hình Arrange - Act - Assert (AAA).
5. **Nhiệm vụ 5**: Quản lý lịch sử mã nguồn với Git, thực hiện commit theo chuẩn Conventional Commits.

---

## 2. Cấu Trúc Thư Mục Dự Án

```
Ch2_0024417193/
├── Ch02.Refactor.sln                       # Giải pháp Visual Studio .NET 10
├── README.md                               # Tài liệu hướng dẫn sử dụng
├── docs/
│   └── solid-analysis.md                   # Báo cáo phân tích vi phạm SOLID
│
├── Ch02.Refactor/                          # Dự án chính (Console Application)
│   ├── Ch02.Refactor.csproj
│   ├── Program.cs                          # Điểm hợp thành đối tượng (Composition Root)
│   ├── Domain/                             # Tầng miền nghiệp vụ cốt lõi
│   │   ├── Club.cs                         # Thực thể CLB (Create, HasRoom)
│   │   ├── Event.cs                        # Thực thể sự kiện (Register, CheckIn, AttendanceRate)
│   │   └── Member.cs                       # Thực thể thành viên
│   ├── Logging/                            # Hạ tầng ghi nhật ký
│   │   ├── IAppLogger.cs
│   │   ├── ConsoleLogger.cs
│   │   └── FileLogger.cs
│   ├── Notifications/                      # Hạ tầng gửi thông báo (Factory Method Pattern)
│   │   ├── NotificationChannel.cs
│   │   ├── INotificationService.cs
│   │   ├── EmailNotificationService.cs
│   │   ├── SmsNotificationService.cs
│   │   ├── SlackNotificationService.cs
│   │   ├── INotificationServiceFactory.cs
│   │   └── NotificationServiceFactory.cs
│   ├── Patterns/
│   │   ├── Observers/                      # Mẫu Observer (Publisher - Subscriber)
│   │   │   ├── IEventObserver.cs
│   │   │   ├── EmailNotifier.cs
│   │   │   └── LogNotifier.cs
│   │   └── Strategies/                     # Mẫu Strategy (Thuật toán xuất báo cáo)
│   │       ├── ClubStat.cs
│   │       ├── IReportStrategy.cs
│   │       ├── ConsoleReportStrategy.cs
│   │       └── CsvReportStrategy.cs
│   ├── Repositories/                       # Tầng kho lưu trữ (Decorator Pattern & ISP)
│   │   ├── IClubRepository.cs
│   │   ├── InMemoryClubRepository.cs
│   │   ├── LoggingClubRepository.cs
│   │   ├── IMemberRepository.cs
│   │   ├── InMemoryMemberRepository.cs
│   │   ├── IEventRepository.cs
│   │   └── InMemoryEventRepository.cs
│   └── Services/                           # Tầng dịch vụ điều phối nghiệp vụ
│       ├── IClubService.cs & ClubService.cs
│       ├── IEventService.cs & EventService.cs
│       └── IReportService.cs & ReportService.cs
│
└── Ch02.Refactor.Tests/                    # Dự án kiểm thử xUnit theo mô hình AAA
    ├── Ch02.Refactor.Tests.csproj
    ├── ClubTests.cs                        # Kiểm thử thực thể Club
    ├── EventTests.cs                       # Kiểm thử thực thể Event
    └── PatternTests.cs                     # Kiểm thử 4 Design Patterns
```

---

## 3. Hướng Dẫn Biên Dịch Và Chạy Thử

### 3.1 Biên dịch toàn bộ giải pháp
```powershell
dotnet build Ch02.Refactor.sln
```

### 3.2 Chạy bộ kiểm thử tự động (Unit Tests)
```powershell
dotnet test Ch02.Refactor.sln
```
*(Yêu cầu kết quả: Toàn bộ các bài test đều Passed 100%)*

### 3.3 Chạy chương trình ứng dụng Demo
```powershell
dotnet run --project Ch02.Refactor
```
Chương trình sẽ tự động thực thi 5 kịch bản chính:
1. Tạo câu lạc bộ và thêm thành viên qua `ClubService`.
2. Tạo sự kiện và kiểm chứng 2 Observer tự động nhận tin (Email + Log).
3. Xuất báo cáo cùng một nguồn số liệu ra hai định dạng khác nhau (Console và CSV) qua Strategy.
4. Tạo kênh thông báo động theo tham số Enum bằng Factory Method.
5. Kiểm tra bẫy các ngoại lệ nghiệp vụ (`ArgumentException`, `InvalidOperationException`).
