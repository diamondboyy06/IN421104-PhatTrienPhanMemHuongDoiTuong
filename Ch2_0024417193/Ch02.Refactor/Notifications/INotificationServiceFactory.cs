namespace EduHub.Ch02.Notifications;

/// <summary>FACTORY: nơi duy nhất biết cách tạo từng loại INotificationService.</summary>
public interface INotificationServiceFactory
{
    INotificationService Create(NotificationChannel channel);
}
