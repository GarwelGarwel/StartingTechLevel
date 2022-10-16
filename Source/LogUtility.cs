using Verse;

namespace StartingTechLevel
{
    enum LogLevel
    {
        Message = 0,
        Important,
        Error
    };

    internal static class LogUtility
    {
        internal static void Log(string message, LogLevel logLevel = LogLevel.Message)
        {
            message = $"[StartingTechLevel] {message}";
            switch (logLevel)
            {
                case LogLevel.Message:
                    if (Prefs.DevMode)
                        Verse.Log.Message(message);
                    break;

                case LogLevel.Important:
                    Verse.Log.Warning(message);
                    break;

                case LogLevel.Error:
                    Verse.Log.Error(message);
                    break;
            }
        }
    }
}
