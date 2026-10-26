# EduHub Chương 3: Clean Architecture & RESTful API Cơ Bản
## Học Phần: Phát Triển Phần Mềm Hướng Đối Tượng (IN4211) — Trường Đại học Đồng Tháp

- **Sinh viên thực hiện**: Lê Tuấn Kiệt
- **Mã số sinh viên**: 0024417193
- **Lớp**: ĐHCNTT24B-IT
- **Thư mục bài làm**: `OOP_0024417193/Ch3_0024417193`
- **Nhánh Git**: `feature/ch03-skeleton`

---

## 1. Mục Tiêu Dự Án & Tóm Tắt Nhiệm Vụ
Xây dựng khung ứng dụng (skeleton) hệ thống quản lý câu lạc bộ sinh viên **EduHub** tuân thủ nghiêm ngặt mô hình **Clean Architecture 4 tầng** kết hợp với **CQRS** (Command Query Responsibility Segregation) và chuẩn **RESTful Web API** trên nền tảng .NET 10:

1. **Tầng Domain (`EduHub.Domain`)**: 
   - Độc lập hoàn toàn, không tham chiếu bất kỳ tầng nào.
   - Chứa thực thể `Club` tự bảo vệ tính toàn vẹn (Encapsulation, `private set`, Factory Method `Club.Create`) và hợp đồng trừu tượng `IClubRepository`, `IMemberRepository`.
2. **Tầng Application (`EduHub.Application`)**:
   - Chỉ phụ thuộc vào `Domain`, độc lập với công nghệ lưu trữ và UI/API.
   - Triển khai mỗi Use Case là một cặp Query/Command và Handler riêng biệt:
     - `GetClubsQueryHandler`: Truy vấn danh sách toàn bộ câu lạc bộ.
     - `GetClubByIdQueryHandler`: Truy vấn chi tiết câu lạc bộ theo ID (trả về `null` nếu không tìm thấy, không ném exception).
     - `CreateClubCommandHandler`: Kiểm tra tính hợp lệ (tên không rỗng, tối đa 50 ký tự), tạo thực thể qua `Club.Create` và lưu qua repository.
     - `DeleteClubCommandHandler`: Xóa câu lạc bộ theo ID.
3. **Tầng Infrastructure (`EduHub.Infrastructure`)**:
   - Phụ thuộc vào `Domain` và `Application`.
   - Cài đặt kho lưu trữ `InMemoryClubRepository` an toàn đa luồng (`lock (_lock)`), sử dụng `private static readonly List<Club>` với vòng đời `Scoped` và sao chép danh sách `.ToList()` để tránh rò rỉ tham chiếu.
4. **Tầng Presentation (`EduHub.API`)**:
   - Phụ thuộc vào `Application` và `Infrastructure`.
   - Đăng ký Dependency Injection (`AddScoped`) cho repositories và use case handlers trong `Program.cs`.
   - Cấu hình Swagger/OpenAPI (`EduHub API v1`).
   - Xây dựng **Thin Controller** `ClubsController` với Resource-based Routing (`/api/clubs`), ánh xạ đúng các mã trạng thái HTTP:
     - `GET /api/clubs`: `200 OK`.
     - `GET /api/clubs/{id}`: `200 OK` nếu tìm thấy, `404 Not Found` nếu không tìm thấy.
     - `POST /api/clubs`: `201 Created` bằng `CreatedAtAction` kèm header `Location`, hoặc `400 Bad Request` nếu dữ liệu không hợp lệ.
     - `DELETE /api/clubs/{id}`: `204 NoContent` khi xóa thành công, hoặc `404 Not Found` nếu ID không tồn tại.
5. **Kiểm thử tự động (`EduHub.Tests`)**:
   - Viết test với xUnit + Moq theo mô hình **Arrange - Act - Assert (AAA)**.
   - Hoàn thành đầy đủ 5/5 unit tests (bao gồm test mở rộng `GetClubByIdQueryTests` sử dụng Mock).

---

## 2. Cấu Trúc Dự Án Clean Architecture

