namespace Document.Core.Interfaces
{
    public interface ISeriLogRepository
    {
        void LogInfo(string message);

        void LogDebug(string message);

        void LogWarn(string message);

        void LogError(string message);

        void LogFatal(string message);

        void Dispose();
    }
}
