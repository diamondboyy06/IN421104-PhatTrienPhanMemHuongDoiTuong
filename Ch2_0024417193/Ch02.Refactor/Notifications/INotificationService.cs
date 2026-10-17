namespace EduHub.Ch02.Notifications;

/// <summary>Gửi thông báo. Thêm kênh mới (SMS, Slack) = thêm class, không sửa code cũ (OCP).</summary>
public interface INotificationService
{
    void Send(string to, string message);
}
