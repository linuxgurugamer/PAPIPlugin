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
        private readonly Icon<DefaultLightArrayManager> _groupWindowIcon;

        private GroupWindow<ILightArrayConfig> _groupWindow;

        private ILightArrayConfig _lightConfig;

        public DefaultLightArrayManager()
        {
            _groupWindowIcon = new Icon<DefaultLightArrayManager>(new Rect(Screen.width * 0.8f, 0.0f, 80.0f, 20.0f), "icon.png", "Light groups",
                "Opens the light group overview", OnIconClickHandler);
            _groupWindowIcon.SetVisible(true);
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

            _groupWindowIcon.SetVisible(false);

            if (_groupWindow != null)
            {
                _groupWindow.SetVisible(false);
            }
        }
    }
}
