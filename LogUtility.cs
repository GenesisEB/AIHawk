namespace AIHawk
{
    internal static class LogUtility
    {
        internal static void Log(string s) => File.AppendAllText("./log.txt", s);
    }
}
