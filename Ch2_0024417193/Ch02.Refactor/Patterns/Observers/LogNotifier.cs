using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;

namespace EduHub.Ch02.Patterns.Observers;

/// <summary>Observer thứ hai: chỉ ghi log. Thêm vào mà không sửa EventService.</summary>
public class LogNotifier : IEventObserver
{
    private readonly IAppLogger _logger;
    public LogNotifier(IAppLogger logger) => _logger = logger;

    public void OnEventCreated(Event ev, Club club) =>
        _logger.Info($"Event created: '{ev.Title}' ({club.Name})");
}
