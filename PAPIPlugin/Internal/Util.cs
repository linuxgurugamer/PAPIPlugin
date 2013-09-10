using Debug = UnityEngine.Debug;

namespace PAPIPlugin.Internal
{
    public class Util
    {
        public static void LogInfo(object msg)
        {
            Debug.Log("PAPIPlugin: " + msg.SafeToString());
        }

        public static void LogWarning(object msg)
        {
            Debug.LogWarning("PAPIPlugin: " + msg.SafeToString());
        }

        public static void LogError(object msg)
        {
            Debug.LogError("PAPIPlugin: " + msg.SafeToString());
        }

        public static string GetFileContents(string name)
        {
            return KSP.IO.File.ReadAllText<Util>(name);
        }
    }
}