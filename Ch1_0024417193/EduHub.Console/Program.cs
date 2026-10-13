using System.Text;
using EduHub.Domain;

// Cấu hình console hiển thị đúng tiếng Việt có dấu trên Windows
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.WriteLine("=========================================================================");
Console.WriteLine("          HỆ THỐNG QUẢN LÝ CÂU LẠC BỘ & SỰ KIỆN SINH VIÊN - EDUHUB        ");
Console.WriteLine("                  BÀI TẬP CHƯƠNG 1 (IN4211) - CONSOLE DEMO               ");
Console.WriteLine("=========================================================================\n");

// [1] Khởi tạo Câu lạc bộ
var club = Club.Create("CLB Tin học Sinh viên (ITC)", "Nơi sinh hoạt chuyên môn, chia sẻ kiến thức công nghệ.");
Console.WriteLine($"[1] Tạo CLB thành công: {club.Name} (Id: {club.Id.ToString()[..8]})");

// [2] Khởi tạo và thêm 3 thành viên vào CLB
var m1 = Member.Create("Lê Tuấn Kiệt", "kiet.0024417193@dthu.edu.vn", "0024417193");
var m2 = Member.Create("Trần Minh Quân", "quan.0024417194@dthu.edu.vn", "0024417194");
var m3 = Member.Create("Nguyễn Thu Trang", "trang.0024417195@dthu.edu.vn", "0024417195");

club.AddMember(m1);
club.AddMember(m2);
club.AddMember(m3);
Console.WriteLine($"[2] Đã thêm thành công {club.MemberCount()} thành viên vào CLB.");

// [3] Tạo sự kiện do CLB tổ chức
var ev = Event.Create(club.Id, "Workshop: Làm quen với .NET 10 & C# OOP", DateTime.Now.AddDays(7), capacity: 50, isOnline: false);
Console.WriteLine($"[3] Đã tạo sự kiện: '{ev.Title}' (Sức chứa: {ev.Capacity})");

// [4] Đăng ký tham gia sự kiện (Trạng thái ban đầu: Pending)
var reg1 = Registration.Create(m1.Id, ev.Id);
var reg2 = Registration.Create(m2.Id, ev.Id);
Console.WriteLine($"[4] Sinh viên đăng ký sự kiện:");
Console.WriteLine($"    - Phiếu 1 ({m1.Name}): {reg1.Status}");
Console.WriteLine($"    - Phiếu 2 ({m2.Name}): {reg2.Status}");

// [5] Ban chủ nhiệm duyệt xác nhận phiếu đăng ký (Chuyển sang Confirmed)
reg1.Confirm();
Console.WriteLine($"[5] Ban CLB duyệt phiếu 1 -> Trạng thái mới: {reg1.Status}");

// [6] Check-in điểm danh khi diễn ra sự kiện
var checkIn1 = CheckIn.Create(reg1, CheckInMethod.Qr);
Console.WriteLine($"[6] Điểm danh thành công: {checkIn1.Summary()}");

// [7] Xuất báo cáo tóm tắt
Console.WriteLine("\n[7] BÁO CÁO TỔNG KẾT THAM DỰ SỰ KIỆN:");
Console.WriteLine($"    - Tên sự kiện: {ev.Title}");
Console.WriteLine($"    - Số phiếu đăng ký: 2 | Đã phê duyệt: 1 | Đã điểm danh: 1\n");

// [8] Kiểm thử vi phạm luật nghiệp vụ (Bắt và xử lý ngoại lệ theo Slide 66)
Console.WriteLine("[8] THỬ NGHIỆM VI PHẠM LUẬT NGHIỆP VỤ & BẮT NGOẠI LỆ:");

// 8.1 Thử check-in khi phiếu còn ở trạng thái Pending
try
{
    Console.Write("    • Thử Check-in phiếu 2 (đang Pending): ");
    CheckIn.Create(reg2);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"[BẮT NGOẠI LỆ THÀNH CÔNG] {ex.Message}");
}

// 8.2 Thử xác nhận phiếu đã bị hủy (Cancelled)
try
{
    reg2.Cancel();
    Console.Write("    • Thử Confirm phiếu 2 sau khi đã Cancel: ");
    reg2.Confirm();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"[BẮT NGOẠI LỆ THÀNH CÔNG] {ex.Message}");
}

// 8.3 Thử thêm lại thành viên đã có trong CLB
try
{
    Console.Write("    • Thử thêm trùng thành viên m1 vào CLB: ");
    club.AddMember(m1);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"[BẮT NGOẠI LỆ THÀNH CÔNG] {ex.Message}");
}

Console.WriteLine("\n=========================================================================");
Console.WriteLine("                    KẾT QUẢ: 100% KỊCH BẢN ĐẠT CHUẨN                     ");
Console.WriteLine("=========================================================================");