```
Ch3_0024417193/
├── EduHub.sln                         # Solution tổng hợp .NET 10
├── README.md                          # Tài liệu báo cáo dự án
│
├── EduHub.Domain/                     # Tầng Lõi Nghiệp Vụ (Không phụ thuộc tầng khác)
│   ├── Entities/
│   │   ├── Club.cs                    # Thực thể Club (Factory Method, private set)
│   │   └── Member.cs                  # Thực thể Member
│   └── Interfaces/
│       ├── IClubRepository.cs         # Hợp đồng kho lưu trữ Club
│       └── IMemberRepository.cs       # Hợp đồng kho lưu trữ Member
│
├── EduHub.Application/                # Tầng Ca Sử Dụng (Chỉ phụ thuộc Domain)
│   └── Clubs/
│       ├── CreateClubCommand.cs       # Command + Handler tạo CLB (Validate & Save)
│       ├── DeleteClubCommand.cs       # Command + Handler xóa CLB
│       ├── GetClubByIdQuery.cs        # Query + Handler lấy CLB theo ID
│       └── GetClubsQuery.cs           # Query + Handler lấy danh sách CLB
│
├── EduHub.Infrastructure/             # Tầng Kỹ Thuật Hạ Tầng (Lưu trữ bộ nhớ)
│   └── Repositories/
│       ├── InMemoryClubRepository.cs   # Kho lưu trữ in-memory Thread-Safe (lock)
│       └── InMemoryMemberRepository.cs # Kho lưu trữ in-memory Member
│
├── EduHub.API/                        # Tầng Giao Tiếp (Presentation - ASP.NET Core)
│   ├── Controllers/
│   │   └── ClubsController.cs         # Thin Controller chuẩn RESTful (/api/clubs)
│   ├── appsettings.json
│   └── Program.cs                     # Composition Root (DI Scoped & Swagger UI)
│
└── EduHub.Tests/                      # Tầng Kiểm Thử Tự Động (xUnit & Moq)
    └── Clubs/
        └── CreateClubCommandTests.cs  # Bộ 5 bài kiểm thử đơn vị AAA
```

---

## 3. Danh Sách 8 Nhóm TODO Đã Hoàn Thành

| STT | Vị trí TODO | Tệp tin | Trạng thái |
|:---:|:---|:---|:---:|
| 1 | **TODO 1a–1e** | `EduHub.Infrastructure/Repositories/InMemoryClubRepository.cs` | **Đã hoàn thành** (Thread-safe với `lock`) |
| 2 | **TODO 2** | `EduHub.Application/Clubs/GetClubsQuery.cs` | **Đã hoàn thành** |
| 3 | **TODO 3** | `EduHub.Application/Clubs/GetClubByIdQuery.cs` | **Đã hoàn thành** |
| 4 | **TODO 4a–4d** | `EduHub.Application/Clubs/CreateClubCommand.cs` | **Đã hoàn thành** (Validate rỗng & $\le 50$ ký tự) |
| 5 | **TODO 5** | `EduHub.Application/Clubs/DeleteClubCommand.cs` | **Đã hoàn thành** |
| 6 | **TODO 6a–6c** | `EduHub.API/Program.cs` | **Đã hoàn thành** (Đăng ký DI Scoped) |
| 7 | **TODO 7** | `EduHub.API/Program.cs` | **Đã hoàn thành** (Cấu hình SwaggerDoc v1) |
| 8 | **TODO 8a–8i** | `EduHub.API/Controllers/ClubsController.cs` | **Đã hoàn thành** (4 Action Async, mã HTTP chuẩn) |
| 9 | **Test thứ 5** | `EduHub.Tests/Clubs/CreateClubCommandTests.cs` | **Đã hoàn thành** (`GetClubById_WhenIdExists_ReturnsCorrectClub`) |

---

## 4. Hướng Dẫn Biên Dịch, Chạy Kiểm Thử & Chạy API

### 4.1 Biên dịch toàn bộ Solution
```powershell
dotnet build EduHub.sln
```
*(Kết quả: 0 Warning, 0 Error)*

### 4.2 Chạy bộ Unit Tests xUnit + Moq
```powershell
dotnet test EduHub.sln
```
*(Kết quả: 5 Passed, 0 Failed, 0 Skipped)*

### 4.3 Khởi chạy Web API & Truy cập Swagger UI
```powershell
dotnet run --project EduHub.API
```
Sau khi ứng dụng khởi chạy, mở trình duyệt truy cập:
👉 `http://localhost:5000/swagger` (hoặc cổng hiển thị trên Console) để thử nghiệm các endpoint RESTful qua Swagger UI.
