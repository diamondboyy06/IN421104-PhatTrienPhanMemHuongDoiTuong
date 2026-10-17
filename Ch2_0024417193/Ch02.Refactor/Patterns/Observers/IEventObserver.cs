using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Patterns.Observers;

/// <summary>OBSERVER: bên lắng nghe khi có sự kiện mới được tạo.</summary>
public interface IEventObserver
{
    void OnEventCreated(Event ev, Club club);
}
