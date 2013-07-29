#region Usings

using System.Collections.Generic;
using System.Linq;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;

#endregion

namespace PAPIPlugin.Impl
{
    public class DefaultLightArrayConfig : ILightArrayConfig
    {
        private const string LightGroupNodeName = "LightGroup";

        private const string LightConfigNodeName = "LightConfig";

        private readonly ICollection<ILightGroup> _lightGroups = new List<ILightGroup>();

        #region ILightArrayConfig Members

        public IEnumerable<ILightGroup> LightArrayGroups
        {
            get { return _lightGroups; }
        }

        public bool DebugMode { get; set; }

        public void Destroy()
        {
            foreach (var lightArray in _lightGroups.SelectMany(group => group.LightArrays))
            {
                lightArray.Destroy();
            }
        }

        #endregion

        public void LoadConfig()
        {
            foreach (var configNode in GameDatabase.Instance.GetConfigNodes(LightGroupNodeName))
            {
                var lightGroup = CreateLightGroup();

                var success = ConfigNode.LoadObjectFromConfig(lightGroup, configNode);

                if (!success)
                {
                    continue;
                }

                var node = lightGroup as IConfigNode;
                if (node != null)
                {
                    node.Load(configNode);
                }

                _lightGroups.Add(lightGroup);
            }

            foreach (var configNode in GameDatabase.Instance.GetConfigNodes(LightConfigNodeName))
            {
                DebugMode = configNode.ConvertValue("Debug", false);
            }
        }

        protected virtual ILightGroup CreateLightGroup()
        {
            return new DefaultLightGroup();
        }
    }
}
