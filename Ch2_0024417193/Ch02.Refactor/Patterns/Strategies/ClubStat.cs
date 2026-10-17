namespace EduHub.Ch02.Patterns.Strategies;

/// <summary>Dòng thống kê của một CLB — dữ liệu đầu vào chung cho mọi strategy.</summary>
public record ClubStat(string ClubName, int Members, int Events, int Registered, int CheckedIn);
