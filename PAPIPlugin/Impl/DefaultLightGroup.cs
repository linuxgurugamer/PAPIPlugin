#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;

#endregion

namespace PAPIPlugin.Impl
{
    public class DefaultLightGroup : ILightGroup, IConfigNode
    {
        private const string LightNodeName = "LightArray";

        private readonly ICollection<ILightArray> _lightArrays = new List<ILightArray>();

        private readonly IDictionary<Type, ILightTypeManager> _managers = new Dictionary<Type, ILightTypeManager>();

        #region IConfigNode Members

        public void Load(ConfigNode node)
        {
            var name = node.GetValue("Name");

            if (string.IsNullOrEmpty(name))
            {
                Util.LogWarning("Name value not found in config!");
                return;
            }

            Name = name;

            var bodyName = node.GetValue("Body");

            if (string.IsNullOrEmpty(bodyName))
            {
                Util.LogWarning(string.Format("The parent body value of light group {0} is missing!", Name));
                return;
            }

            ParentBody = FlightGlobals.Bodies.FirstOrDefault(body => body.name == bodyName);

            if (ParentBody == null)
            {
                Util.LogWarning(string.Format("The parent body {0} of light group {1} could not be found.", bodyName, Name));
                return;
            }

            Util.LogInfo(string.Format("Found light group {0} on body {1}.", Name, ParentBody.name));

            var configNodes = node.GetNodes(LightNodeName);

            foreach (var configNode in configNodes)
            {
                var arrayType = GetArrayType(configNode);

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

                var success = ConfigNode.LoadObjectFromConfig(arrayObject, configNode);

                if (success)
                {
                    var lightArray = arrayObject as ILightArray;

                    var asConfigNode = lightArray as IConfigNode;
                    if (asConfigNode != null)
                    {
                        asConfigNode.Load(configNode);
                    }

                    AddArray(lightArray);

                    lightArray.Initialize(this);
                }
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ILightGroup Members

        public event EventHandler<LightArrayEventArguments> LightArrayAdded;

        public string Name { get; private set; }

        public CelestialBody ParentBody { get; private set; }

        public IEnumerable<ILightArray> LightArrays
        {
            get { return _lightArrays; }
        }

        public void Update()
        {
            foreach (var lightArray in _lightArrays)
            {
                lightArray.Update();
            }
        }

        public T GetOrAddTypeManager<T>() where T : ILightTypeManager, new()
        {
            if (_managers.ContainsKey(typeof(T)))
            {
                return (T) _managers[typeof(T)];
            }

            var newManager = new T();

            _managers.Add(typeof(T), newManager);

            newManager.Initialize(this);

            return newManager;
        }

        public T GetOrAddTypeManager<T>(Func<T> creatorFunc) where T : ILightTypeManager
        {
            if (_managers.ContainsKey(typeof(T)))
            {
                return (T) _managers[typeof(T)];
            }

            var newManager = creatorFunc();

            _managers.Add(typeof(T), newManager);

            newManager.Initialize(this);

            return newManager;
        }

        public void Destroy()
        {
            foreach (var lightArray in _lightArrays)
            {
                lightArray.Destroy();
            }
        }

        #endregion

        protected virtual void OnLightArrayAdded(LightArrayEventArguments e)
        {
            var handler = LightArrayAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void AddArray(ILightArray array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            _lightArrays.Add(array);

            OnLightArrayAdded(new LightArrayEventArguments(array));
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

        ~DefaultLightGroup()
        {
            Destroy();
        }
    }
}
