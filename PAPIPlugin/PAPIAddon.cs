#region Usings

using PAPIPlugin.Impl;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;

#endregion

namespace PAPIPlugin
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class PAPIAddon : MonoBehaviour
    {
        static internal PAPIAddon fetch;

        internal ILightArrayManager _arrayManager;
        private Callback postDrawCallbacks;

        private static ILightArrayConfig _config;

        public void Awake()
        {
            Util.LogInfo("Awake!");
            fetch = this;
            _arrayManager = new DefaultLightArrayManager();

            if (_config == null)
            {
                _config = _arrayManager.LoadConfig();
            }
            else
            {
                _arrayManager.LightConfig = _config;
            }

            GameEvents.onGUIApplicationLauncherReady.Add(_arrayManager.InitializeButton);
            postDrawCallbacks = new Callback(_arrayManager.OnGUI);
        }

        public void Update()
        {
            if (_arrayManager != null)
            {
                _arrayManager.Update();
            }
        }

        public void OnGUI()
        {
            if (postDrawCallbacks != null)
            {
                postDrawCallbacks();
            }
        }

        public void OnDestroy()
        {
            if (_arrayManager == null)
            {
                return;
            }

            Util.LogInfo("OnDestroy!");

            _arrayManager.Dispose();
            GameEvents.onGUIApplicationLauncherReady.Remove(_arrayManager.InitializeButton);
        }
    }
}
