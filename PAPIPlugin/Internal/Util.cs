using Debug = UnityEngine.Debug;

namespace PAPIPlugin.Internal
{
    public class Util
    {
        const string MODNAME = "PAPIPlugin: ";
        public static void LogDebugInfo(object msg)
        {
            Debug.Log(MODNAME + msg);
        }
        public static void LogInfo(object msg)
        {
            Debug.Log(MODNAME + msg);
        }

        public static void LogWarning(object msg)
        {
            Debug.LogWarning(MODNAME + msg);
        }

        public static void LogError(object msg)
        {
            Debug.LogError(MODNAME + msg);
        }

        public static void LogTrace(object msg)
        {
            Debug.Log(string.Format(MODNAME + "{ 0}\n{1}", msg, System.Environment.StackTrace));
        }
    }
}