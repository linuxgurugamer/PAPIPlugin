#region Usings

using PAPIPlugin.Impl;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using Tac;
using UnityEngine;

#endregion

namespace PAPIPlugin
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class PAPIAddon : MonoBehaviour
    {
        private ILightArrayManager _arrayManager;

        private ILightArrayConfig _config;

        public void Awake()
        {
            if (HighLogic.LoadedScene != GameScenes.FLIGHT && HighLogic.LoadedScene != GameScenes.SPACECENTER)
            {

                return;
            }

            Util.LogInfo("Awake!");

            _arrayManager = new DefaultLightArrayManager();

            if (_config == null)
            {
                _arrayManager.LoadConfig();
                _config = _arrayManager.LightConfig;
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
