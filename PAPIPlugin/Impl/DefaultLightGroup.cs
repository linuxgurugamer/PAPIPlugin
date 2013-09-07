#region Usings

using System;
using System.Collections.Generic;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;

#endregion

namespace PAPIPlugin.Impl
{
    public class DefaultLightGroup : ILightGroup, IConfigNode
    {
        private readonly ICollection<ILightArray> _lightArrays = new HashSet<ILightArray>();

        private readonly IDictionary<Type, ILightTypeManager> _managers = new Dictionary<Type, ILightTypeManager>();

        #region ILightGroup Members

        public event EventHandler<LightArrayEventArguments> LightArrayAdded;

        public string Name { get; private set; }

        public IEnumerable<ILightArray> LightArrays
        {
            get { return _lightArrays; }
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

        public void OnGui(int windowId)
        {
            foreach (var lightTypeManager in _managers.Values)
            {
                lightTypeManager.OnGui(windowId);
            }
        }

        public void AddArray(ILightArray array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (_lightArrays.Contains(array))
            {
                return;
            }

            _lightArrays.Add(array);

            OnLightArrayAdded(new LightArrayEventArguments(array));
        }

        public bool RemoveArray(ILightArray array)
        {
            if (array == null)
            {
                return false;
            }

            return _lightArrays.Remove(array);
        }

        #endregion

        public void Load(ConfigNode node)
        {
            Name = node.GetValue("Name");

            if (string.IsNullOrEmpty(Name))
            {
                Util.LogWarning("No name for light group found!");
            }
        }

        public void Save(ConfigNode node)
        {
            throw new NotSupportedException();
        }

        protected virtual void OnLightArrayAdded(LightArrayEventArguments e)
        {
            var handler = LightArrayAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
