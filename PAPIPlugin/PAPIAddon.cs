#region Usings

using PAPIPlugin.Impl;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;

#endregion

namespace PAPIPlugin
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class PAPIAddon : MonoBehaviour
    {
        private ILightArrayManager _arrayManager;

        private static ILightArrayConfig _config;

        public void Awake()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT && HighLogic.LoadedScene != GameScenes.SPACECENTER)
            {
                enabled = false;
                return;
            }

            Util.LogInfo("Awake!");

            _arrayManager = new DefaultLightArrayManager();

            if (_config == null)
            {
                _config = _arrayManager.LoadConfig();
            }
            else
            {
                _arrayManager.LightConfig = _config;
            }
        }

        public void Update()
        {
            if (_arrayManager != null)
            {
                _arrayManager.Update();
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
        }
    }
}
