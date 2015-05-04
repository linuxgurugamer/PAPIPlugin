#region Usings

using PAPIPlugin.Impl;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;

#endregion

namespace PAPIPlugin
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class PPAPIAddonSpaceCentre : PAPIAddon
    {
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class PPAPIAddonFlight : PAPIAddon
    {
    }

    public class PAPIAddon : MonoBehaviour
    {
        private ILightArrayManager _arrayManager;

        private static ILightArrayConfig _config;

        public void Awake()
        {
            Util.LogInfo("Awake!");

            _arrayManager = new DefaultLightArrayManager();
            _arrayManager.AllLightConfigReloaded += (sender, e) => _config = _arrayManager.LightConfig;

            if (_config == null)
            {
                _config = _arrayManager.LoadConfig();
            }
            else
            {
                _arrayManager.LightConfig = _config;
            }

            _arrayManager.InitializeButton();
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
