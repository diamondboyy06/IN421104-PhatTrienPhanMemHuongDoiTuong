namespace EduHub.Ch02.Notifications;

/// <summary>Một cài đặt cho mỗi loại email — thay cho chuỗi if/else Gmail/Yahoo của starter.</summary>
public class EmailNotificationService : INotificationService
{
    public void Send(string to, string message) =>
        Console.WriteLine($"  [EMAIL] to {to}: {message}");
}
