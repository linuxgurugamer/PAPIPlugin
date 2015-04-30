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
        private ApplicationLauncherButton _appButtonStock = null;

        private IButton _blizzy78Button = null;

        private GroupWindow<ILightArrayConfig> _groupWindow;

        private ILightArrayConfig _lightConfig;

        public DefaultLightArrayManager()
        {
        }

        #region ILightArrayManager Members

        public event EventHandler ParsingFinished;

        public event EventHandler AllLightConfigReloaded;

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

        public void SaveConfig()
        {
            if (LightConfig != null)
            {
                var config = LightConfig as DefaultLightArrayConfig;
                config.SaveConfig();
            }
        }

        public void InitializeButton()
        {
            if (LightConfig != null && LightConfig.UseBlizzy78Toolbar && ToolbarManager.ToolbarAvailable)
            {
                AddBlizzy78ToolbarButton();
            }
            else
            {
                if (_appButtonStock == null)
                {
                    OnGUIAppLauncherReady();
                }
            }
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

        private void AddBlizzy78ToolbarButton()
        {
            if (_blizzy78Button == null)
            {
                _blizzy78Button = ToolbarManager.Instance.add("PAPIPlugin", "PAPIPluginSetting");
                _blizzy78Button.TexturePath = "PAPIPlugin/icon_button24";
                _blizzy78Button.ToolTip = "PAPIPlugin Setting";
                _blizzy78Button.Visibility = new GameScenesVisibility(GameScenes.FLIGHT, GameScenes.SPACECENTER);
                _blizzy78Button.OnClick += (e) => OnIconClickHandler();
            }
        }

        private void DummyVoid() { }

        private void OnIconClickHandler()
        {
            if (_groupWindow == null)
            {
                _groupWindow = new GroupWindow<ILightArrayConfig>(LightConfig);
                _groupWindow.AllLightConfigReloaded += (sender, e) =>
                    {
                        LoadConfig();
                        _groupWindow.SetVisible(false);
                        _groupWindow = null;
                        AllLightConfigReloaded(this, e);
                    };
                _groupWindow.AllLightConfigSaved += (sender, e) => SaveConfig();
                _groupWindow.SetVisible(true);
            }
            else
            {
                _groupWindow.ToggleVisible();
            }

            if ((LightConfig != null && !LightConfig.UseBlizzy78Toolbar) || !ToolbarManager.ToolbarAvailable)
            {
                // Don't lock highlight on the button since it's just a toggle
                _appButtonStock.SetFalse(false);
            }
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

            if (LightConfig != null && LightConfig.UseBlizzy78Toolbar && ToolbarManager.ToolbarAvailable)
            {
                if (_blizzy78Button != null)
                {
                    _blizzy78Button.Destroy();
                    _blizzy78Button = null;
                }
            }
            else
            {
                if (_appButtonStock != null)
                {
                    ApplicationLauncher.Instance.RemoveModApplication(_appButtonStock);
                    _appButtonStock = null;
                }
            }

            if (_groupWindow != null)
            {
                _groupWindow.SetVisible(false);
            }
        }
    }
}
