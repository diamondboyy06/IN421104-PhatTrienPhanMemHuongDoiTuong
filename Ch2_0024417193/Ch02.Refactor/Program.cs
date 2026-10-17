using System.Text;
using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Patterns.Observers;
using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;

// Cấu hình console hỗ trợ Unicode tiếng Việt đầy đủ
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.WriteLine("=========================================================================");
Console.WriteLine("          HỆ THỐNG QUẢN LÝ CÂU LẠC BỘ & SỰ KIỆN SINH VIÊN - EDUHUB        ");
Console.WriteLine("               BÀI TẬP CHƯƠNG 2 (IN4211) - REFACTORING & PATTERNS        ");
Console.WriteLine("          SV: Lê Tuấn Kiệt - MSSV: 0024417193 - Lớp: ĐHCNTT24B-IT         ");
Console.WriteLine("=========================================================================\n");

// ─── Composition root: Nơi DUY NHẤT dùng "new" cho class cụ thể (DIP) ───
IAppLogger logger = new ConsoleLogger();

// Factory: Chọn kênh thông báo bằng enum, không new class cụ thể ở nơi khác
INotificationServiceFactory notificationFactory = new NotificationServiceFactory();
INotificationService notifier = notificationFactory.Create(NotificationChannel.Email);

// Decorator: Bọc repository gốc để ghi log truy cập (LoggingClubRepository)
IClubRepository clubRepo = new LoggingClubRepository(new InMemoryClubRepository(), logger);
IMemberRepository memberRepo = new InMemoryMemberRepository();
IEventRepository eventRepo = new InMemoryEventRepository();

// Services: Mọi phụ thuộc được inject qua Constructor (DIP, SRP)
IClubService clubService = new ClubService(clubRepo, memberRepo, notifier, logger);
IEventService eventService = new EventService(clubRepo, memberRepo, eventRepo);
IReportService reportService = new ReportService(clubRepo, memberRepo, eventRepo);

// ─── 1. Dùng service thay cho ClubManager cũ (SRP, OCP, DIP) ───
Console.WriteLine("== 1. Tạo CLB và thêm thành viên (ClubService) ==");
var club = clubService.CreateClub("Tech Club", "Câu lạc bộ công nghệ DThU", "president@gmail.com");
var a = clubService.AddMember(club.Id, "Lê Tuấn Kiệt", "kiet.0024417193@dthu.edu.vn");
var b = clubService.AddMember(club.Id, "Trần Minh Quân", "quan.0024417194@dthu.edu.vn");

// ─── 2. Observer: Đăng ký 2 observer lắng nghe, tạo sự kiện và check-in ───
Console.WriteLine("\n== 2. Observer Pattern: Tạo sự kiện -> 2 observers được thông báo ==");
eventService.Subscribe(new LogNotifier(logger));
eventService.Subscribe(new EmailNotifier(memberRepo, notifier));
var ev = eventService.CreateEvent(club.Id, "Workshop: Clean Architecture & SOLID", new DateTime(2026, 11, 20));
eventService.Register(ev.Id, a.Id);
eventService.Register(ev.Id, b.Id);
eventService.CheckIn(ev.Id, a.Id);

// ─── 3. Strategy: Cùng dữ liệu, xuất hai định dạng báo cáo linh hoạt ───
Console.WriteLine("\n== 3. Strategy Pattern: Báo cáo Console ==");
IReportStrategy console = new ConsoleReportStrategy();
Console.Write(reportService.Generate(console));

Console.WriteLine("== 3. Strategy Pattern: Báo cáo CSV ==");
IReportStrategy csv = new CsvReportStrategy();
Console.Write(reportService.Generate(csv));

// ─── 4. Factory Method: Tạo kênh thông báo theo yêu cầu lúc chạy ───
Console.WriteLine("== 4. Factory Method: Tạo kênh thông báo linh hoạt ==");
foreach (var channel in Enum.GetValues<NotificationChannel>())
{
    var service = notificationFactory.Create(channel);
    service.Send("kiet.0024417193@dthu.edu.vn", $"Thử nghiệm kênh thông báo {channel}");
}

// ─── 5. Kiểm tra bẫy ngoại lệ và ràng buộc nghiệp vụ (Data Invariants) ───
Console.WriteLine("\n== 5. Kiểm tra tính toàn vẹn dữ liệu & bắt ngoại lệ ==");
try
{
    clubService.CreateClub("", "mô tả", "president@gmail.com");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  [Bắt ngoại lệ thành công] Lỗi đúng dự kiến: {ex.Message}");
}

try
{
    ev.CheckIn(Guid.NewGuid());
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  [Bắt ngoại lệ thành công] Lỗi đúng dự kiến: {ex.Message}");
}

Console.WriteLine("\n Chương trình chạy thành công toàn bộ kịch bản kiến trúc sạch!");
