#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Kerbtown;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;

#endregion

namespace PAPIPlugin
{
    public class KerbTownModule : StaticObjectModule, IConfigNode
    {
        private IList<ILightArray> _lightArrays;

        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;

                if (_lightArrays != null)
                {
                    _lightArrays.ForEach(array => array.Enabled = value);
                }
            }
        }

        public void Awake()
        {
            _lightArrays = new List<ILightArray>();
        }

        public void FixedUpdate()
        {
            foreach (var lightArray in _lightArrays)
            {
                lightArray.Update();
            }
        }

        public void OnDestroy()
        {
            foreach (var lightArray in _lightArrays)
            {
                lightArray.Destroy();
            }
        }

        public void Load(ConfigNode node)
        {
            foreach (var lightNode in node.GetNodes("LightArray"))
            {
                var arrayType = GetArrayType(lightNode);

                if (arrayType == null)
                {
                    continue;
                }

                Util.LogInfo(string.Format("Found array of type {0}.", arrayType.FullName));

                var arrayObject = Activator.CreateInstance(arrayType);

                if (!(arrayObject is ILightArray))
                {
                    Util.LogWarning(string.Format("The type {0} is no light array!", arrayType));
                    continue;
                }

                var success = ConfigNode.LoadObjectFromConfig(arrayObject, lightNode);

                if (success)
                {
                    var lightArray = arrayObject as ILightArray;

                    var asConfigNode = lightArray as IConfigNode;
                    if (asConfigNode != null)
                    {
                        asConfigNode.Load(lightNode);
                    }

                    _lightArrays.Add(lightArray);

                    var group = GetGroup(lightNode.GetValue("Group"));

                    if (group != null)
                    {
                        group.AddArray(lightArray);
                    }

                    lightArray.Initialize(group, gameObject);
                }
            }
        }

        private static ILightGroup GetGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return null;
            }
            else
            {
                return PAPIAddon.Config.LightArrayGroups.FirstOrDefault(group => group.Name == groupName);
            }
        }

        private static Type GetArrayType(ConfigNode configNode)
        {
            var typeName = configNode.GetValue("Type");
            var namespaceName = configNode.GetValue("Namespace");

            if (string.IsNullOrEmpty(typeName))
            {
                Util.LogWarning("Type name is required for light array definition!");
                return null;
            }

            Type type;

            if (string.IsNullOrEmpty(namespaceName))
            {
                type = AssemblyLoader.loadedAssemblies.SelectMany(asm => asm.assembly.GetTypes()).FirstOrDefault(t => t.Name == typeName);

                if (type == null)
                {
                    Util.LogWarning(string.Format("Type name \"{0}\" is not a known type.", typeName));
                }
            }
            else
            {
                type =
                    AssemblyLoader.loadedAssemblies.SelectMany(asm => asm.assembly.GetTypes())
                                  .FirstOrDefault(t => t.Namespace == namespaceName && t.Name == typeName);

                if (type == null)
                {
                    Util.LogWarning(string.Format("Type name \"{0}.{1}\" is not a known type.", namespaceName, typeName));
                }
            }

            return type;
        }

        public void Save(ConfigNode node)
        {
        }
    }
}
