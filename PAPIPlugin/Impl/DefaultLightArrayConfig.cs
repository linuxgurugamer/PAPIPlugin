#region Usings

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;

using UnityEngine;

#endregion

namespace PAPIPlugin.Impl
{
    public class DefaultLightArrayConfig : ILightArrayConfig
    {
        private const string LIGHTGROUP = "LightGroup";

        private const string LIGHTCONFIG = "LightConfig";

        private readonly ICollection<ILightGroup> _lightGroups = new List<ILightGroup>();

        #region ILightArrayConfig Members

        public IEnumerable<ILightGroup> LightArrayGroups
        {
            get { return _lightGroups; }
        }

        public bool DebugMode { get; set; }


        public PositionDecision positionDecision { get; set; }

        public void Destroy()
        {
            foreach (var lightArray in _lightGroups.SelectMany(group => group.LightArrays))
            {
                lightArray.Destroy();
            }
        }

        #endregion

        void LoadRuntimeConfig()
        {
            var fname = KSPUtil.ApplicationRootPath + "GameData/PAPIPlugin/PAPI.config";
            if (File.Exists(fname))
            {
                var config = ConfigNode.Load(fname);
                var nodes = config.GetNodes(LIGHTGROUP);
                foreach (var configNode in nodes)
                {
                    var lightGroup = CreateLightGroup();
                    lightGroup.stdConfig = false;
                    var success = ConfigNode.LoadObjectFromConfig(lightGroup, configNode);

                    if (success)
                    {
                        var node = lightGroup as IConfigNode;
                        if (node != null)
                        {
                            node.Load(configNode);
                        }

                        _lightGroups.Add(lightGroup);
                    }
                }
            }
        }

        public void LoadConfig()
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<PAPI>().debug)
                Util.LogDebugInfo("LoadConfig");

            ConfigNode allNodes = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/PAPIPlugin/PluginData/lights.cfg");
            foreach (var configNode in allNodes.GetNodes(LIGHTGROUP))
            {
                var lightGroup = CreateLightGroup();
                lightGroup.stdConfig = true;
                var success = ConfigNode.LoadObjectFromConfig(lightGroup, configNode);

                if (success)
                {
                    var node = lightGroup as IConfigNode;
                    if (node != null)
                    {
                        node.Load(configNode);
                    }
                    if (HighLogic.CurrentGame.Parameters.CustomParams<PAPI>().debug)
                        Util.LogDebugInfo("LoadConfig: LightGroup: " + lightGroup.ToString());

                    _lightGroups.Add(lightGroup);
                }
            }
            LoadRuntimeConfig();
            foreach (var configNode in GameDatabase.Instance.GetConfigNodes(LIGHTCONFIG))
            {
                DebugMode = configNode.ConvertValue("Debug", false) | HighLogic.CurrentGame.Parameters.CustomParams<PAPI>().debug;
                positionDecision = configNode.ConvertValue("PositionDecision", PositionDecision.Auto);
            }
            PAPIPlugin.Arrays.PAPIArray.positionDecision = positionDecision;
        }

        public void SaveConfig(bool saveAll = false)
        {
            var configNode = new ConfigNode();
            configNode.name = "PAPIPlugin";

            if (HighLogic.CurrentGame.Parameters.CustomParams<PAPI>().debug)
                Util.LogDebugInfo("SaveConfig");

            foreach (var lightGroup in _lightGroups)
            {
                if (saveAll || lightGroup.stdConfig)
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

                        if (HighLogic.CurrentGame.Parameters.CustomParams<PAPI>().debug)
                            Util.LogDebugInfo("SaveConfig:LightArray: " + arrayConfigNode);

                        groupConfigNode.AddNode(arrayConfigNode);
                    }
                    configNode.AddNode(groupConfigNode);
                }
            }
            configNode.Save(KSPUtil.ApplicationRootPath + "GameData/PAPIPlugin/PluginData/lights.cfg", "PAPIPlugin");
        }

        protected virtual ILightGroup CreateLightGroup()
        {
            return new DefaultLightGroup();
        }
    }
}
