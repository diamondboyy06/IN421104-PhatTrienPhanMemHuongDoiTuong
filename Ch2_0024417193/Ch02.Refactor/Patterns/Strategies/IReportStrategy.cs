namespace EduHub.Ch02.Patterns.Strategies;

/// <summary>STRATEGY: mỗi định dạng báo cáo là một cách "vẽ" cùng một dữ liệu.</summary>
public interface IReportStrategy
{
    string Name { get; }
    string Render(IReadOnlyList<ClubStat> stats);
}
