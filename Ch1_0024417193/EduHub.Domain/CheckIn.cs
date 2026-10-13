namespace EduHub.Domain;

public class CheckIn
{
    public Guid Id { get; private set; }
    public Guid RegistrationId { get; private set; }
    public DateTime CheckedInAt { get; private set; }
    public string Method { get; private set; }

    private CheckIn(Guid registrationId, string method)
    {
        Id = Guid.NewGuid();
        RegistrationId = registrationId;
        CheckedInAt = DateTime.UtcNow;
        Method = method;
    }

    public static CheckIn Create(Registration registration, string method = CheckInMethod.Manual)
    {
        ArgumentNullException.ThrowIfNull(registration);
        if (registration.Status != RegistrationStatus.Confirmed)
            throw new InvalidOperationException("Chỉ phiếu đăng ký đã được xác nhận (Confirmed) mới được phép Check-in.");

        return new CheckIn(registration.Id, method);
    }

    public string Summary() => $"[CheckIn {Id.ToString()[..8]}] Phiếu: {RegistrationId.ToString()[..8]} lúc {CheckedInAt:HH:mm:ss dd/MM/yyyy} ({Method})";
}
