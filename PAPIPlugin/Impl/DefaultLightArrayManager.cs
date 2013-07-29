#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;

#endregion

namespace PAPIPlugin.Impl
{
    public class DefaultLightArrayManager : ILightArrayManager
    {
        private ILightArrayConfig _lightConfig;

        #region ILightArrayManager Members

        public event EventHandler ParsingFinished;

        public ILightArrayConfig LightConfig
        {
            get { return _lightConfig; }
            set
            {
                if (Equals(_lightConfig, value))
                    return;

                _lightConfig = value;

                InitializeConfig(_lightConfig);
            }
        }

        private void InitializeConfig(ILightArrayConfig lightConfig)
        {
            foreach (var lightArray in lightConfig.LightArrayGroups.SelectMany(group => group.LightArrays))
            {
                lightArray.InitializeDisplay(this);
            }
        }

        public void LoadConfig()
        {
            Util.LogInfo("Starting to parse light definitions...");

            var stopwatch = Stopwatch.StartNew();

            var defaultConfig = new DefaultLightArrayConfig();
            defaultConfig.LoadConfig();

            LightConfig = defaultConfig;

            Util.LogInfo(string.Format("Finished parsing definitions. Found {0} light groups with a total of {1} light arrays in a time of {2}.",
                LightConfig.LightArrayGroups.Count(), LightConfig.LightArrayGroups.Sum(group => group.LightArrays.Count()), stopwatch.Elapsed));

            OnParsingFinished();
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
        }
    }
}
