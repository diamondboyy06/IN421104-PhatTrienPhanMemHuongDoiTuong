namespace EduHub.Domain;

public enum MemberStatus
{
    Active,
    Inactive
}

public enum RegistrationStatus
{
    Pending,
    Confirmed,
    Cancelled
}

public static class CheckInMethod
{
    public const string Qr = "QR_CODE";
    public const string Manual = "MANUAL";
}
