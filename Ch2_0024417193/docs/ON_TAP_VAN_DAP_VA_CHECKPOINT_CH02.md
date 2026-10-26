# CẨM NANG ÔN TẬP VẤN ĐÁP & BỘ CÂU HỎI CHECKPOINT CHƯƠNG 2
## HỌC PHẦN: PHÁT TRIỂN PHẦN MỀM HƯỚNG ĐỐI TƯỢNG (IN4211)
### Chuyên Đề: 5 Nguyên Lý SOLID, 4 Mẫu Thiết Kế GoF, Kiểm Thử xUnit AAA & Quy Trình Git
> **Trường Đại học Đồng Tháp — Khoa Công nghệ và Kỹ thuật**  
> - **Giảng viên giảng dạy**: ThS. Nguyễn Tuấn Kiệt  
> - **Sinh viên thực hiện**: Lê Tuấn Kiệt  
> - **Mã số sinh viên**: 0024417193  
> - **Lớp**: ĐHCNTT24B-IT  
> - **Dự án thực nghiệm**: Refactoring hệ thống quản lý câu lạc bộ sinh viên **EduHub**

---

## MỤC LỤC
1. [PHẦN I: 5 CÂU HỎI CHECKPOINT TRỌNG TÂM TRONG SLIDE (SLIDE 70)](#phần-i-5-câu-hỏi-checkpoint-trọng-tâm-slide-70)
2. [PHẦN II: 7 CÂU HỎI VẤN ĐÁP "BẪY KIẾN TRÚC" THỰC CHIẾN CỦA GIẢNG VIÊN](#phần-ii-7-câu-hỏi-vấn-đáp-bẫy-kiến-trúc-thực-chiến)
3. [PHẦN III: BẢNG CHEATSHEET PHẢN XẠ NHANH (MÃ CŨ VS MÃ MỚI)](#phần-iii-bảng-cheatsheet-phản-xạ-nhanh-mã-cũ-vs-mã-mới)
4. [PHẦN IV: KỊCH BẢN 5 BƯỚC BẢO VỆ BÀI LÀM TRƯỚC GIẢNG VIÊN ĐẠT ĐIỂM 10/10](#phần-iv-kịch-bản-5-bước-bảo-vệ-bài-làm-đạt-điểm-tuyệt-đối)

---

## PHẦN I: 5 CÂU HỎI CHECKPOINT TRỌNG TÂM (SLIDE 70)

### ❓ Câu 1: "God Class" là gì? Nó vi phạm những nguyên lý nào trong SOLID? Hãy chỉ ra ví dụ cụ thể trong lớp `ClubManager` ban đầu?
* **Khái niệm God Class**: Là một "Lớp Thần Thánh" (Anti-pattern) — một lớp duy nhất ôm đồm quá nhiều trách nhiệm, biết quá nhiều thứ và làm hầu hết mọi việc trong hệ thống.
* **Các nguyên lý vi phạm trong `ClubManager`**:
  1. **SRP (Single Responsibility Principle)**: Phương thức `CreateClub` vừa làm nhiệm vụ kiểm tra tính hợp lệ dữ liệu (Validate), vừa lưu trữ vào bộ nhớ (`_clubs.Add`), vừa ghi nhật ký ra tập tin cứng (`File.AppendAllText`), vừa phân nhánh gửi thông báo (`Console.WriteLine`).
  2. **OCP (Open/Closed Principle)**: Khi muốn hỗ trợ thêm kênh gửi tin (ví dụ: gửi SMS hoặc Slack), lập trình viên bắt buộc phải mở file `ClubManager.cs` ra và thêm các nhánh `else if (presidentEmail.Contains(...))` $\rightarrow$ Mã nguồn bị sửa đổi thay vì mở rộng.
  3. **DIP (Dependency Inversion Principle)**: `ClubManager` phụ thuộc trực tiếp vào các chi tiết tầng thấp như `File.AppendAllText("club-log.txt")`, `Console.WriteLine` và tự ý khởi tạo danh sách nội bộ thay vì phụ thuộc vào các giao diện trừu tượng (`IClubRepository`, `IAppLogger`, `INotificationService`).

---

### ❓ Câu 2: Vì sao việc sử dụng Interface và Dependency Injection (DI) lại giúp cho việc viết Kiểm thử đơn vị (Unit Test) trở nên dễ dàng và khả thi?
* **Trả lời**:
  - Khi một lớp phụ thuộc trực tiếp vào cài đặt cụ thể (Concrete Implementation) hoặc gọi trực tiếp tài nguyên vật lý (Database thật, ghi file đĩa cứng, gọi dịch vụ gửi Email ngoài đời thật qua mạng Internet), Unit Test sẽ chạy rất chậm, phụ thuộc vào môi trường bên ngoài và có thể làm hỏng dữ liệu thật.
  - Khi áp dụng **Interface** kết hợp với **Constructor Injection** (DIP), lớp nghiệp vụ chỉ làm việc với các bản hợp đồng trừu tượng (`IClubRepository`, `IAppLogger`). 
  - Trong môi trường Unit Test, ta có thể dễ dàng sử dụng thư viện **Moq** hoặc tạo các lớp giả lập (**Mock / Stub / Fake**) để "dặn trước" hành vi trả về (`mock.Setup(...).ReturnsAsync(...)`) và kiểm tra xem phương thức có được gọi đúng hay không (`mock.Verify(..., Times.Once)`). Nhờ đó bài test chạy hoàn toàn trong RAM với tốc độ hàng mili-giây, độc lập 100% với môi trường bên ngoài.

---

### ❓ Câu 3: Muốn thêm một kênh gửi thông báo mới (ví dụ: Zalo hoặc Telegram), theo Factory Method và OCP ta cần thêm gì và sửa gì?
* **Trả lời**:
  - **Cần THÊM**:
    1. Thêm một giá trị mới vào kiểu liệt kê: `NotificationChannel.Zalo` (hoặc `Telegram`).
    2. Tạo một lớp mới hiện thực giao diện `INotificationService`:
       ```csharp
       public class ZaloNotificationService : INotificationService
       {
           public void Send(string recipient, string message) => ...;
       }
       ```
    3. Thêm một nhánh ánh xạ khởi tạo trong lớp nhà máy `NotificationServiceFactory`.
  - **TUYỆT ĐỐI KHÔNG SỬA**:
    - Không cần sửa bất kỳ dòng mã nào trong các lớp nghiệp vụ cốt lõi: `ClubService`, `EventService`, hay các Controller ở Presentation! Tầng nghiệp vụ hoàn toàn không bị ảnh hưởng, đảm bảo nguyên lý **Đóng với sửa đổi nhưng Mở với mở rộng (OCP)**.

---

### ❓ Câu 4: Phân biệt sự khác nhau giữa Decorator Pattern và Kế thừa tĩnh? Phân biệt giữa Observer Pattern và Strategy Pattern?
* **Decorator vs Kế thừa**:
  - **Kế thừa tĩnh (Inheritance)**: Mở rộng chức năng tại thời điểm biên dịch (Compile-time). Tính năng bị trói chặt vào lớp cha cụ thể. Nếu có $N$ loại kho lưu trữ (Memory, Postgres, Mongo) và $M$ hành vi bổ sung (Log, Cache, Audit), kế thừa sẽ gây ra bùng nổ số lượng lớp ($N \times M$).
  - **Decorator (Composition/Wrapping)**: Mở rộng chức năng linh hoạt lúc chương trình đang chạy (Runtime) bằng cách bọc lấy Interface. Một `LoggingClubRepository` duy nhất có thể bọc bất kỳ đối tượng nào hiện thực `IClubRepository`.
* **Observer vs Strategy**:
  - **Observer Pattern**: Thiết lập quan hệ **Một - Nhiều (1-to-N)**. Khi trạng thái Subject thay đổi (ví dụ: tạo sự kiện mới), nó sẽ tự động phát tin cho toàn bộ các Subscriber/Observer đã đăng ký mà không cần biết các Subscriber đó sẽ làm gì.
  - **Strategy Pattern**: Thiết lập quan hệ **Một - Một (1-to-1)**. Đóng gói các thuật toán độc lập (ví dụ: Xuất báo cáo Console, Xuất báo cáo CSV) và cho phép phía Client chủ động chọn và hoán đổi thuật toán linh hoạt cho một ngữ cảnh (Context) cụ thể.

---

### ❓ Câu 5: Lệnh `git rebase` khác lệnh `git merge` ở điểm nào? Trong trường hợp nào TUYỆT ĐỐI KHÔNG ĐƯỢC dùng `git rebase`?
* **So sánh**:
  - **`git merge`**: Tạo ra một commit gộp (Merge Commit) mới để kết nối 2 nhánh lại với nhau, giữ nguyên toàn bộ các nhánh rẽ và thứ tự thời gian gốc của commit. Lịch sử commit sẽ có dạng nhánh cây đan xen.
  - **`git rebase`**: "Nhấc" toàn bộ các commit của nhánh hiện tại ra và đặt nối tiếp lên trên đỉnh commit mới nhất của nhánh đích (ví dụ `main`). Lịch sử commit được viết lại thành một đường thẳng tuyến tính, sạch đẹp và rất dễ theo dõi.
* **Nguyên tắc vàng cấm kỵ**:
  - **TUYỆT ĐỐI KHÔNG REBASE trên các nhánh công khai (Public / Shared Branches)** như nhánh `main` hoặc `develop` trên Remote mà nhiều thành viên khác đang cùng kéo về làm việc. Vì rebase sẽ thay đổi mã băm SHA-1 của commit, làm sai lệch lịch sử của các lập trình viên khác và gây xung đột nghiêm trọng. Chỉ rebase trên **nhánh cá nhân (`feature/...`)** trước khi mở Pull Request.

---

## PHẦN II: 7 CÂU HỎI VẤN ĐÁP "BẪY KIẾN TRÚC" THỰC CHIẾN

### 🧠 Câu 1: Tại sao trong `LoggingClubRepository` ta lại cài đặt Decorator Pattern bằng cách bọc Interface `IClubRepository` thay vì kế thừa `class LoggingClubRepository : InMemoryClubRepository`?
> **Đáp án chuẩn**:  
> Nếu kế thừa `InMemoryClubRepository`, ta đã đóng đinh hành vi ghi nhật ký vào riêng công nghệ lưu tạm trong RAM. Sau này ở Chương 4, khi nhà trường chuyển sang dùng cơ sở dữ liệu `PostgreSqlClubRepository`, lớp ghi log cũ sẽ trở nên vô dụng và ta lại phải viết thêm một lớp `LoggingPostgreSqlClubRepository`.  
> Dùng Decorator bọc lấy Interface `IClubRepository`, ta có được tính tái sử dụng tuyệt đối: một lớp Decorator duy nhất có thể ghi log cho **bất kỳ kho lưu trữ nào** (RAM, PostgreSQL, MongoDB, SQL Server)!

---

### 🧠 Câu 2: Trong hàm `CreateEvent` của `EventService`, tại sao ta phải duyệt danh sách qua `_observers.ToList()` (`foreach (var obs in _observers.ToList())`) mà không duyệt trực tiếp `_observers`?
> **Đáp án chuẩn**:  
> Trong C#, vòng lặp `foreach` sử dụng `IEnumerator`. Nếu trong quá trình duyệt mà danh sách nguồn bị thêm hoặc xóa phần tử, trình thông dịch .NET sẽ ngay lập tức ném ngoại lệ nghiêm trọng:  
> `InvalidOperationException: Collection was modified; enumeration operation may not execute.`  
> Trong thực tế, một Observer khi nhận thông báo có thể xử lý xong và tự động gọi `eventService.Unsubscribe(this)` để hủy nhận tin. Việc gọi `.ToList()` sẽ sao chép tạm thời một danh sách con trong RAM, đảm bảo vòng lặp duyệt trên bản sao một cách an toàn tuyệt đối 100%.

---

### 🧠 Câu 3: Làm thế nào để phân biệt giữa Strategy Pattern và State Pattern khi biểu đồ lớp UML của chúng gần như giống hệt nhau?
> **Đáp án chuẩn**:  
> - **Strategy Pattern**: Do phía **Client (người gọi bên ngoài)** chủ động lựa chọn thuật toán để truyền vào (Inject) nhằm xử lý một yêu cầu nghiệp vụ cụ thể (Ví dụ: người dùng chọn định dạng báo cáo Console hoặc CSV). Các Strategy không cần biết đến sự tồn tại của nhau.
> - **State Pattern**: Trạng thái của đối tượng tự động biến chuyển từ trạng thái này sang trạng thái khác bên trong chính nó dựa trên các sự kiện nội tại (Ví dụ: Đơn đăng ký từ `Pending` $\rightarrow$ `Approved` $\rightarrow$ `Cancelled`). Các lớp State thường biết nhau để bàn giao trạng thái tiếp theo cho Context.

---

### 🧠 Câu 4: Phân biệt 3 vòng đời Service trong Dependency Injection của .NET: `Transient`, `Scoped` và `Singleton`? Hiện tượng "Captive Dependency" là gì?
> **Đáp án chuẩn**:
> 1. **Transient (`AddTransient`)**: Mỗi lần có class nào yêu cầu phụ thuộc, DI Container sẽ tạo một thể hiện (instance) hoàn toàn mới. Thích hợp cho các lớp xử lý nhẹ, không lưu trạng thái (Stateless).
> 2. **Scoped (`AddScoped`)**: Một instance duy nhất được tạo ra và dùng chung trong suốt vòng đời của một yêu cầu HTTP (Web Request). Hết request thì đối tượng tự động bị tiêu hủy. Đây là chuẩn mực cho DbContext, Handler, Repository.
> 3. **Singleton (`AddSingleton`)**: Đúng một instance duy nhất được tạo ra khi ứng dụng khởi động và sống xuyên suốt cho tới khi tắt server. Dùng cho Cache, Logger, Configuration.
> - **Bẫy Captive Dependency**: Là hiện tượng một service có vòng đời dài (ví dụ Singleton) lại đi tiêm (inject) một service có vòng đời ngắn hơn (ví dụ Scoped). Khi đó, đối tượng Scoped sẽ bị "bắt giữ" và sống mãi theo Singleton, dẫn đến rò rỉ bộ nhớ hoặc dữ liệu của request trước bị lẫn sang request sau!

---

### 🧠 Câu 5: Tại sao trong Domain Entity (`Club`, `Event`), các thuộc tính đều để `{ get; private set; }` và phải khởi tạo qua Factory Method `Club.Create(...)`?
> **Đáp án chuẩn**:  
> Đây là nguyên tắc cốt lõi của **Tính đóng gói (Encapsulation)** và **Mô hình miền giàu tính năng (Rich Domain Model)**, đối lập với mô hình "Thiếu máu" (Anemic Domain Model) chỉ có `{ get; set; }` trần trụi.  
> Nếu để `public set`, bất kỳ code nào ở bên ngoài cũng có thể gán `club.Name = ""` (tên rỗng) hoặc `club.Name = null`, phá vỡ tính toàn vẹn của dữ liệu (Data Invariants). Khi đặt `private set` và bắt buộc qua `Club.Create(name, desc)`, chính thực thể sẽ tự kiểm tra: nếu tên rỗng hoặc vượt quá 100 ký tự thì ném lỗi ngay từ cửa vào!

---

### 🧠 Câu 6: Trong Unit Test xUnit, làm sao thư viện `Moq` có thể xác minh được một hàm nghiệp vụ "tuyệt đối không được lưu dữ liệu" khi đầu vào bị sai?
> **Đáp án chuẩn**:  
> Thư viện `Moq` cung cấp phương thức xác minh hành vi `mockRepo.Verify(...)`.  
> - Khi dữ liệu hợp lệ: Ta kiểm tra `mockRepo.Verify(r => r.Add(It.IsAny<Club>()), Times.Once)` (Kho phải được gọi đúng 1 lần).
> - Khi dữ liệu sai (tên rỗng): Ta kiểm tra `mockRepo.Verify(r => r.Add(It.IsAny<Club>()), Times.Never)` (Kho **tuyệt đối không bao giờ được phép gọi**). Điều này đảm bảo dữ liệu rác không bao giờ lọt xuống tầng lưu trữ!

---

### 🧠 Câu 7: Composition Root là gì? Trong bài làm Chương 2, Composition Root nằm ở tập tin nào?
> **Đáp án chuẩn**:  
> - **Composition Root** là một địa điểm duy nhất và tập trung trong toàn bộ ứng dụng — nơi tất cả các mô-đun, các lớp cụ thể và các giao diện trừu tượng được lắp ráp (wire-up) lại với nhau tại thời điểm ứng dụng khởi động.
> - Trong dự án Console Chương 2, Composition Root nằm tại tệp [**`Program.cs`**](file:///T:/HK1_26_27/built_app_oop/OOP_0024417193/Ch2_0024417193/Ch02.Refactor/Program.cs). Đây là nơi duy nhất được phép sử dụng từ khóa `new` để tạo các thể hiện cụ thể (`InMemoryClubRepository`, `LoggingClubRepository`, `ClubService`, `NotificationServiceFactory`) và tiêm chúng vào nhau.

---

## PHẦN III: BẢNG CHEATSHEET PHẢN XẠ NHANH (MÃ CŨ VS MÃ MỚI)

| Vấn đề kiến trúc | Mã nguồn cũ (`starter/`) | Mã nguồn mới (`solution/`) | Nguyên lý / Pattern cứu cánh |
|:---|:---|:---|:---|
| **Cấu trúc lớp** | Gom toàn bộ vào God Class `ClubManager`. | Tách thành các lớp Domain, Service, Repository, Notification riêng biệt. | **SRP** (Trách nhiệm đơn lẻ). |
| **Bảo vệ dữ liệu** | Thuộc tính `{ get; set; }` trần trụi, `ClubManager` kiểm tra từ ngoài. | Thuộc tính `{ get; private set; }`, kiểm tra bất biến bên trong `Club.Create`. | **Encapsulation** & **Domain Invariants**. |
| **Gửi thông báo** | Dùng chuỗi `if/else` kiểm tra đuôi `@gmail`, `@yahoo`. | Dùng `INotificationService` kết hợp với `NotificationServiceFactory`. | **OCP** & **Factory Method Pattern**. |
| **Ghi nhật ký** | Gọi trực tiếp `File.AppendAllText` trong hàm nghiệp vụ. | Dùng `IAppLogger` và bọc kho lưu trữ bằng `LoggingClubRepository`. | **DIP** & **Decorator Pattern**. |
| **Xuất báo cáo** | In cứng chuỗi bảng biểu ra màn hình Console bằng `PrintClubReport()`. | Tách thành `IReportStrategy` với `ConsoleReportStrategy` và `CsvReportStrategy`. | **OCP** & **Strategy Pattern**. |
| **Thông báo sự kiện** | Ghi log và in thông báo trực tiếp bên trong `CreateEvent()`. | `EventService` làm Subject phát tin tới danh sách các `IEventObserver`. | **Observer Pattern** (Loose Coupling). |
| **Khởi tạo đối tượng** | Các lớp tự `new` phụ thuộc cụ thể bên trong phương thức. | Nhận toàn bộ phụ thuộc qua Constructor (Constructor Injection) tại `Program.cs`. | **DIP** (Đảo ngược phụ thuộc). |
| **Kiểm thử tự động** | Không thể viết Unit Test vì bị dính chặt vào Console và file đĩa cứng thật. | Bộ kiểm thử tự động xUnit theo mô hình AAA (21 bài tests xanh 100%). | **Arrange - Act - Assert (AAA)**. |

---

## PHẦN IV: KỊCH BẢN 5 BƯỚC BẢO VỆ BÀI LÀM ĐẠT ĐIỂM TUYỆT ĐỐI (10/10)

Khi lên báo cáo trước Giảng viên, bạn tự tin thực hiện tuần tự theo 5 bước sau:

* **Bước 1: Giới thiệu kiến trúc tổng thể (1 phút)**:
  - *"Thưa thầy, mã nguồn ban đầu trong starter là một God Class `ClubManager` vi phạm nghiêm trọng 3 nguyên lý SRP, OCP và DIP. Em đã tái cấu trúc toàn diện thành 4 tầng độc lập theo Clean Architecture."*
* **Bước 2: Chứng minh chất lượng mã nguồn bằng Unit Test (30 giây)**:
  - Mở terminal và chạy lệnh:
    ```powershell
    dotnet test OOP_0024417193/Ch2_0024417193/Ch02.Refactor.sln
    ```
  - Chỉ ra kết quả: **21/21 tests Passed** (Bao phủ cả Domain rules và 4 Design Patterns theo mẫu Arrange-Act-Assert).
* **Bước 3: Trình diễn 5 khối Demo đầu ra (1 phút)**:
  - Chạy lệnh ứng dụng console:
    ```powershell
    dotnet run --project OOP_0024417193/Ch2_0024417193/Ch02.Refactor
    ```
  - Chỉ rõ cho thầy 5 khối hiển thị tương ứng với 4 mẫu thiết kế:
    1. Khối 1: `ClubService` + `LoggingClubRepository` (Decorator log `[repo]`) + Email.
    2. Khối 2: Observer phát tin tự động cho 2 Observers (1 dòng Log + 2 dòng Email thành viên).
    3. Khối 3: Strategy xuất cùng một tập dữ liệu ra 2 dạng: Console và CSV.
    4. Khối 4: Factory Method chuyển đổi 3 kênh Email, SMS, Slack không dùng `if/else`.
    5. Khối 5: Bắt ngoại lệ bảo vệ Domain Invariants khi tên rỗng hoặc thành viên chưa đăng ký.
* **Bước 4: Chứng minh sức mạnh của Decorator Pattern (30 giây)**:
  - Mở tệp `Program.cs`, tạm thời bỏ bọc `LoggingClubRepository` và truyền thẳng `innerRepo` vào `ClubService`.
  - Chạy lại demo: Cho thầy thấy dòng chữ `[repo] ...` lập tức biến mất nhưng toàn bộ logic nghiệp vụ của `ClubService` hoàn toàn không thay đổi!
* **Bước 5: Trình bày lịch sử Git & Báo cáo SOLID (30 giây)**:
  - Mở tệp [`docs/solid-analysis.md`](file:///T:/HK1_26_27/built_app_oop/OOP_0024417193/Ch2_0024417193/docs/solid-analysis.md) chỉ ra bảng 4 cột phân tích chi tiết.
  - Cho thầy xem lịch sử commit chuẩn mực bằng lệnh:
    ```powershell
    git log --oneline -n 6
    ```
  - Đã có Release Tag **`v2.0-ch02`** và gói nộp bài nén chuẩn mực sẵn sàng.
