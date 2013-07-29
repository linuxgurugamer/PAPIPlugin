using Debug = UnityEngine.Debug;

namespace PAPIPlugin.Internal
{
    public class Util
    {
        public static void LogInfo(string msg)
        {
            Debug.Log("PAPIPlugin: " + msg);
        }

        public static void LogWarning(string msg)
        {
            Debug.LogWarning("PAPIPlugin: " + msg);
        }

        public static void LogError(string msg)
        {
            Debug.LogError("PAPIPlugin: " + msg);
        }
    }
}