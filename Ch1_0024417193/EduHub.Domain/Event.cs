namespace EduHub.Domain;

public class Event
{
    public Guid Id { get; private set; }
    public Guid ClubId { get; private set; }
    public string Title { get; private set; }
    public DateTime Date { get; private set; }
    public int Capacity { get; private set; }
    public bool IsOnline { get; private set; }

    private Event(Guid clubId, string title, DateTime date, int capacity, bool isOnline)
    {
        Id = Guid.NewGuid();
        ClubId = clubId;
        Title = title;
        Date = date;
        Capacity = capacity;
        IsOnline = isOnline;
    }

    public static Event Create(Guid clubId, string title, DateTime date, int capacity, bool isOnline = false)
    {
        if (clubId == Guid.Empty)
            throw new ArgumentException("ClubId không hợp lệ.", nameof(clubId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Tiêu đề sự kiện không được để trống.", nameof(title));
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Sức chứa sự kiện phải lớn hơn 0.");

        return new Event(clubId, title.Trim(), date, capacity, isOnline);
    }

    public bool HasCapacity(int currentRegistrationCount) => currentRegistrationCount < Capacity;
}
