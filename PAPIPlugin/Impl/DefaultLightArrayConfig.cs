#region Usings

using System;
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

        public bool UseBlizzy78Toolbar { get; set; }

        public PositionDecision positionDecision { get; set; }

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
                UseBlizzy78Toolbar = configNode.ConvertValue("UseBlizzy78Toolbar", false);
                positionDecision = configNode.ConvertValue("PositionDecision", PositionDecision.Auto);
            }
            PAPIPlugin.Arrays.PAPIArray.positionDecision = positionDecision;
        }

        public void SaveConfig()
        {
            var configNode = new ConfigNode();
            configNode.name = "PAPIPlugin";

            foreach (var lightGroup in _lightGroups)
            {
                var groupConfigNode = new ConfigNode();
                groupConfigNode.name = "LightGroup";

                groupConfigNode.AddValue("Name", lightGroup.Name);
                groupConfigNode.AddValue("Body", lightGroup.ParentBody.name);

                foreach (var lightArray in lightGroup.LightArrays)
                {
                    var papi = lightArray as PAPIPlugin.Arrays.PAPIArray;

                    var arrayConfigNode = new ConfigNode();
                    arrayConfigNode.name = "LightArray";

                    arrayConfigNode.AddValue("Type", lightArray.GetType().Name);
                    arrayConfigNode.AddValue("Namespace", lightArray.GetType().Namespace);

                    arrayConfigNode.AddValue("Latitude", papi.Latitude);
                    arrayConfigNode.AddValue("Longitude", papi.Longitude);

                    arrayConfigNode.AddValue("Heading", papi.Heading * 180 / Math.PI);

                    arrayConfigNode.AddValue("GlideslopeTolerance", papi.GlideslopeTolerance);
                    arrayConfigNode.AddValue("TargetGlideslope", papi.TargetGlideslope);
                    arrayConfigNode.AddValue("HeightAboveTerrain", papi.HeightAboveTerrain);
                    arrayConfigNode.AddValue("PartCount", papi.PartCount);
                    arrayConfigNode.AddValue("LightRadius", papi.LightRadius);
                    arrayConfigNode.AddValue("LightDistance", papi.LightDistance);

                    groupConfigNode.AddNode(arrayConfigNode);
                }
                configNode.AddNode(groupConfigNode);
            }
            configNode.Save(KSPUtil.ApplicationRootPath + "GameData/PAPIPlugin/lights.cfg", "PAPIPlugin");
        }

        protected virtual ILightGroup CreateLightGroup()
        {
            return new DefaultLightGroup();
        }
    }
}
