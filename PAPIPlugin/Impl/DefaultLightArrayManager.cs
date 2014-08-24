#region Usings

using System;
using System.Diagnostics;
using System.Linq;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using PAPIPlugin.UI;
using Tac;
using UnityEngine;

#endregion

namespace PAPIPlugin.Impl
{
    public class DefaultLightArrayManager : ILightArrayManager
    {
        private ApplicationLauncherButton _appButtonStock;

        private GroupWindow<ILightArrayConfig> _groupWindow;

        private ILightArrayConfig _lightConfig;

        public DefaultLightArrayManager()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
        }

        #region ILightArrayManager Members

        public event EventHandler ParsingFinished;

        public ILightArrayConfig LightConfig
        {
            get { return _lightConfig; }
            set
            {
                if (Equals(_lightConfig, value))
                {
                    return;
                }

                _lightConfig = value;

                InitializeConfig(_lightConfig);
            }
        }

        public ILightArrayConfig LoadConfig()
        {
            Util.LogInfo("Starting to parse light definitions...");

            var stopwatch = Stopwatch.StartNew();

            var defaultConfig = new DefaultLightArrayConfig();
            defaultConfig.LoadConfig();

            LightConfig = defaultConfig;

            Util.LogInfo(string.Format("Finished parsing definitions. Found {0} light groups with a total of {1} light arrays in a time of {2}.",
                LightConfig.LightArrayGroups.Count(), LightConfig.LightArrayGroups.Sum(group => group.LightArrays.Count()), stopwatch.Elapsed));

            OnParsingFinished();

            return LightConfig;
        }

        public void Update()
        {
            foreach (var lightGroup in LightConfig.LightArrayGroups)
            {
                lightGroup.Update();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        private void OnGUIAppLauncherReady()
        {
            if (ApplicationLauncher.Ready)
            {
                _appButtonStock = ApplicationLauncher.Instance.AddModApplication(
                    OnIconClickHandler,
                    OnIconClickHandler,
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPACECENTER,
                    (Texture)GameDatabase.Instance.GetTexture("PAPIPlugin/icon_button", false)
                );
            }
        }

        private void DummyVoid() { }

        private void OnIconClickHandler()
        {
            if (_groupWindow == null)
            {
                _groupWindow = new GroupWindow<ILightArrayConfig>(LightConfig);
                _groupWindow.SetVisible(true);
            }
            else
            {
                _groupWindow.ToggleVisible();
            }

            // Don't lock highlight on the button since it's just a toggle
            _appButtonStock.SetFalse(false);
        }

        private void InitializeConfig(ILightArrayConfig lightConfig)
        {
            foreach (var lightArray in lightConfig.LightArrayGroups.SelectMany(group => group.LightArrays))
            {
                lightArray.InitializeDisplay(this);
            }
        }

        protected virtual void OnParsingFinished()
        {
            var handler = ParsingFinished;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        ~DefaultLightArrayManager()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            LightConfig.Destroy();

            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            if (_appButtonStock != null)
                ApplicationLauncher.Instance.RemoveModApplication(_appButtonStock);

            if (_groupWindow != null)
            {
                _groupWindow.SetVisible(false);
            }
        }
    }
}
