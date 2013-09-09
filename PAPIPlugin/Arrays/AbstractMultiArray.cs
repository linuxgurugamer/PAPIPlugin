#region Usings

using System;
using PAPIPlugin.Interfaces;
using PAPIPlugin.Internal;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace PAPIPlugin.Arrays
{
    public abstract class AbstractMultiArray<T> : AbstractLightArray, IConfigNode where T : class, ILightArray
    {
        private const string ChildNodeName = "ChildArray";

        protected GameObject _arrayObject;

        protected T[] _lightArrays;

        protected GameObject[] _lightGameObjects;

        protected float LightDistance { get; set; }

        protected Vector3 LocalPosition { get; set; }

        protected Vector3 LocalEulerAngles { get; set; }

        protected int LightCount { get; set; }

        public override bool Enabled
        {
            set
            {
                base.Enabled = value;

                if (_lightArrays != null)
                {
                    _lightArrays.ForEach(array => array.Enabled = value);
                }

                if (_lightGameObjects != null)
                {
                    _lightGameObjects.ForEach(obj => obj.SetActive(value));
                }
            }
            protected get { return base.Enabled; }
        }

        public override void Initialize(ILightGroup @group, GameObject parentObj)
        {
            _arrayObject = new GameObject();
            _arrayObject.transform.parent = parentObj.transform;
            _arrayObject.transform.localPosition = LocalPosition;
            _arrayObject.transform.localEulerAngles = Vector3.zero;

            _lightGameObjects = new GameObject[LightCount];
            _lightGameObjects.Fill(i =>
                {
                    var retVal = _lightArrays == null ? CreateLightDisplay() : new GameObject();

                    retVal.transform.parent = _arrayObject.transform;
                    retVal.transform.localPosition = GetLocalLighPosition(i);
                    retVal.transform.localEulerAngles = Vector3.zero;

                    return retVal;
                });
        }

        public override void Update()
        {
        }

        public override void Destroy()
        {
            if (_lightGameObjects != null)
            {
                foreach (var lightGameObject in _lightGameObjects)
                {
                    Object.Destroy(lightGameObject);
                }
            }

            if (_arrayObject != null)
            {
                Object.Destroy(_arrayObject);
            }
        }

        /// <summary>
        ///     Gets the local position given a zero-based index.
        /// </summary>
        /// <param name="i">The index of the light, zero-based</param>
        /// <returns>A local position specifying the light position</returns>
        private Vector3 GetLocalLighPosition(int i)
        {
            float mult;

            // Some special handling to position the center always at the center of the array
            if (LightCount % 2 != 0)
            {
                mult = i - (float) LightCount / 2;
            }
            else
            {
                mult = i - ((float) LightCount / 2) - 0.5f;
            }

            return Vector3.right * LightDistance * mult;
        }

        protected abstract GameObject CreateLightDisplay();

        protected abstract T CreateChildArray();

        public virtual void Load(ConfigNode node)
        {
            LightCount = node.ConvertValue("LightCount", 1).ClampAndLog(1, int.MaxValue);
            LightDistance = node.ConvertValue("LightDistance", 0.0f).ClampAndLog(0.0f, float.MaxValue);
            LocalPosition = node.ConvertValue("LocalPosition", Vector3.zero);
            LocalEulerAngles = node.ConvertValue("LocalEulerAngles", Vector3.zero);

            if (node.HasNode(ChildNodeName))
            {
                if (_lightArrays == null)
                {
                    _lightArrays = new T[LightCount];
                    _lightArrays.Fill(CreateChildArray);
                }

                var childArrayNode = node.GetNode(ChildNodeName);

                for (var i = 0; i < _lightArrays.Length; i++)
                {
                    var array = _lightArrays[i];

                    var configNode = array as IConfigNode;
                    if (configNode != null)
                    {
                        var iConfigNode = configNode;

                        iConfigNode.Load(childArrayNode);

                        // Allow configuration if specific arrays
                        if (node.HasNode(ChildNodeName + i))
                        {
                            iConfigNode.Load(node.GetNode(ChildNodeName + i));
                        }
                    }
                }
            }
        }

        public virtual void Save(ConfigNode node)
        {
            throw new NotImplementedException();
        }
    }
}
