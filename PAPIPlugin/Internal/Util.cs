using Debug = UnityEngine.Debug;

namespace PAPIPlugin.Internal
{
    public class Util
    {
        public static void LogInfo(object msg)
        {
            Debug.Log("PAPIPlugin: " + msg);
        }

        public static void LogWarning(object msg)
        {
            Debug.LogWarning("PAPIPlugin: " + msg);
        }

        public static void LogError(object msg)
        {
            Debug.LogError("PAPIPlugin: " + msg);
        }
    }
}