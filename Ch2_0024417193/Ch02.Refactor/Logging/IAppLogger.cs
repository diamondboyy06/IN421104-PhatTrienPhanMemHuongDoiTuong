namespace EduHub.Ch02.Logging;

/// <summary>Abstraction cho việc ghi log (thay cho File.AppendAllText trong starter).</summary>
public interface IAppLogger
{
    void Info(string message);
}
